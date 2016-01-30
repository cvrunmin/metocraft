using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MetoCraft.Profile
{
    public class MProfiles
    {
        public class MProfile {
            public string name;
            public DirectoryInfo gameDir;
            public string lastVersionId;
            public string javaDir;
            public string javaArgs;
            public Resolution resolution;
            public HashSet<MinecraftReleaseType> allowedReleaseTypes;
            public string playerUUID;
            public bool useHopperCrashService;
            public LauncherVisibilityRule launcherVisibilityOnGameClose;
            public class Resolution
            {
                private int width;
                private int height;

                public int Width { get; set; }

                public int Height { get; set; }

                public Resolution() { }

                public Resolution(Resolution resolution) : this(resolution.Width, resolution.Height) { }
                public Resolution(int width, int height)
                {
                    Width = width;
                    Height = height;
                }
            }
            public class LauncherVisibilityRule
            {
                public const string HIDE_LAUNCHER = "Hide launcher and re-open when game closes";
                public const string CLOSE_LAUNCHER = "Close launcher when game starts";
                public const string DO_NOTHING = "Keep the launcher open";

                private readonly string name;

                private LauncherVisibilityRule(string name)
                {
                    this.name = name;
                }

                public string getName()
                {
                    return name;
                }

                public string toString()
                {
                    return name;
                }
            }
            public class MinecraftReleaseType
            {
            }
        }
    }
}
