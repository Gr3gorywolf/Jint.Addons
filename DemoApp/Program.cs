using System;
using System.Reflection;
using Jint;
using System.Linq;
using System.IO;
using System.Threading;
using JintAddons;
using System.Configuration;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace DemoApp
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.Clear();
            Console.WriteLine("Welcome to script loader.from where do you what to load the script?");
            Console.WriteLine("1-Local file");
            Console.WriteLine("2-Remote file");
            int option;
            try
            {
                option = int.Parse(Console.ReadLine());
                if(option > 2 || option < 1)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                Console.Clear();
                Console.WriteLine("Invalid script.  Press any key to continue");
                Console.ReadKey();
                Main(new string[] { });
                return;
            }

            switch (option)
            {
                case 1:
                    FileLoader();
                    break;
                case 2:
                    RemoteLoader();
                    break;
            }

            


           
            while (true)
            {
              var pressedKey =  Console.ReadKey();
                if (pressedKey.Key == ConsoleKey.F5)
                {
                    Main(new string[] { });
                    break;
                }
            }

            
        }


        public static void FileLoader()
        {
            Console.Clear();
            Console.WriteLine("Select a script from the list");
            var scripts = Directory.GetFiles("../../../scripts").ToList();
            foreach (var scr in scripts)
            {
                var index = scripts.IndexOf(scr) + 1;
                Console.WriteLine(index + "-" + Path.GetFileNameWithoutExtension(scr));
            }

            var selectedScript = "";
            try
            {
                var selectedIndex = int.Parse(Console.ReadLine()) - 1;
                selectedScript = Path.GetFileNameWithoutExtension(scripts[selectedIndex]);
            }
            catch (Exception)
            {
                Console.Clear();
                Console.WriteLine("Invalid script.  Press any key to continue");
                Console.ReadKey();
                FileLoader();
                return;
            }
            Console.Clear();
            Console.WriteLine("Loading " + selectedScript + " press F5 to return to menu anytime");
            var engine = new Engine(cfg => { });
            JintAddons.JintAddons.Inject(engine, true);
            JintAddons.JintAddons.RunJintScript(engine, ReadScript(selectedScript));
        }

        public static void RemoteLoader()
        {
            Console.Clear();
            Console.WriteLine("Enter your script url");
            var url = Console.ReadLine();
            var engine = new Engine(cfg => { });
            JintAddons.JintAddons.Inject(engine, true);
            JintAddons.JintAddons.SetCatchingStrategy((string key, string val) =>
            {
                if (!Directory.Exists("./cache"))
                {
                    Directory.CreateDirectory("./cache");
                }
               var file =  File.CreateText("./cache/" + key);
               file.Write(val);
               file.Close();
            },
            (string key) =>
            {
                if(File.Exists("./cache/"+ key))
                {
                    return File.ReadAllText("./cache/" + key);
                }
                else
                {
                    return null;
                }
               
            }

            );
           try
            {
                Console.WriteLine("Loading script...");
                JintAddons.JintAddons.LoadHostedScript(engine, url);
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine("Invalid url.  Press any key to continue");
                Console.ReadKey();
                RemoteLoader();
                return;
            }

        }
        

      

        public static string ReadScript(string scriptName)
        {
           return File.ReadAllText($"../../../scripts/{scriptName}.js");
        }
   
    }
}
