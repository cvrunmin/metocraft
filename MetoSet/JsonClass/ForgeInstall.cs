using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MetoCraft.JsonClass
{
    [DataContract]
    public class ForgeInstall
    {
        [DataMember]
        public Install install;
        [DataMember]
        public Version versionInfo;
        [DataContract]
        public class Install {
            [DataMember]
            public string profileName, target, path, version, filePath, minecraft, mirrorList;
            [DataMember]
            public bool stripMeta;
            public FileInfo GetLibraryPath(DirectoryInfo directory) {
                string[] split = path.Split(':');
                string[] subsplit = split[0].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                string root = directory.FullName + "\\";
                foreach (var item in subsplit)
                {
                    root += item + "\\";
                }
                root += split[1] + "\\" + split[2] + "\\";
                string file = split[1] + "-" + split[2] + ".jar";
                return new FileInfo(root + file);
            }
        }
        [DataContract]
        public class Version {
            [DataMember]
            public string id, time, releaseTime, type, minecraftArguments, mainClass, assets, inheritsFrom, jar;
            [DataMember]
            public int minimumLauncherVersion;
            [DataMember]
            public Library[] libraries;
            [DataContract]
            public class Library {
                [DataMember]
                public string name, comment, url;
                [DataMember]
                public bool clientreq, serverreq;
                [DataMember]
                public string[] checksums;
            }
            public bool isInherited() {
                return (!string.IsNullOrWhiteSpace(inheritsFrom)) && (!string.IsNullOrWhiteSpace(jar));
            }
        }
    }
}
