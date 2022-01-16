using System;
using System.Collections.Generic;

namespace SuperPortLibrary
{
    public class ProtocolNull : Protocol
    {
        bool DataReceived = false;
        List<byte> DataBuffer = new List<byte>();

        public ProtocolNull()
        {
            _IdentityCode = -1;
        }

        protected override ProtocolData OnCompileData()
        {
            if (DataReceived)
            {
                byte[] buffer = DataBuffer.ToArray();

                DataBuffer.Clear();
                DataReceived = false;

                ProtocolData pdata = new ProtocolDataNull();
                pdata.SetBytes(buffer);
                return pdata;
            }
            else
            {
                return null;
            }
        }

        protected override ProtocolData OnCompileData(byte data)
        {
            DataBuffer.Add(data);
            DataReceived = true;
            return null;
        }

        protected override ProtocolData OnDecompileData(ApplicationData data)
        {
            ProtocolData pdata = new ProtocolDataNull();
            pdata.SetApplication(data);
            return pdata;
        }

        protected override ProtocolData OnAutoWriteData()
        {
            return null;
        }

        protected override ProtocolData OnFailWriteData()
        {
            return null;
        }

        protected override ProtocolData OnErrorWriteData()
        {
            return null;
        }

        protected override void OnReadTick()
        {

        }

        protected override void OnWriteTick()
        {

        }

        protected override byte[] OnTransmitData(ProtocolData data)
        {
            return data.BytesData;
        }
    }

    public class ProtocolDataNull : ProtocolData
    {
        protected override void OnCompileData()
        {
            AppDataNull appdata = new AppDataNull();

            appdata.Data = new byte[BytesLength];
            Buffer.BlockCopy(BytesData, 0, appdata.Data, 0, BytesLength);

            SetApplication(appdata);
        }

        protected override void OnDecompileData()
        {
            byte[] bytesdata = null;
            if (ApplicationData is AppDataNull)
            {
                AppDataNull appdata = ApplicationData as AppDataNull;
                byte[] data = appdata.Data;
                if (data != null && data.Length > 0)
                {
                    bytesdata = new byte[data.Length];
                    Buffer.BlockCopy(data, 0, bytesdata, 0, data.Length);
                }
                else
                {
                    bytesdata = new byte[] { };
                }
            }
            else
            {
                bytesdata = new byte[] { };
            }
            SetBytes(bytesdata);
        }
    }

    public class AppDataNull : ApplicationData
    {
        public byte[] Data { get; set; }
    }
}
