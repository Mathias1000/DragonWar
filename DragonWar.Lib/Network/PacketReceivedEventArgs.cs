using System;
using DragonWar.Lib.Packet;

namespace DragonWar.Lib.Network
{
    public class PacketReceivedEventArgs : EventArgs
    {
        public PacketReceivedEventArgs(PacketReader pReader)
        {
            Packet = pReader;
        }

        public PacketReader Packet { get; private set; }
    }
}
