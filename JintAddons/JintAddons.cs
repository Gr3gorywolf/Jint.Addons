using Jint;
using System;
using System.Collections.Generic;
using System.Text;


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
        public static bool debug = false;
        public static bool safeMode = false;
    }
}
