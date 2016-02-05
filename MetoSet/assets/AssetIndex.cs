using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MetoCraft.Assets
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
