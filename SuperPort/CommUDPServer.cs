using SuperPort;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SuperPortLibrary
{
    public class CommUDPServer : Communication
    {
        private Socket _Client = null;
        private bool _AutoLocalIP = true;
        private bool _Broadcast = false;
        private IPAddress _LocalIP = IPAddress.Loopback;
        private int _LocalPort = 0;
        private IPAddress _TransmitIP = IPAddress.Loopback;
        private int _TransmitPort = 0;
        private byte[] ReadBuff = new byte[1024 * 1024];
        private Thread SocketReadThread;
        private bool SocketReadRunning = false;
        private ConcurrentQueue<ReceivedPack> _SocketReadDate;

        private bool ServerInfoChanged = false;
        private Socket Client { get { return _Client; } }
        private ConcurrentQueue<ReceivedPack> SocketReadDate { get { return _SocketReadDate; } }

        public bool AutoLocalIP { get { return _AutoLocalIP; } }
        public bool Broadcast { get { return _Broadcast; } }
        public IPAddress LocalIP { get { return AutoLocalIP ? IPAddress.Any : _LocalIP; } }
        public int LocalPort { get { return _LocalPort; } }
        public IPAddress TransmitIP { get { return Broadcast ? IPAddress.Broadcast : _TransmitIP; } }
        public int TransmitPort { get { return _TransmitPort; } }

        public CommUDPServer()
        {
            _Type = CommunicationType.UDPServer;
            _Name = "UDPServer";

            ReconnectInterval = 2000;
            ReceiveInterval = 10;
            SendInterval = 10;
            HandlerInterval = 10;

            SocketReadStart();
            Start();
        }

        public void SetTransmitIP(IPAddress ip)
        {
            if (!TransmitIP.Equals(ip))
            {
                _TransmitIP = ip;
            }
        }

        public void SetTransmitPort(int port)
        {
            if (TransmitPort != port)
            {
                _TransmitPort = port;
            }
        }

        public void SetBroadcast(bool flag)
        {
            if (Broadcast != flag)
            {
                _Broadcast = flag;
            }
        }

        public void SetLocalIP(IPAddress ip)
        {
            if (!LocalIP.Equals(ip))
            {
                _LocalIP = ip;
                ServerInfoChanged = true;
            }
        }

        public void SetLocalPort(int port)
        {
            if (LocalPort != port)
            {
                _LocalPort = port;
                ServerInfoChanged = true;
            }
        }

        public void SetAutoLocalIP(bool flag)
        {
            if (AutoLocalIP != flag)
            {
                _AutoLocalIP = flag;
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

                if (TransmitPort == 0 || LocalPort == 0)
                {

                }
                else
                {
                    try
                    {
                        ServerInfoChanged = false;
                        _Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        Client.Bind(new IPEndPoint(LocalIP, LocalPort));

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
                    Client.Shutdown(SocketShutdown.Both);
                    Thread.Sleep(10);
                    Client.Close();
                    mConnected = false;
                    _Client = null;
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
            if (Connected)
            {
                ReceivedPack pack = null;
                if (!SocketReadDate.IsEmpty && SocketReadDate.TryDequeue(out pack))
                {
                    return pack;
                }
            }
            return null;
        }

        protected override void DeviceSend(byte[] buff)
        {
            if (Connected && TransmitPort > 0)
            {
                try
                {
                    Client.SendTo(buff, (new IPEndPoint(TransmitIP, TransmitPort)) as EndPoint);
                        }
                catch (Exception ex)
                {
                    //Disconnect();
                    Logger.Error(ex.Message + " @" + this.GetType().FullName + "- " + "DeviceSend");
                }
            }
        }

        protected override void StopDevice()
        {
            Disconnect();
            SocketReadRunning = false;
        }

        private void SocketReadStart()
        {
            _SocketReadDate = new ConcurrentQueue<ReceivedPack>();

            SocketReadThread = new Thread(SocketRead);
            SocketReadThread.IsBackground = true;
            SocketReadThread.Name = "SocketClient-ReadAsync";
            SocketReadRunning = true;
            SocketReadThread.Start();
        }

        private void SocketRead()
        {
            while (SocketReadRunning)
            {
                if (Client != null && Connected)
                {
                    try
                    {
                        EndPoint ep = new IPEndPoint(IPAddress.Any, 9999);

                        int length = Client.ReceiveFrom(ReadBuff, ref ep);

                        if (length > 0)
                        {
                            byte[] buff = new byte[length];
                            Buffer.BlockCopy(ReadBuff, 0, buff, 0, length);

                            SocketReadDate.Enqueue(new ReceivedPack()
                            {
                                Datas = buff,
                                Time = DateTime.Now,
                                Source = ep as IPEndPoint
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        //Disconnect();
                        Logger.Error(ex.Message + " @" + this.GetType().FullName + "- " + "SocketRead");
                    }
                    Thread.Sleep(ReceiveInterval);
                }
                else
                {
                    Thread.Sleep(ReconnectInterval);
                }
            }
        }
    }
}
