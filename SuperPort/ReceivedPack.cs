using System;

namespace SuperPortLibrary
{
    public class ReceivedPack
    {
        public byte[] Datas { get; set; }
        public DateTime Time { get; set; }
        public object Source { get; set; }
    }
}
