using SuperPort;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SuperPortLibrary
{
    public class CommTCPClient : Communication
    {
        private Socket _Client = null;
        private IPEndPoint _ServerEndPoint = new IPEndPoint(IPAddress.None, 0);
        private byte[] ReadBuff = new byte[1024 * 1024];
        private IPEndPoint _RemoteEndPoint = new IPEndPoint(IPAddress.None, 0);
        private IPEndPoint _LocalEndPoint = new IPEndPoint(IPAddress.None, 0);
        private int _ClientPort = 0;
        private Thread SocketReadThread;
        private bool SocketReadRunning = false;
        private ConcurrentQueue<byte[]> _SocketReadDate;

        private bool ServerInfoChanged = false;
        private Socket Client { get { return _Client; } }
        private ConcurrentQueue<byte[]> SocketReadDate { get { return _SocketReadDate; } }
        private IPEndPoint ServerEndPoint { get { return _ServerEndPoint; } }

        public IPAddress ServerIP { get { return ServerEndPoint.Address; } }
        public int ServerPort { get { return ServerEndPoint.Port; } }
        public IPEndPoint LocalEndPoint { get { return _LocalEndPoint; } }
        public IPAddress LocalIPAddress { get { return _LocalEndPoint.Address; } }
        public int LocalPort { get { return _LocalEndPoint.Port; } }
        public IPEndPoint RemoteEndPoint { get { return _RemoteEndPoint; } }
        public IPAddress RemoteIPAddress { get { return _RemoteEndPoint.Address; } }
        public int RemotePort { get { return _RemoteEndPoint.Port; } }
        public int ClientPort { get { return _ClientPort; } }

        public CommTCPClient()
        {
            _Type = CommunicationType.TCPClient;
            _Name = "TCPClient";

            ReconnectInterval = 2000;
            ReceiveInterval = 10;
            SendInterval = 10;
            HandlerInterval = 10;

            SocketReadStart();
            Start();
        }

        public void SetServerEndPoint(IPAddress ip, int port)
        {
            if (!ServerIP.Equals(ip) || ServerPort != port)
            {
                _ServerEndPoint = new IPEndPoint(ip, port);
                ServerInfoChanged = true;
            }
        }

        public void SetLocalPort(int port)
        {
            if (LocalPort != port)
            {
                _ClientPort = port;
                ServerInfoChanged = true;
            }
        }

        protected override void ExtendTimerTick()
        {
            if (Connected)
            {
                if (ServerInfoChanged)
                {
                    Disconnect();
                }
            }
        }

        protected override void Connect()
        {
            if (!Connected)
            {
                ServerInfoChanged = false;

                if (ServerEndPoint.Address.Equals(IPAddress.None)
                    || ServerEndPoint.Address.Equals(IPAddress.Any))
                {

                }
                else
                {
                    try
                    {
                        ServerInfoChanged = false;
                        _Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        Client.Bind(new IPEndPoint(IPAddress.Any, ClientPort));

#if false
                        // 抛出异常连接方式
                        Client.Connect(ServerEndPoint);
#else
                        // 不抛出异常连接方式
                        var e = new SocketAsyncEventArgs()
                        {
                            RemoteEndPoint = ServerEndPoint,
                        };
                        Client?.ConnectAsync(e);

                        bool isTimeout = !SpinWait.SpinUntil(() => Client?.Connected ?? false, 3000);

                        if (isTimeout)
                        {
                            Socket.CancelConnectAsync(e);
                        }
                        if (!(Client?.Connected ?? false))
                        {
                            Disconnect();
                            Logger.Error($"Cant connect to server `{ServerEndPoint.Address}`.");
                            return;
                        }
#endif

                        _RemoteEndPoint = Client.RemoteEndPoint as IPEndPoint;
                        _LocalEndPoint = Client.LocalEndPoint as IPEndPoint;

                        mConnected = true;
                    }
                    catch (Exception ex)
                    {
                        Disconnect();
                        Logger.Error(ex.Message + " @" + this.GetType().FullName + "- " + "Connect");
                    }
                }
            }
            else
            {
                mConnected = true;
            }
        }

        protected override void Disconnect()
        {
            if (Connected)
            {
                try
                {
                    Client?.Shutdown(SocketShutdown.Both);
                    Thread.Sleep(10);
                    Client?.Close();
                    mConnected = false;
                    _Client = null;
                    _RemoteEndPoint = new IPEndPoint(IPAddress.None, 0);
                    _LocalEndPoint = new IPEndPoint(IPAddress.None, 0);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message + " @" + this.GetType().FullName + "- " + "DisConnect");
                }
            }
            else
            {
                if (Client != null)
                {
                    Client.Close();
                    _Client = null;
                }

                mConnected = false;
            }
        }

        protected override ReceivedPack DeviceReceive()
        {
            if (Connected && Client.Connected)
            {
                byte[] buff = null;
                if (!SocketReadDate.IsEmpty && SocketReadDate.TryDequeue(out buff))
                {
                    return new ReceivedPack()
                    {
                        Datas = buff,
                        Time = DateTime.Now,
                        Source = RemoteEndPoint
                    };
                }
            }
            else
            {
                Disconnect();
                Logger.Error("Disconnect detected!!! @" + this.GetType().FullName + "- " + "DeviceSend");
            }
            return null;
        }

        protected override void DeviceSend(byte[] buff)
        {
            if (Connected && Client.Connected)
            {
                try
                {
                    Client.Send(buff);
                }
                catch (Exception ex)
                {
                    Disconnect();
                    Logger.Error(ex.Message + " @" + this.GetType().FullName + "- " + "DeviceSend");
                }
            }
            else
            {
                Disconnect();
                Logger.Error("Disconnect detected!!! @" + this.GetType().FullName + "- " + "DeviceSend");
            }
        }

        protected override void StopDevice()
        {
            Disconnect();
            SocketReadRunning = false;
        }

        private void SocketReadStart()
        {
            _SocketReadDate = new ConcurrentQueue<byte[]>();

            SocketReadThread = new Thread(SocketRead);
            SocketReadThread.IsBackground = true;
            SocketReadThread.Name = "SocketClient-ReadAsync";
            SocketReadRunning = true;
            SocketReadThread.Start();
        }

        private void SocketRead()
        {
            int failcount = 0;
            while (SocketReadRunning)
            {
                if (Client != null && Connected)
                {
                    try
                    {
                        int length = Client.Receive(ReadBuff);

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
}
