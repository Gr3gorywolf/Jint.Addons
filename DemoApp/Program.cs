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

            Console.Clear();
            const string scriptsFolder = "";
            Thread engineThread = null ;
             
            Console.WriteLine("Welcome to script loader. Select a script");
            var scripts = Directory.GetFiles("../../../scripts").ToList();
            foreach(var scr in scripts)
            {
                 var index = scripts.IndexOf(scr) + 1;
                 Console.WriteLine(index + "-"+ Path.GetFileNameWithoutExtension(scr));
            }

            var selectedScript = "";
            try
            {
                var selectedIndex = int.Parse(Console.ReadLine())-1;
                selectedScript = Path.GetFileNameWithoutExtension(scripts[selectedIndex]);
            }
            catch(Exception)
            {
                Console.Clear();
                Console.WriteLine("Invalid script.  Press any key to continue");
                Console.ReadKey();
                Main(new string[] { });
                return;
            }
            Console.Clear();
            Console.WriteLine("Loading " + selectedScript + " press F5 to return to menu anytime");
            engineThread = new Thread(new ThreadStart(() =>
            {
                var engine = new Engine(cfg => {

                    

                });
                JintAddons.JintAddons.Inject(engine, true);
                engine.Execute(ReadScript(selectedScript));
            }));
            engineThread.Start();
            
            while (true)
            {
              var pressedKey =  Console.ReadKey();
                if (pressedKey.Key == ConsoleKey.F5)
                {

                    try
                    {
                        engineThread.Abort();
                    }
                    catch (Exception ex)
                    {

                    }
                    Main(new string[] { });
                    break;
                }
            }

            
        }

       


        public static string ReadScript(string scriptName)
        {
           return File.ReadAllText($"../../../scripts/{scriptName}.js");
        }
   
    }
}
