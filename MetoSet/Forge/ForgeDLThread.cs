using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MTMCL.Forge
{
    class ForgeDLThread
    {
        public void Start()
        {
            var thread = new Thread(Run);
            thread.Start();
        }

        private void Run()
        {
        }
    }
}
