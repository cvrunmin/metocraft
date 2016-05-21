using System;

namespace MTMCL.Resources
{
    public class UrlReplacer
    {
        public static string getForgeMaven(string srcUrl, int? src = null) {
            if (string.IsNullOrWhiteSpace(srcUrl))
            {
                throw new InvalidOperationException("The source is blank: " + srcUrl);
            }
            if (!srcUrl.StartsWith("http://") && !srcUrl.StartsWith("https://"))
            {
                throw new InvalidOperationException("The source isn\'t a url: " + srcUrl);
            }
            switch (src != null ? src : MeCore.Config.DownloadSource)
            {
                case 0:
                    return srcUrl;
                case 1:
                    return srcUrl.Replace("http://files.minecraftforge.net/maven", "http://bmclapi2.bangbang93.com/maven");
                default:
                    return srcUrl;
            }
        }
        public static string toGoodLibUrl(string srcUrl, int? src = null) {
            return srcUrl.Replace("https://libraries.minecraft.net/", src != null ? getLibraryUrl(src) : getLibraryUrl());
        }
        public static string getResourceUrl() {
            switch (MeCore.Config.DownloadSource)
            {
                case 0:
                    return "http://resources.download.minecraft.net/";
                case 1:
                    return "http://bmclapi2.bangbang93.com/assets/";
                default:
                    return "http://resources.download.minecraft.net/";
            }
        }
        public static string getLibraryUrl(int? src = null)
        {
            switch (src != null ? src : MeCore.Config.DownloadSource)
            {
                case 0:
                    return "https://libraries.minecraft.net/";
                case 1:
                    return "http://bmclapi2.bangbang93.com/libraries/";
                default:
                    return "https://libraries.minecraft.net/";
            }
        }
        public static string getDownloadUrl()
        {
            switch (MeCore.Config.DownloadSource)
            {
                case 0:
                    return "https://s3.amazonaws.com/Minecraft.Download/";
                case 1:
                    return "http://bmclapi.bangbang93.com/";
                default:
                    return "https://s3.amazonaws.com/Minecraft.Download/";
            }
        }
        public static string getVersionsUrl()
        {
            switch (MeCore.Config.DownloadSource)
            {
                case 0:
                    return "https://launchermeta.mojang.com/mc/game/version_manifest.json";
                case 1:
                    return "http://bmclapi.bangbang93.com/mc/game/version_manifest.json";
                default:
                    return "https://s3.amazonaws.com/Minecraft.Download/versions/versions.json";
            }
        }
    }
}
