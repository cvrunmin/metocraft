using MTMCL.Lang;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace MTMCL.Versions
{
    public static class VersionReader
    {
        [HandleProcessCorruptedStateExceptions]
        public static VersionJson GetFurtherVersion(string MCPath, string version)
        {
            VersionJson shadow = GetVersion(MCPath, version);
            if (shadow == null) return null;
            if (string.IsNullOrWhiteSpace(shadow.inheritsFrom))
            {
                return shadow;
            }
            VersionJson deep = GetFurtherVersion(MCPath, shadow.inheritsFrom);
            if (deep != null)
            {
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
            }
            else
            {
                Logger.log(string.Format("Inherit version {0} doesn't have a valid base version {1}. Skip it.", shadow.id, shadow.inheritsFrom));
                shadow.errored = true;
                return shadow;
            }
            return shadow;

        }
        public static VersionJson GetFurtherVersion(string MCPath, VersionJson version)
        {
            VersionJson shadow = version;
            if (shadow == null) return null;
            if (string.IsNullOrWhiteSpace(shadow.inheritsFrom))
            {
                return shadow;
            }
            VersionJson deep = GetFurtherVersion(MCPath, shadow.inheritsFrom);
            if (deep != null)
            {
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
            }
            else
            {
                Logger.log(string.Format("Inherit version {0} doesn't have a valid base version {1}. Skip it.", shadow.id, shadow.inheritsFrom));
                shadow.errored = true;
                return shadow;
            }
            return shadow;
        }
        public static VersionJson GetVersion(string MCPath, string version)
        {
            try
            {
                if (Directory.Exists(MCPath))
                {
                    if (!MCPath.Contains("versions")) MCPath = Path.Combine(MCPath, "versions");
                    if (Directory.Exists(MCPath))
                    {
                        if (Directory.Exists(Path.Combine(MCPath, version)))
                        {
                            var a = Newtonsoft.Json.JsonConvert.DeserializeObject<VersionJson>(File.ReadAllText(Path.Combine(MCPath, version, version + ".json")));
                            return a;
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
                Dictionary<string, List<string>> err = new Dictionary<string, List<string>>();
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
                                VersionJson verj = GetFurtherVersion(MCPath, Newtonsoft.Json.JsonConvert.DeserializeObject<VersionJson>(File.ReadAllText(ver)));

                                if (verj != null)
                                    if (!verj.errored)
                                        list.Add(verj);
                                    else
                                    {
                                        if (!err.ContainsKey(verj.inheritsFrom))
                                            err.Add(verj.inheritsFrom, new List<string>());
                                        err[verj.inheritsFrom].Add(verj.id);
                                    }

                            }
                        }
                    }
                }
                if(err.Count > 0) MeCore.Dispatcher.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.InheritMissingBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), err))));
                return list.ToArray();
            }
            catch (Exception e)
            {
                Logger.log(e);
                return new VersionJson[] { };
            }
        }
        public static string GetVersionJsonPath(string MCPath, string version)
        {
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
