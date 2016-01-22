using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DragonWar.Lib.Packet
{

    public abstract class PacketReader : IDisposable
    {
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        public Encoding Encoding { get; protected set; } = DefaultEncoding;

        protected MemoryStream Buffer;
        protected BinaryReader Reader;

        private readonly Dictionary<Type, Func<BinaryReader, object>> readFunctions;

        protected PacketReader()
        {
            readFunctions = new Dictionary<Type, Func<BinaryReader, object>>();
            RegisterFunctionsForPrimitives();
        }

        protected PacketReader(byte[] pBuffer) : this()
        {
            Buffer = new MemoryStream(pBuffer);
            Reader = new BinaryReader(Buffer);
        }

        protected PacketReader(MemoryStream pBuffer) : this()
        {
            Buffer = pBuffer;
            Reader = new BinaryReader(Buffer);
        }

        public object ReadFromBinary<T>()
        {
            if (TypeHasRegisteredFunction(typeof(T)))
            {
                // we have a registered method for that type, so we can call that one
                return CallReadFunctionForType(typeof(T));
            }
            else
            {
                // we don't know how to handle anything else though!
                throw new InvalidOperationException("No known function to read this type");
            }
        }

        public string ReadString(int pLength)
        {
            // we cannot simply wrap this as we need to specify the length of the string
            byte[] buffer = ReadBytes(pLength);
            string data = Encoding.ASCII.GetString(buffer);
            return data.Trim().Replace("\0", "");
        }

        public void SkipBytes(int pCount)
        {
            // the same as reading bytes, but discarding the return value
            ReadBytes(pCount);
        }

        public byte[] ReadBytes(int pLength)
        {
            // this cannot be wrapped easily, as we need to specify just how many bytes we want to read
            return Reader.ReadBytes(pLength);
        }

        public long BytesLeft() => Buffer.Length - Buffer.Position;

        protected void RegisterFunction(Type pType, Func<BinaryReader, object> pFunction)
        {
            // we only allow one method per type. we wouldn't know how to handle it otherwise
            // its a simple first come - first serve here
            if (TypeHasRegisteredFunction(pType))
            {
                throw new InvalidOperationException("The given Type already has an registered function.");
            }
            readFunctions.Add(pType, pFunction);
        }

        protected bool TypeHasRegisteredFunction(Type pType)
        {
            // again a simple wrapper, but makes things cleaner
            return readFunctions.ContainsKey(pType);
        }

        protected object CallReadFunctionForType(Type pType)
        {
            // we just call the methods registered for the type.
            // we don't care about error handling here
            return readFunctions[pType](Reader);
        }

        protected void RegisterFunctionsForPrimitives()
        {
            // these are simple wrappers for the methods of BinaryReader
            RegisterFunction(typeof(bool), r => r.ReadBoolean());
            RegisterFunction(typeof(Byte), r => r.ReadByte());
            RegisterFunction(typeof(SByte), r => r.ReadSByte());
            RegisterFunction(typeof(Int16), r => r.ReadInt16());
            RegisterFunction(typeof(UInt16), r => r.ReadUInt16());
            RegisterFunction(typeof(Int32), r => r.ReadInt32());
            RegisterFunction(typeof(UInt32), r => r.ReadUInt32());
            RegisterFunction(typeof(Int64), r => r.ReadInt64());
            RegisterFunction(typeof(UInt64), r => r.ReadUInt64());
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).    
                    Reader.Dispose();
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
