using System;
using System.Threading;

namespace SuperPortLibrary
{
    public abstract class Communication
    {        
        private Thread ReceiveThread, SendThread, HandlerThread;
        private bool IsReceiving, IsSending, IsHandling;

        protected CommunicationType _Type;
        protected string _Name = "";
        protected Protocol _Protocol;
        protected int RandomInterval = 0;
        protected double _CommunicationLevel = 0;
        protected bool AutoConnect = true;
        private bool _Connected = false;
        private bool _Communicating = false;
        private bool _Pausing = false;
        private bool _Disposed = false;

        protected int ReconnectInterval = 1000;
        protected int ReceiveInterval = 10;
        protected int SendInterval = 10;
        protected int HandlerInterval = 10;
        protected int TickInterval = 100;
        protected int ExtendInterval = 1500;
        protected int AutoSendInterval = 200;
        protected int CommunicationInterval = 10000;

        private CommunicationEventArgs EventArg;
        public event ProtocolDataEventHandler DataReceived, DataVarifyFailed, DataCompileError;
        public event ProtocolDataEventHandler DataTransmited, DataAutoTransmited;
        public event CommunicationEventHandler ConnectionChanged;
        public event CommunicationEventHandler CommunicatedChanged;
        public event CommunicationEventHandler CommunicationLevelChanged;
        public event CommunicationEventHandler PausingChanged;

        public Communication()
        {
            SetProtocol(new ProtocolNull());

            EventArg = new CommunicationEventArgs(this);
            RandomInterval = (new Random()).Next(2000);
        }

        public string Name { get { return _Name; } }
        public Protocol Protocol { get { return _Protocol; } }
        public bool Connected { get { return _Connected; } }
        public bool Communicating { get { return _Communicating; } }
        public double CommunicationLevel { get { return _CommunicationLevel; } }
        public CommunicationType Type { get { return _Type; } }
        public bool Pausing { get { return _Pausing; } }
        public bool Disposed { get { return _Disposed; } }

        protected bool mConnected 
        {
            set
            {
                if (_Connected != value)
                {
                    if (Communicating && !value)
                    {
                        mCommunicating = false;
                    }

                    _Connected = value;
                    if (ConnectionChanged != null) ConnectionChanged(this, EventArg);
                }
            }
            get 
            { 
                return _Connected; 
            } 
        }

        protected bool mCommunicating
        {
            set
            {
                if (_Communicating != value)
                {
                    _Communicating = value;
                    if (CommunicatedChanged != null) CommunicatedChanged(this, EventArg);
                }
            }
            get
            {
                return _Communicating;
            }
        }

        protected double mCommunicationLevel 
        { 
            get
            { 
                return _CommunicationLevel;
            }
            set
            {
                if (_CommunicationLevel != value)
                {
                    _CommunicationLevel = value;
                    if (CommunicationLevelChanged != null) CommunicationLevelChanged(this, EventArg);
                }
            }
        }

        protected bool mPausing
        {
            get { return _Pausing; }
            set
            {
                if (_Pausing != value)
                {
                    _Pausing = value;
                    if (PausingChanged != null)
                    {
                        PausingChanged(this, EventArg);
                    }
                }
            }
        }

        protected void Start()
        {
            StartReceive();
            StartSend();
            StartHandler();
        }

        public void SetProtocol(Protocol p)
        {
            if (p != null)
            {
                _Protocol = p;
            }
            else
            {
                if (Protocol.IdentityCode != -1)
                    _Protocol = new ProtocolNull();
            }
        }

        public void Dispose()
        {
            _Disposed = true;
            SetProtocol(null);

            AutoConnect = false;
            IsReceiving = false;
            IsSending = false;
            IsHandling = false;

            StopDevice();
        }

        private void StartReceive()
        {
            ReceiveThread = new Thread(ReceiveLoop);
            ReceiveThread.Name = "Communication-Receive-" + Name;
            ReceiveThread.IsBackground = true;
            IsReceiving = true;
            ReceiveThread.Start();
        }

        private void StartSend()
        {
            SendThread = new Thread(SendLoop);
            SendThread.Name = "Communication-Send-" + Name;
            SendThread.IsBackground = true;
            IsSending = true;
            SendThread.Start();
        }

        private void StartHandler()
        {
            HandlerThread = new Thread(HandlerLoop);
            HandlerThread.Name = "Communication-Handler-" + Name;
            HandlerThread.IsBackground = true;
            IsHandling = true;
            HandlerThread.Start();
        }

        private void ReceiveLoop()
        {
            int TickCount = 0, ExtendCount = 0;
            //Thread.Sleep(3000 + RandomInterval);
            while (IsReceiving)
            {
                if (mConnected)
                {
                    ReceivedPack pack = DeviceReceive();

                    if (!Pausing)
                    {
                        if (pack != null && pack.Datas.Length > 0)
                        {
                            foreach (byte dt in pack.Datas)
                            {
                                ProtocolData pd1 = Protocol.ReadData(dt);
                                if (pd1 != null)
                                {
                                    pd1.Source = pack.Source;
                                    pd1.Time = pack.Time;
                                }
                            }

                            ProtocolData pd2 = Protocol.ReadData();
                            if (pd2 != null)
                            {
                                pd2.Source = pack.Source;
                                pd2.Time = pack.Time;
                            }
                        }
                    }

                    if ((TickCount += ReceiveInterval) >= TickInterval)
                    {
                        TickCount = 0;
                        Protocol.ReadTimerTick();
                    }

                    if ((ExtendCount += ReceiveInterval) >= ExtendInterval)
                    {
                        ExtendCount = 0;
                        ExtendTimerTick();
                    }

                    Thread.Sleep(ReceiveInterval);
                }
                else
                {
                    if (AutoConnect) Connect();
                    Thread.Sleep(ReconnectInterval);
                }
            }
        }

        private void SendLoop()
        {
            int SendCount = 0;
            int TickCount = 0;
            //Thread.Sleep(3000 + RandomInterval);
            while (IsSending)
            {
                ProtocolData orderbuff = Protocol.GetWriteBuffer();
                if (!Pausing && orderbuff != null && Connected)
                {
                    byte[] bytebuff = Protocol.GetTransmitData(orderbuff);
                    if (bytebuff != null && bytebuff.Length > 0) DeviceSend(bytebuff);

                    if (DataTransmited != null)
                    {
                        DataTransmited(this, new ProtocolDataEventArgs(orderbuff));
                    }
                }

                if ((SendCount += SendInterval) >= AutoSendInterval)
                {
                    SendCount = 0;

                    ProtocolData autobuff = Protocol.GetAutoWriteBuffer();
                    if (!Pausing && autobuff != null && Connected)
                    {
                        byte[] bytebuff = Protocol.GetTransmitData(autobuff);
                        if (bytebuff != null && bytebuff.Length > 0) DeviceSend(bytebuff);

                        if (DataAutoTransmited != null)
                        {
                            DataAutoTransmited(this, new ProtocolDataEventArgs(autobuff));
                        }
                    }
                }

                if ((TickCount += SendInterval) >= TickInterval)
                {
                    TickCount = 0;
                    Protocol.WriteTimerTick();
                }
                
                Thread.Sleep(SendInterval);
            }
        }

        private void HandlerLoop()
        {
            int CommunicationCount = CommunicationInterval;
            while (IsHandling)
            {
                ProtocolData data = Protocol.GetReadBuffer();

                if (data != null)
                {
                    if (data.Varified)
                    {
                        data.CompileData();

                        if (data.ApplicationData != null)
                        {
                            data.ApplicationChecked();

                            if (DataReceived != null)
                            {
                                ProtocolDataEventArgs e = new ProtocolDataEventArgs(data);
                                DataReceived(this, e);

                                if (e.Handler)
                                {
                                    mCommunicating = true;
                                    CommunicationCount = 0;
                                }
                            }
                        }
                        else
                        {
                            OnReadDataError();
                            DataCompileError?.Invoke(this, new ProtocolDataEventArgs(data));

                            data = Protocol.GetErrorWriteBuffer();
                            if (data != null && Connected)
                            {
                                DataTransmit(data);
                            }
                        }
                    }
                    else
                    {
                        OnVerifyFailed();
                        DataVarifyFailed?.Invoke(this, new ProtocolDataEventArgs(data));

                        data = Protocol.GetFailWriteBuffer();
                        if (data != null && Connected)
                        {
                            DataTransmit(data);
                        }
                    }
                }
                else
                {
                    if (mCommunicating)
                    {
                        if (CommunicationCount < CommunicationInterval)
                        {
                            CommunicationCount += HandlerInterval;
                        }
                        else
                        {
                            CommunicationCount = CommunicationInterval;
                            mCommunicating = false;
                        }
                    }
                    mCommunicationLevel = (1 - (CommunicationCount * 1.0 / CommunicationInterval)) * 100;

                    Thread.Sleep(HandlerInterval);
                }
            }
        }

        public void DataTransmit(ApplicationData data)
        {
            Protocol.WriteData(data);
        }

        public void DataTransmit(ProtocolData data)
        {
            Protocol.WriteData(data);
        }

        public void DiscardWriteBuffers()
        {
            Protocol.ClearWriteBuffer();
        }

        public void DiscardReadBuffers()
        {
            Protocol.ClearReadBuffer();
        }

        public void Suspend()
        {
            mPausing = true;
        }

        public void Resume()
        {
            mPausing = false;
        }

        protected abstract void Connect();
        protected abstract void Disconnect();
        protected abstract void ExtendTimerTick();
        protected abstract void StopDevice();
        protected abstract ReceivedPack DeviceReceive();
        protected abstract void DeviceSend(byte[] buff);

        protected virtual void OnVerifyFailed() { }
        protected virtual void OnReadDataError() { }
    }
}
