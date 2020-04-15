using Jint;
using System;
using System.Collections.Generic;
using System.Text;


namespace JintAddons
{
    public class JintAddons
    {
        public static void Inject(Engine engine, bool debug = false, bool safeMode = false)
        {
            new Plugins.Injector().Inject(engine);
            new Extensions.Injector().Inject(engine);
        }
        public static bool debug = false;
        public static bool safeMode = false;
    }
}
