using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading;
using MetoSet.Lang;
using MetoSet;

namespace MetoSet.Versions
{
    static class VersionHelper
    {
        public delegate void ImportProgressChangeEventHandler(string status);
        public static event ImportProgressChangeEventHandler ImportProgressChangeEvent;

        public delegate void ImportFinishEventHandler();
        public static event ImportFinishEventHandler ImportFinish;

        private static void OnImportFinish()
        {
            ImportFinishEventHandler handler = ImportFinish;
            if (handler != null) MeCore.Invoke(handler);
        }


        private static void OnImportProgressChangeEvent(string status)
        {
            ImportProgressChangeEventHandler handler = ImportProgressChangeEvent;
            if (handler != null) MeCore.Invoke(handler, new[] {status});
        }
/**
        public static void ImportOldMc(string importName, string importFrom, Delegate callback = null)
        {
            var thread = new Thread(() =>
            {
                OnImportProgressChangeEvent(("ImportMain"));
                Directory.CreateDirectory(MeCore.Config.MCPath + "\\versions\\" + importName);
                File.Copy(importFrom + "\\bin\\minecraft.jar",
                    MeCore.Config.MCPath + "\\versions\\" + importName + "\\" + importName + ".jar");
                OnImportProgressChangeEvent(("ImportCreateJson"));
                var info = new gameinfo {id = importName};
                string timezone = DateTimeOffset.Now.Offset.ToString();
                if (timezone[0] != '-')
                {
                    timezone = "+" + timezone;
                }
                info.time = DateTime.Now.GetDateTimeFormats('s')[0] + timezone;
                info.releaseTime = DateTime.Now.GetDateTimeFormats('s')[0] + timezone;
                info.type = "Port by Metocraft,code By BMCL";
                info.minecraftArguments = "${auth_player_name}";
                info.mainClass = "net.minecraft.client.Minecraft";
                OnImportProgressChangeEvent(("ImportSolveNative"));
                var libs = new ArrayList();
                var bin = new DirectoryInfo(importFrom + "\\bin");
                foreach (FileInfo file in bin.GetFiles("*.jar"))
                {
                    var libfile = new libraries.libraryies();
                    if (file.Name == "minecraft.jar")
                        continue;
                    if (
                        !Directory.Exists(MeCore.Config.MCPath + "\\libraries\\" + importName + "\\" +
                                          file.Name.Substring(0, file.Name.Length - 4) + "\\BMCL\\"))
                    {
                        Directory.CreateDirectory(MeCore.Config.MCPath + "\\libraries\\" + importName + "\\" +
                                                  file.Name.Substring(0, file.Name.Length - 4) + "\\BMCL\\");
                    }
                    File.Copy(file.FullName,
                        MeCore.Config.MCPath + "\\libraries\\" + importName + "\\" + file.Name.Substring(0, file.Name.Length - 4) +
                        "\\BMCL\\" + file.Name.Substring(0, file.Name.Length - 4) + "-BMCL.jar");
                    libfile.name = importName + ":" + file.Name.Substring(0, file.Name.Length - 4) + ":BMCL";
                    libs.Add(libfile);
                }
                var nativejar = new ICSharpCode.SharpZipLib.Zip.FastZip();
                if (!Directory.Exists(MeCore.Config.MCPath + "\\libraries\\" + importName + "\\BMCL\\"))
                {
                    Directory.CreateDirectory(MeCore.Config.MCPath + "\\libraries\\" + importName + "\\native\\BMCL\\");
                }
                nativejar.CreateZip(
                    MeCore.Config.MCPath + "\\libraries\\" + importName + "\\native\\BMCL\\native-BMCL-natives-windows.jar",
                    importFrom + "\\bin\\natives", false, @"\.dll");
                var nativefile = new libraries.libraryies {name = importName + ":native:BMCL"};
                var nativeos = new libraries.OS {windows = "natives-windows"};
                nativefile.natives = nativeos;
                nativefile.extract = new libraries.extract();
                libs.Add(nativefile);
                info.libraries = (libraries.libraryies[]) libs.ToArray(typeof (libraries.libraryies));
                OnImportProgressChangeEvent(("ImportWriteJson"));
                var wcfg = new FileStream(MeCore.Config.MCPath + "\\versions\\" + importName + "\\" + importName + ".json",
                    FileMode.Create);
                var infojson = new DataContractJsonSerializer(typeof (gameinfo));
                infojson.WriteObject(wcfg, info);
                wcfg.Close();
                OnImportProgressChangeEvent(("ImportSolveLib"));
                if (Directory.Exists(importFrom + "\\lib"))
                {
                    if (!Directory.Exists(MeCore.Config.MCPath + "\\lib"))
                    {
                        Directory.CreateDirectory(MeCore.Config.MCPath + "\\lib");
                    }
                    foreach (
                        string libfile in Directory.GetFiles(importFrom + "\\lib", "*", SearchOption.AllDirectories))
                    {
                        if (!File.Exists(MeCore.Config.MCPath + "\\lib\\" + System.IO.Path.GetFileName(libfile)))
                        {
                            File.Copy(libfile, MeCore.Config.MCPath + "\\lib\\" + System.IO.Path.GetFileName(libfile));
                        }
                    }
                }
                OnImportProgressChangeEvent(("ImportSolveMod"));
                if (Directory.Exists(importFrom + "\\mods"))
                    util.FileHelper.dircopy(importFrom + "\\mods", MeCore.Config.MCPath + "\\versions\\" + importName + "\\mods");
                else
                    Directory.CreateDirectory(MeCore.Config.MCPath + "\\versions\\" + importName + "\\mods");
                if (Directory.Exists(importFrom + "\\coremods"))
                    util.FileHelper.dircopy(importFrom + "\\coremods",
                        MeCore.Config.MCPath + "\\versions\\" + importName + "\\coremods");
                else
                    Directory.CreateDirectory(MeCore.Config.MCPath + "\\versions\\" + importName + "\\coremods");
                if (Directory.Exists(importFrom + "\\config"))
                    util.FileHelper.dircopy(importFrom + "\\config", MeCore.Config.MCPath + "\\versions\\" + importName + "\\config");
                else
                    Directory.CreateDirectory(MeCore.Config.MCPath + "\\versions\\" + importName + "\\configmods");
                OnImportFinish();
                if (callback != null)
                {
                    MeCore.Invoke(callback);
                }
            });
            thread.Start();
        }*/
    }
}
