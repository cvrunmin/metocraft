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
        [DataMember]
        [LitJson.JsonPropertyName("background")]
        public string BackGround;
        [DataMember]
        [LitJson.JsonPropertyName("color")]
        public byte[] color;
        [DataMember]
        [LitJson.JsonPropertyName("downloadsource")]
        public int DownloadSource;
        [DataMember]
        [LitJson.JsonPropertyName("guid")]
        public string GUID;
        [DataMember]
        [LitJson.JsonPropertyName("expand-task-gui")]
        public bool ExpandTaskGui;
        [DataMember]
        [LitJson.JsonPropertyName("check-update")]
        public bool CheckUpdate;
        [DataMember]
        public string username;
        [DataMember]
        public string token;
        public Config()
        {
            Javaw = KMCCC.Tools.SystemTools.FindValidJava().First() ?? "javaw.exe";
            MCPath = MeCore.BaseDirectory + ".minecraft";
            Javaxmx = (KMCCC.Tools.SystemTools.GetTotalMemory() / 4 / 1024 / 1024).ToString(CultureInfo.InvariantCulture);
            ExtraJvmArg = " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true";
            BackGround = "default";
            color = new byte[] { 255, 255, 255 };
            DownloadSource = 0;
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
                MessageBox.Show("cannot save config file");
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
            sb.AppendLine("Last Played Version: " + LastPlayVer);
            sb.AppendLine("GUID" + GUID);
            sb.AppendLine("Background: " + BackGround);
            sb.AppendLine("Color: " + color[0] + ", " + color[1] + ", " + color[2]);
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
