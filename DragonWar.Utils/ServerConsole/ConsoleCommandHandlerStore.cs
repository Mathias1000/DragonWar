using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace DragonWar.Utils.ServerConsole
{
    [ServerModule(ServerType.Match, InitializationStage.PreData)]
    [ServerModule(ServerType.Service, InitializationStage.PreData)]
    public class ConsoleCommandHandlerStore
    {
        static Dictionary<string, MethodInfo> SingleConsoleCommands;
        //Category Command Function
        static Dictionary<string, Dictionary<string, MethodInfo>> CategoryConsoleCommands;

        [InitializerMethod]
        public static bool Initialize()
        {
            try
            {
                SingleConsoleCommands = Reflector.GiveSingleConsoleMethods();
                CategoryConsoleCommands = Reflector.GiveCategoryConsoleMethods();
            }
            catch (Exception ex)
            {

                EngineLog.Write(EngineLogLevel.Exception, "Failed to Initial Console Commands {0}", ex.ToString());
                return false;
            }
            return true;
        }


        public static bool GetCommandFromCategory(
            string CategoryName,
            string CommandText,
            out MethodInfo pMethod)
        {

            pMethod = null;
            Dictionary<string, MethodInfo> _Category;
            if (!CategoryConsoleCommands.TryGetValue(CategoryName, out _Category)) return false;
            if (!_Category.TryGetValue(CommandText.ToUpper(), out pMethod)) return false;

            return true;
        }

        public static bool InvokeConsoleCommand(string input, params string[] args)
        {
    
            string cmdText = args[0].ToUpper();
            if (CategoryConsoleCommands.ContainsKey(cmdText))
            {
                if (args.Length >= 2)
                {
                    MethodInfo pInfo;
                    if (GetCommandFromCategory(cmdText, args[1], out pInfo))
                    {
                        return (bool)pInfo.Invoke(null, new object[] { args.Skip(2).ToArray() });
                    }
                }
            }
            else
            {
                MethodInfo pInfo;
                if (SingleConsoleCommands.TryGetValue(cmdText, out pInfo))
                {
                    return (bool)pInfo?.Invoke(null, new object[] { args });
                }
            }

            return false;
        }
    }
}
