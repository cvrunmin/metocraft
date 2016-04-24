using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTMCL.Versions;

namespace MTMCL.Launch
{
    public static class LaunchHandler
    {
        public static string CreateArgument(LaunchGameInfo info, Dictionary<string, string> addArg, string addJvmArg)
        {
            StringBuilder sb = new StringBuilder();
            //Append Jvm Arguments
            sb.Append("-Xincgc -Xmx").Append(info.JavaXmx).Append("M").Append(addJvmArg);
            sb.Append(" -Djava.library.path=\"").Append(info.MCPath).Append("\\$natives").Append("\" ");
            sb.Append(" -cp \"");
            foreach (var item in info.Version.libraries.ToUniversalLibrary())
            {
                if (!item.isNative)
                    sb.Append(item.path).Append(";");
            }
            sb.Append(info.MCPath).Append("\\versions\\").Append(info.Version.id).Append("\\").Append(info.Version.id).Append(".jar\" ");
            sb.Append(info.Version.mainClass);
            StringBuilder argsb = new StringBuilder(info.Version.minecraftArguments);
            foreach (var item in addArg)
            {
                argsb.Replace("${" + item.Key + "}", item.Value);
            }
            sb.Append(" ").Append(argsb);
            return sb.ToString();
        }
    }
}
