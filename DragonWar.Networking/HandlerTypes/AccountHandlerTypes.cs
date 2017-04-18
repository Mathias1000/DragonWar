using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Networking.HandlerTypes
{
    public class AccountHandlerTypes : IHandlerType
    {
        public byte Header => (byte)HeaderTypes.Account;

        public const byte Bulshit = 1;
    }
}
