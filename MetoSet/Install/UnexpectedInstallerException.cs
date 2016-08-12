using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace MTMCL.Install
{
    [Serializable]
    public class UnexpectedInstallerException : Exception, ISerializable
    {
        public string FilePath { get; private set; }
        public UnexpectedInstallerException(string fp) {
            FilePath = fp;
        }
        public UnexpectedInstallerException(string fp, string message) : base(message + " File: " + fp) {
            FilePath = fp;
        }
        public UnexpectedInstallerException(string fp, string message, Exception innerException) : base(message + " File: " + fp, innerException) {
            FilePath = fp;
        }
        protected UnexpectedInstallerException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            base.GetObjectData(info, context);
            info.AddValue("FilePath", FilePath);
        }
    }
}
