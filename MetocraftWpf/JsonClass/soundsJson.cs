using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MetoCraft.JsonClass
{
    [DataContract]
    class soundsEntity
    {
        [DataMember]
        public string name;
        [DataMember]
        public bool stream;
    }
}
