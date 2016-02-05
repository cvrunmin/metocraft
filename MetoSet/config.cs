using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management;
using Microsoft.Win32;
using System.Runtime.Serialization;
using System.IO;
using System.Windows;
using System.Drawing;
using System.Linq;

namespace MetoCraft
{
    [DataContract]
    public class Config : ICloneable
    {
        [DataMember]
        [LitJson.JsonPropertyName("javapath")]
        public string Javaw;
        [DataMember]
        [LitJson.JsonPropertyName("minecraftpath")]
        public string MCPath;
        [DataMember]
        [LitJson.JsonPropertyName("javaXmx")]
        public string Javaxmx;
        [DataMember]
        [LitJson.JsonPropertyName("lastversion")]
        public string LastPlayVer;
        [DataMember]
        [LitJson.JsonPropertyName("jvmarg")]
        public string ExtraJvmArg;
        [DataMember]
        [LitJson.JsonPropertyName("language")]
        public string Lang;
//        [DataMember]
//        public bool Autostart, Report,CheckUpdate;
//        [DataMember]
//        [LitJson.JsonPropertyName("transparency")]
//        public double WindowTransparency;
        [DataMember]
        [LitJson.JsonPropertyName("background")]
        public string BackGround;
        [DataMember]
        [LitJson.JsonPropertyName("color")]
        public byte[] color;
        [DataMember]
        [LitJson.JsonPropertyName("downloadsource")]
        public int DownloadSource;
//        [DataMember]
//        public Dictionary<string, object> PluginConfig = new Dictionary<string, object>();

        [DataMember]
        [LitJson.JsonPropertyName("guid")]
        public string GUID;

        public Config()
        {
            Javaw = KMCCC.Tools.SystemTools.FindValidJava().First() ?? "javaw.exe";
            MCPath = MeCore.BaseDirectory + ".minecraft";
            Javaxmx = (/*GetMemory()*/KMCCC.Tools.SystemTools.GetTotalMemory() / 4 / 1024 / 1024).ToString(CultureInfo.InvariantCulture);
//            Autostart = false;
            ExtraJvmArg = " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true";
//            WindowTransparency = 1;
            BackGround = "default";
            color = new byte[] { 255, 255, 255 };
//            Report = true;
            DownloadSource = 0;
            Lang = GetValidLang();
//            CheckUpdate = true;
//            PluginConfig = null;
            GUID = GetGuid();
        }
        public string GetValidLang() {
            if (CultureInfo.CurrentUICulture.Parent.Name != "zh-CHT" && CultureInfo.CurrentUICulture.Parent.Name != "zh-CHS"
                && CultureInfo.CurrentUICulture.Parent.Name != "en") {
                return "en";
            }
            return CultureInfo.CurrentUICulture.Parent.Name;
        }
        /**
        public object GetPluginConfig(string key)
        {
            if (PluginConfig.ContainsKey(key))
            {
                return PluginConfig[key];
            }
            return null;
        }
        
        public void SetPluginConfig(string key, object value)
        {
            if (PluginConfig.ContainsKey(key))
            {
                PluginConfig[key] = value;
            }
            else
            {
                PluginConfig.Add(key, value);
            }
        }
        **/
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
                var cfg = LitJson.JsonMapper.ToObject<Config>(new LitJson.JsonReader(new StreamReader(fs)));
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
                MessageBox.Show("errer occurred when loading the config file, try to use default config.");
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
                //var jw = new LitJson.JsonWriter(new StreamWriter(fs));
                //LitJson.JsonMapper.ToJson(cfg);
            File.WriteAllText(file, LitJson.JsonMapper.ToJson(cfg), System.Text.Encoding.UTF8);
                //fs.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("cannot save config file");
            }
        }

        public void Save(string file = null)
        {
            Save(this, file);
        }
        /// <summary>
        /// 读取注册表，寻找安装的java路径
        /// </summary>
        /// <returns>javaw.exe路径</returns>
        /// <remarks>This method has been replaced by the similar method in KMCCC</remarks>
        [Obsolete]
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
        /// <remarks>This method has been replaced by the similar method in KMCCC</remarks>
        [Obsolete]
        public static ulong GetMemory()
        {
            try
            {
                double capacity = 0.0;
                var cimobject1 = new ManagementClass("Win32_PhysicalMemory");
                ManagementObjectCollection moc1 = cimobject1.GetInstances();
                foreach (var o in moc1)
                {
                    var mo1 = (ManagementObject) o;
                    capacity += ((Math.Round(long.Parse(mo1.Properties["Capacity"].Value.ToString()) / 1024 / 1024.0, 1)));
                }
                moc1.Dispose();
                cimobject1.Dispose();
                ulong qmem = Convert.ToUInt64(capacity.ToString(CultureInfo.InvariantCulture));
                return qmem;
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                Logger.error("获取内存失败");
                Logger.error(ex);
                return ulong.MaxValue;

            }
        }

        public static string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
