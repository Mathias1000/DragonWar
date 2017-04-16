using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Utils.Network
{
    public class SessionEventArgs<TSession> : EventArgs
    {
        public TSession Session { get; private set; }

        public SessionEventArgs(TSession mSesion)
        {
            Session = mSesion;
        }

 
    }
}
