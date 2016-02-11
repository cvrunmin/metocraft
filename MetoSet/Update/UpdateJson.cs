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
        [DataMember]
        public Version[] versions;
        [DataContract]
        public class Version {
            [DataMember]
            public string version;
            [DataMember]
            public string url;
        }
    }
}
