
public enum InitializationStage : uint
{
    PreData = uint.MinValue,
    Data = 0x01,
    Logic = 0x02,
    InternNetwork = 0x03,
    CharacterData = 0x04,
    Networking = uint.MaxValue,
}

