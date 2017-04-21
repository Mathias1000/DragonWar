using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Packet
{
    public class BinaryPacket : IDisposable
    {
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        public Encoding Encoding { get; protected set; } = DefaultEncoding;

        protected MemoryStream Buffer;
        protected BinaryWriter Writer;
        protected BinaryReader Reader;

        private Dictionary<Type, Action<object, BinaryWriter>> writeMethods;
        private readonly Dictionary<Type, Func<BinaryReader, object>> readFunctions;


        protected BinaryPacket()
        {
            writeMethods = new Dictionary<Type, Action<object, BinaryWriter>>();
            Buffer = new MemoryStream();

            Writer = new BinaryWriter(Buffer);
            RegisterPrimitiveTypeWriteMethods();
        }

        public BinaryPacket(byte[] pBuffer) : this()
        {
            Buffer = new MemoryStream(pBuffer);
            Reader = new BinaryReader(Buffer);
            readFunctions = new Dictionary<Type, Func<BinaryReader, object>>();
            RegisterPrimitiveTypeReadMethods();
        }

        protected BinaryPacket(int pBufferSize)
        {
            writeMethods = new Dictionary<Type, Action<object, BinaryWriter>>();
            Buffer = new MemoryStream(pBufferSize);
            Writer = new BinaryWriter(Buffer);
            RegisterPrimitiveTypeWriteMethods();
        }

        public BinaryPacket(MemoryStream pBuffer) : this()
        {
            Buffer = pBuffer;
            Reader = new BinaryReader(Buffer);
            readFunctions = new Dictionary<Type, Func<BinaryReader, object>>();
            RegisterPrimitiveTypeReadMethods();

        }

        public void Write<T>(object pObj)
        {
            if (writeMethods.ContainsKey(typeof(T)))
            {
                writeMethods[typeof(T)](pObj, Writer);
            }
            else
            {
                throw new InvalidOperationException("No method registered for given type");
            }
        }

        public bool Read<T>(out T value)
        {
            if (TypeHasRegisteredFunction(typeof(T)))
            {
                // we have a registered method for that type, so we can call that one
                value = (T)CallReadFunctionForType(typeof(T));
                return true;
            }
            else
            {
                // we don't know how to handle anything else though!
                value = default(T);

                EngineLog.Write(EngineLogLevel.Exception, "No known function to read this type");
                return false;
            }
        }

        public object Read<T>()
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
            Write<byte[]>(data);
        }

        public void WriteHexAsBytes(string Hex)
        {
            Write<byte[]>(ByteUtils.HexToBytes(Hex));
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

        protected void RegisterPrimitiveTypeWriteMethods()
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

        protected void RegisterPrimitiveTypeReadMethods()
        {
            // these are simple wrappers for the methods of BinaryReader
            RegisterReadFunction(typeof(bool), r => r.ReadBoolean());
            RegisterReadFunction(typeof(Byte), r => r.ReadByte());
            RegisterReadFunction(typeof(SByte), r => r.ReadSByte());
            RegisterReadFunction(typeof(Int16), r => r.ReadInt16());
            RegisterReadFunction(typeof(UInt16), r => r.ReadUInt16());
            RegisterReadFunction(typeof(Int32), r => r.ReadInt32());
            RegisterReadFunction(typeof(UInt32), r => r.ReadUInt32());
            RegisterReadFunction(typeof(Int64), r => r.ReadInt64());
            RegisterReadFunction(typeof(UInt64), r => r.ReadUInt64());
        }

        protected void RegisterReadFunction(Type pType, Func<BinaryReader, object> pFunction)
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


        public void Fill(int pLength, byte pValue)
        {
            for (int i = 0; i < pLength; ++i)
            {
                Write<byte>(pValue);
            }
        }

        public bool ReadString(out string Value)
        {
            Value = "";

            if (!Read(out byte length))
                return false;

            return ReadString(out Value, length);
        }

        public bool ReadString(out string Value, int Length)
        {
            Value = "";

            if (!ReadBytes(Length, out byte[] buffer))
                return false;


            //remove nulls
            var nullsLength = 0;
            if (buffer[(Length - 1)] != 0)
            {
                nullsLength = Length;
            }
            else
            {
                while (buffer[nullsLength] != 0x00
                    && nullsLength < Length)
                {
                    nullsLength++;
                }
            }

            if (Length > 0)
            {

                Value = Encoding.GetString(buffer, 0, nullsLength);
            }

            return true;
        }

        public bool ReadEncodeString(out string mString, int pLength)
        {
            // we cannot simply wrap this as we need to specify the length of the string
            if (ReadBytes(pLength, out byte[] buffer))
            {
                string data = Encoding.ASCII.GetString(buffer);
                mString = data.Trim().Replace("\0", "");

                return mString != null;
            }
            mString = null;
            return false;
        }

        public bool SkipBytes(int pCount)
        {
            // the same as reading bytes, but discarding the return value

            if (!ReadBytes(pCount, out byte[] skip))
                return false;

            return true;
        }

        public bool ReadBytes(int pLength, out byte[] Bytes)
        {
            Bytes = null;

            if (Reader.BaseStream.Position + pLength > Reader.BaseStream.Length) return false;
            // this cannot be wrapped easily, as we need to specify just how many bytes we want to read
            Bytes = Reader.ReadBytes(pLength);
            return true;
        }

        public long BytesLeft() => Buffer.Length - Buffer.Position;


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).   

                    Reader?.Dispose();
                    Writer?.Dispose();
                    Buffer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources. 
        // ~PacketWriter() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

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
