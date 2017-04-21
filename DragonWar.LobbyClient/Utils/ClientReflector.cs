using System;
using System.Linq;
using System.Collections.Generic;

public class ClientReflector
{
    public static IEnumerable<Func<bool>> GetInitializerLobbyModulesMethods(LobbyModuleType InitType)
    {
        return (from assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.GlobalAssemblyCache)
                from type in assembly.GetTypes()
                let serverModuleAttribute = (Attribute.GetCustomAttributes(type, typeof(LobbyModuleAtrribute)) as LobbyModuleAtrribute[]).FirstOrDefault(m => m.InitialType == InitType)
                where serverModuleAttribute != null
                from method in type.GetMethods()
                let initMethodAttribute = Attribute.GetCustomAttribute(method, typeof(InitializerMethodAttribute)) as InitializerMethodAttribute
                where initMethodAttribute != null
                orderby serverModuleAttribute.InitializationStage ascending
                select (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), method));
    }

    public static IEnumerable<Action> GetCleanupLobbyMethods(LobbyModuleType InitType)
    {
        return (from assembly in AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.GlobalAssemblyCache)
                from type in assembly.GetTypes()
                let serverModuleAttribute = (Attribute.GetCustomAttributes(type, typeof(LobbyModuleAtrribute)) as LobbyModuleAtrribute[]).FirstOrDefault(m => m.InitialType == InitType)
                where serverModuleAttribute != null && serverModuleAttribute.InitialType == InitType
                from method in type.GetMethods()
                let initMethodAttribute = Attribute.GetCustomAttribute(method, typeof(CleanUpMethodAttribute)) as CleanUpMethodAttribute
                where initMethodAttribute != null
                orderby serverModuleAttribute.InitializationStage descending
                select (Action)Delegate.CreateDelegate(typeof(Action), method));
    }
}

