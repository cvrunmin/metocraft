using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTMCL.Launch.Login
{
    public interface IAuth
    {
        string Type { get; }
        AuthInfo Login();
    }
}
