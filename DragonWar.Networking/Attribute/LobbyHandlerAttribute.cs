using System;



[AttributeUsage(AttributeTargets.Class)]
    public class LobbyHandlerClassAttribute : Attribute
    {
        public LobbyHandlerClassAttribute(LobbyHeaderType header)
        {
            this.Header = header;
        }

        public LobbyHeaderType Header { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class LobbyHandlerAttribute : Attribute
    {
        public LobbyHandlerAttribute(byte Type)
        {
            this.Type = Type;
        }

        public byte Type { get; private set; }
    }
