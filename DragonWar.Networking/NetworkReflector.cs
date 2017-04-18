using DragonWar.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class NetworkReflector
{
    public static Dictionary<byte, Dictionary<byte, MethodInfo>> GivePacketMethods()
    {
        IEnumerable<Pair<byte, Type>> types = from a in AppDomain.CurrentDomain.GetAssemblies()
                                              where !a.GlobalAssemblyCache
                                              from t in a.GetTypes()
                                              where t.GetCustomAttributes(typeof(ServiceHandlerClassAttribute), false).Length > 0
                                              let at = t.GetCustomAttributes(typeof(ServiceHandlerClassAttribute), false)[0]
                                                     as ServiceHandlerClassAttribute
                                              select new Pair<byte, Type>(at.Header, t);

        IEnumerable<Pair<byte, Pair<byte, MethodInfo>>> methods =
            from t in types
            from m in t.Second.GetMethods()
            let a =
                Attribute.GetCustomAttribute(m, typeof(ServiceHandlerAttribute))
                    as ServiceHandlerAttribute
            where a != null
            select new Pair<byte, Pair<byte, MethodInfo>>(t.First,
                                                            new Pair<byte, MethodInfo>(
                                                                    a.Type,
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

