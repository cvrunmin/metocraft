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
    public class Assets : IDisposable
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
            try
            {
                string gameVersion = _ver.assets;
                /*if (string.IsNullOrWhiteSpace(gameVersion) | gameVersion != "legacy")
                {
                    Logger.log("version isn\'t legacy, return.");
                    return;
                }*/
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
                    Logger.log(obj.Count.ToString(CultureInfo.InvariantCulture), " assets in total");
                    List<string> failed = new List<string>();
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
                            failed.Add(file);
                            _init = false;
                            continue;
                        }
                        try
                        {
                            if (!FileHelper.IfFileVaild(file, entity.Value.size))
                            {
                                failed.Add(file);
                                _init = false;
                                continue;
                            }
                            if (assetsObject._virtual)
                            {
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
                        }
                        catch (Exception ex)
                        {
                            Logger.error(ex);
                        }
                    }
                    if (failed.Count != 0) {

                    }
                    if (!_init)
                    {
                        Logger.log("load and fix assets failed");
                    }
                }
            }
            catch
            {
                Logger.log("load assets index failed");
            }

        }

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置 Managed 狀態 (Managed 物件)。
                    _downloader.Dispose();
                    _downloadUrlPathPair = null;
                    obj = null;
                }
                // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
                // TODO: 將大型欄位設為 null。
                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
        // ~Assets() {
        //   // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose()
        {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果上方的完成項已被覆寫，即取消下行的註解狀態。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
