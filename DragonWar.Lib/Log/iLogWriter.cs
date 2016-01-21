using System;

namespace DragonWar.Lib.Log
{
    public interface iLogWriter
    {
        void Write(string LogName, string Message, params object[] args);
        void WriteException(Exception Exception, string Commend, params object[] args);
    }
}