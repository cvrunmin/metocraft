using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MTMCL.Versions
{
    public static class VersionReader
    {
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
                            return LitJson.JsonMapper.ToObject<VersionJson>(new LitJson.JsonReader(new StreamReader(File.OpenRead(Path.Combine(MCPath, version, version + ".json")))));
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
                                list.Add(LitJson.JsonMapper.ToObject<VersionJson>(new LitJson.JsonReader(new StreamReader(File.OpenRead(ver)))));
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
