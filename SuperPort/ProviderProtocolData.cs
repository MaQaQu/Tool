using System;

namespace SuperPortLibrary
{
    public abstract class ProviderProtocolData : ProtocolData
    {
        public ProtocolData Parent { get; protected set; }

        public override int BytesLength
        {
            get => Parent.BytesLength;
            set => Parent.BytesLength = value;
        }

        public override byte[] BytesData
        {
            get => Parent.BytesData;
            set => Parent.BytesData = value;
        }

        public override ApplicationData ApplicationData
        {
            get => Parent.ApplicationData;
            set => Parent.ApplicationData = value;
        }

        public override bool Varified
        {
            get => Parent.Varified;
            set => Parent.Varified = value;
        }

        public override object Source
        {
            get => Parent.Source;
            set => Parent.Source = value;
        }

        public override DateTime Time
        {
            get => Parent.Time;
            set => Parent.Time = value;
        }

        public override object Tag
        {
            get => Parent.Tag;
            set => Parent.Tag = value;
        }

        public ProviderProtocolData(ProtocolData parent)
        {
            Parent = parent;
        }
    }
}
