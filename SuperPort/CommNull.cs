namespace SuperPortLibrary
{
    public class CommNull : Communication
    {
        public CommNull()
        {
            _Type = CommunicationType.None;
            _Name = "None";
        }

        protected override void Connect()
        {
        }

        protected override void Disconnect()
        {
        }

        protected override void ExtendTimerTick()
        {
        }

        protected override void StopDevice()
        {
        }

        protected override ReceivedPack DeviceReceive()
        {
            return null;
        }

        protected override void DeviceSend(byte[] buff)
        {
        }
    }
}
