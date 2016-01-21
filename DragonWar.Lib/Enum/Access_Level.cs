using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Lib.Enum
{
    public enum Access_Level : byte
    {
        Player = 0,
        VIP = 1,
        Mod = 2,
        GM = 3,
        Dev = 4,
        Admin = 5,
    }
}
