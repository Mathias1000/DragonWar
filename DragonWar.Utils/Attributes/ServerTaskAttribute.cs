using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ServerTaskClass : Attribute
{
    public ServerTaskTimes serverGC_Interval { get; private set; }

    public ServerTaskClass(ServerTaskTimes _serverGC_Interval)
    {
        serverGC_Interval = _serverGC_Interval;
    }
}

