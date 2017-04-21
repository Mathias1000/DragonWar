using DragonWar.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class NetworkReflector
{
    public static Dictionary<ServiceHeaderType, Dictionary<byte, MethodInfo>> GiveServicePacketMethods()
    {
        IEnumerable<Pair<ServiceHeaderType, Type>> types = from a in AppDomain.CurrentDomain.GetAssemblies()
                                              where !a.GlobalAssemblyCache
                                              from t in a.GetTypes()
                                              where t.GetCustomAttributes(typeof(ServiceHandlerClassAttribute), false).Length > 0
                                              let at = t.GetCustomAttributes(typeof(ServiceHandlerClassAttribute), false)[0]
                                                     as ServiceHandlerClassAttribute
                                              select new Pair<ServiceHeaderType, Type>(at.Header, t);

        IEnumerable<Pair<ServiceHeaderType, Pair<byte, MethodInfo>>> methods =
            from t in types
            from m in t.Second.GetMethods()
            let a =
                Attribute.GetCustomAttribute(m, typeof(ServiceHandlerAttribute))
                    as ServiceHandlerAttribute
            where a != null
            select new Pair<ServiceHeaderType, Pair<byte, MethodInfo>>(t.First,
                                                            new Pair<byte, MethodInfo>(
                                                                    a.Type,
                                                                    m));
        var toRet = new Dictionary<ServiceHeaderType, Dictionary<byte, MethodInfo>>();
        foreach (var pair in methods)
        {
            if (!toRet.ContainsKey(pair.First))
                toRet.Add(pair.First, new Dictionary<byte, MethodInfo>());
            toRet[pair.First].Add(pair.Second.First, pair.Second.Second);


        }

        return toRet;
    }

    public static Dictionary<LobbyHeaderType, Dictionary<ushort, MethodInfo>> GiveLobbyPacketMethods()
    {
        IEnumerable<Pair<LobbyHeaderType, Type>> types = from a in AppDomain.CurrentDomain.GetAssemblies()
                                                           where !a.GlobalAssemblyCache
                                                           from t in a.GetTypes()
                                                           where t.GetCustomAttributes(typeof(LobbyHandlerClassAttribute), false).Length > 0
                                                           let at = t.GetCustomAttributes(typeof(LobbyHandlerClassAttribute), false)[0]
                                                                  as LobbyHandlerClassAttribute
                                                           select new Pair<LobbyHeaderType, Type>(at.Header, t);

        IEnumerable<Pair<LobbyHeaderType, Pair<ushort, MethodInfo>>> methods =
            from t in types
            from m in t.Second.GetMethods()
            let a =
                Attribute.GetCustomAttribute(m, typeof(LobbyHandlerAttribute))
                    as LobbyHandlerAttribute
            where a != null
            select new Pair<LobbyHeaderType, Pair<ushort, MethodInfo>>(t.First,
                                                            new Pair<ushort, MethodInfo>(
                                                                    a.Type,
                                                                    m));
        var toRet = new Dictionary<LobbyHeaderType, Dictionary<ushort, MethodInfo>>();
        foreach (var pair in methods)
        {
            if (!toRet.ContainsKey(pair.First))
                toRet.Add(pair.First, new Dictionary<ushort, MethodInfo>());
            toRet[pair.First].Add(pair.Second.First, pair.Second.Second);


        }

        return toRet;
    }


}

