using System;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceHandlerClassAttribute : Attribute
{
    public ServiceHandlerClassAttribute(byte header)
    {
        this.Header = header;
    }

    public byte Header { get; private set; }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ServiceHandlerAttribute : Attribute
{
    public ServiceHandlerAttribute(byte Type)
    {
        this.Type = Type;
    }

    public byte Type { get; private set; }
}