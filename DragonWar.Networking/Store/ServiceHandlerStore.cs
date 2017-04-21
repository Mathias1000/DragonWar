using DragonWar.Networking.Network;
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
    [ServerModule(ServerType.Service, InitializationStage.Networking)]
    public class ServiceHandlerStore
    {

        private Dictionary<ServiceHeaderType, Dictionary<byte, MethodInfo>> packetHandlers;

        public static ServiceHandlerStore Instance { get; private set; }

        public delegate void PacketHandler(ServicePacket reader);
        public event PacketHandler UnknownPacket;

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

        public void HandlePacket(ServicePacket pPacket, ServiceSessionBase pSession)
        {
            CallMethod(pPacket.Header, pPacket.Handling, pPacket, pSession);
        }

        protected void CallMethod(ServiceHeaderType pHeader, byte pType, ServicePacket pPacket, ServiceSessionBase pSession)
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
                    UnknownPacket?.Invoke(pPacket);
                }
            }
            catch (Exception ex)
            {
                EngineLog.Write(EngineLogLevel.Exception, "Error Handling {0} : {1} {2}", pHeader, pType, ex.ToString());
            }

        }
    }
}
