using System.Runtime.Serialization;
using static MTMCL.util.FileHelper;
namespace MTMCL.Assets
{
    [DataContract]
    public class AssetsEntity
    {
        [DataMember(Name = "hash")]
        public string Hash { get; set; }
        [DataMember(Name = "size")]
        public int Size { get; set; }
    }

    public class PackedAssetsEntity : AssetsEntity {
        public string Assets { get; set; }

        public bool Exist => AssetsExist(new System.Collections.Generic.KeyValuePair<string, AssetsEntity>(Assets, this));
    }
}
