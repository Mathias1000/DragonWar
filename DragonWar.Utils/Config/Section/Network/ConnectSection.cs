using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonWar.Utils.Config.Section
{
    public class ConnectSection
    {
     
        public virtual string ConnectIP { get; set; } = "127.0.0.1";

        public virtual int ConnectPort { get; set; } = 8800;

        public virtual string ConnectPassword { get; set; } = "Dubistdoof";

    }

}