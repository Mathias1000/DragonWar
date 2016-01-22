using DragonWar.Lib.Packet;

namespace DragonWar.Lib.Cryptography
{
    public interface ICryptoProvider
    {
        byte[] Encrypt(PacketWriter pWriter);
        byte[] Encrypt(byte[] pData, int pOffset, int pLength);
        byte[] Decrypt(byte[] pData, int pOffset, int pLength);
    }
}
