using DragonWar.Networking.Network;

using System.Collections.Concurrent;

namespace DragonWar.Service.Network
{
    public class SessionManagerBase<TSession> where TSession : SessionBase
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
            if (Sessions.TryRemove(SessionId, out TSession mSession))
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
