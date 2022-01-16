using System;
using System.Collections.Concurrent;

namespace SuperPortLibrary
{
    public abstract class Protocol
    {
        private ConcurrentQueue<ProtocolData> ReadBuffers;
        private ConcurrentQueue<ProtocolData> WriteBuffers;

        protected int _IdentityCode = 0;

        public int IdentityCode { get { return _IdentityCode; } }

        public Protocol()
        {
            ReadBuffers = new ConcurrentQueue<ProtocolData>();
            WriteBuffers = new ConcurrentQueue<ProtocolData>();
        }

        private void AddReadBuffer(ProtocolData buff)
        {
            ReadBuffers.Enqueue(buff);
        }

        private void AddWriteBuffer(ProtocolData buff)
        {
            WriteBuffers.Enqueue(buff);
        }

        protected virtual ProtocolData OnCompileData() { return null; }

        protected abstract ProtocolData OnCompileData(byte data);

        protected abstract ProtocolData OnDecompileData(ApplicationData data);

        protected abstract byte[] OnTransmitData(ProtocolData data);

        protected abstract ProtocolData OnAutoWriteData();

        protected abstract ProtocolData OnFailWriteData();

        protected abstract ProtocolData OnErrorWriteData();

        protected abstract void OnReadTick();

        protected abstract void OnWriteTick();

        internal void ClearReadBuffer()
        {
            while (!ReadBuffers.IsEmpty)
            {
                ProtocolData tmp = null;
                ReadBuffers.TryDequeue(out tmp);
            }
        }

        internal void ClearWriteBuffer()
        {
            while (!WriteBuffers.IsEmpty)
            {
                ProtocolData tmp = null;
                WriteBuffers.TryDequeue(out tmp);
            }
        }

        internal ProtocolData GetReadBuffer()
        {
            ProtocolData output = null;
            ReadBuffers.TryDequeue(out output);
            return output;
        }

        internal ProtocolData GetWriteBuffer()
        {
            ProtocolData output = null;
            WriteBuffers.TryDequeue(out output);
            return output;
        }

        internal byte[] GetTransmitData(ProtocolData data)
        {
            return OnTransmitData(data);
        }

        internal ProtocolData GetAutoWriteBuffer()
        {
            ProtocolData output = OnAutoWriteData();
            return output;
        }

        internal ProtocolData GetFailWriteBuffer()
        {
            ProtocolData output = OnFailWriteData();
            return output;
        }

        internal ProtocolData GetErrorWriteBuffer()
        {
            ProtocolData output = OnErrorWriteData();
            return output;
        }

        internal ProtocolData ReadData()
        {
            ProtocolData buff = OnCompileData();

            if (buff != null)
            {
                AddReadBuffer(buff);
            }
            return buff;
        }

        internal ProtocolData ReadData(byte data)
        {
            ProtocolData buff = OnCompileData(data);

            if (buff != null)
            {
                AddReadBuffer(buff);
            }
            return buff;
        }

        internal void WriteData(ApplicationData data)
        {
            ProtocolData buff = OnDecompileData(data);

            if (buff != null)
            {
                WriteData(buff);
            }
        }

        internal void WriteData(ProtocolData data)
        {
            AddWriteBuffer(data);
        }

        internal void ReadTimerTick()
        {
            OnReadTick();
        }

        internal void WriteTimerTick()
        {
            OnWriteTick();
        }

        public Protocol GetCopy()
        {
            return Activator.CreateInstance(this.GetType()) as Protocol;  
        }
    }
}
