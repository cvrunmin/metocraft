using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetoCraft.Profile
{
    public sealed class Profile
    {
        public string oldName { get; set; }
        public string name { get; set; }

        public string version { get; set; }

        public int winSizeX { get; set; }

        public int winSizeY { get; set; }

        public int Xmx { get; set; }
        [Obsolete]
        public string auth { get; set; }
        [Obsolete]
        public string token { get; set; }
    }
}
