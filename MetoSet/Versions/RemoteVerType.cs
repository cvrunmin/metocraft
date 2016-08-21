using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MTMCL.Versions
{
    [DataContract]
    class RemoteVerType
    {
        [DataMember(IsRequired = true)]
        public string id { get; set; }
        [DataMember(IsRequired = true)]
        public string time { get; set; }
        [DataMember(IsRequired = true)]
        public string releaseTime { get; set; }
        [IgnoreDataMember]
        public DateTime relTime { get
            {
                return DateTime.Parse(releaseTime);
            }
        }
        [DataMember(IsRequired = true)]
        public string type { get; set; }
        [DataMember(IsRequired = false)]
        public string url { get; set; }
        public RemoteVerType()
        {
            id = "";
            time = "";
            releaseTime = "";
            type = "";
            url = "";
        }
    }
}
