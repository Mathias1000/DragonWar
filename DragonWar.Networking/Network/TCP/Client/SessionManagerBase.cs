using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.Network.TCP.Client
{
    public class SessionManagerBase<TClient> where TClient : ClientBase
    {
        public int CountOfSessions => Sessions.Count;

        private ConcurrentQueue<ushort> SessionIds;

        private ConcurrentDictionary<ushort, TClient> Sessions { get; set; }

        public SessionManagerBase(ushort MaxSessions)
        {
            SessionIds = new ConcurrentQueue<ushort>();
            SessionIds.Fill(1, MaxSessions);
            Sessions = new ConcurrentDictionary<ushort, TClient>();
        }

        public bool AddSession(TClient mSession)
        {
            if (SessionIds.TryDequeue(out ushort SessionId))
            {
                if (Sessions.TryAdd(SessionId, mSession))
                {
                    mSession.SessiondId = SessionId;

                    return true;
                }
            }

            return false;
        }

        public bool RemoveSession(ushort SessionId)
        {
            if (Sessions.TryRemove(SessionId, out TClient mSession))
            {
                SessionIds.Enqueue(SessionId);

                return true;
            }

            return false;
        }

        public bool RemoveSession(TClient mSession) => RemoveSession(mSession.SessiondId);

        public bool GetSessionById(ushort Id, out TClient mSession) => Sessions.TryGetValue(Id, out mSession);

    }
}
