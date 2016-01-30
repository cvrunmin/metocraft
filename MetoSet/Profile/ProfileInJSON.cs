using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace MetoCraft.Profile
{
    public class ProfileInJson
    {
        private FileInfo doc;
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
        public bool writeProfile(string path, Profiles pro) {
            doc = new FileInfo(path);
            try
            {
                //If there is no current file, then create a new one
                if (!File.Exists(path))
                {
                    StreamWriter stream = doc.CreateText();
                    LitJson.JsonWriter writer = new LitJson.JsonWriter(stream);
                    writer.Write(LitJson.JsonMapper.ToJson(pro));
                    stream.Close();
                    return true;
                }
                else //If there is already a file
                {
                    StreamWriter stream = new StreamWriter(doc.FullName);
                    LitJson.JsonWriter writer = new LitJson.JsonWriter(stream);
                    writer.Write(LitJson.JsonMapper.ToJson(pro));
                    stream.Close();
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
        public Profiles readProfile(string path) {
            doc = new FileInfo(path);
            try
            {
                if (!File.Exists(path))
                {
                    newFile(path);
                }
                FileStream openfileStrm = File.OpenRead(path);
                Profiles pro = LitJson.JsonMapper.ToObject<Profiles>(new LitJson.JsonReader(new StreamReader(openfileStrm)));
                openfileStrm.Close();
                return pro;
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
