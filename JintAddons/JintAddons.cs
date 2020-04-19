using Jint;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace JintAddons
{
    public class JintAddons
    {
        public static void Inject(Engine engine, bool debugMode = false, bool forceSafeMode = false)
        {
            new Plugins.Injector().Inject(engine);
            new Extensions.Injector().Inject(engine);
            debug = debugMode;
            safeMode = forceSafeMode;
        }

          public static void ListenVariableChanges(Engine eng,string variableName, Action<dynamic> callback)
        {
           
                var timer = new Timer();
                string oldValue = null;
                timer.Elapsed += delegate
                {
                    var engineVal =JsonConvert.SerializeObject(eng.GetValue(variableName).ToObject());
                    if (oldValue != engineVal)
                    {
                        oldValue = engineVal;
                        callback.Invoke(JsonConvert.DeserializeObject(engineVal));
                    }
                  
                };
                timer.Interval = 350;
                timer.Start();
        }
        public static bool debug = false;
        public static bool safeMode = false;


    }

}
