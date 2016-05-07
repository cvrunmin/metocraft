using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System;
using Newtonsoft.Json;
using System.ComponentModel;

namespace MTMCL.Versions
{
    public class VersionJsonDisposable : IDisposable
    {
        #region IDisposable Support
        // Pointer to an external unmanaged resource.
        private IntPtr handle;
        // Other managed resource this class uses.
        private Component component = new Component();
        private bool disposedValue = false; // 偵測多餘的呼叫
        protected VersionJsonDisposable() { }
        public VersionJsonDisposable(IntPtr handle)
        {
            this.handle = handle;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置 Managed 狀態 (Managed 物件)。
                    component.Dispose();
                }

                // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
                // TODO: 將大型欄位設為 null。
                CloseHandle(handle);
                handle = IntPtr.Zero;
                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
        ~VersionJsonDisposable()
        {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(false);
        }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose()
        {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果上方的完成項已被覆寫，即取消下行的註解狀態。
            GC.SuppressFinalize(this);
        }

        [System.Runtime.InteropServices.DllImport("Kernel32")]
        private extern static bool CloseHandle(IntPtr handle);
        #endregion
    }
    [DataContract]
    public class SimplifyVersionJson : VersionJsonDisposable
    {
        [DataMember]
        public string assets;
        [DataMember]
        public string id;
        [DataMember]
        public string mainClass;
        [DataMember]
        public string minecraftArguments;
        [DataMember]
        public int minimumLauncherVersion;
        [DataMember]
        public string releaseTime;
        [DataMember]
        public string time;
        [DataMember]
        public string type;
        [DataMember]
        public string inheritsFrom;
        [DataMember]
        public AssetIndex assetIndex;
        [DataMember]
        public Downloads downloads;
        [DataMember]
        public Library[] libraries;
        [DataContract]
        public class Library
        {
            [DataMember]
            public string name;
            [DataMember]
            public string url;
            [DataMember]
            public Rule[] rules;
            [DataContract]
            public class Rule
            {
                [DataMember]
                public string action;
                [DataMember]
                public OS os;
                [DataContract]
                public class OS {[DataMember]public string name; }
            }
            [DataMember]
            public Extract extract;
            [DataContract]
            public class Extract
            {
                [DataMember]
                public string[] exclude;
            }
            [DataMember]
            public Native natives;
            [DataContract]
            public class Native
            {
                [DataMember]
                public string linux;
                [DataMember]
                public string osx;
                [DataMember]
                public string windows;
            }
        }
        [DataContract]
        public class AssetIndex
        {
            [DataMember]
            public bool known;
            [DataMember]
            public string id;
            [DataMember]
            public string sha1;
            [DataMember]
            public int size;
            [DataMember]
            public string url;
            [DataMember]
            public int totalSize;
        }
    }
    [DataContract]
    public class VersionJson : VersionJsonDisposable
    {
        [IgnoreDataMember]
        [JsonIgnore]
        public bool errored;
        [DataMember]
        public string assets;
        [DataMember]
        public string id;
        [DataMember]
        public string jar;
        [DataMember]
        public string mainClass;
        [DataMember]
        public string minecraftArguments;
        [DataMember]
        public int minimumLauncherVersion;
        [DataMember]
        public string releaseTime;
        [DataMember]
        public string time;
        [DataMember]
        public string type;
        [DataMember]
        public string inheritsFrom;
        [DataMember]
        public AssetIndex assetIndex;
        [DataMember]
        public Downloads downloads;
        [DataMember]
        public Library[] libraries;
        [DataContract]
        public class Library : SimplifyVersionJson.Library
        {
            [DataMember]
            public Downloads download;
            [DataContract]
            public class Downloads
            {
                [DataMember]
                public Artifact artifact;
                [DataContract]
                public class Artifact
                {
                    [DataMember]
                    public int size;
                    [DataMember]
                    public string sha1;
                    [DataMember]
                    public string path;
                    [DataMember]
                    public string url;
                }
                [DataMember]
                public Classifier classifiers;
                [DataContract]
                public class Classifier
                {
                    [DataMember(Name = "natives-linux")]
                    [JsonProperty("natives-linux")]
                    public Artifact natives_linux;
                    [DataMember(Name = "natives-osx")]
                    [JsonProperty("natives-osx")]
                    public Artifact natives_osx;
                    [DataMember(Name = "natives-windows")]
                    [JsonProperty("natives-windows")]
                    public Artifact natives_windows;
                }
            }
        }
        [DataContract]
        public class AssetIndex
        {
            [DataMember]
            public bool known;
            [DataMember]
            public string id;
            [DataMember]
            public string sha1;
            [DataMember]
            public int size;
            [DataMember]
            public string url;
            [DataMember]
            public int totalSize;
        }
    }
    [DataContract]
    public class Downloads
    {
        [DataMember]
        public Side client;
        [DataMember]
        public Side server;
        [DataContract]
        public class Side
        {
            [DataMember]
            public string sha1;
            [DataMember]
            public int size;
            [DataMember]
            public string url;
        }
    }
    public class LibraryUniversal
    {
        public string name { get; set; }
        public string sha1 { get; set; }
        public int size { get; set; }
        public string path { get; set; }
        public string url { get; set; }
        public bool isNative { get; set; }
        public SimplifyVersionJson.Library.Extract extract { get; set; }
    }

    public static class VersionJsonController
    {
        public static SimplifyVersionJson Simplify(this VersionJson json)
        {
            SimplifyVersionJson simjson = new SimplifyVersionJson
            {
                id = json.id,
                type = json.type,
                assetIndex = new SimplifyVersionJson.AssetIndex
                {
                    id = json.assetIndex.id,
                    sha1 = json.assetIndex.sha1,
                    size = json.assetIndex.size,
                    totalSize = json.assetIndex.totalSize,
                    url = json.assetIndex.url,
                    known = true
                },
                assets = json.assets,
                downloads = json.downloads,
                libraries = json.libraries.Simplify(),
                mainClass = json.mainClass,
                minecraftArguments = json.minecraftArguments,
                minimumLauncherVersion = json.minimumLauncherVersion,
                time = json.time,
                releaseTime = json.releaseTime
            };
            return simjson;
        }
        public static SimplifyVersionJson.Library[] Simplify(this VersionJson.Library[] list)
        {
            SimplifyVersionJson.Library[] simlist = new SimplifyVersionJson.Library[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                simlist[i] = new SimplifyVersionJson.Library
                {
                    name = list[i].name,
                    extract = list[i].extract,
                    natives = list[i].natives,
                    rules = list[i].rules
                };
            }
            return simlist;
        }
        public static void BackupJson(string file, VersionJson json)
        {
            file = file + ".bak";
            TextWriter writer = File.CreateText(file);
            writer.Write(JsonConvert.SerializeObject(json, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
            writer.Close();
        }
        /*public static List<LibraryUniversal> ToUniversalLibrary(this List<Library> libs)
        {
            List<LibraryUniversal> liblist = new List<LibraryUniversal>();
            foreach (var item in libs)
            {
                liblist.Add(new LibraryUniversal
                {
                    name = item.NS + ":" + item.Name + ":" + item.Version,
                    path = App.core.GetLibPath(item),
                    url = item.Url
                });
            }
            return liblist;
        }
        public static List<LibraryUniversal> ToUniversalLibrary(this List<Native> libs)
        {
            List<LibraryUniversal> liblist = new List<LibraryUniversal>();
            foreach (var item in libs)
            {
                liblist.Add(new LibraryUniversal
                {
                    name = item.NS + ":" + item.Name + ":" + item.Version,
                    path = App.core.GetNativePath(item),
                    url = item.Url
                });
            }
            return liblist;
        }*/
        public static List<LibraryUniversal> ToUniversalLibrary(this SimplifyVersionJson.Library[] libs)
        {
            List<LibraryUniversal> liblist = new List<LibraryUniversal>();
            foreach (var item in libs)
            {
                if (!IsAllowed(item.rules))
                {
                    continue;
                }
                string[] name = item.name.Split(':');
                if (item.natives != null)
                {
                    liblist.Add(new LibraryUniversal
                    {
                        name = item.name,
                        path = Path.Combine(MeCore.Config.MCPath, "libraries", name[0].Replace(".", "\\"), name[1], name[2], name[1] + "-" + name[2] + "-" + item.natives.windows.Replace("${arch}", Environment.Is64BitOperatingSystem ? "64" : "32") + ".jar"),
                        url = item.url,
                        isNative = true,
                        extract = item.extract
                    });
                }
                else
                {
                    liblist.Add(new LibraryUniversal
                    {
                        name = item.name,
                        path = Path.Combine(MeCore.Config.MCPath, "libraries", name[0].Replace(".", "\\"), name[1], name[2], name[1] + "-" + name[2] + ".jar"),
                        url = item.url
                    });
                }
            }
            return liblist;
        }
        public static List<LibraryUniversal> ToUniversalLibrary(this VersionJson.Library[] libs)
        {
            List<LibraryUniversal> liblist = new List<LibraryUniversal>();
            foreach (var item in libs)
            {
                if (!IsAllowed(item.rules))
                {
                    continue;
                }
                if (item.download != null)
                {
                    if (item.download.classifiers != null)
                    {
                        liblist.Add(new LibraryUniversal
                        {
                            name = item.name,
                            path = Path.Combine(MeCore.Config.MCPath, "libraries", item.download.classifiers.natives_windows.path),
                            url = item.download.classifiers.natives_windows.url,
                            sha1 = item.download.classifiers.natives_windows.sha1,
                            size = item.download.classifiers.natives_windows.size,
                            isNative = true,
                            extract = item.extract
                        });
                    }
                    else if (item.download.artifact != null)
                    {
                        liblist.Add(new LibraryUniversal
                        {
                            name = item.name,
                            path = Path.Combine(MeCore.Config.MCPath, "libraries", item.download.artifact.path),
                            url = item.download.artifact.url,
                            sha1 = item.download.artifact.sha1,
                            size = item.download.artifact.size
                        });
                    }
                }
                else
                {
                    liblist.AddRange(ToUniversalLibrary((SimplifyVersionJson.Library[])libs));
                    break;
                }
            }
            return liblist;
        }
        public static bool IsAllowed(SimplifyVersionJson.Library.Rule[] rules)
        {
            if (rules == null)
            {
                return true;
            }
            if (rules.Length == 0)
            {
                return true;
            }
            var allowed = false;
            foreach (var rule in rules)
            {
                if (rule.os == null)
                {
                    allowed = rule.action == "allow";
                    continue;
                }
                if (rule.os.name == "windows")
                {
                    allowed = rule.action == "allow";
                }
            }
            return allowed;
        }
    }
}
