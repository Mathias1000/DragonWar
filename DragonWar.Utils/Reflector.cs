using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DragonWar.Utils;

//TODO Make it better
public class Reflector
{
    public static Reflector Global
    {
        get
        {
            return _Global == null ? _Global = new Reflector(AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("DragonWar.")).ToArray()) : _Global;
        }
    }
    private static Reflector _Global;

    public Assembly[] Assemblies { get; private set; }

    public Reflector(params Assembly[] Assemblies)
    {
        this.Assemblies = Assemblies;
    }

    public Type[] GetTypes()
    {
        return (from type in Assemblies.SelectMany(a => a.GetTypes())
                select type).ToArray();
    }
    public MethodInfo[] GetMethods()
    {
        return (from method in GetTypes().SelectMany(t => t.GetMethods())
                select method).ToArray();
    }
    public Pair<pAttribute, Type>[] GetTypesWithAttribute<pAttribute>()
           where pAttribute : Attribute
    {
        return (from type in GetTypes()
                let attr = Attribute.GetCustomAttribute(type, typeof(pAttribute)) as pAttribute
                where attr != null
                select new Pair<pAttribute, Type>(attr, type)).ToArray();
    }

    public Pair<pAttribute, MethodInfo>[] GetMethodsWithAttribute<pAttribute>()
    where pAttribute : Attribute
    {
        return (from method in GetMethods()
                let attr = Attribute.GetCustomAttribute(method, typeof(pAttribute)) as pAttribute
                where attr != null
                select new Pair<pAttribute, MethodInfo>(attr, method)).ToArray();
    }

    public static IEnumerable<Func<bool>> GetInitializerServerMethods(ServerType InitType)
    {
        return (from assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.GlobalAssemblyCache)
                from type in assembly.GetTypes()
                let serverModuleAttribute = (Attribute.GetCustomAttributes(type, typeof(ServerModuleAttribute)) as ServerModuleAttribute[]).FirstOrDefault(m => m.InitialType == InitType)
                where serverModuleAttribute != null
                from method in type.GetMethods()
                let initMethodAttribute = Attribute.GetCustomAttribute(method, typeof(InitializerMethodAttribute)) as InitializerMethodAttribute
                where initMethodAttribute != null
                orderby serverModuleAttribute.InitializationStage ascending
                select (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), method));
    }

    public static IEnumerable<Func<bool>> GetInitializerGameMethods(ServerType InitType)
    {
        return (from assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.GlobalAssemblyCache)
                from type in assembly.GetTypes()
                let serverModuleAttribute = (Attribute.GetCustomAttributes(type, typeof(GameServerModuleAttribute)) as GameServerModuleAttribute[]).FirstOrDefault(m => m.InitialType == InitType)
                where serverModuleAttribute != null
                from method in type.GetMethods()
                let initMethodAttribute = Attribute.GetCustomAttribute(method, typeof(InitializerMethodAttribute)) as InitializerMethodAttribute
                where initMethodAttribute != null
                orderby serverModuleAttribute.InitializationStage ascending
                select (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), method));
    }

    public static IEnumerable<Action> GetStartUPCleans(ServerType InitType)
    {
        return (from assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.GlobalAssemblyCache)
                from type in assembly.GetTypes()
                let serverModuleAttribute = (Attribute.GetCustomAttributes(type, typeof(GameServerModuleAttribute)) as GameServerModuleAttribute[]).FirstOrDefault(m => m.InitialType == InitType)
                where serverModuleAttribute != null && serverModuleAttribute.InitialType == InitType
                from method in type.GetMethods()
                let initMethodAttribute = Attribute.GetCustomAttribute(method, typeof(StartUpCleanOut)) as StartUpCleanOut
                where initMethodAttribute != null
                orderby serverModuleAttribute.InitializationStage descending
                select (Action)Delegate.CreateDelegate(typeof(Action), method));
    }

    public static IEnumerable<Action> GetCleanupGameMethods(ServerType InitType)
    {
        return (from assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.GlobalAssemblyCache)
                from type in assembly.GetTypes()
                let serverModuleAttribute = (Attribute.GetCustomAttributes(type, typeof(ServerModuleAttribute)) as ServerModuleAttribute[]).FirstOrDefault(m => m.InitialType == InitType)
                where serverModuleAttribute != null && serverModuleAttribute.InitialType == InitType
                from method in type.GetMethods()
                let initMethodAttribute = Attribute.GetCustomAttribute(method, typeof(CleanUpMethodAttribute)) as CleanUpMethodAttribute
                where initMethodAttribute != null
                orderby serverModuleAttribute.InitializationStage descending
                select (Action)Delegate.CreateDelegate(typeof(Action), method));
    }

    public static IEnumerable<Action> GetCleanupServerMethods(ServerType InitType)
    {
        return (from assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.GlobalAssemblyCache)
                from type in assembly.GetTypes()
                let serverModuleAttribute = (Attribute.GetCustomAttributes(type, typeof(ServerModuleAttribute)) as ServerModuleAttribute[]).FirstOrDefault(m => m.InitialType == InitType)
                where serverModuleAttribute != null && serverModuleAttribute.InitialType == InitType
                from method in type.GetMethods()
                let initMethodAttribute = Attribute.GetCustomAttribute(method, typeof(CleanUpMethodAttribute)) as CleanUpMethodAttribute
                where initMethodAttribute != null
                orderby serverModuleAttribute.InitializationStage descending
                select (Action)Delegate.CreateDelegate(typeof(Action), method));
    }

    public static Dictionary<string, Dictionary<string, MethodInfo>> GiveCategoryConsoleMethods()
    {
        IEnumerable<Pair<string, Type>> catergorytypes = from a in AppDomain.CurrentDomain.GetAssemblies()
                                                         where !a.GlobalAssemblyCache
                                                         from t in a.GetTypes()
                                                         where t.GetCustomAttributes(typeof(ConsoleCommandCategory), false).Length > 0
                                                         let at = t.GetCustomAttributes(typeof(ConsoleCommandCategory), false)[0]
                                                                as ConsoleCommandCategory
                                                         select new Pair<string, Type>(at.Category.ToUpper(), t);

        IEnumerable<Pair<string, Pair<string, MethodInfo>>> methods =
            from t in catergorytypes
            from m in t.Second.GetMethods()
            let a = Attribute.GetCustomAttribute(m, typeof(ConsoleCommandAttribute)) as ConsoleCommandAttribute
            where a != null
            select new Pair<string, Pair<string, MethodInfo>>(t.First,
                                                            new Pair<string, MethodInfo>(
                                                                    a.Command.ToUpper(), m));

        var toRet = new Dictionary<string, Dictionary<string, MethodInfo>>();
        foreach (var pair in methods)
        {
            if (!toRet.ContainsKey(pair.First))
                toRet.Add(pair.First, new Dictionary<string, MethodInfo>());

            toRet[pair.First].Add(pair.Second.First, pair.Second.Second);
        }

        return toRet;
    }

    public static Dictionary<string, MethodInfo> GiveSingleConsoleMethods()
    {
        IEnumerable<Pair<string, MethodInfo>> All = (from assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.GlobalAssemblyCache)
                                                     from type in assembly.GetTypes()
                                                     from meth in type.GetMethods()
                                                     let sModuleAttribute = Attribute.GetCustomAttribute(meth, typeof(SingleConsoleCommand)) as SingleConsoleCommand
                                                     where sModuleAttribute != null
                                                     select new Pair<string, MethodInfo>(sModuleAttribute.CmdText.ToUpper(), meth));

        var toRet = new Dictionary<string, MethodInfo>();
        foreach (var CmdM in All)
        {
            if (!toRet.ContainsKey(CmdM.First))
                toRet.Add(CmdM.First.ToUpper(), CmdM.Second);

        }
        return toRet;


    }

    public static IEnumerable<Pair<ServerTaskTimes, Type>> GiveServerTasks()
    {
        return from a in AppDomain.CurrentDomain.GetAssemblies()
               where !a.GlobalAssemblyCache
               from t in a.GetTypes()
               where t.GetCustomAttributes(typeof(ServerTaskClass), false).Length > 0
               let at = t.GetCustomAttributes(typeof(ServerTaskClass), false)[0]
                      as ServerTaskClass
               select new Pair<ServerTaskTimes, Type>(at.serverGC_Interval, t);
    }
}

