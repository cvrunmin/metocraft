using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MTMCL.Assets
{
    [DataContract]
    class AssetIndex
    {
        [DataMember(Name = "virtual")]
        [LitJson.JsonPropertyName("virtual")]
        public bool _virtual;
        [DataMember]
        public Dictionary<string, AssetsEntity> objects;
    }
}
