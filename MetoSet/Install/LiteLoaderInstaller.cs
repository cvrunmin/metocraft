using ICSharpCode.SharpZipLib.Zip;
using MTMCL.JsonClass;
using MTMCL.Lang;
using MTMCL.util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace MTMCL.Install
{
    class LiteLoaderInstaller
    {
        bool withForge = false;
        string inheritVer;
        public LiteLoaderInstaller() { }
        public LiteLoaderInstaller(bool withForge, string inherit)
        {
            this.withForge = withForge;
            inheritVer = inherit;
        }
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
                LiteLoaderInstall info = JsonConvert.DeserializeObject<LiteLoaderInstall>(new StreamReader(zip.GetInputStream(infoentry)).ReadToEnd());
                string path = MeCore.Config.MCPath;
                if (MeCore.IsServerDedicated)
                {
                    if (!string.IsNullOrWhiteSpace(MeCore.Config.Server.ClientPath))
                    {
                        path = path.Replace(MeCore.Config.MCPath, Path.Combine(MeCore.BaseDirectory, MeCore.Config.Server.ClientPath));
                    }
                }
                if (withForge)
                {
                    info.install.target = info.install.target + "-" + inheritVer;
                    info.versionInfo.id = info.versionInfo.id + "-" + inheritVer;
                }
                DirectoryInfo verroot = new DirectoryInfo(path + "\\versions\\");
                DirectoryInfo targetdir = new DirectoryInfo(verroot.FullName + info.install.target + "\\");
                targetdir.Create();
                DirectoryInfo libdir = new DirectoryInfo(path + "\\libraries\\");
                FileInfo verjson = new FileInfo(targetdir + info.install.target + ".json");

                FileInfo lib = info.install.GetLibraryPath(libdir);

                if (!lib.Directory.Exists)
                {
                    lib.Directory.Create();
                }
                try
                {
                    TextWriter writer = new StreamWriter(verjson.Create(), Encoding.UTF8);
                    if (withForge)
                    {
                        info.versionInfo.inheritsFrom = inheritVer;
                        info.versionInfo.minecraftArguments = "--tweakClass com.mumfrey.liteloader.launch.LiteLoaderTweaker " + Versions.VersionReader.GetFurtherVersion(MeCore.Config.MCPath, info.versionInfo.inheritsFrom).minecraftArguments;
                    }
                    writer.Write(JsonConvert.SerializeObject(info.versionInfo, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
                    writer.Close();
                }
                catch (Exception e)
                {
                    MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLocalized("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), e.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
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
                    MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLocalized("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), e.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                    //zip.Close();
                    //return false;
                }
                zip.Close();
                return true;
            }
            catch (UnexpectedInstallerException e)
            {
                MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLocalized("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), e.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
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
            catch (Exception e)
            {
                MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLocalized("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), e.ToWellKnownExceptionString()) { ImgSrc = new BitmapImage(new Uri("pack://application:,,,/Resources/error-banner.jpg")) })));
                return false;
            }
        }
    }
}
