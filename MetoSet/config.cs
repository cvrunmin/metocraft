using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Serialization;
using System.Text;
using MTMCL.util;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MTMCL
{
    public enum EnumGraphic
    {
        Fast = 0,
        Fancy = 1,
        BakaXL = 2,
        EffectiveFirst = 0,
        ExperienceFirst = 1,
        Duang = 2
    }
    [DataContract]
    public class Config : ICloneable
    {
        [DataMember]
        public EnumGraphic graphic { get; set; }
        [DataMember]
        public bool requiredGuide { get; set; }
        [DataMember]
        [JsonProperty("javapath")]
        public string Javaw { get; set; }
        [DataMember]
        [JsonProperty("minecraftpath")]
        public string MCPath { get; set; }
        [DataMember]
        [JsonProperty("javaXmx")]
        public double Javaxmx { get; set; }
        [DataMember]
        [JsonProperty("lastversion")]
        public string LastPlayVer { get; set; }
        [JsonProperty("lastlaunchmode")]
        public string LastLaunchMode { get; set; }
        [DataMember]
        [JsonProperty("jvmarg")]
        public string ExtraJvmArg { get; set; }
        [DataMember]
        [JsonProperty("language")]
        public string Lang { get; set; }
        [DataMember]
        [JsonProperty("background")]
        public string Background { get; set; }
        [DataMember]
        [JsonProperty("color-scheme")]
        public string ColorScheme { get; set; }
        [DataMember]
        [JsonProperty("download-source")]
        public int DownloadSource { get; set; }
        [DataMember]
        [JsonProperty("download-once")]
        public bool DownloadOnceOnly { get; set; }
        [DataMember]
        public string GUID { get; set; }
        [DataMember]
        [JsonProperty("expand-task-gui")]
        public bool ExpandTaskGui { get; set; }
        [DataMember]
        [JsonProperty("check-update")]
        public bool CheckUpdate { get; set; }
        [DataMember]
        public string username { get; set; }
        [DataMember]
        public string password { get; private set; }
        [DataMember]
        public string token;
        [DataMember]
        [JsonProperty("update-source")]
        public byte UpdateSource { get; set; }
        [DataMember]
        [JsonProperty("search-latest-update")]
        public bool SearchLatest { get; set; }
        [DataMember]
        [JsonProperty("server-info")]
        public ServerInfo Server { get; set; }
        [DataMember]
        [JsonProperty("saved-auths")]
        public Dictionary<string, SavedAuth> SavedAuths { get; set; }
        [DataMember]
        [JsonProperty("default-auth")]
        public string DefaultAuth { get; set; }
        [JsonProperty("reverse-color")]
        public bool reverseColor { get; set; }
        [JsonProperty("fail-update-last-time")]
        public bool failUpdateLastTime { get; set; }
        [DataContract]
        public class ServerInfo {
            [JsonProperty("title")]
            public string Title;
            [DataMember(Name = "server-name")]
            [JsonProperty("server-name")]
            public string ServerName;
            [DataMember(Name = "server-ip")]
            [JsonProperty("server-ip")]
            public string ServerIP;
            [DataMember(Name = "client-name")]
            [JsonProperty("client-name")]
            public string ClientPath;
            [DataMember(Name = "need-server-pack")]
            [JsonProperty("need-server-pack")]
            public bool NeedServerPack;
            [DataMember(Name = "server-pack-url", IsRequired = false)]
            [JsonProperty("server-pack-url")]
            public string ServerPackUrl;
            [DataMember(Name = "allow-self-download-client")]
            [JsonProperty("allow-self-download-client")]
            public bool AllowSelfDownloadClient;
            [DataMember(Name = "lock-background")]
            [JsonProperty("lock-background")]
            public bool LockBackground;
            [DataMember(Name = "background-path", IsRequired = false)]
            [JsonProperty("background-path")]
            public string BackgroundPath;
            [DataMember(Name = "auths", IsRequired = false)]
            [JsonProperty("auths")]
            public List<Auth> Auths;
            public class Auth
            {
                [DataMember(Name = "auth-name", IsRequired = true)]
                [JsonProperty("auth-name")]
                public string Name;
                [DataMember(Name = "auth-url", IsRequired = true)]
                [JsonProperty("auth-url")]
                public string Url;
            }
        }
        [DataContract]
        public class SavedAuth
        {
            [DataMember]
            [JsonProperty("auth-type")]
            public string AuthType { get; set; }
            [DataMember]
            [JsonProperty("display-name")]
            public string DisplayName { get; set; }
            [DataMember]
            [JsonProperty("access-token")]
            public string AccessToken { get; set; }
            [DataMember]
            [JsonProperty("uuid")]
            public string UUID { get; set; }
            [DataMember]
            [JsonProperty("properies")]
            public string Properies { get; set; }
            [DataMember]
            [JsonProperty("user-type")]
            public string UserType { get; set; }
        }
        [IgnoreDataMember]
        [JsonIgnore]
        public static bool ConfigReadOnly { get; set; }
        public Config()
        {
            requiredGuide = true;
            try
            {
                Javaw = GetJavaDir() ?? "javaw.exe";
            }
            catch
            {
                Javaw = "undefined";
            }
            MCPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft");
            Javaxmx = (GetMemory() / 4);
            ExtraJvmArg = " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true";
            Background = "default";
            ColorScheme = "Green";
            DownloadSource = 0;
            DownloadOnceOnly = false;
            UpdateSource = 0;
            Lang = GetValidLang();
            reverseColor = false;
            ExpandTaskGui = true;
            GUID = GetGuid();
            CheckUpdate = true;
            SearchLatest = false;
            Server = null;
            SavedAuths = new Dictionary<string, SavedAuth>();
        }
        public string GetOver50String() {
            return string.IsNullOrWhiteSpace(password) ? password : Encoding.Default.GetString(Convert.FromBase64String(password)).Decrystal();
        }
        public void ScoreStringToOver50(string score) {
            password = string.IsNullOrWhiteSpace(score) ? score : Convert.ToBase64String(Encoding.Default.GetBytes(score.Encrystal()));
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
                //var fs = new FileStream(file, FileMode.Open);
                var ser = new DataContractSerializer(typeof(Config));
                ///for json
                var cfg = JsonConvert.DeserializeObject<Config>(File.ReadAllText(file));
                ///for xml
                //var cfg = (Config)ser.ReadObject(fs);
                //fs.Close();
                if (cfg.GUID == null)
                {
                    cfg.GUID = GetGuid();
                }
                return cfg;
            }
            catch(UnauthorizedAccessException e)
            {
                Logger.log(e);
                ConfigReadOnly = true;
                return new Config();
            }
            catch (Exception e)
            {
                Logger.log(e);
                //MessageBox.Show("errer occurred when loading the config file, try to use default config.");
                return new Config();
            }
        }
        public static void Save(Config cfg = null ,string file = null)
        {
            if (ConfigReadOnly) return;
            try
            {
                if (cfg == null)
                {
                    cfg = MeCore.Config;
                }
                if (file == null)
                {
                    ///for json
                    file = MeCore.DataDirectory + "mtmcl_config.json";
                    ///for xml
                    //file = MeCore.BaseDirectory + "mtmcl_config.xml";
                }
                //var fs = new FileStream(file, FileMode.Create);
                ///for xml
                /*var ser = new DataContractSerializer(typeof(Config));
                ser.WriteObject(fs, cfg);*/
                ///for json
                StringBuilder sbuild = new StringBuilder();
            File.WriteAllText(file, JsonConvert.SerializeObject(cfg, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }), Encoding.UTF8);
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
                property.SetValue(this, Convert.ChangeType(value, property.PropertyType),null);
                Save();
            }
            catch {
                try
                {
                    Type type = GetType();
                    System.Reflection.PropertyInfo property = type.GetProperty(field,System.Reflection.BindingFlags.IgnoreCase);
                    property.SetValue(this, Convert.ChangeType(value, property.PropertyType),null);
                    Save();
                }
                catch {
                    try
                    {
                        Type type = GetType();
                        System.Reflection.PropertyInfo[] properties = type.GetProperties();
                        foreach (var property in properties)
                        {
                            if (property.IsDefined(typeof(JsonProperty), true))
                            {
                                if (field.Equals(((JsonProperty)property.GetCustomAttributes(typeof(JsonProperty), true)[0]).PropertyName))
                                {
                                    property.SetValue(this, Convert.ChangeType(value, property.PropertyType),null);
                                    Save();
                                    break;
                                }
                            }
                        }
                    }
                    catch
                    {
                        try
                        {
                            Type type = GetType();
                            System.Reflection.FieldInfo property = type.GetField(field);
                            property.SetValue(this, Convert.ChangeType(value, property.FieldType));
                            Save();
                        }
                        catch
                        {
                            try
                            {
                                Type type = GetType();
                                System.Reflection.FieldInfo property = type.GetField(field, System.Reflection.BindingFlags.IgnoreCase);
                                property.SetValue(this, Convert.ChangeType(value, property.FieldType));
                                Save();
                            }
                            catch
                            {
                                try
                                {
                                    Type type = GetType();
                                    System.Reflection.FieldInfo[] properties = type.GetFields();
                                    foreach (var property in properties)
                                    {
                                        if (property.IsDefined(typeof(JsonProperty), true))
                                        {
                                            if (field.Equals(((JsonProperty)property.GetCustomAttributes(typeof(JsonProperty), true)[0]).PropertyName))
                                            {
                                                property.SetValue(this, Convert.ChangeType(value, property.FieldType));
                                                Save();
                                                break;
                                            }
                                        }
                                    }
                                }
                                catch(Exception e)
                                {
                                    Logger.log(e);
                                    throw;
                                }

                            }
                        }
                    }

                }
            }
        }
        public string ToReadableLog() {
            StringBuilder sb = new System.Text.StringBuilder();
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
            sb.AppendLine("Color Scheme: " + ColorScheme);
            sb.AppendLine("Expand Task Gui on loading: " + ExpandTaskGui);
            sb.AppendLine("Check update: " + CheckUpdate);
            return sb.ToString();
        }

        public static string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 读取注册表，寻找安装的java路径
        /// </summary>
        /// <returns>javaw.exe路径</returns>
        public static string GetJavaDir()
        {
            try
            {
                RegistryKey reg = Registry.LocalMachine;
                var openSubKey = reg.OpenSubKey("SOFTWARE");
                if (openSubKey != null)
                {
                    var registryKey = openSubKey.OpenSubKey("JavaSoft");
                    if (registryKey != null)
                        reg = registryKey.OpenSubKey("Java Runtime Environment");
                }
                if (reg != null)
                    foreach (string ver in reg.GetSubKeyNames())
                    {
                        try
                        {
                            RegistryKey command = reg.OpenSubKey(ver);
                            if (command != null)
                            {
                                string str = command.GetValue("JavaHome").ToString();
                                if (str != "")
                                    return str + @"\bin\javaw.exe";
                            }
                        }
                        catch { return null; }
                    }
                return null;
            }
            catch { return null; }

        }
        /// <summary>
        /// 获取系统物理内存大小
        /// </summary>
        /// <returns>系统物理内存大小，支持64bit,单位MB</returns>
        public static ulong GetMemory()
        {
            try
            {
                double capacity = 0.0;
                var cimobject1 = new ManagementClass("Win32_PhysicalMemory");
                ManagementObjectCollection moc1 = cimobject1.GetInstances();
                foreach (var o in moc1)
                {
                    var mo1 = (ManagementObject)o;
                    capacity += ((Math.Round(long.Parse(mo1.Properties["Capacity"].Value.ToString()) / 1024 / 1024.0, 1)));
                }
                moc1.Dispose();
                cimobject1.Dispose();
                ulong qmem = Convert.ToUInt64(capacity.ToString(CultureInfo.InvariantCulture));
                return qmem;
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                Logger.error("Failed to get the physical memory");
                Logger.error(ex);
                return ulong.MaxValue;

            }
        }
    }
}
