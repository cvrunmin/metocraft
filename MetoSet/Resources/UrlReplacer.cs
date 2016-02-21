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
                case 2:
                    return srcUrl.Replace("http://files.minecraftforge.net/maven", "http://mirrors.rapiddata.org/forge/maven");
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
                    return Url.URL_RESOURCE_BASE;
                case 1:
                    return Url.URL_RESOURCE_bangbang93;
                case 2:
                    return Url.URL_RESOURCE_rapiddata;
                default:
                    return Url.URL_RESOURCE_BASE;
            }
        }
        public static string getLibraryUrl(int? src = null)
        {
            switch (src != null ? src : MeCore.Config.DownloadSource)
            {
                case 0:
                    return Url.URL_LIBRARIES_BASE;
                case 1:
                    return Url.URL_LIBRARIES_bangbang93;
                case 2:
                    return Url.URL_LIBRARIES_rapiddata;
                default:
                    return Url.URL_LIBRARIES_BASE;
            }
        }
        public static string getDownloadUrl()
        {
            switch (MeCore.Config.DownloadSource)
            {
                case 0:
                    return Url.URL_DOWNLOAD_BASE;
                case 1:
                    return Url.URL_DOWNLOAD_bangbang93;
                case 2:
                    return Url.URL_DOWNLOAD_rapiddata;
                default:
                    return Url.URL_DOWNLOAD_BASE;
            }
        }
        public static string getVersionsUrl()
        {
            switch (MeCore.Config.DownloadSource)
            {
                case 0:
                    return "https://launchermeta.mojang.com/mc/game/version_manifest.json";
                case 1:
                    return Url.URL_DOWNLOAD_bangbang93 + "versions/versions.json";
                case 2:
                    return Url.URL_DOWNLOAD_rapiddata + "versions/versions.json";
                default:
                    return Url.URL_DOWNLOAD_BASE + "versions/versions.json";
            }
        }
    }
}
