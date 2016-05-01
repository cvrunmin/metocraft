using ICSharpCode.SharpZipLib.Zip;
using MTMCL.Install;
using MTMCL.JsonClass;
using MTMCL.Lang;
using MTMCL.util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media.Imaging;

namespace MTMCL.Forge
{
    class ForgeInstaller
    {
        private List<Artifact> grabbed { get; set; }
        public bool install(string target)
        {
            try
            {
                if (!File.Exists(target))
                {
                    return false;
                }
                if (MeCore.Config.Javaw.Equals("undefined"))
                {
                    return false;
                }
                if (!target.EndsWith(".jar") & !target.EndsWith(".exe"))
                {
                    throw new UnexpectedInstallerException(target, "Target file doesn\'t end with .jar or .exe");
                }
                ZipFile zip = new ZipFile(target);
                ZipEntry infoentry = zip.GetEntry("install_profile.json");
                if (infoentry == null) throw new UnexpectedInstallerException(target, "Target file doesn't a valid installer");
                ForgeInstall info = JsonConvert.DeserializeObject<ForgeInstall>(new StreamReader(zip.GetInputStream(infoentry)).ReadToEnd());
                string path = MeCore.Config.MCPath;
                if (MeCore.IsServerDedicated)
                {
                    if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath))
                    {
                        path = path.Replace(MeCore.Config.MCPath, Path.Combine(MeCore.BaseDirectory, MeCore.Config.Server.ClientPath));
                    }
                }
                DirectoryInfo verroot = new DirectoryInfo(path + "\\versions\\");
                DirectoryInfo targetdir = new DirectoryInfo(verroot.FullName + info.install.target + "\\");
                targetdir.Create();
                DirectoryInfo libdir = new DirectoryInfo(path + "\\libraries\\");
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
                            string url = Resources.UrlReplacer.getDownloadUrl() + "versions/" + info.install.minecraft + "/" + info.install.minecraft + ".jar";
                            if (!downloadFileETag(url, mcjar.FullName))
                            {
                                mcjar.Delete();
                                zip.Close();
                                return false;
                            }
                        }
                        if (!verjar.Exists) {
                            if (info.install.stripMeta)
                            {
                                CopyAndStrip(mcjar.FullName, verjar.FullName);
                            }
                            else
                            {
                                File.Copy(mcjar.FullName, verjar.FullName);
                            }
                        }
                        if (requiredelete)
                        {
                            mcjar.Delete();
                        }
                    }
                    catch (Exception e)
                    {
                        MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), e.Message, string.Format(Lang.LangManager.GetLangFromResource("ForgeNoVersionSolve"), info.install.minecraft)) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                        //zip.Close();
                        //return false;
                    }
                }
                FileInfo lib = info.install.GetLibraryPath(libdir);
                List<Artifact> bad = new List<Artifact>();
                downloadInstalledLibrary(info, libdir, grabbed, bad);
                /*if (bad.Count > 0)
                {
                    zip.Close();
                    return false;
                }*/
                if (!lib.Directory.Exists)
                {
                    lib.Directory.Create();
                }
                try
                {
                    TextWriter writer = new StreamWriter(verjson.Create(), Encoding.UTF8);
                    writer.Write(JsonConvert.SerializeObject(info.versionInfo, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
                    writer.Close();
                }
                catch (Exception e)
                {
                    MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), e.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                    //zip.Close();
                    //return false;
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
                    MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), e.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                    //zip.Close();
                    //return false;
                }
                zip.Close();
                return true;
            }
            catch (UnexpectedInstallerException e) {
                MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), e.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                return false;
            }
            catch (ZipException e)
            {
                throw new UnexpectedInstallerException(target, "Failed to read the jar", e);
            }
            catch (JsonException e)
            {
                throw new UnexpectedInstallerException(target, "Unexpected install profile json", e);
            }
            catch (Exception e) {
                MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), e.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                return false;
            }
        }

        string etag;
        private bool downloadFileETag(string url, string file)
        {
            try
            {
                Uri _url = new Uri(Resources.UrlReplacer.getForgeMaven(url));
                WebClient connect = new WebClient();
                //HttpWebRequest connect = WebRequest.CreateHttp(_url);
                connect.DownloadFile(url, file);
                //connect.Timeout = 5000;
                //connect.ReadWriteTimeout = 5000;
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
                    Logger.log("  ETag: " + etag);
                    Logger.log("  MD5:  " + hash);
                    return etag.Equals(hash, StringComparison.InvariantCultureIgnoreCase);
                }
                catch (Exception e)
                {
                    Logger.error(e.StackTrace);
                    MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice( new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), e.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                    return false;
                }
            }
            catch (Exception e)
            {
                MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), e.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                return false;
            }
        }
        public bool downloadFile(string libName, string libPath, string libURL, List<string> checksums)
        {
            try
            {
                Uri url = new Uri(Resources.UrlReplacer.getForgeMaven(libURL));
                using (WebClient connect = new WebClient())
                {
                    connect.Headers.Add("User-Agent", "MTMCL" + MeCore.version);
                    connect.DownloadFile(url, libPath);
                    return checksumsValid(new FileInfo(libPath), checksums);
                } 
            }
            catch (FileNotFoundException e)
            {
                if (!libURL.EndsWith(".pack.xz"))
                {
                    MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice( new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), e.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                }
                return false;
            }
            catch (Exception e)
            {
                MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()),e.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                return false;
            }
        }
        private string hashMD5(byte[] data)
        {
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
        private void CopyAndStrip(string source, string target)
        {
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
        private void CopyEntry(FileInfo source, ZipOutputStream target)
        {
            ZipFile srczip = new ZipFile(source.FullName);
            ZipInputStream srcstream = new ZipInputStream(File.OpenRead(source.FullName));
            ZipOutputStream targetstream = target;
            ZipEntry entry;
            while ((entry = srcstream.GetNextEntry()) != null)
            {
                if (entry.IsDirectory)
                {
                    targetstream.PutNextEntry(entry);
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
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        private void downloadInstalledLibrary(ForgeInstall info, DirectoryInfo dir, List<Artifact> grabbed, List<Artifact> bad)
        {
            Stream a = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MTMCL.Resources.unPackXZ.jar");
            Stream s = File.Open("unpacker.jar", FileMode.Create);
            a.CopyTo(s);
            s.Close();
            a.Close();
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
                    string liburl = Resources.UrlReplacer.getLibraryUrl();
                    if (!string.IsNullOrWhiteSpace(item.url))
                    {
                        liburl = item.url + "/";
                    }
                    if (file.Exists && checksumsValid(file, checksums))
                    {
                        continue;
                    }
                    file.Directory.Create();
                    //file.Create();
                    liburl += artifact.Path;
                    FileInfo packFile = new FileInfo(file + ".pack.xz");
                    if (!downloadFile(artifact.Descriptor, liburl.StartsWith("http://files.minecraftforge.net/maven") ? packFile.FullName : file.FullName, liburl.StartsWith("http://files.minecraftforge.net/maven") ? liburl + ".pack.xz" : liburl, null))
                    {
                        if (!downloadFile(artifact.Descriptor, packFile.FullName, liburl, checksums))
                        {
                            if (!liburl.StartsWith(Resources.UrlReplacer.getLibraryUrl()))
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
                            unpackLibrary(packFile, file);
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
                            artifact.Memo = "Out of Memory";
                        }
                        catch (Exception)
                        {
                            bad.Add(artifact);
                        }
                    }
                }
            }
            File.Delete("unpacker.jar");
        }
        private bool checksumsValid(FileInfo file, List<string> sums)
        {
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
                MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()),e.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                Logger.log(e);
                return false;
            }
        }
        private bool validateJar(FileInfo file, byte[] data, List<string> sums)
        {
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
        private void unpackLibrary(FileInfo input, FileInfo output)
        {
            if (output.Exists)
            {
                output.Delete();
            }
            Stream s = input.OpenRead();
            byte[] decompressed = ReadFully(new ManagedXZ.XZDecompressStream(s));
            string end = new string(Encoding.UTF8.GetChars(decompressed));
            if (!end.EndsWith("SIGN"))
            {
                return;
            }

            int x = decompressed.Length;
            int len =
                    ((decompressed[x - 8] & 0xFF)) |
                    ((decompressed[x - 7] & 0xFF) << 8) |
                    ((decompressed[x - 6] & 0xFF) << 16) |
                    ((decompressed[x - 5] & 0xFF) << 24);
            Logger.log("  Signed");
            Logger.log("  Checksum Length: " + len);
            Logger.log("  Total Length:    " + (decompressed.Length - len - 8));
            byte[] checksums = new byte[len];
            Array.Copy(decompressed, decompressed.Length - len - 8, checksums, 0, len);
            decompressed = null;
            x = 0;
            len = 0;
            s.Close();
            GC.Collect();
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo()
            {
                UseShellExecute = false,
                FileName = MeCore.Config.Javaw,
                Arguments = "-jar " + "unpacker.jar" + " -XZFilePath \"" + input.FullName + "\" -unXZFilePath \"" + input.FullName.Replace(".xz", "") + "\" -unPackPath \"" + output.FullName.Replace(".pack.xz", "") + "\""
            };
            process.Start();
            process.WaitForExit();
            FileStream jarBytes = new FileStream(output.FullName, FileMode.Open);
            ZipOutputStream jos = new ZipOutputStream(jarBytes);
            ZipEntry checksumsFile = new ZipEntry("checksums.sha1");
            checksumsFile.DateTime = DateTime.FromFileTime(0);
            jos.PutNextEntry(checksumsFile);
            jos.Write(checksums, 0, checksums.Length);
            jos.CloseEntry();
            checksums = null;
            jos.Close();
            jarBytes.Close();
            input.Delete();
            File.Delete(input.FullName.Replace(".xz", ""));
        }
    }
}
