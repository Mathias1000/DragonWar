using System;
using DragonWar.Lib.Packet;

namespace DragonWar.Lib.Network
{
    public class PacketSentEventArgs : EventArgs
    {
        public DateTime TimeStamp { get; private set; }
        public PacketWriter Packet { get; private set; }

        public PacketSentEventArgs(PacketWriter pWriter)
        {
            Packet = pWriter;
            TimeStamp = DateTime.Now;
        }
    }
}
