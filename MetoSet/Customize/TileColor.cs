using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace MTMCL.Customize
{
    [DataContract]
    class TileColor
    {
        [DataMember]
        [JsonProperty(PropertyName = "task-list-tile-color", NullValueHandling = NullValueHandling.Ignore)]
        public string tileColorTaskList = "default";
        [DataMember]
        [JsonProperty(PropertyName = "notice-tile-color", NullValueHandling = NullValueHandling.Ignore)]
        public string tileColorNotice = "default";
        [DataMember]
        [JsonProperty(PropertyName = "play-tile-color", NullValueHandling = NullValueHandling.Ignore)]
        public string tileColorPlay = "default";
        [DataMember]
        [JsonProperty(PropertyName = "setting-tile-color", NullValueHandling = NullValueHandling.Ignore)]
        public string tileColorSetting = "default";
        [DataMember]
        [JsonProperty(PropertyName = "download-tile-color", NullValueHandling = NullValueHandling.Ignore)]
        public string tileColorDownload = "default";
        [DataMember]
        [JsonProperty(PropertyName = "install-tile-color", NullValueHandling = NullValueHandling.Ignore)]
        public string tileColorInstall = "default";
        [DataMember]
        [JsonProperty(PropertyName = "server-admin-tile-color", NullValueHandling = NullValueHandling.Ignore)]
        public string tileColorServerAdmin = "default";
        [DataMember]
        [JsonProperty(PropertyName = "gradle-tile-color", NullValueHandling = NullValueHandling.Ignore)]
        public string tileColorGradle = "default";
        [DataMember]
        [JsonProperty(PropertyName = "quick-play-tile-color", NullValueHandling = NullValueHandling.Ignore)]
        public string tileColorQuickPlay = "default";

        public static TileColor Load(string file)
        {
            if (!File.Exists(file))
                return new TileColor();
            try
            {
                //var fs = new FileStream(file, FileMode.Open);
                var ser = new DataContractSerializer(typeof(TileColor));
                ///for json
                var cfg = JsonConvert.DeserializeObject<TileColor>(File.ReadAllText(file));
                ///for xml
                //var cfg = (Config)ser.ReadObject(fs);
                //fs.Close();
                return cfg;
            }
            catch (UnauthorizedAccessException e)
            {
                Logger.log(e);
                return new TileColor();
            }
            catch (Exception e)
            {
                Logger.log(e);
                //MessageBox.Show("errer occurred when loading the config file, try to use default config.");
                return new TileColor();
            }
        }
        public static void Save(TileColor cfg = null, string file = null)
        {
            try
            {
                if (cfg == null)
                {
                    cfg = MeCore.TileColor;
                }
                if (file == null)
                {
                    ///for json
                    file = MeCore.DataDirectory + "mtmcl_tile_color.json";
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
    }
}
