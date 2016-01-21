using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DragonWar.Lib.Util
{
    public static class Reflector
    {

        public static IEnumerable<Pair<TAttribute, MethodInfo>> FindMethodsByAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            return (from method in AppDomain.CurrentDomain.GetAssemblies()
                        .Where(assembly => !assembly.GlobalAssemblyCache)
                        .SelectMany(assembly => assembly.GetTypes())
                        .SelectMany(type => type.GetMethods())
                    let attribute = Attribute.GetCustomAttribute(method, typeof(TAttribute), false) as TAttribute
                    where attribute != null
                    select new Pair<TAttribute, MethodInfo>(attribute, method));
        }

        public static IEnumerable<Func<bool>> GetInitializerMethods()
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.GlobalAssemblyCache)
                    from type in assembly.GetTypes()
                    let serverModuleAttribute = Attribute.GetCustomAttribute(type, typeof(ServerModuleAttribute)) as ServerModuleAttribute
                    where serverModuleAttribute != null
                    from method in type.GetMethods()
                    let initMethodAttribute = Attribute.GetCustomAttribute(method, typeof(InitializerMethodAttribute)) as InitializerMethodAttribute
                    where initMethodAttribute != null
                    orderby serverModuleAttribute.InitializationStage ascending
                    select (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), method));
        }
        public static IEnumerable<Action> GetCleanupMethods()
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.GlobalAssemblyCache)
                    from type in assembly.GetTypes()
                    let serverModuleAttribute = Attribute.GetCustomAttribute(type, typeof(ServerModuleAttribute)) as ServerModuleAttribute
                    where serverModuleAttribute != null
                    from method in type.GetMethods()
                    let initMethodAttribute = Attribute.GetCustomAttribute(method, typeof(CleanUpMethodAttribute)) as CleanUpMethodAttribute
                    where initMethodAttribute != null
                    orderby serverModuleAttribute.InitializationStage descending
                    select (Action)Delegate.CreateDelegate(typeof(Action), method));
        }

        public static Dictionary<string, MethodInfo> GiveConsoleMethods()
        {
            IEnumerable<Pair<string, MethodInfo>> All = (from assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.GlobalAssemblyCache)
                                                         from type in assembly.GetTypes()
                                                         from meth in type.GetMethods()
                                                         let sModuleAttribute = Attribute.GetCustomAttribute(meth, typeof(ConsoleCommandAttribute)) as ConsoleCommandAttribute
                                                         where sModuleAttribute != null
                                                         select new Pair<string, MethodInfo>(sModuleAttribute.CmdText, meth));

            var toRet = new Dictionary<string, MethodInfo>();
            foreach (var CmdM in All)
            {
                if (!toRet.ContainsKey(CmdM.First))
                    toRet.Add(CmdM.First.ToLower(), CmdM.Second);

            }
            return toRet;


        }
        public static Dictionary<byte, Dictionary<byte, MethodInfo>> GivePacketMethods()
        {
            IEnumerable<Pair<byte, Type>> types = from a in AppDomain.CurrentDomain.GetAssemblies()
                                                  where !a.GlobalAssemblyCache
                                                  from t in a.GetTypes()
                                                  where t.GetCustomAttributes(typeof(PacketHandlerClassAttribute), false).Length > 0
                                                  let at = t.GetCustomAttributes(typeof(PacketHandlerClassAttribute), false)[0]
                                                         as PacketHandlerClassAttribute
                                                  select new Pair<byte, Type>(at.Header, t);

            IEnumerable<Pair<byte, Pair<byte, MethodInfo>>> methods =
                from t in types
                from m in t.Second.GetMethods()
                let a =
                    Attribute.GetCustomAttribute(m, typeof(PacketHandlerAttribute))
                        as PacketHandlerAttribute
                where a != null
                select new Pair<byte, Pair<byte, MethodInfo>>(t.First,
                                                                new Pair<byte, MethodInfo>(
                                                                        a.OpCode,
                                                                        m));
            var toRet = new Dictionary<byte, Dictionary<byte, MethodInfo>>();
            foreach (var pair in methods)
            {
                if (!toRet.ContainsKey(pair.First))
                    toRet.Add(pair.First, new Dictionary<byte, MethodInfo>());
                toRet[pair.First].Add(pair.Second.First, pair.Second.Second);


            }

            return toRet;
        }
    }
}
