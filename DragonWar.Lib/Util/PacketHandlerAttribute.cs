using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Class)]
public class PacketHandlerClassAttribute : Attribute
{
    public PacketHandlerClassAttribute(byte header)
    {

        this.Header = header;
    }

    public byte Header { get; private set; }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class PacketHandlerAttribute : Attribute
{
    public PacketHandlerAttribute(byte opcode)
    {

        OpCode = opcode;
    }

    public byte OpCode { get; private set; }
}


