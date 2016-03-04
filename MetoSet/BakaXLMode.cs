namespace KMCCC.Launcher
{
    class BakaXLMode : LaunchMode
    {
        public override bool Operate(LauncherCore core, MinecraftLaunchArguments args)
        {
            string id = (args.GetType().GetField("Version", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(args) as KMCCC.Launcher.Version).Id;
            core.CopyVersionDirectory("mods", id);
            core.CopyVersionDirectory("coremods", id);
            core.CopyVersionDirectory("saves", id);
            core.CopyVersionDirectory("resourcepacks", id);
            core.CopyVersionDirectory("config", id);
            args.Tokens["game_directory"] = string.Format(@".\versions\{0}\", id);
            return true;
        }
    }
}
