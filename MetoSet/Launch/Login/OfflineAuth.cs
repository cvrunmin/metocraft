using System;
using System.Linq;

namespace MTMCL.Launch.Login
{
    public class OfflineAuth : IAuth
    {
        public string UN { get; private set; }
        public OfflineAuth(string UN) {
            this.UN = UN;
        }
        public AuthInfo Login()
        {
            if (string.IsNullOrWhiteSpace(UN))
            {
                return new AuthInfo() { Pass = false, ErrorMsg = "irregular username" };
            }
            else if (UN.Contains(' '))
            {
                return new AuthInfo() { Pass = false, ErrorMsg = "irregular username" };
            }
            return new AuthInfo() { Pass = true, DisplayName = UN, UUID = Guid.NewGuid(), UserType = "mojang", Prop = "{}", Session = Guid.NewGuid()};
        }
    }
}
