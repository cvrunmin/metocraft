﻿using MTMCL.util;
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
        readonly KMCCC.Launcher.Version _ver;
        Dictionary<string, string> _downloadUrlPathPair = new Dictionary<string, string>();
        private readonly string _urlDownloadBase;
        private readonly string _urlResourceBase;
        public Dictionary<string, AssetsEntity> obj;
        public Assets(KMCCC.Launcher.Version ver, string urlDownloadBase = null, string urlResourceBase = null)
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
            string gameVersion = _ver.Assets;
            if (string.IsNullOrWhiteSpace(gameVersion) | gameVersion != "legacy")
            {
                Logger.log("version isn\'t legacy, return.");
                return;
            }
            string path = Path.Combine(MeCore.Config.MCPath, "assets\\indexes", gameVersion + ".json");
            if (File.Exists(path))
            {
                var sr = new StreamReader(path);
                var assetsObject = LitJson.JsonMapper.ToObject<AssetIndex>(sr.ReadToEnd());
                obj = assetsObject.objects;
                Logger.log("共", obj.Count.ToString(CultureInfo.InvariantCulture), "项assets");
                if (assetsObject._virtual)
                {
                    foreach (KeyValuePair<string, AssetsEntity> entity in obj)
                    {
                        string file = MeCore.Config.MCPath + @"\assets\objects\" + entity.Value.hash.Substring(0, 2) + "\\" + entity.Value.hash;
                        if (!File.Exists(file))
                        {
                            _init = false;
                            break;
                        }
                        try
                        {
                            if (!FileHelper.IfFileVaild(file, entity.Value.size))
                            {
                                _init = false;
                                break;
                            }
                            string finfile = Path.Combine(MeCore.Config.MCPath, "assets\\virtual\\legacy", entity.Key);
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
        }
    }
}
