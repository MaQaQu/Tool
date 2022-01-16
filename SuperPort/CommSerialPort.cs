using SuperPort;
using System;
using System.Diagnostics;
using System.IO.Ports;

namespace SuperPortLibrary
{
    public class CommSerialPort : Communication
    {
        private SerialPort _Comport;
        private int _PortNumber = 0;
        private int _PortBand = 9600;
        private Parity _PortParity = Parity.None;
        private int _PortDataBits = 8;
        private StopBits _PortStopBits = StopBits.One;
        private string _PortName = "COM0";
        private bool _RtsEnable = false;
        private bool _DtrEnable = false;
        private bool ParameterChanged = false;

        private SerialPort Comport { get { return _Comport; } }
        public int PortNumber { get { return _PortNumber; } }
        public int PortBand { get { return _PortBand; } }
        public Parity PortParity { get { return _PortParity; } }
        public int PortDataBits { get { return _PortDataBits; } }
        public StopBits PortStopBits { get { return _PortStopBits; } }
        public string PortName { get { return _PortName; } }
        public bool RtsEnable { get { return _RtsEnable; } }
        public bool DtrEnable { get { return _DtrEnable; } }

        public CommSerialPort()
        {
            _Type = CommunicationType.SerialPort;
            _Name = "SerialPort";

            ReconnectInterval = 200;
            ReceiveInterval = 10;
            SendInterval = 10;
            HandlerInterval = 10;

            _Comport = new SerialPort(PortName, PortBand, PortParity, PortDataBits, PortStopBits);
            Comport.RtsEnable = RtsEnable;
            Comport.DtrEnable = DtrEnable;

            Start();
        }

        public void SetComport(int port)
        {
            SetComport(port, PortBand);
        }

        public void SetComport(int port, int band)
        {
            SetComport(port, band, PortParity, PortDataBits, PortStopBits);
        }

        public void SetComport(int port, int band, Parity parity, int databits, StopBits stopbits)
        {
            if (PortNumber != port)
            {
                if (port >= 0)
                {
                    _PortNumber = port;
                    _PortName = string.Format("COM{0}", PortNumber);
                    ParameterChanged = true;
                }
            }

            if (PortBand != band)
            {
                if (band > 0)
                {
                    _PortBand = band;
                    ParameterChanged = true;
                }
            }

            if (PortParity != parity)
            {
                _PortParity = parity;
                ParameterChanged = true;
            }

            if (PortDataBits != databits)
            {
                if (databits < 9 && databits > 0)
                {
                    _PortDataBits = databits;
                    ParameterChanged = true;
                }
            }

            if (PortStopBits != stopbits)
            {
                _PortStopBits = stopbits;
                ParameterChanged = true;
            }
        }

        public void SetComport(string name, int band, Parity parity, int databits, StopBits stopbits)
        {
            if (PortName != name)
            {
                _PortName = name;
                _PortNumber = 0;
                ParameterChanged = true;
            }

            if (PortBand != band)
            {
                if (band > 0)
                {
                    _PortBand = band;
                    ParameterChanged = true;
                }
            }

            if (PortParity != parity)
            {
                _PortParity = parity;
                ParameterChanged = true;
            }

            if (PortDataBits != databits)
            {
                if (databits < 9 && databits > 0)
                {
                    _PortDataBits = databits;
                    ParameterChanged = true;
                }
            }

            if (PortStopBits != stopbits)
            {
                _PortStopBits = stopbits;
                ParameterChanged = true;
            }
        }

        public void SetComport(bool rts, bool dtr)
        {
            if (RtsEnable != rts)
            {
                _RtsEnable = rts;
                ParameterChanged = true;
            }

            if (DtrEnable != dtr)
            {
                _DtrEnable = dtr;
                ParameterChanged = true;
            }
        }

        protected override void ExtendTimerTick()
        {
            if (ParameterChanged)
            {
                Disconnect();
            }
        }

        protected override void Connect()
        {
            if (!Comport.IsOpen)
            {
                if (ParameterChanged)
                {
                    ParameterChanged = false;
                    Comport.PortName = PortName;
                    Comport.BaudRate = PortBand;
                    Comport.Parity = PortParity;
                    Comport.DataBits = PortDataBits;
                    Comport.StopBits = PortStopBits;
                    Comport.RtsEnable = RtsEnable;
                    Comport.DtrEnable = DtrEnable;
                }

                if (PortNumber >= 0)
                {
                    try
                    {
                        Comport.Open();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message + " @" + this.GetType().FullName + "- " + "Connect");
                    }

                    if (Comport.IsOpen)
                    {
                        mConnected = true;
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
            if (Comport.IsOpen)
            {
                try
                {
                    Comport.Close();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message + " @" + this.GetType().FullName + "- " + "DisConnect");
                }

                if (!Comport.IsOpen)
                {
                    mConnected = false;
                }
            }
            else
            {
                mConnected = false;
            }
        }

        protected override ReceivedPack DeviceReceive()
        {
            if (Connected)
            {
                int length = 0;

                try
                {
                    length = Comport.BytesToRead;
                }
                catch (Exception ex)
                {
                    Disconnect();
                    Logger.Error(ex.Message + " @" + this.GetType().FullName + "- " + "DeviceReceive");
                }

                if(length > 0)
                {
                    byte[] buff = new byte[length];
                    try
                    {
                        Comport.Read(buff, 0, length);

                        return new ReceivedPack()
                        {
                            Datas = buff,
                            Time = DateTime.Now,
                            Source = PortName
                        };
                    }
                    catch (Exception ex)
                    {
                        Disconnect();
                        Logger.Error(ex.Message + " @" + this.GetType().FullName + "- " + "DeviceReceive");
                    }
                }
            }
            return null;
        }

        protected override void DeviceSend(byte[] buff)
        {
            if (Connected)
            {
                try
                {
                    Comport.Write(buff, 0, buff.Length);
                }
                catch (Exception ex)
                {
                    Disconnect();
                    Logger.Error(ex.Message + " @" + this.GetType().FullName + "- " + "DeviceSend");
                }
            }
        }

        protected override void StopDevice()
        {
            if (Comport != null)
            {
                Disconnect();
                Comport.Dispose();
            }
        }
    }
}
