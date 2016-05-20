using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTMCL.Launch.Login
{
    public class OfflineAuth : IAuth
    {
        public string UN { get; private set; }
        public string Type
        {
            get
            {
                return "Offline";
            }
        }

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
            return new AuthInfo() { Pass = true, DisplayName = UN, UUID = Guid.NewGuid(), UserType = "Mojang", Prop = "{}", Session = Guid.NewGuid()};
        }

        public Task<AuthInfo> LoginAsync(CancellationToken token)
        {
            return System.Threading.Tasks.Task.Factory.StartNew((Func<AuthInfo>)Login, token);
        }
    }

    public class AuthWarpper : IAuth
    {
        public AuthInfo info { get; private set; }
        public string Type
        {
            get
            {
                return "Warpper";
            }
        }
        public AuthWarpper(AuthInfo info)
        {
            this.info = info;
        }
        public AuthInfo Login()
        {
            return info;
        }

        public Task<AuthInfo> LoginAsync(CancellationToken token)
        {
            return System.Threading.Tasks.Task.Factory.StartNew((Func<AuthInfo>)Login, token);
        }
    }
}
