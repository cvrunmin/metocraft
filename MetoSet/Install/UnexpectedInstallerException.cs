using System;
using System.Runtime.Serialization;

namespace MTMCL.Forge
{
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
    }
}
