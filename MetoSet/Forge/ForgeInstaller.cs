using ICSharpCode.SharpZipLib.Zip;
using MetoCraft.JsonClass;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace MetoCraft.Forge
{
    class ForgeInstaller
    {
        private List<Artifact> grabbed;
        public bool install(string target) {
            if (!File.Exists(target))
            {
                return false;
            }
            ZipFile zip = new ZipFile(target);
            ZipEntry infoentry = zip.GetEntry("install_profile.json");
            ForgeInstall info = LitJson.JsonMapper.ToObject<ForgeInstall>(new LitJson.JsonReader(new StreamReader(zip.GetInputStream(infoentry))));
            DirectoryInfo verroot = new DirectoryInfo(MeCore.Config.MCPath + "\\versions\\");
            DirectoryInfo targetdir = new DirectoryInfo(verroot.FullName + info.install.target + "\\");
            targetdir.Create();
            DirectoryInfo libdir = new DirectoryInfo(MeCore.Config.MCPath + "\\libraries\\");
            FileInfo verjson = new FileInfo(targetdir + info.install.target + ".json");
            if (!info.versionInfo.isInherited())
            {
                FileInfo verjar = new FileInfo(targetdir + info.install.target + ".jar");
                FileInfo mcjar = new FileInfo(verroot + info.install.minecraft + "\\" + info.install.minecraft + ".jar");
                try
                {
                    bool requiredelete = false;
                    if (!mcjar.Exists)
                    {
                        mcjar.Create();
                        requiredelete = true;
                        string url = MetoCraft.Resources.UrlReplacer.getDownloadUrl() + "versions/" + info.install.minecraft + "/" + info.install.minecraft + ".jar";
                        if (!downloadFileETag(url, mcjar.FullName))
                        {
                            mcjar.Delete();
                            return false;
                        }
                    }
                    if (info.install.stripMeta)
                    {
                        CopyAndStrip(mcjar.FullName, verjar.FullName);
                    }
                    else
                    {
                        File.Copy(mcjar.FullName, verjar.FullName);
                    }
                    if (requiredelete)
                    {
                        mcjar.Delete();
                    }
                }
                catch (Exception e)
                {
                    MeCore.Invoke(new Action(() => new KnownErrorReport(e.Message, e.StackTrace).Show()));
                    return false;
                }
            }
            FileInfo lib = info.install.GetLibraryPath(libdir);
/*            List<Artifact> bad = new List<Artifact>();
            downloadInstalledLibrary(info, libdir, grabbed, bad);
            if (bad.Count > 0)
            {
                return false;
            }*/
            if (!lib.Directory.Exists)
            {
                lib.Directory.Create();
            }
            try
            {
                TextWriter writer = new StreamWriter(verjson.Create(), Encoding.UTF8);
                LitJson.JsonWriter jsonwriter = new LitJson.JsonWriter(writer);
                LitJson.JsonMapper.ToJson(info.versionInfo,jsonwriter);
                writer.Close();
            }
            catch (Exception e)
            {
                MeCore.Invoke(new Action(() => new KnownErrorReport(e.Message, e.StackTrace).Show()));
                return false;
            }
            try
            {
                ZipEntry entry = zip.GetEntry(info.install.filePath);
                Stream ins = zip.GetInputStream(entry);
                FileStream stream = File.Create(lib.FullName);
                int size = 4096;
                byte[] data = new byte[4096];
                while (true)
                {
                    size = ins.Read(data, 0, data.Length);
                    if (size > 0)
                    {
                        stream.Write(data, 0, size);
                    }
                    else
                    {
                        break;
                    }
                }
                stream.Close();
                ins.Close();
            }
            catch (Exception e)
            {
                MeCore.Invoke(new Action(() => new KnownErrorReport(e.Message, e.StackTrace).Show()));
                return false;
            }
            return true;
        }

        string etag;
        private bool downloadFileETag(string url, string file) {
            try
            {
                Uri _url = new Uri(url);
                WebClient connect = new WebClient();
                connect.DownloadFile(url, file);
                etag = connect.Headers.Get("ETag");
                if (string.IsNullOrWhiteSpace(etag))
                {
                    etag = "-";
                }
                else if (etag.StartsWith("\"") && etag.EndsWith("\""))
                {
                    etag = etag.Substring(1, etag.Length - 2);
                }
                if (etag.Equals("-")) return true;
                try
                {
                    byte[] filedata = File.ReadAllBytes(file);
                    string hash = hashMD5(filedata);
                    return etag.Equals(hash, StringComparison.InvariantCultureIgnoreCase);
                }
                catch (Exception e)
                {
                    Logger.error(e.StackTrace);
                    MeCore.Invoke(new Action(() => new KnownErrorReport(e.Message, e.StackTrace).Show()));
                    return false;
                }
            }
            catch (Exception e)
            {
                MeCore.Invoke(new Action(() => new KnownErrorReport(e.Message, e.StackTrace).Show()));
                return false;
            }
        }
        public bool downloadFile(string libName, string libPath, string libURL, List<string> checksums)
        {
            try
            {
                Uri url = new Uri(libURL);
                WebClient connect = new WebClient();
                connect.DownloadFile(url, libPath);
                if (checksumsValid(new FileInfo(libPath), checksums))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (FileNotFoundException e)
            {
                MeCore.Invoke(new Action(() => new KnownErrorReport(e.Message, e.StackTrace).Show()));
                return false;
            }
            catch (Exception e)
            {
                MeCore.Invoke(new Action(() => new KnownErrorReport(e.Message, e.StackTrace).Show()));
                return false;
            }
        }
        private string hashMD5(byte[] data) {
            byte[] hashed = MD5.Create().ComputeHash(data);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashed.Length; i++)
            {
                builder.Append(hashed[i].ToString());
            }
            return builder.ToString();
        }
        private string hashSHA1(byte[] data)
        {
            byte[] hashed = SHA1.Create().ComputeHash(data);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashed.Length; i++)
            {
                builder.Append(hashed[i].ToString());
            }
            return builder.ToString();
        }
        private void CopyAndStrip(string source, string target) {
            ZipFile srczip = new ZipFile(source);
            ZipInputStream srcstream = new ZipInputStream(File.OpenRead(source));
            ZipOutputStream targetstream = new ZipOutputStream(new BufferedStream(File.OpenRead(target)));
            ZipEntry entry;
            while ((entry = srcstream.GetNextEntry()) != null)
            {
                if (entry.IsDirectory)
                {
                    targetstream.PutNextEntry(entry);
                }
                else if (entry.Name.StartsWith("META-INF"))
                {
                    continue;
                }
                else
                {
                    ZipEntry ine = new ZipEntry(entry.Name);
                    ine.DateTime = entry.DateTime;
                    targetstream.PutNextEntry(ine);
                    byte[] buffer = ReadEntry(srczip, entry);
                    targetstream.Write(buffer, 0, buffer.Length);
                }
            }
            srczip.Close();
            srcstream.Close();
            targetstream.Close();
        }
        private static byte[] ReadEntry(ZipFile file, ZipEntry entry)
        {
            return ReadFully(file.GetInputStream(entry));
        }
        private static byte[] ReadFully(Stream input)
        {
            long bufferSize = input.Length < 4096 ? input.Length : 4096;
            byte[] buffer = new byte[bufferSize];
            MemoryStream stream = new MemoryStream();
            int len = 0;
            do
            {
                len = input.Read(buffer, 0, buffer.Length);
                if (len > 0)
                {
                    stream.Write(buffer, 0, len);
                }
            } while (len != -1);
            return stream.ToArray();
        }
        private void downloadInstalledLibrary(ForgeInstall info, DirectoryInfo dir, List<Artifact> grabbed, List<Artifact> bad) {
            foreach (var item in info.versionInfo.libraries)
            {
                Artifact artifact = new Artifact(item.name);
                List<string> checksums = null;
                if (item.checksums != null)
                {
                    checksums = item.checksums.ToList();
                }
                if (item.clientreq)
                {
                    FileInfo file = artifact.GetLocalPath(dir);
                    string liburl = MetoCraft.Resources.UrlReplacer.getLibraryUrl();
                    if (!string.IsNullOrWhiteSpace(item.url))
                    {
                        liburl = item.url + "/";
                    }
                    if (file.Exists && checksumsValid(file, checksums))
                    {
                        continue;
                    }
                    file.Directory.Create();
                    file.Create();
                    liburl += artifact.Path;
                    FileInfo packFile = new FileInfo(file + (file.Name.Remove(file.Name.IndexOf(file.Extension))) + ".pack.xz");
                    if (!downloadFile(artifact.Descriptor, packFile.FullName, liburl + ".pack.xz", null))
                    {
                        if (!downloadFile(artifact.Descriptor, packFile.FullName, liburl, checksums))
                        {
                            if (!liburl.StartsWith(MetoCraft.Resources.UrlReplacer.getLibraryUrl()))
                            {
                                bad.Add(artifact);
                            }
                            else
                            {
                                grabbed.Add(artifact);
                            }
                        }

                    }
                    else
                    {
                        try
                        {
                            unpackLibrary(file, File.ReadAllBytes(packFile.FullName));
                            packFile.Delete();
                            if (checksumsValid(file, checksums))
                            {
                                grabbed.Add(artifact);
                            }
                            else
                            {
                                bad.Add(artifact);
                            }
                        }
                        catch (OutOfMemoryException)
                        {
                            bad.Add(artifact);
                            artifact.Memo = "Out of Memory: Try restarting installer with JVM Argument: -Xmx1G";
                        }
                        catch (Exception)
                        {
                            bad.Add(artifact);
                        }
                    }
                }
            }

        }
        private bool checksumsValid(FileInfo file, List<string> sums) {
            try
            {
                byte[] data = File.ReadAllBytes(file.FullName);
                bool valid = sums == null || sums.Count == 0 || sums.Contains(hashSHA1(data));
                if (!valid && file.FullName.EndsWith(".jar"))
                {
                    valid = validateJar(file, data, sums);
                }
                return valid;
            }
            catch (Exception e)
            {
                MeCore.Invoke(new Action(() => new KnownErrorReport(e.Message, e.StackTrace).Show()));
                return false;
            }
        }
        private bool validateJar(FileInfo file, byte[] data, List<string> sums) {
            Dictionary<string, string> files = new Dictionary<string, string>();
            string[] hashes = null;
            ZipInputStream jar = new ZipInputStream(new MemoryStream(data));
            ZipEntry entry = jar.GetNextEntry();
            while (entry != null)
            {
                byte[] eData = ReadFully(jar);
                if (entry.Name.Equals("checksums.sha1"))
                {
                    hashes = new string(Encoding.UTF8.GetChars(eData)).Split('\n');
                }
                if (!entry.IsDirectory)
                {
                    files.Add(entry.Name, hashSHA1(eData));
                }
                entry = jar.GetNextEntry();
            }
            jar.Close();
            if (hashes != null)
            {
                string a = "";
                files.TryGetValue("checksums.sha1", out a);
                bool failed = !sums.Contains(a);
                if (failed)
                {
                    
                }
                else
                {
                    foreach (var hash in hashes)
                    {
                        if (hash.Trim().Equals("") || !hash.Contains(" ")) continue;
                        string[] e = hash.Split(' ');
                        string validChecksum = e[0];
                        string target = hash.Substring(validChecksum.Length + 1);
                        string checksum = "";
                        files.TryGetValue(target, out checksum);
                        if (!files.ContainsKey(target) || checksum == null)
                        {
                            failed = true;
                        }
                        else if (!checksum.Equals(validChecksum))
                        {
                            failed = true;
                        }
                    }
                }
                return !failed;
            }
            else
            {
                return false;
            }
        }
        private void unpackLibrary(FileInfo output, byte[] data)
        {
            if (output.Exists)
            {
                output.Delete();
            }

            byte[] decompressed = ReadFully(new ZipInputStream(new MemoryStream(data)));
            string end = new string(Encoding.UTF8.GetChars(decompressed), decompressed.Length - 4, 4);
            if (!end.Equals("SIGN"))
            {
                return;
            }

            int x = decompressed.Length;
            int len =
                    ((decompressed[x - 8] & 0xFF)) |
                    ((decompressed[x - 7] & 0xFF) << 8) |
                    ((decompressed[x - 6] & 0xFF) << 16) |
                    ((decompressed[x - 5] & 0xFF) << 24);
            FileInfo temp = new FileInfo("art.pack");

            byte[] checksums = new byte[len];
            Array.Copy(decompressed, decompressed.Length - len - 8, checksums, 0, len);
            FileStream o = new FileStream(temp.FullName, FileMode.Open);
            o.Write(decompressed, 0, decompressed.Length - len - 8);
            o.Close();
            decompressed = null;
            data = null;
            GC.Collect();

            FileStream jarBytes = new FileStream(output.FullName, FileMode.Open);
            ZipOutputStream jos = new ZipOutputStream(jarBytes);

//            Pack200.newUnpacker().unpack(temp, jos);

            ZipEntry checksumsFile = new ZipEntry("checksums.sha1");
            checksumsFile.DateTime = DateTime.FromFileTime(0);
            jos.PutNextEntry(checksumsFile);
            jos.Write(checksums, 0, checksums.Length);
            jos.CloseEntry();

            jos.Close();
            jarBytes.Close();
            temp.Delete();
        }
    }
}
