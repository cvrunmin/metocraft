﻿using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Windows.Media.Imaging;

namespace MTMCL.Themes
{
    internal class Theme
    {
        public virtual System.Windows.Media.ImageSource Image { get; private set; }
        public virtual string ImageSource { get; private set; }
        public virtual MahApps.Metro.Accent Accent { get; private set; }
        public virtual string AccentName { get; private set; }
        public virtual string Name { get; private set; }
        internal bool isTmp { get; set; }
        private string defaultPath => Path.Combine(MeCore.DataDirectory, "Themes", Name + ".mtheme");

        public void SaveMTMCLTheme () { SaveMTMCLTheme(defaultPath); }
        public void SaveMTMCLTheme (string path)
        {
            using (var jw = new Newtonsoft.Json.JsonTextWriter(new StreamWriter(path)))
            {
                jw.Formatting = Newtonsoft.Json.Formatting.Indented;
                var colorh = Accent.Resources["HighlightColor"];
                var colora = Accent.Resources["AccentBaseColor"];
                string hc = ((System.Windows.Media.Color) colorh).ToString().Remove(0, 3),
                    ac = ((System.Windows.Media.Color) colora).ToString().Remove(0, 3);
                ThemeInfo info = new ThemeInfo() { AccentColor = ac, HighlightColor = hc, AccentName = AccentName, Name = Name, Background = ImageSource };
                Newtonsoft.Json.JsonSerializer.Create().Serialize(jw, info, typeof(ThemeInfo));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Theme path</param>
        public static Theme LoadMTMCLTheme (string path, bool skipReadImg = false)
        {
            return LoadMTMCLTheme(new StreamReader(path), skipReadImg);
        }
        public static Theme LoadMTMCLTheme (StreamReader reader, bool skipReadImg = false)
        {
            using (var jr = new Newtonsoft.Json.JsonTextReader(reader))
            {
                ThemeInfo info = Newtonsoft.Json.JsonSerializer.Create().Deserialize<ThemeInfo>(jr);
                Theme theme = new Theme();
                theme.ImageSource = info.Background;
                if (!skipReadImg)
                    using (var ms = new MemoryStream())
                    {
                        Image img = System.Drawing.Image.FromFile(info.Background);
                        img.Save(ms, img.RawFormat);
                        ms.Seek(0, SeekOrigin.Begin);
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.StreamSource = ms;
                        bi.EndInit();
                        theme.Image = bi;
                    }
                theme.Name = info.Name;
                theme.AccentName = info.AccentName;
                int hc = Convert.ToInt32(info.HighlightColor, 16),
                    ac = Convert.ToInt32(info.AccentColor, 16);
                System.Windows.Media.Color colorh = System.Windows.Media.Color.FromRgb((byte) ((hc >> 16) & 255), (byte) ((hc >> 8) & 255), (byte) (hc & 255)),
                    colora = System.Windows.Media.Color.FromRgb((byte) ((ac >> 16) & 255), (byte) ((ac >> 8) & 255), (byte) (ac & 255));
                theme.Accent = MahApps.Metro.ThemeManager.GetAccent(theme.AccentName) ?? new MahApps.Metro.Accent() { Resources = Accents.AccentHelper.createResourceDictionary(colorh, colora) };
                return theme;
            }
        }

        public void PackMTMCLTheme (string path)
        {
            using (var zos = new ZipOutputStream(new FileStream(path, FileMode.Create)))
            {
                zos.SetLevel(4);
                FileInfo info = new FileInfo(ImageSource);
                var zip = new ZipEntry(ZipEntry.CleanName("Background/" + Path.GetFileName(ImageSource)));
                zip.DateTime = DateTime.Now;
                zip.Size = info.Length;
                zos.PutNextEntry(zip);
                byte[] buffer = new byte[4096];
                using (FileStream streamReader = info.OpenRead())
                {
                    StreamUtils.Copy(streamReader, zos, buffer);
                }
                zos.CloseEntry();
                if (!File.Exists(defaultPath))
                {
                    string tmp = ImageSource;
                    ImageSource = "Background/" + Path.GetFileName(ImageSource);
                    SaveMTMCLTheme();
                    ImageSource = tmp;
                }
                var info1 = new FileInfo(defaultPath);
                var zip1 = new ZipEntry(ZipEntry.CleanName(Path.GetFileName(defaultPath)));
                zip1.DateTime = DateTime.Now;
                zip1.Size = info1.Length;
                zos.PutNextEntry(zip1);
                using (FileStream streamReader = info1.OpenRead())
                {
                    StreamUtils.Copy(streamReader, zos, buffer);
                }
                zos.CloseEntry();
                zos.IsStreamOwner = true;
            }
        }
        internal static void UnpackMTMCLTheme (string path)
        {
            ZipFile zf = null;
            try
            {
                FileStream fs = File.OpenRead(path);
                zf = new ZipFile(fs);
                foreach (ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile)
                    {
                        continue;           // Ignore directories
                    }
                    string entryFileName = zipEntry.Name;


                    byte[] buffer = new byte[4096];     // 4K is optimum
                    Stream zipStream = zf.GetInputStream(zipEntry);

                    // Manipulate the output filename here as desired.
                    string fullZipToPath = Path.Combine(MeCore.DataDirectory, "Themes", entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            finally
            {
                if (zf != null)
                {
                    zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zf.Close(); // Ensure we release resources
                }
            }
        }

        public static Theme readMTMCLThemeInstantly (string path)
        {
            ZipFile zf = null;
            bool imgFound = false;
            Theme theme = new Theme();
            try
            {
                FileStream fs = File.OpenRead(path);
                zf = new ZipFile(fs);
                foreach (ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile)
                    {
                        continue;           // Ignore directories
                    }
                    string entryFileName = zipEntry.Name;

                    byte[] buffer = new byte[4096];     // 4K is optimum
                    Stream zipStream = zf.GetInputStream(zipEntry);

                    // Manipulate the output filename here as desired.
                    string fullZipToPath = Path.Combine(MeCore.DataDirectory, "Themes", entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (MemoryStream stream = new MemoryStream())
                    {
                        StreamUtils.Copy(zipStream, stream, buffer);
                        if (imgFound & System.Text.RegularExpressions.Regex.IsMatch(entryFileName, "/\\w+\\/+\\w+(.png|.jpg|.jpeg|.jpe|.jfif|.tif|.tiff|.bmp|.dib|.gif)/i"))
                        {
                            BitmapImage bi = new BitmapImage();
                            bi.BeginInit();
                            bi.CacheOption = BitmapCacheOption.OnLoad;
                            bi.StreamSource = stream;
                            bi.EndInit();
                            theme.Image = bi;
                        }
                        else if (System.Text.RegularExpressions.Regex.IsMatch(entryFileName, "/\\w+.mtheme/i"))
                        {
                            Theme tmpt = LoadMTMCLTheme(new StreamReader(stream), true);
                            theme.Accent = tmpt.Accent;
                            theme.AccentName = tmpt.AccentName;
                            theme.Name = theme.Name;
                            theme.ImageSource = tmpt.ImageSource;
                            tmpt = null;
                        }
                    }
                }
            }
            finally
            {
                if (zf != null)
                {
                    zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zf.Close(); // Ensure we release resources
                }
            }
            return theme;
        }
        public Theme MakeChanges (string type, object value)
        {
            //try {
            var properties = GetType().GetProperties(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                var property = GetType().GetProperty(type, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (property != null)
                {
                    property.SetValue(this, value, null);
                }
            //} catch { }
            return this;
        }
    }
    internal class DefaultTheme : Theme
    {
        public override System.Windows.Media.ImageSource Image
        { get { return new BitmapImage(new Uri(MeCore.DefaultBG)); } }
        public override string ImageSource { get { return MeCore.DefaultBG; } }
        public override MahApps.Metro.Accent Accent
        { get { return MahApps.Metro.ThemeManager.GetAccent("Green"); } }
        public override string AccentName
        { get { return "Green"; } }
        public override string Name
        { get { return "Default"; } }
    }
    internal class ThemeHelper {
        internal static Theme NormalizeTheme (DefaultTheme theme) {
            Theme t = new Theme();
            return t.MakeChanges("Image", theme.Image).MakeChanges("ImageSource", theme.ImageSource).MakeChanges("Name", theme.Name).MakeChanges("Accent", theme.Accent).MakeChanges("AccentName", theme.AccentName);
        }
    }
    [DataContract]
    internal class ThemeInfo
    {
        [DataMember]
        [Newtonsoft.Json.JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [DataMember]
        [Newtonsoft.Json.JsonProperty(PropertyName = "accent-name")]
        public string AccentName { get; set; }
        [DataMember]
        [Newtonsoft.Json.JsonProperty(PropertyName = "accent-color")]
        public string AccentColor { get; set; }
        [DataMember]
        [Newtonsoft.Json.JsonProperty(PropertyName = "highlight-color")]
        public string HighlightColor { get; set; }
        [DataMember]
        [Newtonsoft.Json.JsonProperty(PropertyName = "background")]
        public string Background { get; set; }
    }
}
