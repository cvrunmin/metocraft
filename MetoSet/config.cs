using LitJson;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Serialization;
using System.Windows;

namespace MTMCL
{
    [DataContract]
    public class Config : ICloneable
    {
        [DataMember]
        [LitJson.JsonPropertyName("javapath")]
        public string Javaw { get; set; }
        [DataMember]
        [LitJson.JsonPropertyName("minecraftpath")]
        public string MCPath { get; set; }
        [DataMember]
        [LitJson.JsonPropertyName("javaXmx")]
        public double Javaxmx { get; set; }
        [DataMember]
        [LitJson.JsonPropertyName("lastversion")]
        public string LastPlayVer { get; set; }
        [DataMember]
        [LitJson.JsonPropertyName("jvmarg")]
        public string ExtraJvmArg { get; set; }
        [DataMember]
        [LitJson.JsonPropertyName("language")]
        public string Lang { get; set; }
        [DataMember]
        [LitJson.JsonPropertyName("background")]
        public string Background { get; set; }
        [DataMember]
        [LitJson.JsonPropertyName("color")]
        public byte[] Color { get; set; }
        [DataMember]
        [LitJson.JsonPropertyName("color-scheme")]
        public string ColorScheme { get; set; }
        [DataMember]
        [LitJson.JsonPropertyName("download-source")]
        public int DownloadSource { get; set; }
        [DataMember]
        public string GUID { get; set; }
        [DataMember]
        [LitJson.JsonPropertyName("expand-task-gui")]
        public bool ExpandTaskGui { get; set; }
        [DataMember]
        [LitJson.JsonPropertyName("check-update")]
        public bool CheckUpdate { get; set; }
        [DataMember]
        public string username { get; set; }
        [DataMember]
        public string token;
        [DataMember]
        [LitJson.JsonPropertyName("update-source")]
        public byte UpdateSource { get; set; }
        [DataMember]
        [JsonPropertyName("search-latest-update")]
        public bool SearchLatest { get; set; }
        [DataMember]
        [JsonPropertyName("server-info")]
        public ServerInfo Server { get; set; }
        [DataContract]
        public class ServerInfo {
            [LitJson.JsonPropertyName("title")]
            public string Title;
            [DataMember(Name = "server-name")]
            [LitJson.JsonPropertyName("server-name")]
            public string ServerName;
            [DataMember(Name = "server-ip")]
            [LitJson.JsonPropertyName("server-ip")]
            public string ServerIP;
            [DataMember(Name = "client-name")]
            [LitJson.JsonPropertyName("client-name")]
            public string ClientPath;
            [DataMember(Name = "need-server-pack")]
            [LitJson.JsonPropertyName("need-server-pack")]
            public bool NeedServerPack;
            [DataMember(Name = "server-pack-url", IsRequired = false)]
            [LitJson.JsonPropertyName("server-pack-url")]
            public string ServerPackUrl;
            [DataMember(Name = "allow-redownload-library-and-asset")]
            [LitJson.JsonPropertyName("allow-redownload-library-and-asset")]
            public bool AllowReDownloadLibAndAsset;
            [DataMember(Name = "allow-self-download-client")]
            [LitJson.JsonPropertyName("allow-self-download-client")]
            public bool AllowSelfDownloadClient;
            [DataMember(Name = "lock-background")]
            [LitJson.JsonPropertyName("lock-background")]
            public bool LockBackground;
            [DataMember(Name = "background-path", IsRequired = false)]
            [LitJson.JsonPropertyName("background-path")]
            public string BackgroundPath;
            [DataMember(Name = "auths", IsRequired = false)]
            [LitJson.JsonPropertyName("auths")]
            public List<Auth> Auths;
            public class Auth
            {
                [DataMember(Name = "auth-name", IsRequired = true)]
                [LitJson.JsonPropertyName("auth-name")]
                public string Name;
                [DataMember(Name = "auth-url", IsRequired = true)]
                [LitJson.JsonPropertyName("auth-url")]
                public string Url;
            }
        }
        public class SavedAuth
        {
            [DataMember]
            [JsonPropertyName("auth-type")]
            public Type AuthType { get; set; }
            [JsonPropertyName("display-name")]
            public string DisplayName { get; set; }
            [JsonPropertyName("uuid")]
            public string UUID { get; set; }
        }
        public Config()
        {
            try
            {
                Javaw = KMCCC.Tools.SystemTools.FindValidJava().First();
            }
            catch
            {
                Javaw = "undefined";
            }
            MCPath = MeCore.BaseDirectory + ".minecraft";
            Javaxmx = (KMCCC.Tools.SystemTools.GetTotalMemory() / 4 / 1024 / 1024);
            ExtraJvmArg = " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true";
            Background = "default";
            Color = new byte[] { 255, 255, 255 };
            ColorScheme = "Green";
            DownloadSource = 0;
            UpdateSource = 0;
            Lang = GetValidLang();
            ExpandTaskGui = true;
            GUID = GetGuid();
            CheckUpdate = true;
            SearchLatest = false;
            Server = null;
        }
        public string GetValidLang() {
            if (CultureInfo.CurrentUICulture.Parent.Name != "zh-CHT" && CultureInfo.CurrentUICulture.Parent.Name != "zh-CHS"
                && CultureInfo.CurrentUICulture.Parent.Name != "en") {
                return "en";
            }
            return CultureInfo.CurrentUICulture.Parent.Name;
        }
        public object Clone()
        {
            return (Config)MemberwiseClone();
        }

        public static Config Load(string file)
        {
            if (!File.Exists(file))
                return new Config();
            try
            {
                var fs = new FileStream(file, FileMode.Open);
                var ser = new DataContractSerializer(typeof(Config));
                ///for json
                var cfg = JsonMapper.ToObject<Config>(new LitJson.JsonReader(new StreamReader(fs)));
                ///for xml
                //var cfg = (Config)ser.ReadObject(fs);
                fs.Close();
                if (cfg.GUID == null)
                {
                    cfg.GUID = GetGuid();
                }
                return cfg;
            }
            catch
            {
                //MessageBox.Show("errer occurred when loading the config file, try to use default config.");
                return new Config();
            }
        }
        public static void Save(Config cfg = null ,string file = null)
        {
            try
            {
                if (cfg == null)
                {
                    cfg = MeCore.Config;
                }
                if (file == null)
                {
                    ///for json
                    file = MeCore.BaseDirectory + "mtmcl_config.json";
                    ///for xml
                    //file = MeCore.BaseDirectory + "mtmcl_config.xml";
                }
                //var fs = new FileStream(file, FileMode.Create);
                ///for xml
                /*var ser = new DataContractSerializer(typeof(Config));
                ser.WriteObject(fs, cfg);*/
                ///for json
                System.Text.StringBuilder sbuild = new System.Text.StringBuilder();
                var jw = new JsonWriter(sbuild);
                jw.PrettyPrint = true;
                LitJson.JsonMapper.ToJson(cfg, jw);
            File.WriteAllText(file, sbuild.ToString(), System.Text.Encoding.UTF8);
                //fs.Close();
            }
            catch (Exception e)
            {
                Logger.log(e);
                //MessageBox.Show("cannot save config file");
            }
        }

        public void Save()
        {
            Save(this, null);
        }
        public void Save(string file)
        {
            Save(this, file);
        }
        public void QuickChange(string field, object value) {
            try
            {
                Type type = GetType();
                System.Reflection.PropertyInfo property = type.GetProperty(field);
                property.SetValue(this, Convert.ChangeType(value, property.PropertyType));
                Save();
            }
            catch {
                try
                {
                    Type type = GetType();
                    System.Reflection.PropertyInfo property = type.GetProperty(field,System.Reflection.BindingFlags.IgnoreCase);
                    property.SetValue(this, Convert.ChangeType(value, property.PropertyType));
                    Save();
                }
                catch {
                    try
                    {
                        Type type = GetType();
                        System.Reflection.PropertyInfo[] properties = type.GetProperties();
                        foreach (var property in properties)
                        {
                            if (property.IsDefined(typeof(JsonPropertyName), true))
                            {
                                if (field.Equals(((JsonPropertyName)property.GetCustomAttributes(typeof(JsonPropertyName), true)[0]).Name))
                                {
                                    property.SetValue(this, Convert.ChangeType(value, property.PropertyType));
                                    Save();
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                }
            }
        }
        public string ToReadableLog() {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("Language: " + Lang);
            sb.AppendLine("Java Path: " + Javaw);
            sb.AppendLine("MineCraft Path: " + MCPath);
            sb.AppendLine("Java Xmx: " + Javaxmx);
            sb.AppendLine("Extra JVM Arg: " + ExtraJvmArg);
            sb.AppendLine("Download Source: " + DownloadSource);
            sb.AppendLine("Update Source: " + UpdateSource);
            sb.AppendLine("Last Played Version: " + LastPlayVer);
            sb.AppendLine("GUID" + GUID);
            sb.AppendLine("Background: " + Background);
            sb.AppendLine("Color: " + Color[0] + ", " + Color[1] + ", " + Color[2]);
            sb.AppendLine("Expand Task Gui on loading: " + ExpandTaskGui);
            sb.AppendLine("Check update: " + CheckUpdate);
            return sb.ToString();
        }

        public static string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
