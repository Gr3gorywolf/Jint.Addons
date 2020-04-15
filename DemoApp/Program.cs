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
            const string scriptsFolder = "";

            var engine = new Engine(cfg => cfg.AllowClr());

            //Addons injection 
            JintAddons.JintAddons.Inject(engine,true);
            engine.Execute(ReadScript("server"));
            
        }


        public static string ReadScript(string scriptName)
        {
           return File.ReadAllText($"../../../scripts/{scriptName}.js");
        }
    }
}
