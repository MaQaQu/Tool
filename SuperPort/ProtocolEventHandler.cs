using System;

namespace SuperPortLibrary
{
    public delegate void ProtocolDataEventHandler(object sender, ProtocolDataEventArgs e);

    public class ProtocolDataEventArgs : EventArgs
    {
        private ProtocolData _Data;

        public ProtocolData Data { get { return _Data; } }
        public bool Handler { get; set; }

        public ProtocolDataEventArgs(ProtocolData data)
        {
            Handler = false;
            _Data = data;
        }
    }
}
