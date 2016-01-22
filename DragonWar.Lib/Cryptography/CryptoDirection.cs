using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Lib.Cryptography
{
    [Flags]
    public enum CryptoDirection
    {
        None = 0,
        Outgoing = 1,
        Incoming = 2
    }
}
