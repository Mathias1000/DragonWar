using DragonWar.Networking.Packet;
using System;

namespace DragonWar.Networking.Network.TCP
{
    public class DataRecievedEventArgs : EventArgs
    {
        public BinaryPacket CurrentDataPacket { get; private set; }

        public DataRecievedEventArgs(BinaryPacket mPacket)
        {
            CurrentDataPacket = mPacket;
        }
    }
}