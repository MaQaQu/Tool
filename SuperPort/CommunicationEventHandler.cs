using System;

namespace SuperPortLibrary
{
    public delegate void CommunicationEventHandler(object sender, CommunicationEventArgs e);

    public class CommunicationEventArgs : EventArgs
    {
        private Communication _Communication;
        public Communication Communication { get { return _Communication; } }

        public CommunicationEventArgs(Communication comm)
        {
            _Communication = comm;
        }
    }
}
