using MTMCL.JsonClass;
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
        private ForgeVersion _forge;
        public Dictionary<string, string> ForgeDownloadUrl = new Dictionary<string, string>(),
            ForgeChangeLogUrl = new Dictionary<string, string>();
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
                    MeCore.Dispatcher.Invoke(new Action(() => _forge = JsonConvert.DeserializeObject<ForgeVersion>(new System.IO.StreamReader(getJsonAns.GetResponseStream()).ReadToEnd())));
                    Logger.log("Success to get Forge list");
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

        public object[] GetNew()
        {
            if (_forge == null) return new object[] { };
            ArrayList arrayList = new ArrayList(_forge.promos.Count);
            ForgeVersion forge = _forge;
            foreach (var item in forge.promos)
            {
                ForgeVersion.Version ver = null;
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
                    //Logger.log("获取Forge", ver.version);
                }
            }

            return arrayList.ToArray();
        }
        public object[] GetNew(ForgeVersionListFilter filter)
        {
            if (_forge == null) return new object[] { };
            ArrayList arrayList = new ArrayList(GetFilterredVersionCount(filter));
            ForgeVersion forge = _forge;
            foreach (var item in forge.promos)
            {
                ForgeVersion.Version ver = null;
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
                    //Logger.log("获取Forge", ver.version);
                }
            }

            return arrayList.ToArray();
        }
        public TreeViewItem[] Get()
        {
            ArrayList arrayList = new ArrayList(_forge.number.Count);
            TreeViewItem treeViewItem = new TreeViewItem();
            arrayList.Add(treeViewItem);
            ForgeVersion forge = _forge;
            foreach (var item in forge.number.Values)
            {
                bool install = false;
                for (int i = 0; i < item.files.GetLength(1); i++)
                {
                    if (item.files[i][2].Equals("changelog"))
                    {
                        ForgeChangeLogUrl[item.version] = forge.artifact + "-" + item.mcversion + "-" + item.version + "-" + item.files[i][1] + "." + item.files[i][0];
                    }
                    if (item.files[i][2].Equals("installer"))
                    {
                        install = true;
                        ForgeDownloadUrl[item.version] = forge.artifact + "-" + item.mcversion + "-" + item.version + "-" + item.files[i][1] + "." + item.files[i][0];
                    }
                }
                if (!install)
                {
                    Logger.log("MinecraftForge" + item.version, " for ", item.mcversion, "does not have installer");
                }
                if (treeViewItem.Header == null)
                {
                    treeViewItem.Header = item.mcversion;
                }
                else
                {
                    if (treeViewItem.Header.ToString() != item.mcversion)
                    {
                        treeViewItem = new TreeViewItem();
                        arrayList.Add(treeViewItem);
                        treeViewItem.Header = item.mcversion;
                    }
                }
                Logger.log(treeViewItem.Header.ToString());
                Logger.log(item.mcversion);

                treeViewItem.Items.Add(item.version);
                Logger.log("获取Forge", item.version);
            }

            return arrayList.ToArray(typeof(TreeViewItem)) as TreeViewItem[];
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
            if (filter.HiddenVersion.Length != 0)
            {
                foreach (var item in _forge.mcversion.Where(kvg => filter.HiddenVersion.Contains(kvg.Key)))
                {
                    i -= item.Value.Length;
                }
            }
            return i;
        }
        public List<ForgeVersion.Version> GetFilterredVersion(ForgeVersionListFilter filter)
        {
            List<ForgeVersion.Version> list = new List<ForgeVersion.Version>();
            var ignore = new HashSet<int>();
            if (!filter.ShowRecommended)
            {
                var a = _forge.promos.SkipWhile(kvg => !kvg.Key.Contains("recommended") & kvg.Key.Equals("recommended"));
                foreach(var item in a)
                {
                    ignore.Add(item.Value);
                }
            }
            if (!filter.ShowLatest)
            {
                var a = _forge.promos.SkipWhile(kvg => !kvg.Key.Contains("latest") & kvg.Key.Equals("latest"));
                foreach (var item in a)
                {
                    ignore.Add(item.Value);
                }
            }
            if (!filter.ShowNonTag)
            {
                var a = _forge.promos.SkipWhile(kvg => kvg.Key.Equals("recommended") | kvg.Key.Equals("latest"));
                var b = new List<int>();
                foreach (var item in a)
                {
                    b.Add(item.Value);
                }
                foreach (var item in _forge.mcversion) {
                    foreach (var i1 in item.Value.Where(e => !b.Contains(e))) {
                        ignore.Add(i1);
                    }
                }
            }
            if (filter.HiddenVersion.Length != 0)
            {
                foreach (var item in _forge.mcversion.Where(kvg => filter.HiddenVersion.Contains(kvg.Key)))
                {
                    foreach(var i1 in item.Value)
                    {
                        ignore.Add(i1);
                    }
                }
            }
            foreach (var item in _forge.number.SkipWhile(kvg => ignore.Cast<string>().Contains(kvg.Key))) {
                list.Add(item.Value);
            }
            return list;
        }
    }
    public class ForgeVersionListFilter
    {
        public bool ShowNonTag { get; set; }
        public bool ShowLatest { get; set; }
        public bool ShowRecommended { get; set; }
        public string[] HiddenVersion { get; set; }
        public ForgeVersionListFilter() { }
    }
}

