using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;

namespace MTMCL.Server
{
    [DataContract]
    public class ServerInfo : ICloneable
    {
        [IgnoreDataMember]
        public bool Ignore = false;
        [DataMember(Name = "title", IsRequired = false)]
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
        [DataMember(Name = "allow-download-library-and-asset")]
        [LitJson.JsonPropertyName("allow-download-library-and-asset")]
        public bool AllowDownloadLibAndAsset;
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

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public static ServerInfo Load(string file)
        {
            if (!File.Exists(file))
                return new ServerInfo();
            try
            {
                var fs = new FileStream(file, FileMode.Open);
                var ser = new DataContractSerializer(typeof(ServerInfo));
                ///for json
                var cfg = LitJson.JsonMapper.ToObject<ServerInfo>(new LitJson.JsonReader(new StreamReader(fs, Encoding.UTF8)));
                ///for xml
                //var cfg = (Config)ser.ReadObject(fs);
                fs.Close();
                return cfg;
            }
            catch(Exception e)
            {
                new ErrorReport(e).Show();
                MessageBox.Show("errer occurred when loading the config file, try to ignore it.");
                return new ServerInfo() { Ignore = true };
            }
        }
        public static void Save(ServerInfo cfg = null, string file = null)
        {
            try
            {
                if (cfg == null)
                {
                    cfg = MeCore.ServerCfg;
                }
                if (file == null)
                {
                    ///for json
                    file = MeCore.BaseDirectory + "mtmcl_server_config.json";
                    ///for xml
                    //file = MeCore.BaseDirectory + "mtmcl_config.xml";
                }
                //var fs = new FileStream(file, FileMode.Create);
                ///for xml
                /*var ser = new DataContractSerializer(typeof(Config));
                ser.WriteObject(fs, cfg);*/
                ///for json
                System.Text.StringBuilder sbuild = new StringBuilder();
                var jw = new LitJson.JsonWriter(sbuild);
                jw.PrettyPrint = true;
                LitJson.JsonMapper.ToJson(cfg, jw);
                File.WriteAllText(file, sbuild.ToString(), Encoding.UTF8);
                //fs.Close();
            }
            catch (Exception e)
            {
                Logger.log(e);
                MessageBox.Show("cannot save server config file");
            }
        }

        public void Save(string file = null)
        {
            Save(this, file);
        }
    }
}
