using System.IO;

namespace MetoCraft.Forge
{
    class Artifact
    {
        private string domain;
        private string name;
        private string version;
        private string classifier = null;
        private string ext = "jar";
        private string path;
        private string file;
        private string descriptor;
        private string memo;

        public string Domain
        {
            get
            {
                return domain;
            }

            private set
            {
                domain = value;
            }
        }
        public string Name
        {
            get
            {
                return name;
            }

            private set
            {
                name = value;
            }
        }
        public string Version
        {
            get
            {
                return version;
            }

            private set
            {
                version = value;
            }
        }
        public string Classifier
        {
            get
            {
                return classifier;
            }

            private set
            {
                classifier = value;
            }
        }
        public string Ext
        {
            get
            {
                return ext;
            }

            private set
            {
                ext = value;
            }
        }
        public string Path
        {
            get
            {
                return path;
            }

            private set
            {
                path = value;
            }
        }
        public string File
        {
            get
            {
                return file;
            }

            private set
            {
                file = value;
            }
        }
        public string Descriptor
        {
            get
            {
                return descriptor;
            }

            private set
            {
                descriptor = value;
            }
        }
        public string Memo
        {
            get
            {
                return memo;
            }

            set
            {
                memo = value;
            }
        }

        public Artifact(string desc) {
            Descriptor = desc;
            string[] parts = desc.Split(':');
            Domain = parts[0];
            Name = parts[1];
            int last = parts.Length - 1;
            int idx = parts[last].IndexOf('@');
            if (idx != -1)
            {
                Ext = parts[last].Substring(idx + 1);
                parts[last] = parts[last].Substring(0,idx);
            }
            Version = parts[2];
            if (parts.Length > 3)
            {
                Classifier = parts[3];
            }
            File = Name + "-" + Version;
            if (Classifier != null) File += Classifier;
            File += "." + Ext;
            Path = Domain.Replace('.','/') + "/" + Name + "/" + Version + "/" + File;
        }
        public FileInfo GetLocalPath(FileInfo b) {
            return new FileInfo(b.FullName.Replace('/', '\\'));
        }
        public FileInfo GetLocalPath(DirectoryInfo b)
        {
            return new FileInfo(b.FullName.Replace('/', '\\') + Path.Replace('/', '\\'));
        }
        public override string ToString()
        {
            if (Memo != null) return Descriptor + "\n    " + Memo;
            return Descriptor;
        }
    }
}
