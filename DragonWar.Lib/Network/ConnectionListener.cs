using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Lib.Network
{
    public abstract class ConnectionListener
    {
        public abstract void StartListening();
        public abstract void StopListening();

        protected virtual void OnNewConnection(Connection connection)
        {
            NewConnection?.Invoke(this, new NewConnectionEventArgs(connection));
        }

        public event EventHandler<NewConnectionEventArgs> NewConnection;
    }
}
