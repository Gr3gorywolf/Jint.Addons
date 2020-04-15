using System;
using System.Reflection;
using Jint;
using System.Linq;
using System.IO;
using System.Threading;
using JintAddons;
namespace DemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new Engine(cfg => cfg.AllowClr());
            JintAddons.JintAddons.Inject(engine);
            engine.Execute(File.ReadAllText("scripts/server.js"));
            
        }
    }
}
