using MTMCL.util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Web.Script.Serialization;

namespace MTMCL.Assets
{
    public class Assets
    {
        private readonly WebClient _downloader = new WebClient();
        bool _init = true;
        readonly Versions.VersionJson _ver;
        Dictionary<string, string> _downloadUrlPathPair = new Dictionary<string, string>();
        private readonly string _urlDownloadBase;
        private readonly string _urlResourceBase;
        public Dictionary<string, AssetsEntity> obj;
        public Assets(Versions.VersionJson ver, string urlDownloadBase = null, string urlResourceBase = null)
        {
            _ver = ver;
            _urlDownloadBase = urlDownloadBase ?? Resources.UrlReplacer.getDownloadUrl();
            _urlResourceBase = urlResourceBase ?? Resources.UrlReplacer.getResourceUrl();
            //Run();
            var thread = new Thread(Run);
            thread.Start();
        }

        private void Run()
        {
            try {
                string gameVersion = _ver.assets;
                if (string.IsNullOrWhiteSpace(gameVersion) | gameVersion != "legacy")
                {
                    Logger.log("version isn\'t legacy, return.");
                    return;
                }
                string path = Path.Combine(MeCore.Config.MCPath, "assets\\indexes", gameVersion + ".json");
                if (MeCore.IsServerDedicated)
                {
                    if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath))
                    {
                        path = path.Replace(MeCore.Config.MCPath, Path.Combine(MeCore.BaseDirectory, MeCore.Config.Server.ClientPath));
                    }
                }
                if (File.Exists(path))
                {
                    var sr = new StreamReader(path);
                    var assetsObject = JsonConvert.DeserializeObject<AssetIndex>(File.ReadAllText(path));
                    obj = assetsObject.objects;
                    Logger.log("共", obj.Count.ToString(CultureInfo.InvariantCulture), "项assets");
                    if (assetsObject._virtual)
                    {
                        foreach (KeyValuePair<string, AssetsEntity> entity in obj)
                        {
                            string file = MeCore.Config.MCPath + @"\assets\objects\" + entity.Value.hash.Substring(0, 2) + "\\" + entity.Value.hash;
                            if (MeCore.IsServerDedicated)
                            {
                                if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath))
                                {
                                    file = file.Replace(MeCore.Config.MCPath, Path.Combine(MeCore.BaseDirectory, MeCore.Config.Server.ClientPath));
                                }
                            }
                            if (!File.Exists(file))
                            {
                                _init = false;
                                continue;
                            }
                            try
                            {
                                if (!FileHelper.IfFileVaild(file, entity.Value.size))
                                {
                                    _init = false;
                                    continue;
                                }
                                string finfile = Path.Combine(MeCore.Config.MCPath, "assets\\virtual\\legacy", entity.Key);
                                if (MeCore.IsServerDedicated)
                                {
                                    if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath))
                                    {
                                        finfile = finfile.Replace(MeCore.Config.MCPath, Path.Combine(MeCore.BaseDirectory, MeCore.Config.Server.ClientPath));
                                    }
                                }
                                if (File.Exists(finfile))
                                {
                                    continue;
                                }
                                FileHelper.CreateDirectoryForFile(finfile);
                                File.Copy(file, finfile);
                            }
                            catch (Exception ex)
                            {
                                Logger.error(ex);
                            }
                        }
                    }
                    if (!_init)
                    {
                        Logger.log("load and fix assets failed");
                    }
                }
            } catch {
                Logger.log("load assets index failed");
            }
            
        }
    }
}
