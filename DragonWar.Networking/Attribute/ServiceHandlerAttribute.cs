using System;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceHandlerClassAttribute : Attribute
{
    public ServiceHandlerClassAttribute(ServiceHeaderType header)
    {
        this.Header = header;
    }

    public ServiceHeaderType Header { get; private set; }
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