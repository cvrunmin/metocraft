using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTMCL.Launch
{
    public abstract class LaunchMode
    {
        public static readonly LaunchMode Normal = new NormalLaunchMode();
        public static readonly LaunchMode BMCL = new BMCLLaunchMode();
        public static readonly LaunchMode BakaXL = new BakaXLLaunchMode();
        public abstract void Do(LaunchGameInfo info, ref Dictionary<string, string> addArg);
        public abstract void DoAfter(LaunchGameInfo info);
        public abstract string ModeType { get; }
        public static LaunchMode GetMode(string type)
        {
            switch (type)
            {
                case "Normal":
                    return Normal;
                case "BMCL":
                    return BMCL;
                case "BakaXL":
                    return BakaXL;
                default:
                    return Normal;
            }
        }
    }
    public class NormalLaunchMode : LaunchMode
    {
        public override string ModeType
        {
            get
            {
                return "Normal";
            }
        }

        public override void Do(LaunchGameInfo info, ref Dictionary<string, string> addArg) { }

        public override void DoAfter(LaunchGameInfo info) { }
    }
    public class BMCLLaunchMode : LaunchMode
    {
        public override string ModeType
        {
            get
            {
                return "BMCL";
            }
        }

        public override void Do(LaunchGameInfo info, ref Dictionary<string, string> addArg)
        {
            util.FileHelper.dircopy(System.IO.Path.Combine(info.MCPath, "mods"), System.IO.Path.Combine(info.MCPath, "versions", info.Version.id, "mods"));
            util.FileHelper.dircopy(System.IO.Path.Combine(info.MCPath, "coremods"), System.IO.Path.Combine(info.MCPath, "versions", info.Version.id, "coremods"));
        }

        public override void DoAfter(LaunchGameInfo info)
        {
            util.FileHelper.dirmove(System.IO.Path.Combine(info.MCPath, "mods"), System.IO.Path.Combine(info.MCPath, "versions", info.Version.id, "mods"));
            util.FileHelper.dirmove(System.IO.Path.Combine(info.MCPath, "coremods"), System.IO.Path.Combine(info.MCPath, "versions", info.Version.id, "coremods"));
        }
    }
    public class BakaXLLaunchMode : LaunchMode
    {
        public override string ModeType
        {
            get
            {
                return "BakaXL";
            }
        }

        public override void Do(LaunchGameInfo info, ref Dictionary<string, string> addArg)
        {
            util.FileHelper.dircopy(System.IO.Path.Combine(info.MCPath, "mods"), System.IO.Path.Combine(info.MCPath, "versions", info.Version.id, "mods"));
            util.FileHelper.dircopy(System.IO.Path.Combine(info.MCPath, "coremods"), System.IO.Path.Combine(info.MCPath, "versions", info.Version.id, "coremods"));
            util.FileHelper.dircopy(System.IO.Path.Combine(info.MCPath, "saves"), System.IO.Path.Combine(info.MCPath, "versions", info.Version.id, "saves"));
            util.FileHelper.dircopy(System.IO.Path.Combine(info.MCPath, "resourcepacks"), System.IO.Path.Combine(info.MCPath, "versions", info.Version.id, "resourcepacks"));
            util.FileHelper.dircopy(System.IO.Path.Combine(info.MCPath, "config"), System.IO.Path.Combine(info.MCPath, "versions", info.Version.id, "config"));
            addArg["game_directory"] = string.Format(@".\versions\{0}\", info.Version.id);
        }

        public override void DoAfter(LaunchGameInfo info)
        {
            util.FileHelper.dirmove(System.IO.Path.Combine(info.MCPath, "mods"), System.IO.Path.Combine(info.MCPath, "versions", info.Version.id, "mods"));
            util.FileHelper.dirmove(System.IO.Path.Combine(info.MCPath, "coremods"), System.IO.Path.Combine(info.MCPath, "versions", info.Version.id, "coremods"));
            util.FileHelper.dirmove(System.IO.Path.Combine(info.MCPath, "saves"), System.IO.Path.Combine(info.MCPath, "versions", info.Version.id, "saves"));
            util.FileHelper.dirmove(System.IO.Path.Combine(info.MCPath, "resourcepacks"), System.IO.Path.Combine(info.MCPath, "versions", info.Version.id, "resourcepacks"));
            util.FileHelper.dirmove(System.IO.Path.Combine(info.MCPath, "config"), System.IO.Path.Combine(info.MCPath, "versions", info.Version.id, "config"));
        }
    }
}
