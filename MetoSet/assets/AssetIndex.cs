using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MTMCL.Assets
{
    [DataContract]
    class AssetIndex
    {
        [DataMember(Name = "virtual")]
        [JsonProperty("virtual")]
        public bool _virtual { get; set; }
        [DataMember]
        public Dictionary<string, AssetsEntity> objects { get; set; }
    }
}
