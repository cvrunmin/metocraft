using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MTMCL.Versions
{
    public static class VersionReader
    {
        public static VersionJson GetFurtherVersion(string MCPath, string version) {
            VersionJson shadow = GetVersion(MCPath, version);
            if (string.IsNullOrWhiteSpace(shadow.inheritsFrom)) {
                return shadow;
            }
            VersionJson deep = GetFurtherVersion(MCPath, shadow.inheritsFrom);
            if (string.IsNullOrWhiteSpace(shadow.assets))
            {
                shadow.assets = deep.assets ?? "legacy";
            }
            if (string.IsNullOrWhiteSpace(shadow.mainClass))
            {
                shadow.mainClass = deep.mainClass;
            }
            if (string.IsNullOrWhiteSpace(shadow.minecraftArguments))
            {
                shadow.minecraftArguments = deep.minecraftArguments;
            }
            if (string.IsNullOrWhiteSpace(shadow.jar))
            {
                shadow.jar = deep.jar;
            }
             shadow.libraries = shadow.libraries.Concat(deep.libraries).ToArray();
            return shadow;
        }
        public static VersionJson GetFurtherVersion(string MCPath, VersionJson version)
        {
            VersionJson shadow = version;
            if (string.IsNullOrWhiteSpace(shadow.inheritsFrom))
            {
                return shadow;
            }
            VersionJson deep = GetFurtherVersion(MCPath, shadow.inheritsFrom);
            if (string.IsNullOrWhiteSpace(shadow.assets))
            {
                shadow.assets = deep.assets ?? "legacy";
            }
            if (string.IsNullOrWhiteSpace(shadow.mainClass))
            {
                shadow.mainClass = deep.mainClass;
            }
            if (string.IsNullOrWhiteSpace(shadow.minecraftArguments))
            {
                shadow.minecraftArguments = deep.minecraftArguments;
            }
            if (string.IsNullOrWhiteSpace(shadow.jar))
            {
                shadow.jar = deep.jar;
            }
            shadow.libraries = shadow.libraries.Concat(deep.libraries).ToArray();
            return shadow;
        }
        public static VersionJson GetVersion(string MCPath, string version) {
            try
            {
                if (Directory.Exists(MCPath))
                {
                    if (!MCPath.Contains("versions")) MCPath = Path.Combine(MCPath, "versions");
                    if (Directory.Exists(MCPath))
                    {
                        if (Directory.Exists(Path.Combine(MCPath, version)))
                        {
                            return Newtonsoft.Json.JsonConvert.DeserializeObject<VersionJson>(File.ReadAllText(Path.Combine(MCPath, version, version + ".json")));
                        }
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                Logger.log(e);
                return null;
            }
        }
        public static VersionJson[] GetVersion(string MCPath)
        {
            try
            {
                List<VersionJson> list = new List<VersionJson>();
                if (Directory.Exists(MCPath))
                {
                    if (!MCPath.Contains("versions")) MCPath = Path.Combine(MCPath, "versions");
                    if (Directory.Exists(MCPath))
                    {
                        foreach (var item in Directory.EnumerateDirectories(MCPath))
                        {
                            string ver = "";
                            if (File.Exists(ver = Path.Combine(item, item.Substring(item.LastIndexOf('\\') + 1) + ".json")))
                            {
                                list.Add(GetFurtherVersion(MCPath, Newtonsoft.Json.JsonConvert.DeserializeObject<VersionJson>(File.ReadAllText(ver))));
                            }
                        }
                    }
                }
                return list.ToArray();
            }
            catch (Exception e)
            {
                Logger.log(e);
                return new VersionJson[] { };
            }
        }
        public static string GetVersionJsonPath(string MCPath, string version) {
            try
            {
                if (Directory.Exists(MCPath))
                {
                    if (!MCPath.Contains("versions")) MCPath = Path.Combine(MCPath, "versions");
                    if (Directory.Exists(MCPath))
                    {
                        if (Directory.Exists(Path.Combine(MCPath, version)))
                        {
                            return Path.Combine(MCPath, version, version + ".json");
                        }
                    }
                }
                return "";
            }
            catch (Exception e)
            {
                Logger.log(e);
                return "";
            }
        }
    }
}
