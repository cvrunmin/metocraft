using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using MTMCL.Lang;
using MTMCL.util;
using System;
using System.IO;

namespace MTMCL.Threads
{
    class ModPackProcesser
    {
        public bool install(string target)
        {
            if (!File.Exists(target))
            {
                return false;
            }
            ZipFile zip = new ZipFile(target);
            bool neednewgamedirectory = true;
            try
            {
                foreach (ZipEntry item in zip)
                {
                    if (item.Name.StartsWith(MeCore.Config.Server.ClientPath ?? ".minecraft"))
                    {
                        neednewgamedirectory = false;
                        continue;
                    }
                    if (!item.IsFile)
                    {
                        continue;
                    }
                    string entryFileName = item.Name;
                    byte[] buffer = new byte[4096];
                    Stream zipStream = zip.GetInputStream(item);
                    string fullZipToPath = "";
                    if (neednewgamedirectory)
                    {
                        fullZipToPath = Path.Combine(MeCore.BaseDirectory, MeCore.Config.Server.ClientPath ?? ".minecraft", entryFileName);
                    }
                    else
                    {
                        fullZipToPath = Path.Combine(MeCore.BaseDirectory, entryFileName);
                    }

                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            catch (Exception ex)
            {
                MeCore.Invoke(new Action(() => MeCore.MainWindow.addNotice(new Notice.CrashErrorBar(string.Format(LangManager.GetLangFromResource("ErrorNameFormat"), DateTime.Now.ToLongTimeString()), ex.ToWellKnownExceptionString()))));

            }
            finally
            {
                if (zip != null)
                {
                    zip.IsStreamOwner = true;
                    zip.Close();
                }
            }
            return true;
        }
    }
}
