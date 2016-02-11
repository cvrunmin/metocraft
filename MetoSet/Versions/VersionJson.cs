using System.Runtime.Serialization;

namespace MTMCL.Versions
{
    [DataContract]
    class VersionJson
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
        public AssetIndex assetIndex;
        [DataContract]
        public class AssetIndex
        {
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
        [DataMember]
        public Downloads downloads;
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
        [DataMember]
        public Library[] libraries;
        [DataContract]
        public class Library
        {
            [DataMember(IsRequired = true)]
            public string name;
            [DataMember(IsRequired = true)]
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
                    [LitJson.JsonPropertyName("natives-linux")]
                    public Artifact natives_linux;
                    [DataMember(Name = "natives-osx")]
                    [LitJson.JsonPropertyName("natives-osx")]
                    public Artifact natives_osx;
                    [DataMember(Name = "natives-windows")]
                    [LitJson.JsonPropertyName("natives-windows")]
                    public Artifact natives_windows;
                }
            }
            [DataMember(IsRequired = false)]
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
            [DataMember(IsRequired = false)]
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
    }
}
