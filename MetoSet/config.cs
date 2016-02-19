using Microsoft.Win32;
using System;
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
        private string javaw;
        [DataMember]
        [LitJson.JsonPropertyName("minecraftpath")]
        private string mcPath;
        [DataMember]
        [LitJson.JsonPropertyName("javaXmx")]
        private string javaxmx;
        [DataMember]
        [LitJson.JsonPropertyName("lastversion")]
        private string lastPlayVer;
        [DataMember]
        [LitJson.JsonPropertyName("jvmarg")]
        private string extraJvmArg;
        [DataMember]
        [LitJson.JsonPropertyName("language")]
        private string lang;
        [DataMember]
        [LitJson.JsonPropertyName("background")]
        private string background;
        [DataMember]
        [LitJson.JsonPropertyName("color")]
        private byte[] color;
        [DataMember]
        [LitJson.JsonPropertyName("download-source")]
        private int downloadSource;
        [DataMember]
        private string guid;
        [DataMember]
        [LitJson.JsonPropertyName("expand-task-gui")]
        private bool expandTaskGui;
        [DataMember]
        [LitJson.JsonPropertyName("check-update")]
        private bool checkUpdate;
        [DataMember]
        private string username;
        [DataMember]
        private string token;
        [DataMember]
        [LitJson.JsonPropertyName("update-source")]
        private byte updateSource;

        public string Javaw
        {
            get
            {
                return javaw;
            }

            set
            {
                javaw = value;
            }
        }

        public string MCPath
        {
            get
            {
                return mcPath;
            }

            set
            {
                mcPath = value;
            }
        }

        public string Javaxmx
        {
            get
            {
                return javaxmx;
            }

            set
            {
                javaxmx = value;
            }
        }

        public string LastPlayVer
        {
            get
            {
                return lastPlayVer;
            }

            set
            {
                lastPlayVer = value;
            }
        }

        public string ExtraJvmArg
        {
            get
            {
                return extraJvmArg;
            }

            set
            {
                extraJvmArg = value;
            }
        }

        public string Lang
        {
            get
            {
                return lang;
            }

            set
            {
                lang = value;
            }
        }

        public string Background
        {
            get
            {
                return background;
            }

            set
            {
                background = value;
            }
        }

        public byte[] Color
        {
            get
            {
                return color;
            }

            set
            {
                color = value;
            }
        }

        public int DownloadSource
        {
            get
            {
                return downloadSource;
            }

            set
            {
                downloadSource = value;
            }
        }

        public string GUID
        {
            get
            {
                return guid;
            }

            set
            {
                guid = value;
            }
        }

        public bool ExpandTaskGui
        {
            get
            {
                return expandTaskGui;
            }

            set
            {
                expandTaskGui = value;
            }
        }

        public bool CheckUpdate
        {
            get
            {
                return checkUpdate;
            }

            set
            {
                checkUpdate = value;
            }
        }

        public string Username
        {
            get
            {
                return username;
            }

            set
            {
                username = value;
            }
        }

        public string Token
        {
            get
            {
                return token;
            }

            set
            {
                token = value;
            }
        }

        public byte UpdateSource
        {
            get
            {
                return updateSource;
            }

            set
            {
                updateSource = value;
            }
        }

        public Config()
        {
            Javaw = KMCCC.Tools.SystemTools.FindValidJava().First() ?? "javaw.exe";
            MCPath = MeCore.BaseDirectory + ".minecraft";
            Javaxmx = (KMCCC.Tools.SystemTools.GetTotalMemory() / 4 / 1024 / 1024).ToString(CultureInfo.InvariantCulture);
            ExtraJvmArg = " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true";
            Background = "default";
            Color = new byte[] { 255, 255, 255 };
            DownloadSource = 0;
            UpdateSource = 0;
            Lang = GetValidLang();
            ExpandTaskGui = true;
            GUID = GetGuid();
            CheckUpdate = true;
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
                var jw = new LitJson.JsonWriter(sbuild);
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

        public void Save(string file = null)
        {
            Save(this, file);
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
