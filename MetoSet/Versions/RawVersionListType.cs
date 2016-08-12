using System.Runtime.Serialization;

namespace MTMCL.Versions
{
    [DataContract]
    class RawVersionListType
    {
        [DataMember(Order = 0,IsRequired = true)]
        private RemoteVerType[] versions;
        [DataMember(Order = 1,IsRequired = true)]
        private latest latest;

        public RemoteVerType[] getVersions()
        {
            return versions;
        }

        public latest getLastestVersion()
        {
            return latest;
        }

        public RawVersionListType()
        {
            versions = null;
            latest = new latest();
        }
    }
    [DataContract]
    class latest
    {
        [DataMember(IsRequired = true)]
        string snapshot { get; set; }
        [DataMember(IsRequired = true)]
        string release { get; set; }

        public latest()
        {
            snapshot = "";

            release = "";
        }
    }
}
