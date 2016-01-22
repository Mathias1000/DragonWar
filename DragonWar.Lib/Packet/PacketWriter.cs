using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DragonWar.Lib.Packet
{
    public abstract class PacketWriter : IDisposable
    {
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        public Encoding Encoding { get; protected set; } = DefaultEncoding;

        protected MemoryStream Buffer;
        protected BinaryWriter Writer;

        private Dictionary<Type, Action<object, BinaryWriter>> writeMethods;

        protected PacketWriter()
        {
            writeMethods = new Dictionary<Type, Action<object, BinaryWriter>>();
            Buffer = new MemoryStream();
            Writer = new BinaryWriter(Buffer);
            RegisterPrimitiveTypeMethods();
        }

        protected PacketWriter(int pBufferSize)
        {
            writeMethods = new Dictionary<Type, Action<object, BinaryWriter>>();
            Buffer = new MemoryStream(pBufferSize);
            Writer = new BinaryWriter(Buffer);
            RegisterPrimitiveTypeMethods();
        }

        public void WriteBinary<T>(object pObj)
        {
            //if (pObj is IWriteBinary) {
            //	((IWriteBinary) pObj).WriteToBinary(Writer);
            //} else 
            if (pObj is IPacketStructure)
            {
                ((IPacketStructure)pObj).WriteToPacket(this);
            }
            else if (writeMethods.ContainsKey(typeof(T)))
            {
                writeMethods[typeof(T)](pObj, Writer);
            }
            else
            {
                throw new InvalidOperationException("No method registered for given type");
            }
        }

        public void WriteString(string pData, int pLength)
        {
            byte[] data = new byte[pLength];
            // fill w/ 00-bytes.
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 0;
            }
            byte[] encoded = Encoding.GetBytes(pData);
            for (int i = 0; i < Math.Min(encoded.Length, data.Length); i++)
            {
                data[i] = encoded[i];
            }
            WriteBinary<byte[]>(data);
        }

        protected void RegisterWriteMethod(Type pType, Action<object, BinaryWriter> pAction)
        {
            if (writeMethods.ContainsKey(pType))
            {
                throw new ArgumentException("Cannot register the same Type twice");
            }
            writeMethods.Add(pType, pAction);
        }

        protected bool WriteMethodRegistered(Type pType)
        {
            return writeMethods.ContainsKey(pType);
        }

        protected void RegisterPrimitiveTypeMethods()
        {
            RegisterWriteMethod(typeof(Boolean), (o, w) => w.Write(Convert.ToBoolean(o)));
            RegisterWriteMethod(typeof(Byte), (o, w) => w.Write(Convert.ToByte(o)));
            RegisterWriteMethod(typeof(SByte), (o, w) => w.Write(Convert.ToSByte(o)));
            RegisterWriteMethod(typeof(Int16), (o, w) => w.Write(Convert.ToInt16(o)));
            RegisterWriteMethod(typeof(UInt16), (o, w) => w.Write(Convert.ToUInt16(o)));
            RegisterWriteMethod(typeof(Int32), (o, w) => w.Write(Convert.ToInt32(o)));
            RegisterWriteMethod(typeof(UInt32), (o, w) => w.Write(Convert.ToUInt32(o)));
            RegisterWriteMethod(typeof(Int64), (o, w) => w.Write(Convert.ToInt64(o)));
            RegisterWriteMethod(typeof(UInt64), (o, w) => w.Write(Convert.ToUInt64(o)));
            RegisterWriteMethod(typeof(byte[]), (o, w) => w.Write((byte[])o));
        }

        public abstract byte[] GetBytes();

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).      
                    Writer.Dispose();
                    Buffer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }



        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
