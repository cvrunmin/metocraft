using LitJson;
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
        public string password { get; private set; }
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
        [DataMember]
        [JsonPropertyName("saved-auths")]
        public Dictionary<string, SavedAuth> SavedAuths { get; set; }
        [DataMember]
        [JsonPropertyName("default-auth")]
        public string DefaultAuth { get; set; }
        [JsonPropertyName("reverse-color")]
        public bool reverseColor { get; set; }
        [DataContract]
        public class ServerInfo {
            [JsonPropertyName("title")]
            public string Title;
            [DataMember(Name = "server-name")]
            [JsonPropertyName("server-name")]
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
        [DataContract]
        public class SavedAuth
        {
            [DataMember]
            [JsonPropertyName("auth-type")]
            public string AuthType { get; set; }
            [DataMember]
            [JsonPropertyName("display-name")]
            public string DisplayName { get; set; }
            [DataMember]
            [JsonPropertyName("access-token")]
            public string AccessToken { get; set; }
            [DataMember]
            [JsonPropertyName("uuid")]
            public string UUID { get; set; }
            [DataMember]
            [JsonPropertyName("properies")]
            public string Properies { get; set; }
            [DataMember]
            [JsonPropertyName("user-type")]
            public string UserType { get; set; }
        }
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
                            if (property.IsDefined(typeof(JsonPropertyName), true))
                            {
                                if (field.Equals(((JsonPropertyName)property.GetCustomAttributes(typeof(JsonPropertyName), true)[0]).Name))
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
                                        if (property.IsDefined(typeof(JsonPropertyName), true))
                                        {
                                            if (field.Equals(((JsonPropertyName)property.GetCustomAttributes(typeof(JsonPropertyName), true)[0]).Name))
                                            {
                                                property.SetValue(this, Convert.ChangeType(value, property.FieldType));
                                                Save();
                                                break;
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                    throw;
                                }

                            }
                        }
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
