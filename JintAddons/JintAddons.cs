using Jint;
using System;
using System.Collections.Generic;
using System.Text;


namespace JintAddons
{
    public class JintAddons
    {
        public static void Inject(Engine engine)
        {
            new Plugins.Injector().Inject(engine);
            new Extensions.Injector().Inject(engine);
        }
    }
}
