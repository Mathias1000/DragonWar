using DragonWar.Networking.Packet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public static class PacketExtension
{

    public static PacketType ToPacket<PacketType>(this byte[] _byteArray)
    {
        PacketType ReturnValue;
        using (var _MemoryStream = new MemoryStream(_byteArray))
        {
            IFormatter _BinaryFormatter = new BinaryFormatter();
            ReturnValue = (PacketType)_BinaryFormatter.Deserialize(_MemoryStream);
        }
        return ReturnValue;
    }
}

