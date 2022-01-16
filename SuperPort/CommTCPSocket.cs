using SuperPort;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SuperPortLibrary
{
    public class CommTCPSocket : Communication
    {
        private static int SocketCodeCreator = 1;

        private int _SocketCode;
        private Socket _Socket;
        private IPEndPoint _RemoteEndPoint = null;
        private IPEndPoint _LocalEndPoint = null;
        private bool _ServerHandle = true;
        private byte[] ReadBuff = new byte[1024 * 1024];
        private Thread SocketReadThread;
        private bool SocketReadRunning = false;
        private ConcurrentQueue<byte[]> _SocketReadDate;

        private Socket Socket { get { return _Socket; } }
        private bool Activated { get; set; }
        private ConcurrentQueue<byte[]> SocketReadDate { get { return _SocketReadDate; } }

        public int SocketCode { get { return _SocketCode; } }
        public IPEndPoint LocalEndPoint { get { return _LocalEndPoint; } }
        public IPAddress LocalIPAddress { get { return LocalEndPoint.Address; } }
        public int LocalPort { get { return LocalEndPoint.Port; } }
        public IPEndPoint RemoteEndPoint { get { return _RemoteEndPoint; } }
        public IPAddress RemoteIPAddress { get { return RemoteEndPoint.Address; } }
        public int RemotePort { get { return RemoteEndPoint.Port; } }
        public bool ServerHandle { get { return _ServerHandle; } }

        internal event ServerSocketDataEventHandler SocketDataReceived;

        internal CommTCPSocket(Socket sok, bool serverhandle)
        {
            _SocketCode = SocketCodeCreator++;
            _Type = CommunicationType.TCPServer;
            _Name = "TCPServer";
            _ServerHandle = serverhandle;

            _Socket = sok;
            _RemoteEndPoint = Socket.RemoteEndPoint as IPEndPoint;
            _LocalEndPoint = Socket.LocalEndPoint as IPEndPoint;
            Activated = true;
            AutoConnect = false;

            ReconnectInterval = 2000;
            ReceiveInterval = 10;
            SendInterval = 10;
            HandlerInterval = 10;

            mConnected = true;
            SocketReadStart();
            Start();
        }

        protected override void Connect()
        {
            
        }

        protected override void Disconnect()
        {
            mConnected = false;
        }

        protected override void ExtendTimerTick()
        {

        }

        protected override void StopDevice()
        {
            CloseSocket();
        }

        protected override ReceivedPack DeviceReceive()
        {
            if (Connected)
            {
                byte[] buff = null;
                if (!SocketReadDate.IsEmpty && SocketReadDate.TryDequeue(out buff))
                {
                    ReceivedPack pack = new ReceivedPack()
                    {
                        Datas = buff,
                        Time = DateTime.Now,
                        Source = RemoteEndPoint
                    };

                    if (ServerHandle)
                    {
                        SocketDataReceived?.Invoke(this, new ServerSocketDataEventArgs(this, pack));
                        return null;
                    }
                    else
                    {
                        return pack;
                    }
                }
            }
            return null;
        }

        protected override void DeviceSend(byte[] buff)
        {
            if (!ServerHandle)
            {
                SocketSend(buff);
            }
        }

        internal void SocketSend(byte[] buff)
        {
            if (Connected)
            {
                try
                {
                    Socket.Send(buff);
                }
                catch (Exception ex)
                {
                    Disconnect();
                    Logger.Error(ex.Message + " @" + this.GetType().FullName + "- " + "SocketSend");
                }
            }
        }

        private void CloseSocket()
        {
            if (Activated)
            {
                mConnected = false;
                Activated = false;
                SocketReadRunning = false;

                try
                {
                    Socket.Shutdown(SocketShutdown.Both);
                    Thread.Sleep(10);
                    Socket.Disconnect(false);
                    Socket.Close();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message + " @" + this.GetType().FullName + "- CloseSocket");
                }
            }
        }

        private void SocketReadStart()
        {
            _SocketReadDate = new ConcurrentQueue<byte[]>();

            SocketReadThread = new Thread(SocketRead);
            SocketReadThread.IsBackground = true;
            SocketReadThread.Name = "SocketServer-ReadAsync";
            SocketReadRunning = true;
            SocketReadThread.Start();
        }

        private void SocketRead()
        {
            int failcount = 0;
            while (SocketReadRunning)
            {
                if (Socket != null && Connected)
                {
                    try
                    {
                        int length = Socket.Receive(ReadBuff);

                        if (length > 0)
                        {
                            byte[] buff = new byte[length];
                            Buffer.BlockCopy(ReadBuff, 0, buff, 0, length);

                            SocketReadDate.Enqueue(buff);
                            failcount = 0;
                        }
                        else
                        {
                            if (failcount++ > 30)
                            {
                                failcount = 0;
                                Disconnect();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Disconnect();
                        Logger.Error(ex.Message + " @" + this.GetType().FullName + "- " + "SocketRead");
                    }
                    Thread.Sleep(ReceiveInterval);
                }
                else
                {
                    failcount = 0;
                    Thread.Sleep(ReconnectInterval);
                }
            }
        }
    }

    public delegate void ServerSocketEventHandler(object sender, ServerSocketEventArgs e);

    public class ServerSocketEventArgs : EventArgs
    {
        private CommTCPSocket _Communication;

        public CommTCPSocket Communication { get { return _Communication; } }
        public bool Handler { get; set; }

        internal ServerSocketEventArgs(CommTCPSocket comm)
        {
            _Communication = comm;
            Handler = false;
        }
    }

    public delegate void ServerSocketDataEventHandler(object sender, ServerSocketDataEventArgs e);

    public class ServerSocketDataEventArgs : EventArgs
    {
        private CommTCPSocket _Communication;
        private ReceivedPack _Pack;

        public bool Handler { get; set; }
        public CommTCPSocket Communication { get { return _Communication; } }
        public ReceivedPack Pack { get { return _Pack; } }

        internal ServerSocketDataEventArgs(CommTCPSocket comm, ReceivedPack pack)
        {
            _Communication = comm;
            _Pack = pack;
            Handler = false;
        }
    }

    internal delegate void SocketEventHandler(object sender, SocketEventArgs e);

    internal class SocketEventArgs : EventArgs
    {
        private Socket _Socket;

        public Socket Socket { get { return _Socket; } }
        public bool Handler { get; set; }

        internal SocketEventArgs(Socket sok)
        {
            Handler = false;
            _Socket = sok;
        }
    }
}
