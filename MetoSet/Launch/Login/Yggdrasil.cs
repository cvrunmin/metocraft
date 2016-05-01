using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using MTMCL.util;
using Newtonsoft.Json;

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

        public string Type
        {
            get
            {
                return "Yggdrasil";
            }
        }

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
                auth.ContentType = "application/json";
                Request ag = new Request(Email, Password);
                string logindata = JsonConvert.SerializeObject(ag, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                byte[] postdata = Encoding.UTF8.GetBytes(logindata);
                auth.ContentLength = postdata.LongLength;
                Stream poststream = auth.GetRequestStream();
                poststream.Write(postdata, 0, postdata.Length);
                poststream.Close();
                HttpWebResponse authans = (HttpWebResponse)auth.GetResponse();
                StreamReader ResponseStream = new StreamReader(authans.GetResponseStream());
                Response response = JsonConvert.DeserializeObject<Response>(ResponseStream.ReadToEnd());
                /*if (Response.getClientToken() != NewLogin.ClientToken)
                {
                    LI.Suc = false;
                    LI.Errinfo = "客户端标识和服务器返回不符，这是个不常见的错误，就算是正版启动器这里也没做任何处理，只是报了这么个错。";
                    return LI;
                }*/
                AI.Pass = true;
                if (response.selectedProfile != null) {
                    AI.DisplayName = response.selectedProfile.name;
                    AI.UUID = Guid.Parse(response.selectedProfile.id);
                }
                AI.Session = Guid.Parse(response.accessToken);
                if (response.user != null)
                {
                    AI.UserType = response.user.legacy ? "Legacy" : "Mojang";
                    AI.Prop = response.user.properties != null ? JsonConvert.SerializeObject(response.user.properties, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }) : "{}";
                }
                else
                {
                    AI.UserType = "Mojang";AI.Prop = "{}";
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
                AI.ErrorMsg = ex.ToWellKnownExceptionString();
                return AI;
            }

        }
    }

    public class YggdrasilRefreshAuth : IAuth
    {
        public string AccessToken { get; private set; }
        public YggdrasilHelper helper { get; private set; }

        public string Type
        {
            get
            {
                return "Yggdrasil";
            }
        }

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
                auth.ContentType = "application/json";
                RefreshRequest ag = new RefreshRequest(AccessToken);
                string logindata = JsonConvert.SerializeObject(ag, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                byte[] postdata = Encoding.UTF8.GetBytes(logindata);
                auth.ContentLength = postdata.LongLength;
                Stream poststream = auth.GetRequestStream();
                poststream.Write(postdata, 0, postdata.Length);
                poststream.Close();
                HttpWebResponse authans = (HttpWebResponse)auth.GetResponse();
                StreamReader ResponseStream = new StreamReader(authans.GetResponseStream());
                Response response = JsonConvert.DeserializeObject<Response>(ResponseStream.ReadToEnd());
                /*if (Response.getClientToken() != NewLogin.ClientToken)
                {
                    LI.Suc = false;
                    LI.Errinfo = "客户端标识和服务器返回不符，这是个不常见的错误，就算是正版启动器这里也没做任何处理，只是报了这么个错。";
                    return LI;
                }*/
                if (response.selectedProfile != null) {
                    AI.DisplayName = response.selectedProfile.name;
                    AI.UUID = Guid.Parse(response.selectedProfile.id);
                }
                AI.Pass = true;
                AI.Session = Guid.Parse(response.accessToken);
                if (response.user != null)
                {
                    AI.UserType = response.user.legacy ? "Legacy" : "Mojang";
                    AI.Prop = response.user.properties != null ? JsonConvert.SerializeObject(response.user.properties, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }) : "{}";
                }
                else
                {
                    AI.UserType = "Mojang"; AI.Prop = "{}";
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
        public string name { get; set; }
        [DataMember]
        public int version { get; set; }
        public Agent() { }
        public Agent(string v1, int v2)
        {
            name = v1;
            version = v2;
        }
    }
    public class Request {
        public Agent agent { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string clientToken { get; set; }
        public bool requestUser { get; set; }
        public Request(string un, string pw) {
            agent = Agent.MINECRAFT;
            username = un;
            password = pw;
            clientToken = MeCore.Config.GUID;
            requestUser = true;
        }
    }
    public class RefreshRequest
    {
        public Agent agent { get; set; }
        public string accessToken { get; set; }
        public string clientToken { get; set; }
        public bool requestUser { get; set; }
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
        public string id { get; set; }
        public string name { get; set; }
        public GameProfile() { }
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
