using System;

namespace DragonWar.Networking.Packet.Lobby.Protocol
{
    [Serializable]
    public class LobbyHandShake : LobbyPacket
    {

      
        public override LobbyHeaderType Header => LobbyHeaderType.Protocol;

        public override ushort HandlingType => (byte)ProtocolTypes.Handshake;

        public ushort Key { get; internal set; }

        public static ushort GenKey()
        {
            return (ushort)new Random().Next(ushort.MinValue, 700);
        }
    }
}
