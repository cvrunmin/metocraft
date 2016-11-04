using MTMCL.JsonClass;
using MTMCL.util;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Windows.Controls;

namespace MTMCL.Forge
{
    class ForgeVersionList
    {
        public delegate void ForgePageReadyHandle();
        public event ForgePageReadyHandle ForgePageReadyEvent;
        private readonly DataContractJsonSerializer _forgeVerJsonParse = new DataContractJsonSerializer(typeof(ForgeVersion));
        private ForgeVersions _forge;
        public Dictionary<string, string> ForgeDownloadUrl = new Dictionary<string, string>(),
            ForgeChangeLogUrl = new Dictionary<string, string>(),
            ForgeUniversalUrl = new Dictionary<string, string>();
        private Dictionary<int, string> promos;
        public void GetVersion()
        {
            _forge = null;
            var getJson = (HttpWebRequest)WebRequest.Create(Resources.UrlReplacer.getForgeMaven("http://files.minecraftforge.net/maven/net/minecraftforge/forge/json"));
            getJson.Timeout = 10000;
            getJson.ReadWriteTimeout = 10000;
            getJson.UserAgent = "MTMCL" + MeCore.version;
            var thGet = new Thread(new ThreadStart(delegate
            {
                try
                {
                    var getJsonAns = (HttpWebResponse)getJson.GetResponse();
                    MeCore.Dispatcher.Invoke(new Action(() => _forge = JsonConvert.DeserializeObject<ForgeVersions>(new System.IO.StreamReader(getJsonAns.GetResponseStream()).ReadToEnd())));
                    Logger.log("Success to get Forge list");
                    promos = _forge.promos.KVReverse();
                }
                catch (Exception e)
                {
                    Logger.log("Fail to get Forge list");
                    Logger.error(e);
                }                
                ForgePageReadyEvent?.Invoke();
            }));
            thGet.Start();
            /*var webClient = new WebClient();
        webClient.DownloadStringAsync(new Uri(Resources.UrlReplacer.getForgeMaven("http://files.minecraftforge.net/maven/net/minecraftforge/forge/json")));
        webClient.DownloadStringCompleted += WebClient_DownloadStringCompleted;*/
        }
        public string[] GetMCVersionsAvailable ()
        {
            if (_forge == null) return new string[0];
            return _forge.mcversion.Keys.ToArray();
        }
        public ForgeMajorVersions[] GetMCVersionsAvailable_ ()
        {
            if (_forge == null) return new ForgeMajorVersions[0];
            var list = new List<ForgeMajorVersions>();
            foreach (var a in _forge.mcversion.Keys) {
                var b = promos.Where(kvp => System.Text.RegularExpressions.Regex.IsMatch(kvp.Value,a+"-[\\s\\S]"));
                var l = b.Where(kvp => kvp.Value.Contains("latest"));
                var r = b.Where(kvp => kvp.Value.Contains("recommended"));
                ForgeVersion lv = null,rv=null;
                if (l.Count() != 0) lv = _forge.number[ l.First().Key.ToString()];
                if (r.Count() != 0) rv = _forge.number[r.First().Key.ToString()];
                if (lv == null) lv = _forge.number[_forge.mcversion[a].Max().ToString()];
                list.Add(new ForgeMajorVersions() { Latest = lv, Recommended = rv, Version = a, VersionsCount = _forge.mcversion[a].Length });
            }
            return list.ToArray();
        }
        public ForgeVersion[] GetForgeVersions (string mcversion) {
            List<ForgeVersion> list = new List<ForgeVersion>();
            foreach (var verno in _forge.mcversion[mcversion])
            {
                ForgeVersion ver;
                _forge.number.TryGetValue(verno.ToString(), out ver);
                if (ver == null) continue;
                string version = ver.mcversion + "-" + ver.version;
                ver.type = "normal";
                if (!string.IsNullOrWhiteSpace(ver.branch))
                {
                    version = version + "-" + ver.branch;
                    ver.type = ver.branch.Equals(ver.mcversion) ? "normal" : "branch";
                }
                if (ver.urls == null) ver.urls = new Dictionary<string, string>();
                for (int i = 0; i < ver.files.GetLength(0); i++)
                {
                    if (ver.urls.ContainsKey(ver.files[i][1])) continue;
                    ver.urls.Add(ver.files[i][1], _forge.webpath + "/" + version + "/" + _forge.artifact + "-" + version + "-" + ver.files[i][1] + "." + ver.files[i][0]);
                }
                if (promos.ContainsKey(verno)) {
                    ver.type = promos[verno].Contains("latest") ? "latest" : (promos[verno].Contains("recommended") ? "recommended" : "normal");
                }
                list.Add(ver);
            }
            return list.ToArray();
        }
        public object[] GetNew()
        {
            if (_forge == null) return new object[] { };
            ArrayList arrayList = new ArrayList();
            ForgeVersions forge = _forge;
            foreach (var item in forge.promos)
            {
                if (item.Key.Equals("recommended") | item.Key.Equals("latest")) continue;
                ForgeVersion ver = null;
                if (forge.number.TryGetValue(item.Value.ToString(), out ver))
                {
                    bool install = false;
                    string version = ver.mcversion + "-" + ver.version;
                    if (!string.IsNullOrWhiteSpace(ver.branch))
                    {
                        version = version + "-" + ver.branch;
                    }
                    for (int i = 0; i < ver.files.GetLength(0); i++)
                    {
                        if (ver.files[i][1].Equals("changelog"))
                        {
                            ForgeChangeLogUrl[ver.version] = forge.webpath + "/" + version + "/" + forge.artifact + "-" + version + "-" + ver.files[i][1] + "." + ver.files[i][0];
                        }
                        if (ver.files[i][1].Equals("installer"))
                        {
                            install = true;
                            ForgeDownloadUrl[ver.version] = forge.webpath + "/" + version + "/" + forge.artifact + "-" + version + "-" + ver.files[i][1] + "." + ver.files[i][0];
                        }
                    }
                    if (!install)
                    {
                        Logger.log("MinecraftForge " + ver.version, " for ", ver.mcversion, " does not have installer");
                    }
                    arrayList.Add(new object[] { ver.version, ver.mcversion, DateTime.SpecifyKind(util.TimeHelper.UnixTimeStampToDateTime((double)ver.modified), DateTimeKind.Local), item.Key.Contains("latest") ? "latest" : (item.Key.Contains("recommended") ? "recommended" : "") });

                }
            }

            return arrayList.ToArray();
        }
        public object[] GetNew(ForgeVersionListFilter filter)
        {
            if (_forge == null) return new object[] { };
            ArrayList arrayList = new ArrayList();
            ForgeVersions forge = _forge;
            foreach (var ver in GetFilterredVersion(filter))
            {
                bool install = false;
                string version = ver.mcversion + "-" + ver.version;
                if (!string.IsNullOrWhiteSpace(ver.branch))
                {
                    version = version + "-" + ver.branch;
                }
                for (int i = 0; i < ver.files.GetLength(0); i++)
                {
                    if (ver.files[i][1].Equals("changelog"))
                    {
                        ForgeChangeLogUrl[ver.version] = forge.webpath + "/" + version + "/" + forge.artifact + "-" + version + "-" + ver.files[i][1] + "." + ver.files[i][0];
                    }
                    if (ver.files[i][1].Equals("installer"))
                    {
                        install = true;
                        ForgeDownloadUrl[ver.version] = forge.webpath + "/" + version + "/" + forge.artifact + "-" + version + "-" + ver.files[i][1] + "." + ver.files[i][0];
                    }
                    if (ver.files[i][1].Equals("universal"))
                    {
                        ForgeUniversalUrl[ver.version] = forge.webpath + "/" + version + "/" + forge.artifact + "-" + version + "-" + ver.files[i][1] + "." + ver.files[i][0];
                    }
                }
                if (!install)
                {
                    Logger.log("MinecraftForge " + ver.version, " for ", ver.mcversion, " does not have installer");
                }
                arrayList.Add(new object[] { ver.version, ver.mcversion, DateTime.SpecifyKind(util.TimeHelper.UnixTimeStampToDateTime((double)ver.modified), DateTimeKind.Local), "" });
            }

            return arrayList.ToArray();
        }

        public List<string> GetVersionBranch()
        {
            List<string> a = new List<string>();
            foreach (var item in _forge.mcversion)
            {
                a.Add(item.Key);
            }
            return a;
        }
        public int GetFilterredVersionCount(ForgeVersionListFilter filter)
        {
            int i = 0;
            if (filter.ShowRecommended)
            {
                i += _forge.promos.Keys.SkipWhile(key => !key.Contains("recommended")).ToList().Count;
                ///Repeated version
                i -= 1;
            }
            if (filter.ShowLatest)
            {
                i += _forge.promos.Keys.SkipWhile(key => !key.Contains("latest")).ToList().Count;
                ///Repeated version
                i -= 1;
            }
            if (filter.ShowNonTag)
            {
                foreach (var item in _forge.mcversion)
                {
                    i += item.Value.Length;
                    i -= _forge.promos.Count;
                }
            }
            if (filter.HiddenVersion.Count != 0)
            {
                foreach (var item in _forge.mcversion.Where(kvg => filter.HiddenVersion.Contains(kvg.Key)))
                {
                    i -= item.Value.Length;
                }
            }
            return i;
        }
        public List<ForgeVersion> GetFilterredVersion(ForgeVersionListFilter filter)
        {
            List<ForgeVersion> list = new List<ForgeVersion>();
            var ignore = new HashSet<int>();
            if (!filter.ShowRecommended)
            {
                var a = _forge.promos.Where(kvg => kvg.Key.Contains("recommended") & !kvg.Key.Equals("recommended"));
                foreach (var item in a)
                {
                    ignore.Add(item.Value);
                }
            }
            if (!filter.ShowLatest)
            {
                var a = _forge.promos.Where(kvg => kvg.Key.Contains("latest") & !kvg.Key.Equals("latest"));
                foreach (var item in a)
                {
                    ignore.Add(item.Value);
                }
            }
            if (!filter.ShowNonTag)
            {
                var a = _forge.promos.Where(kvg => !kvg.Key.Equals("recommended") & !kvg.Key.Equals("latest"));
                var b = new List<int>();
                foreach (var item in a)
                {
                    b.Add(item.Value);
                }
                foreach (var item in _forge.mcversion)
                {
                    foreach (var i1 in item.Value.Where(e => !b.Contains(e)))
                    {
                        ignore.Add(i1);
                    }
                }
            }
            if (filter.HiddenVersion.Count != 0)
            {
                foreach (var item in _forge.mcversion.Where(kvg => filter.HiddenVersion.Contains(kvg.Key)))
                {
                    foreach (var i1 in item.Value)
                    {
                        ignore.Add(i1);
                    }
                }
            }
            var si = Int32SetToStringSet(ignore);
            foreach (KeyValuePair<string, ForgeVersion> it in _forge.number)
            {
                if (si.Contains(it.Key) | string.IsNullOrWhiteSpace(it.Key)) continue;
                if (it.Key.Equals("0")) continue;
                list.Add(it.Value);
            }
            return list;
        }
        private HashSet<string> Int32SetToStringSet(HashSet<int> list)
        {
            var a = new HashSet<string>();
            foreach(var i in list)
            {
                a.Add(i.ToString());
            }
            return a;
        }
    }
    public class ForgeVersionListFilter : IEquatable<ForgeVersionListFilter>
    {
        public bool ShowNonTag { get; set; }
        public bool ShowLatest { get; set; }
        public bool ShowRecommended { get; set; }
        public List<string> HiddenVersion { get; set; }
        public ForgeVersionListFilter() { }

        public bool Equals(ForgeVersionListFilter other)
        {
            return ShowNonTag == other.ShowNonTag & ShowLatest == other.ShowLatest & ShowRecommended == other.ShowRecommended & HiddenVersion.Equals(other.HiddenVersion);
        }
    }
    public class ForgeMajorVersions
    {
        public ForgeVersion Latest { get; set; }
        public ForgeVersion Recommended { get; set; }
        public int VersionsCount { get; set; }
        public string Version { get; set; }
    }
}

