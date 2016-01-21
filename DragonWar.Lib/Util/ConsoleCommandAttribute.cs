using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class ConsoleCommandAttribute : Attribute
{

    public string CmdText { get; set; }

    public ConsoleCommandAttribute(string cmdText)
    {
        CmdText = cmdText;
    }
}

