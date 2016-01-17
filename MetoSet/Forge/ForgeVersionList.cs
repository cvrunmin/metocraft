using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Collections;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;

using MetoCraft.JsonClass;
using System.Web.Script.Serialization;

namespace MetoCraft.Forge
{
    class ForgeVersionList
    {
        public delegate void ForgePageReadyHandle();
        public event ForgePageReadyHandle ForgePageReadyEvent;
        private readonly DataContractJsonSerializer _forgeVerJsonParse = new DataContractJsonSerializer(typeof(ForgeVersion));
        private ForgeVersion _forge;
        public Dictionary<string, string> ForgeDownloadUrl = new Dictionary<string, string>(),
            ForgeChangeLogUrl = new Dictionary<string, string>();
        public void GetVersion()
        {
            var webClient = new WebClient();
            webClient.DownloadStringAsync(new Uri(MeCore.UrlForgeList));
            webClient.DownloadStringCompleted += WebClient_DownloadStringCompleted;
        }

        private void WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Logger.log("获取forge列表失败");
                Logger.error(e.Error);
            }
            else
            {
                _forge =
                    _forgeVerJsonParse.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(e.Result))) as ForgeVersion;
                Logger.log("获取Forge列表成功");
            }
            ForgePageReadyEvent?.Invoke();
        }

        public TreeViewItem[] GetNew()
        {
            ArrayList arrayList = new ArrayList(_forge.number.Count);
            TreeViewItem treeViewItem = new TreeViewItem();
            arrayList.Add(treeViewItem);
            ForgeVersion forge = _forge;
                foreach (var item in forge.number.Values)
                {
                    bool install = false;
                    for (int i = 0; i < item.files.GetLength(1); i++)
                    {
                        if (item.files[i][2].Equals("changelog"))
                        {
                            ForgeChangeLogUrl[item.version] = forge.artifact + "-" + item.mcversion + "-" + item.version + "-" + item.files[i][1] + "." + item.files[i][0];
                        }
                        if (item.files[i][2].Equals("installer"))
                        {
                            install = true;
                            ForgeDownloadUrl[item.version] = forge.artifact +"-" + item.mcversion + "-" + item.version + "-" + item.files[i][1] + "." + item.files[i][0];
                        }
                    }
                    if (!install)
                    {
                        Logger.log("MinecraftForge" + item.version, " for ", item.mcversion, "does not have installer");
                    }
                    if (treeViewItem.Header == null)
                    {
                        treeViewItem.Header = item.mcversion;
                    }
                    else
                    {
                        if (treeViewItem.Header.ToString() != item.mcversion)
                        {
                            treeViewItem = new TreeViewItem();
                            arrayList.Add(treeViewItem);
                            treeViewItem.Header = item.mcversion;
                        }
                    }
                    Logger.log(treeViewItem.Header.ToString());
                    Logger.log(item.mcversion);

                    treeViewItem.Items.Add(item.version);
                    Logger.log("获取Forge", item.version);
                }

            return arrayList.ToArray(typeof(TreeViewItem)) as TreeViewItem[];
        }
        public TreeViewItem[] Get()
        {
            ArrayList arrayList = new ArrayList(_forge.number.Count);
            TreeViewItem treeViewItem = new TreeViewItem();
            arrayList.Add(treeViewItem);
            ForgeVersion forge = _forge;
            foreach (var item in forge.number.Values)
            {
                bool install = false;
                for (int i = 0; i < item.files.GetLength(1); i++)
                {
                    if (item.files[i][2].Equals("changelog"))
                    {
                        ForgeChangeLogUrl[item.version] = forge.artifact + "-" + item.mcversion + "-" + item.version + "-" + item.files[i][1] + "." + item.files[i][0];
                    }
                    if (item.files[i][2].Equals("installer"))
                    {
                        install = true;
                        ForgeDownloadUrl[item.version] = forge.artifact + "-" + item.mcversion + "-" + item.version + "-" + item.files[i][1] + "." + item.files[i][0];
                    }
                }
                if (!install)
                {
                    Logger.log("MinecraftForge" + item.version, " for ", item.mcversion, "does not have installer");
                }
                if (treeViewItem.Header == null)
                {
                    treeViewItem.Header = item.mcversion;
                }
                else
                {
                    if (treeViewItem.Header.ToString() != item.mcversion)
                    {
                        treeViewItem = new TreeViewItem();
                        arrayList.Add(treeViewItem);
                        treeViewItem.Header = item.mcversion;
                    }
                }
                Logger.log(treeViewItem.Header.ToString());
                Logger.log(item.mcversion);

                treeViewItem.Items.Add(item.version);
                Logger.log("获取Forge", item.version);
            }

            return arrayList.ToArray(typeof(TreeViewItem)) as TreeViewItem[];
        }

    }
}

