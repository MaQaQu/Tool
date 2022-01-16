using SuperPort;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SuperPortLibrary
{
    public class CommTCPServer : Communication
    {
        private static int ServerCodeCreator = 0x3FFFFFFF;

        private int _ServerCode = 0;
        private int _WatchPort = 0;
        private bool _ServerHandle = true;
        private IPAddress _ClientAddress = IPAddress.Any;
        private int _ClientPort = 0;
        private bool ClientInfoChanged = false;
        private bool ServerInfoChanged = false;
        private ConcurrentDictionary<int, CommTCPSocket> _Sockets;
        private ConcurrentQueue<ReceivedPack> _SocketsReadDate;
        private int _MaximumClient = 8;
        private int CurrentPort = 0;

        internal int ServerCode { get { return _ServerCode; } }
        private ConcurrentQueue<ReceivedPack> SocketsReadDate { get { return _SocketsReadDate; } }

        public int WatchPort { get { return _WatchPort; } }
        public IPAddress ClientAddress { get { return _ClientAddress; } }
        public int ClientPort { get { return _ClientPort; } }
        public ConcurrentDictionary<int, CommTCPSocket> Sockets { get { return _Sockets; } }
        public bool ServerHandle { get { return _ServerHandle; } }
        public int MaximumClient { get { return _MaximumClient; } }

        public event ServerSocketEventHandler SocketChanged;
        public event ServerSocketEventHandler SocketConnected;
        public event ServerSocketEventHandler SocketDisconnected;

        public CommTCPServer(bool serverhandle, int watchport)
        {
            _ServerCode = ServerCodeCreator++;

            _Type = CommunicationType.TCPServer;
            _Name = "TCPMultiServer";

            _ServerHandle = serverhandle;

            _Sockets = new ConcurrentDictionary<int, CommTCPSocket>();
            _SocketsReadDate = new ConcurrentQueue<ReceivedPack>();

            ReconnectInterval = 2000;
            ReceiveInterval = 10;
            SendInterval = 10;
            HandlerInterval = 10;

            SetWatchPort(watchport);
            Start();
        }

        public void SetMaximumClient(int max)
        {
            _MaximumClient = max;
        }

        public void SetWatchPort(int watchPort)
        {
            if (WatchPort != watchPort)
            {
                _WatchPort = watchPort;
                ServerInfoChanged = true;
            }
        }

        public void SetClientEndPoint(IPAddress clientIp, int clientPort)
        {
            if (!ClientAddress.Equals(clientIp) || ClientPort != clientPort)
            {
                _ClientAddress = clientIp;
                _ClientPort = clientPort;
                ClientInfoChanged = true;
            }
        }

        protected override void ExtendTimerTick()
        {
            if (ServerInfoChanged)
            {
                Disconnect();
            }
            else
            {
                if (ClientInfoChanged)
                {
                    ClientInfoChanged = false;
                    foreach (CommTCPSocket socket in Sockets.Values)
                    {
                        if (!ClientAddress.Equals(IPAddress.Any))
                        {
                            if (!ClientAddress.Equals(socket.RemoteIPAddress))
                            {
                                socket.Dispose();
                            }
                        }
                        else
                        {
                            if (ClientPort > 0)
                            {
                                if (ClientPort != socket.RemotePort)
                                {
                                    socket.Dispose();
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void Connect()
        {
            if (!Connected)
            {
                ServerInfoChanged = false;
                ClientInfoChanged = false;
                ServerAdd();
                mConnected = true;
            }
        }

        protected override void Disconnect()
        {
            if (Connected)
            {
                ServerRemove();
                CloseAll();

                mConnected = false;
            }
        }

        protected override ReceivedPack DeviceReceive()
        {
            ReceivedPack output = null;
            if (ServerHandle)
            {
                SocketsReadDate.TryDequeue(out output);
            }
            return null;
        }

        protected override void DeviceSend(byte[] buff)
        {
            if (ServerHandle)
            {
                foreach (CommTCPSocket client in Sockets.Values)
                {
                    client.SocketSend(buff);
                }
            }
        }

        protected override void StopDevice()
        {
            ServerRemove();
            Disconnect();
        }

        private void CloseAll()
        {
            foreach (CommTCPSocket client in Sockets.Values)
            {
                client.Dispose();
            }
        }

        private bool AddClient(Socket sok)
        {
            if (Sockets.Count >= MaximumClient) return false;

            IPEndPoint remote = sok.RemoteEndPoint as IPEndPoint;
            if ((ClientAddress.Equals(IPAddress.Any) || ClientAddress.Equals(remote.Address))
                    && (ClientPort <= 0 || ClientPort == remote.Port))
            {
                CommTCPSocket comm = new CommTCPSocket(sok, ServerHandle);

                if (!ServerHandle)
                {
                    comm.SetProtocol(Protocol.GetCopy());
                }

                if (Sockets.TryAdd(comm.SocketCode, comm))
                {
                    comm.ConnectionChanged += new CommunicationEventHandler(comm_ConnectionChanged);

                    if (ServerHandle)
                    {
                        comm.SocketDataReceived += new ServerSocketDataEventHandler(comm_SocketDataReceived);
                    }

                    if (SocketConnected != null) SocketConnected(this, new ServerSocketEventArgs(comm));
                    if (SocketChanged != null) SocketChanged(this, new ServerSocketEventArgs(comm));
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        void comm_SocketDataReceived(object sender, ServerSocketDataEventArgs e)
        {
            if (ServerHandle)
            {
                SocketsReadDate.Enqueue(e.Pack);
            }
        }

        private void comm_ConnectionChanged(object sender, CommunicationEventArgs e)
        {
            if (!(e.Communication is CommTCPSocket)) return;

            int code = (e.Communication as CommTCPSocket).SocketCode;
            if (!e.Communication.Connected)
            {
                CommTCPSocket comm = null;
                if (Sockets.TryRemove(code, out comm))
                {
                    if (SocketDisconnected != null) SocketDisconnected(this, new ServerSocketEventArgs(comm));
                    if (SocketChanged != null) SocketChanged(this, new ServerSocketEventArgs(comm));
                    comm.Dispose();
                }
            }
        }

        private void ServerAdd()
        {
            CurrentPort = WatchPort;

            if (CurrentPort > 0)
            {
                TCPServer server = TCPServer.Create(CurrentPort);

                if (server.AddServerCode(ServerCode))
                {
                    server.ClientConnected += new SocketEventHandler(CommTCPServer_ClientConnected);
                }
            }
        }

        private void ServerRemove()
        {
            if (CurrentPort > 0)
            {
                TCPServer server = TCPServer.Get(CurrentPort);

                if (server != null)
                {
                    if (server.RemoveServerCode(ServerCode))
                    {
                        server.ClientConnected -= CommTCPServer_ClientConnected;
                    }
                }
                CurrentPort = 0;
            }
        }

        private void CommTCPServer_ClientConnected(object sender, SocketEventArgs e)
        {
            if (!e.Handler)
            {
                if (AddClient(e.Socket))
                {
                    e.Handler = true;
                }
            }
        }
    }

    internal class TCPServer
    {
        #region TCP Server
        private static Dictionary<int, TCPServer> _TCPServers = new Dictionary<int, TCPServer>();
        private static Dictionary<int, TCPServer> TCPServers { get { return _TCPServers; } }

        public static TCPServer Get(int port)
        {
            if (TCPServers.ContainsKey(port))
            {
                return TCPServers[port];
            }
            return null;
        }

        public static TCPServer Create(int port)
        {
            lock (TCPServers)
            {
                if (!TCPServers.ContainsKey(port))
                {
                    TCPServer server = new TCPServer(port);
                    TCPServers.Add(port, server);
                }
            }
            return TCPServers[port];
        }
        #endregion

        private int _Port;
        private List<int> _Servers;
        private Dictionary<IPAddress, WatchServer> _WatchServers;
        private Thread SearchIPThread;
        private bool IsSearching = false;

        private List<int> Servers { get { return _Servers; } }

        public int Port { get { return _Port; } }
        public Dictionary<IPAddress, WatchServer> WatchServers { get { return _WatchServers; } }

        public event SocketEventHandler ClientConnected;

        public TCPServer(int port)
        {
            _Port = port;
            _Servers = new List<int>();
            _WatchServers = new Dictionary<IPAddress, WatchServer>();

            StartSearchIP();
        }

        public bool AddServerCode(int serverCode)
        {
            if (!Servers.Contains(serverCode))
            {
                Servers.Add(serverCode);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveServerCode(int serverCode)
        {
            if (Servers.Contains(serverCode))
            {
                Servers.Remove(serverCode);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void StartSearchIP()
        {
            SearchIPThread = new Thread(SearchIP);
            SearchIPThread.IsBackground = true;
            IsSearching = true;
            SearchIPThread.Start();
        }

        private void SearchIP()
        {
            while (IsSearching)
            {
                if (Servers.Count > 0)
                {
                    IPHostEntry ips = Dns.GetHostEntry(Dns.GetHostName());
                    List<IPAddress> RemoveList = new List<IPAddress>();

                    foreach (IPAddress ip in WatchServers.Keys)
                    {
                        if (!ips.AddressList.Contains(ip))
                        {
                            RemoveList.Add(ip);
                        }
                    }

                    foreach (IPAddress ip in RemoveList)
                    {
                        WatchServers[ip].StopWatch();
                        WatchServers.Remove(ip);
                    }

                    foreach (IPAddress ip in ips.AddressList)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            if (!WatchServers.ContainsKey(ip))
                            {
                                WatchServers[ip] = new WatchServer(ip, Port);
                                WatchServers[ip].ClientConnected += new SocketEventHandler(TCPServer_ClientConnected);
                            }
                        }
                    }
                }
                else
                {
                    if (WatchServers.Count > 0)
                    {
                        foreach (WatchServer server in WatchServers.Values)
                        {
                            server.StopWatch();
                        }
                        WatchServers.Clear();
                    }
                }

                Thread.Sleep(5000);
            }
        }

        void TCPServer_ClientConnected(object sender, SocketEventArgs e)
        {
            if (ClientConnected != null) ClientConnected(this, e);
        }
    }

    internal class WatchServer
    {
        private int _Port = 0;
        private IPAddress _IP = null;
        private Thread WatchThread;
        private Thread HandlerThread;
        private bool IsWatching = false;
        private Socket sokWatch = null;
        private ConcurrentQueue<Socket> FailureSockets = new ConcurrentQueue<Socket>();

        public int Port { get { return _Port; } }
        public IPAddress IP { get { return _IP; } }

        public event SocketEventHandler ClientConnected;

        public WatchServer(IPAddress ip, int port)
        {
            _IP = ip;
            _Port = port;
            StartWatch();
        }

        private void StartWatch()
        {
            WatchThread = new Thread(Watch);
            WatchThread.IsBackground = true;
            IsWatching = true;
            WatchThread.Start();

            HandlerThread = new Thread(Handler);
            HandlerThread.IsBackground = true;
            HandlerThread.Start();
        }

        public void StopWatch()
        {
            IsWatching = false;
            if (sokWatch != null)
            {
                sokWatch.Close();
            }
        }

        private void Handler()
        {
            while (IsWatching)
            {
                Thread.Sleep(1000);

                while (!FailureSockets.IsEmpty)
                {
                    Socket sok = null;

                    if (FailureSockets.TryDequeue(out sok))
                    {
                        try
                        {
                            sok.Send(new byte[] { 0x2B });
                            Thread.Sleep(200);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.Message + " @" + this.GetType().FullName + "- " + "Handler");
                        }

                        try
                        {
                            sok.Shutdown(SocketShutdown.Both);
                            Thread.Sleep(50);
                            sok.Close();
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.Message + " @" + this.GetType().FullName + "- Handler");
                        }
                    }
                }
            }
        }

        private void Watch()
        {
            sokWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endpoint = new IPEndPoint(IP, Port);
            sokWatch.Bind(endpoint);
            sokWatch.Listen(10);

            while (IsWatching)
            {
                try
                {
                    Socket sokMsg = sokWatch.Accept();
                    SocketEventArgs ee = new SocketEventArgs(sokMsg);
                    if (ClientConnected != null) ClientConnected(this, ee);

                    if (!ee.Handler)
                    {
                        FailureSockets.Enqueue(sokMsg);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message + " @" + this.GetType().FullName + "- Watch");
                }
            }
            if (sokWatch != null) sokWatch.Close();
        }
    }
}
