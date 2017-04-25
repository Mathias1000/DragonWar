using DragonWar.Networking.Network;
using DragonWar.Networking.Network.TCP.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Handling.Store
{
    [ServerModule(ServerType.Match, InitializationStage.Logic)]
    [ServerModule(ServerType.Service, InitializationStage.Logic)]
    public class ServiceHandlerStore
    {

        private Dictionary<ServiceHeaderType, Dictionary<byte, MethodInfo>> packetHandlers;

        public static ServiceHandlerStore Instance { get; private set; }

        public ServiceHandlerStore()
        {
            packetHandlers = new Dictionary<ServiceHeaderType, Dictionary<byte, MethodInfo>>();

        }

        [InitializerMethod]
        public static bool Initialize()
        {
            Instance = new ServiceHandlerStore()
            {
                packetHandlers = NetworkReflector.GiveServicePacketMethods(),
            };

            EngineLog.Write(EngineLogLevel.Startup, "Load {0} Service Packet Handlers", Instance.packetHandlers.Count);
            return true;
        }

        public void HandlePacket(ServicePacket pPacket, ServiceClientBase pSession)
        {
            CallMethod(pPacket.Header, pPacket.Handling, pPacket, pSession);
        }

        protected void CallMethod(ServiceHeaderType pHeader, byte pType, ServicePacket pPacket, ServiceClientBase pSession)
        {
            try
            {
                if (packetHandlers.ContainsKey(pHeader)
                    && packetHandlers[pHeader].ContainsKey(pType))
                {
                    //region

                    packetHandlers[pHeader][pType].Invoke(this, new object[] { pSession, pPacket });

                }
                else
                {
                    SocketLog.Write(SocketLogLevel.Warning, $"No ServicePacket Handler for {(byte)pPacket.Header}:{pPacket.Handling}  found");
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
