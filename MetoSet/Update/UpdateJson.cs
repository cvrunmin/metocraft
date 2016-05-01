using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MTMCL.Update
{
    [DataContract]
    public class UpdateJson
    {
        [JsonProperty("specific-version")]
        public SpecificVersion specific_version;
        public class SpecificVersion {
            public string latest;
            public string recommand;
        }
        [DataMember]
        public Version[] versions;
        [DataContract]
        public class Version {
            [DataMember]
            public string version;
            [DataMember]
            public string url;
            [DataMember]
            public string info;
        }
    }
}
