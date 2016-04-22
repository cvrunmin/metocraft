using System;
using System.Collections.Generic;

namespace MTMCL.Launch.Login
{
    public class AuthInfo
    {
        public string DisplayName { get; set; }
        public Guid UUID { get; set; }
        public Guid Session { get; set; }
        public string UserType { get; set; }
        public bool Pass { get; set; }
        public string ErrorMsg { get; set; }
        public string Prop { get; set; }
        public Dictionary<string,string> AdvancedInfo { get; set; }
    }
}
