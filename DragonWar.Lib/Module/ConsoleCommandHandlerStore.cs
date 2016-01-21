using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DragonWar.Lib.Util;

namespace DragonWar.Lib.Module
{
    [ServerModule(InitializationStage.Metadata)]

    public class ConsoleCommandHandlerStore
    {
        static Dictionary<string, MethodInfo> ConsoleCommands;




        [InitializerMethod]
        public static bool Initialize()
        {
            ConsoleCommands = Reflector.GiveConsoleMethods();

            return true;
        }


        public static bool InvokeConsoleCommand(string cmdText, params string[] args)
        {
            MethodInfo pMethod;
            if(ConsoleCommands.TryGetValue(cmdText,out pMethod))
            {
                pMethod.Invoke(null, new object[] { args.Skip(2).ToArray() });
                return true;
            }
            return false;
        }

    }
}
