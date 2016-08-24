using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MTMCL.JsonClass
{
    [DataContract]
    public class ForgeVersion
    {
        [DataMember]
        public string artifact, webpath, adfocus, homepage, name;
        [DataMember(Name = "branches")]
        public Dictionary<string, int[]> branches;
        [DataMember(Name = "mcversion")]
        public Dictionary<string, int[]> mcversion;
        [DataMember(Name = "promos")]
        public Dictionary<string, int> promos;
        [DataMember(Name = "number")]
        public Dictionary<string, Version> number;
        [DataContract]
        public class Version {
            [DataMember(Name = "branch")]
            public string branch { get; set; }
            [DataMember(Name = "mcversion")]
            public string mcversion { get; set; }
            [DataMember(Name = "jobver")]
            public string jobver { get; set; }
            [DataMember(Name = "version")]
            public string version { get; set; }
            [DataMember(Name = "build")]
            public int build { get; set; }
            [DataMember(Name = "modified")]
            public decimal modified { get; set; }
            [DataMember(Name = "files")]
            public string[][] files { get; set; }
            [IgnoreDataMember]
            [Newtonsoft.Json.JsonIgnore]
            public string type { get; set; }
            [IgnoreDataMember]
            [Newtonsoft.Json.JsonIgnore]
            public DateTime relTime { get {
                    return DateTime.SpecifyKind(util.TimeHelper.UnixTimeStampToDateTime((double) modified), DateTimeKind.Local);
                } }
            [IgnoreDataMember]
            [Newtonsoft.Json.JsonIgnore]
            public Dictionary<string,string> urls { get; set; }
        }
    }

}
