using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MetoCraft.JsonClass
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
            public string branch;
            [DataMember(Name = "mcversion")]
            public string mcversion;
            [DataMember(Name = "jobver")]
            public string jobver;
            [DataMember(Name = "version")]
            public string version;
            [DataMember(Name = "build")]
            public int build;
            [DataMember(Name = "modified")]
            public decimal modified;
            [DataMember(Name = "files")]
            public string[][] files;
        }
    }

}
