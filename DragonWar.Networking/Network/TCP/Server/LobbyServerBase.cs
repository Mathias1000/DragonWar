using DragonWar.Networking.Network.TCP.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using DragonWar.Networking.Packet;
using DragonWar.Networking.Network.Processing;
using DragonWar.Networking.Packet.Lobby.Protocol;
using DragonWar.Networking.Packet.Lobby.Server;

namespace DragonWar.Networking.Network.TCP.Server
{
    public class LobbyServerBase<TLobbyClient> : ServerBase  where TLobbyClient : LobbyClientBase
    {
        public DataProcessingQueue<LobbyClientBase, LobbyPacket> ProcessingQueue { get; set; }

        private int WorkCount = 0;

        public LobbyServerBase(string ip, int port,int WorkCount) : base(ip, port)
        {
            ProcessingQueue = new DataProcessingQueue<LobbyClientBase, LobbyPacket>();
            WorkCount = 0;
        }
        public virtual void Start()
        {
            ProcessingQueue.StartWorkerThreads(WorkCount);
        }
        public virtual void Stop()
        {
            ProcessingQueue.Stop();
        }


        protected void SendVerfiryConnectPacket(LobbyClientBase Session,bool SendingFull)
        {
            if(!SendingFull)
            {
                ServerStatusPacket Full = new ServerStatusPacket
                {
                     State = ServerState.Full,
                };
                Session.SendPacket(Full);

            }
            else
            {
                LobbyHandShake mHandshake = new LobbyHandShake
                {
                    Key = LobbyHandShake.GenKey(),
                };

                Session.HandShakeKey = mHandshake.Key;

                Session.SendPacket(mHandshake);
            }
        }
    }
}
