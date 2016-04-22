using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace MTMCL.Launch.Login
{
    public class YggdrasilHelper {
        public string BaseUrl = "https://authserver.mojang.com/";
        public string Authenticate => BaseUrl + "authenticate";
        public string Refresh => BaseUrl + "refresh";
        public string Validate => BaseUrl + "validate";
        public string Invalidate => BaseUrl + "invalidate";
        public string Signout => BaseUrl + "signout";
        public static readonly YggdrasilHelper instance = new YggdrasilHelper();
        public YggdrasilHelper() { }
        public YggdrasilHelper(string uri) {
            BaseUrl = uri;
        }
    }
    public class YggdrasilLoginAuth : IAuth
    {
        public string Email { get; private set; }
        public string Password { get; private set; }
        public YggdrasilHelper helper { get; private set; }
        public YggdrasilLoginAuth(string em, string pw) : this(em,pw,YggdrasilHelper.instance) {        }
        public YggdrasilLoginAuth(string em, string pw, YggdrasilHelper helper)
        {
            Email = em;
            Password = pw;
            this.helper = helper;
        }
        public AuthInfo Login()
        {
            AuthInfo AI = new AuthInfo();
            try
            {
                HttpWebRequest auth = (HttpWebRequest)WebRequest.Create(helper.Authenticate);
                auth.Method = "POST";
                Request ag = new Request(Email, Password);
                MemoryStream agJsonStream = new MemoryStream();
                LitJson.JsonMapper.ToJson(ag, new LitJson.JsonWriter(new StreamWriter(agJsonStream)));
                //agJsonSerialiaer.WriteObject(agJsonStream, ag);
                agJsonStream.Position = 0;
                string logindata = (new StreamReader(agJsonStream)).ReadToEnd();
                byte[] postdata = Encoding.UTF8.GetBytes(logindata);
                auth.ContentLength = postdata.LongLength;
                Stream poststream = auth.GetRequestStream();
                poststream.Write(postdata, 0, postdata.Length);
                poststream.Close();
                HttpWebResponse authans = (HttpWebResponse)auth.GetResponse();
                StreamReader ResponseStream = new StreamReader(authans.GetResponseStream());
                string ResponseJson = ResponseStream.ReadToEnd();
                MemoryStream ResponseJsonStream = new MemoryStream(Encoding.UTF8.GetBytes(ResponseJson));
                ResponseJsonStream.Position = 0;
                Response response = LitJson.JsonMapper.ToObject<Response>(new LitJson.JsonReader(new StreamReader(ResponseJsonStream)));
                /*if (Response.getClientToken() != NewLogin.ClientToken)
                {
                    LI.Suc = false;
                    LI.Errinfo = "客户端标识和服务器返回不符，这是个不常见的错误，就算是正版启动器这里也没做任何处理，只是报了这么个错。";
                    return LI;
                }*/
                AI.Pass = true;
                AI.DisplayName = response.selectedProfile.name;
                AI.Session = Guid.Parse(response.accessToken);
                AI.UUID = Guid.Parse(response.selectedProfile.id);
                if (response.user != null)
                {
                    AI.UserType = response.user.legacy ? "legacy" : "mojang";
                    AI.Prop = response.user.properties != null ? LitJson.JsonMapper.ToJson(response.user.properties) : "{}";
                }
                else
                {
                    AI.UserType = "mojang";AI.Prop = "{}";
                }
                //AI.c = NewLogin.ClientToken;
                /*DataContractSerializer OtherInfoSerializer = new DataContractSerializer(typeof(SortedList));
                SortedList OtherInfoList = new SortedList();
                OtherInfoList.Add("${auth_uuid}", response.selectedProfile.id);
                OtherInfoList.Add("${auth_access_token}", response.accessToken);
                MemoryStream OtherInfoStream = new MemoryStream();
                OtherInfoSerializer.WriteObject(OtherInfoStream, OtherInfoList);
                OtherInfoStream.Position = 0;*/
                return AI;
            }
            catch (WebException ex)
            {
                AI.Pass = false;
                AI.ErrorMsg = ex.Message + "[" + ex.Source + ",,," + ex.Response + "]";
                return AI;
            }
            catch (Exception ex)
            {
                AI.Pass = false;
                AI.ErrorMsg = ex.Message;
                return AI;
            }

        }
    }

    public class YggdrasilRefreshAuth : IAuth
    {
        public string AccessToken { get; private set; }
        public YggdrasilHelper helper { get; private set; }
        public YggdrasilRefreshAuth(string at) : this(at,YggdrasilHelper.instance) { }
        public YggdrasilRefreshAuth(string at, YggdrasilHelper helper)
        {
            AccessToken = at;
            this.helper = helper;
        }
        public AuthInfo Login()
        {
            AuthInfo AI = new AuthInfo();
            try
            {
                HttpWebRequest auth = (HttpWebRequest)WebRequest.Create(helper.Refresh);
                auth.Method = "POST";
                RefreshRequest ag = new RefreshRequest(AccessToken);
                MemoryStream agJsonStream = new MemoryStream();
                LitJson.JsonMapper.ToJson(ag, new LitJson.JsonWriter(new StreamWriter(agJsonStream)));
                //agJsonSerialiaer.WriteObject(agJsonStream, ag);
                agJsonStream.Position = 0;
                string logindata = (new StreamReader(agJsonStream)).ReadToEnd();
                byte[] postdata = Encoding.UTF8.GetBytes(logindata);
                auth.ContentLength = postdata.LongLength;
                Stream poststream = auth.GetRequestStream();
                poststream.Write(postdata, 0, postdata.Length);
                poststream.Close();
                HttpWebResponse authans = (HttpWebResponse)auth.GetResponse();
                StreamReader ResponseStream = new StreamReader(authans.GetResponseStream());
                string ResponseJson = ResponseStream.ReadToEnd();
                MemoryStream ResponseJsonStream = new MemoryStream(Encoding.UTF8.GetBytes(ResponseJson));
                ResponseJsonStream.Position = 0;
                Response response = LitJson.JsonMapper.ToObject<Response>(new LitJson.JsonReader(new StreamReader(ResponseJsonStream)));
                /*if (Response.getClientToken() != NewLogin.ClientToken)
                {
                    LI.Suc = false;
                    LI.Errinfo = "客户端标识和服务器返回不符，这是个不常见的错误，就算是正版启动器这里也没做任何处理，只是报了这么个错。";
                    return LI;
                }*/
                AI.Pass = true;
                AI.DisplayName = response.selectedProfile.name;
                AI.UUID = Guid.Parse(response.selectedProfile.id);
                if (response.user != null)
                {
                    AI.UserType = response.user.legacy ? "legacy" : "mojang";
                    AI.Prop = response.user.properties != null ? LitJson.JsonMapper.ToJson(response.user.properties) : "{}";
                }
                else
                {
                    AI.UserType = "mojang"; AI.Prop = "{}";
                }
                //AI.c = NewLogin.ClientToken;
                /*DataContractSerializer OtherInfoSerializer = new DataContractSerializer(typeof(SortedList));
                SortedList OtherInfoList = new SortedList();
                OtherInfoList.Add("${auth_uuid}", response.selectedProfile.id);
                OtherInfoList.Add("${auth_access_token}", response.accessToken);
                MemoryStream OtherInfoStream = new MemoryStream();
                OtherInfoSerializer.WriteObject(OtherInfoStream, OtherInfoList);
                OtherInfoStream.Position = 0;*/
                return AI;
            }
            catch (WebException ex) {
                AI.Pass = false;
                AI.ErrorMsg = ex.Message + "[" + ex.Source + ",,," + ex.Response + "]";
                return AI;
            }
            catch (Exception ex) {
                AI.Pass = false;
                AI.ErrorMsg = ex.Message;
                return AI;
            }
        }
    }
    [DataContract]
    public class Agent {
        [IgnoreDataMember]
        public static Agent MINECRAFT = new Agent("Minecraft", 1);
        [DataMember]
        private string name;
        [DataMember]
        private int version;

        public Agent(string v1, int v2)
        {
            name = v1;
            version = v2;
        }
    }
    public class Request {
        public Agent agent { get; private set; }
        public string username { get; private set; }
        public string password { get; private set; }
        public string clientToken { get; private set; }
        public bool requestUser { get; private set; }
        public Request(string un, string pw) {
            agent = Agent.MINECRAFT;username = un; password = pw; clientToken = Guid.NewGuid().ToString("N");requestUser = true;
        }
    }
    public class RefreshRequest
    {
        public Agent agent { get; private set; }
        public string accessToken { get; private set; }
        public string clientToken { get; private set; }
        public bool requestUser { get; private set; }
        public RefreshRequest(string at)
        {
            agent = Agent.MINECRAFT; accessToken = at; clientToken = Guid.NewGuid().ToString("N"); requestUser = true;
        }
    }
    public class Response {
        public class User
        {
            public string id { get; set; }
            public List<Property> properties { get; set; }
            public bool legacy { get; set; }
        }
        public string accessToken { get; set; }
        public string clientToken { get; set; }
        public GameProfile selectedProfile { get; set; }
        public GameProfile[] availableProfiles { get; set; }
        public User user { get; set; }
        public string error { get; set; }
        public string errorMessage { get; set; }
        public string cause { get; set; }
    }
    public class Property
    {
        public string name { get; set; }
        public string value { get; set; }
    }
    public class GameProfile
    {
        public string id { get; private set; }
        public string name { get; private set; }

        public GameProfile(string id, string name)
        {
            if (string.IsNullOrWhiteSpace(id) && string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name and ID cannot both be blank");
            }
            else
            {
                this.id = id;
                this.name = name;
                return;
            }
        }

        public bool isComplete()
        {
            return !string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(name);
        }

        public int hashCode()
        {
            int result = id.GetHashCode();
            result = 31 * result + name.GetHashCode();
            return result;
        }

        public string toString()
        {
            return (new StringBuilder()).Append("GameProfile{id='").Append(id).Append('\'').Append(", name='").Append(name).Append('\'').Append('}').ToString();
        }
    }
}
