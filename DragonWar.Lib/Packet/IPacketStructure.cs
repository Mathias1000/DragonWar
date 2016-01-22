using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Lib.Packet
{
    public interface IPacketStructure
    {
        void ReadFromPacket(PacketReader pReader);
        void WriteToPacket(PacketWriter pWriter);
    }
}
