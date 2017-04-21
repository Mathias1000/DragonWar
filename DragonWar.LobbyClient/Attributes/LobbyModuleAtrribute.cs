using System;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class LobbyModuleAtrribute : Attribute
{
    private readonly ModuleInitializationStage stageInternal;
    private readonly LobbyModuleType  InitTypeInternal;
    public LobbyModuleType InitialType { get { return InitTypeInternal; } }
    public ModuleInitializationStage InitializationStage { get { return stageInternal; } }

    public LobbyModuleAtrribute(LobbyModuleType InitialType, ModuleInitializationStage initializationStage)
    {
        stageInternal = initializationStage;
        InitTypeInternal = InitialType;
    }
}
