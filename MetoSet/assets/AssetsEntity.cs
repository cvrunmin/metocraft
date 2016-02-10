using System.Runtime.Serialization;

namespace MTMCL.Assets
{
    [DataContract]
    public class AssetsEntity
    {
        [DataMember]
        public string hash;
        [DataMember]
        public int size;
    }
}
