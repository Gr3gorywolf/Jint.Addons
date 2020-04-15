using Jint;
using System;
using System.Collections.Generic;
using System.Text;
namespace JintAddons.Plugins
{
    public class Injector
    {
        public void Inject(Engine engine)
        {
            engine.SetValue("File", typeof(FileSystem));
            engine.SetValue("fetch", typeof(Fetch));
            engine.SetValue("Server", typeof(Server));
        }
    }
}
