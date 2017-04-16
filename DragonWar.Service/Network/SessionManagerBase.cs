using DragonWar.Utils.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Service.Network
{
    public class SessionManagerBase<TSession> where TSession : ServiceSessionBase
    {
        public int CountOfSessions => Sessions.Count;

        private ConcurrentQueue<ushort> SessionIds;

        private ConcurrentDictionary<ushort, TSession> Sessions { get; set; }

        public SessionManagerBase(ushort MaxSessions)
        {
            SessionIds = new ConcurrentQueue<ushort>();
            SessionIds.Fill(1, MaxSessions);
            Sessions = new ConcurrentDictionary<ushort, TSession>();
        }

        public bool AddSession(TSession mSession)
        {
            ushort SessionId;
            if (SessionIds.TryDequeue(out SessionId))
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
            TSession mSession;
            if (Sessions.TryRemove(SessionId, out mSession))
            {
                SessionIds.Enqueue(SessionId);

                return true;
            }

            return false;
        }

        public bool RemoveSession(TSession mSession) => RemoveSession(mSession.SessiondId);

        public bool GetSessionById(ushort Id, out TSession mSession) => Sessions.TryGetValue(Id, out mSession);
       
    }
}
