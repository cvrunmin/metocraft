using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace MetoCraft.Profile
{
    public class ProfileInXML
    {
        private XmlDocument doc;
        public bool newFile(string dir) {
            try
            {
                if (File.Exists(dir))
                {
                    return false;
                }
                else
                {
                    File.Create(dir);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MeCore.Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                {
                    new ErrorReport(ex).Show();
                }));
                return false;
            }
        }
        public bool writeProfile(string path, Profile pro) {
            doc = new XmlDocument();
            try
            {
                //If there is no current file, then create a new one
                if (!File.Exists(path))
                {
                    newFile(path);
                    //Create neccessary nodes
                    XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                    XmlComment comment = doc.CreateComment("This is an XML Generated File");
                    XmlElement root = doc.CreateElement("Profiles");
                    XmlElement profile = doc.CreateElement("Profile");
                    XmlAttribute name = doc.CreateAttribute("name");
                    XmlElement version = doc.CreateElement("Version");
                    XmlElement winSizeX = doc.CreateElement("WindowSizeX");
                    XmlElement winSizeY = doc.CreateElement("WindowSizeY");
                    XmlElement xmx = doc.CreateElement("Xmx");

                    //Add the values for each nodes
                    name.Value = pro.name;
                    version.InnerText = pro.version;
                    winSizeX.InnerText = pro.winSizeX.ToString();
                    winSizeY.InnerText = pro.winSizeY.ToString();
                    xmx.InnerText = pro.Xmx.ToString();

                    //Construct the document
                    doc.AppendChild(declaration);
                    doc.AppendChild(comment);
                    doc.AppendChild(root);
                    root.AppendChild(profile);
                    profile.Attributes.Append(name);
                    profile.AppendChild(version);
                    profile.AppendChild(winSizeX);
                    profile.AppendChild(winSizeY);
                    profile.AppendChild(xmx);

                    doc.Save(path);
                    return true;
                }
                else //If there is already a file
                {
                    //Load the XML File
                    doc.Load(path);

                    //Get the root element
                    XmlElement root = doc.DocumentElement;

                    XmlElement profile = doc.CreateElement("Profile");
                    XmlAttribute name = doc.CreateAttribute("name");
                    XmlElement version = doc.CreateElement("Version");
                    XmlElement winSizeX = doc.CreateElement("WindowSizeX");
                    XmlElement winSizeY = doc.CreateElement("WindowSizeY");
                    XmlElement xmx = doc.CreateElement("Xmx");

                    //Add the values for each nodes
                    name.Value = pro.name;
                    version.InnerText = pro.version;
                    winSizeX.InnerText = pro.winSizeX.ToString();
                    winSizeY.InnerText = pro.winSizeY.ToString();
                    xmx.InnerText = pro.Xmx.ToString();

                    //Construct the Person element
                    profile.Attributes.Append(name);
                    profile.AppendChild(version);
                    profile.AppendChild(winSizeX);
                    profile.AppendChild(winSizeY);
                    profile.AppendChild(xmx);

                    //Add the New person element to the end of the root element
                    root.AppendChild(profile);

                    //Save the document
                    doc.Save(path);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MeCore.Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                {
                    new ErrorReport(ex).Show();
                }));
                return false;
            }
        }
        public bool rewriteProfile(string path, Profile pro)
        {
            doc = new XmlDocument();
            try
            {
                //If there is no current file, then create a new one
                if (!File.Exists(path))
                {
                    return writeProfile(path, pro);
                }
                else //If there is already a file
                {
                    //Load the XML File
                    doc.Load(path);

                    //Get the root element
                    XmlElement root = doc.DocumentElement;

                    foreach (XmlElement item in root.GetElementsByTagName("Profile"))
                    {
                        if (item.Attributes["name"].Value == pro.oldName) {
                            item.Attributes["name"].Value = pro.name;
                            item.GetElementsByTagName("Version")[0].InnerText = pro.version;
                            item.GetElementsByTagName("WindowSizeX")[0].InnerText = pro.winSizeX.ToString();
                            item.GetElementsByTagName("WindowSizeY")[0].InnerText = pro.winSizeY.ToString();
                            item.GetElementsByTagName("Xmx")[0].InnerText = pro.Xmx.ToString();
                        }
                    }

                    //Save the document
                    doc.Save(path);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MeCore.Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                {
                    new ErrorReport(ex).Show();
                }));
                return false;
            }
        }
        public bool deleteProfile(string path, Profile pro)
        {
            doc = new XmlDocument();
            try
            {
                //If there is no current file, then create a new one
                if (!File.Exists(path))
                {
                    return writeProfile(path, pro);
                }
                else //If there is already a file
                {
                    //Load the XML File
                    doc.Load(path);

                    //Get the root element
                    XmlElement root = doc.DocumentElement;
                    for (int i = 0; i < root.GetElementsByTagName("Profile").Count; i++)
                    {
                        XmlElement item = (XmlElement)root.ChildNodes[i];
                        if (item.Attributes["name"].Value == pro.oldName)
                        {
                            root.RemoveChild(item);
                        }
                    }
/*                    foreach (XmlElement item in root.GetElementsByTagName("Profile"))
                    {
                        if (item.Attributes["name"].Value == pro.oldName)
                        {
                            root.RemoveChild(item);
                        }
                    }*/

                    //Save the document
                    doc.Save(path);
                    return true;
                }
            }
            catch (Exception ex)
            {
                MeCore.Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                {
                    new ErrorReport(ex).Show();
                }));
                return false;
            }
        }
        public bool writeFileNoProfile(string path)
        {
            doc = new XmlDocument();
            try
            {
                //If there is no current file, then create a new one
                if (!File.Exists(path))
                {
//                    newFile(path);
                    //Create neccessary nodes
                    XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                    XmlComment comment = doc.CreateComment("This is an XML Generated File");
                    XmlElement root = doc.CreateElement("Profiles");

                    //Construct the document
                    doc.AppendChild(declaration);
                    doc.AppendChild(comment);
                    doc.AppendChild(root);

                    doc.Save(path);
                    return true;
                }
                else //If there is already a file
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MeCore.Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                {
                    new ErrorReport(ex).Show();
                }));
                return false;
            }
        }
        public Profile[] readProfile(string path) {
            List<Profile> prolist = new List<Profile>();
            doc = new XmlDocument();
            try
            {
                if (!File.Exists(path))
                {
                    writeFileNoProfile(path);
                }
                FileStream openfileStrm = new FileStream(path, FileMode.Open);
                doc.Load(openfileStrm);
                openfileStrm.Close();
                XmlElement root = doc.DocumentElement;
                for (int i = 0; i < root.GetElementsByTagName("Profile").Count; i++)
                {
                    XmlElement item = (XmlElement)root.ChildNodes[i];
                    Profile temp = new Profile
                    {
                        oldName = item.Attributes["name"].Value,
                        name = item.Attributes["name"].Value,
                        version = item.GetElementsByTagName("Version")[0].InnerText,
                        winSizeX = int.Parse(item.GetElementsByTagName("WindowSizeX")[0].InnerText),
                        winSizeY = int.Parse(item.GetElementsByTagName("WindowSizeY")[0].InnerText),
                        Xmx = int.Parse(item.GetElementsByTagName("Xmx")[0].InnerText)
                    };
                    prolist.Add(temp);
                }
/*                foreach (XmlElement item in root.GetElementsByTagName("Profile"))
                {
                    Profile temp = new Profile
                    {
                        oldName = item.Attributes["name"].Value,
                        name = item.Attributes["name"].Value,
                        version = item.GetElementsByTagName("Version")[0].Value,
                        winSizeX = int.Parse(item.GetElementsByTagName("WindowSizeX")[0].Value),
                        winSizeY = int.Parse(item.GetElementsByTagName("WindowSizeY")[0].Value),
                        Xmx = int.Parse(item.GetElementsByTagName("Xmx")[0].Value)
                    };
                    prolist.Add(temp);
                }*/
                return prolist.ToArray();
            }
            catch (Exception ex)
            {
                MeCore.Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(delegate
                {
                    new ErrorReport(ex).Show();
                }));
                return null;
            }
        }
    }
}
