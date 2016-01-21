using System;


namespace DragonWar.Lib.Database
{

    [Serializable()]
    public class DatabaseException : Exception
    {
        internal DatabaseException(string sMessage) : base(sMessage) { }
    }
}
