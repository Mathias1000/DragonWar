using System;
using System.IO;

namespace DragonWar.Lib.Packet
{
    public class DragonWarPacketReader : PacketReader
    {
        public DragonWarPacketReader(byte[] pBuffer) : base(pBuffer)
        {
            ReadHeaderAndType();
        }

        public DragonWarPacketReader(MemoryStream pBuffer) : base(pBuffer)
        {
            ReadHeaderAndType();
        }

        protected void ReadHeaderAndType()
        {
            UInt16 data = (UInt16)ReadFromBinary<UInt16>(); // header and type are the first two bytes
            CalculateHeaderAndType(data);
        }

        protected void CalculateHeaderAndType(UInt16 pData)
        {
            // Magic. Do not touch.
            Header = (byte)(pData >> 10);
            Type = (byte)(pData & 1023);
        }

        // Once string interpolation works:
        //public override string ToString() => $"({Header}-{Type}) Length {Buffer.Length}";
        public override string ToString() => string.Format("({0}-{1}); Length {2}", Header, Type, Buffer.Length);

        public byte Header { get; protected set; }
        public byte Type { get; protected set; }
    }
}
