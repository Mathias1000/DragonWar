using DragonWar.Networking.Network;
using DragonWar.Networking.Network.TCP.Client;
using DragonWar.Networking.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Store
{

    [ServerModule(ServerType.Service, InitializationStage.Networking)]
    public class LobbyHandlerStore
    {

        private Dictionary<LobbyHeaderType, Dictionary<ushort, MethodInfo>> packetHandlers;

        public static LobbyHandlerStore Instance { get; private set; }

        public LobbyHandlerStore()
        {
            packetHandlers = new Dictionary<LobbyHeaderType, Dictionary<ushort, MethodInfo>>();
        }

 
        [InitializerMethod]
        public static bool Initialize()
        {
            Instance = new LobbyHandlerStore()
            {
                packetHandlers = NetworkReflector.GiveLobbyPacketMethods()
            };

            EngineLog.Write(EngineLogLevel.Startup, "Load {0} Lobby Packet Handlers", Instance.packetHandlers.Count);
            return true;
        }

        private void LobbyHandlerStore_UnknownPacket(LobbyPacket pPacket)
        {
            SocketLog.Write(SocketLogLevel.Warning, "No Packet Handler for {0} found", pPacket.Header);
            SocketLog.Write(SocketLogLevel.Warning, pPacket.ToString());
        }

        public void HandlePacket(LobbyPacket pPacket,ClientBase pSession)
        {
            CallMethod(pPacket.Header, pPacket.HandlingType, pPacket, pSession);
        }
    
        protected void CallMethod(LobbyHeaderType pHeader, ushort pType, LobbyPacket pPacket, ClientBase pSession)
        {
            try
            {
                if (packetHandlers.ContainsKey(pHeader)
                    && packetHandlers[pHeader].ContainsKey(pType))
                {

                    packetHandlers[pHeader][pType].Invoke(this, new object[] { pSession, pPacket });

                }
                else
                {
                    SocketLog.Write(SocketLogLevel.Warning, $"No LobbyPacket Handler for {(byte)pPacket.Header}:{pPacket.HandlingType}  found");
                    SocketLog.Write(SocketLogLevel.Warning, pPacket.ToString());
                }
            }
            catch (Exception ex)
            {
                EngineLog.Write(EngineLogLevel.Exception, "Error Handling {0} : {1} {2}", pHeader, pType, ex.ToString());
            }

        }
    }
}
