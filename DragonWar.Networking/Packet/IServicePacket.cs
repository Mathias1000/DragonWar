public interface IServicePacket
{
    byte Header { get; }
    byte Type { get; }
    bool Read();
    bool Write();
}
