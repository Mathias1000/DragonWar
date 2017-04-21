using DragonWar.Networking.Packet;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public abstract class ServicePacket
{

    public abstract ServiceHeaderType Header { get; }
    public abstract byte Handling { get; }

    public dynamic Read()
    {
        return this;
    }
    private bool ToArray(out byte[] Array)
    {
       
        using (var _MemoryStream = new MemoryStream())
        {
            try
            {
                IFormatter _BinaryFormatter = new BinaryFormatter();
                _BinaryFormatter.Serialize(_MemoryStream, this);
                Array = _MemoryStream.ToArray();

                return true;
            }
            catch(Exception ex)
            {
                SocketLog.Write(SocketLogLevel.Exception, "Failed to pack msg {0} : {1}", this.ToString(), ex.Message);
                Array = null;
                return false;
            }
        }
    }


    public byte[] Write()
    {
        if (!ToArray(out byte[] Packet))
        {
            return null;
        }

        byte[] newData = new byte[Packet.Length + 4];


        byte[] BodySize =  BitConverter.GetBytes(Packet.Length);

        //LenghtBytes
        newData[0] = BodySize[0];
        newData[1] = BodySize[1];
        newData[3] = BodySize[2];
        newData[4] = BodySize[3];

        Buffer.BlockCopy(Packet, 0, newData, 4, Packet.Length);


        return newData;
    }
}
