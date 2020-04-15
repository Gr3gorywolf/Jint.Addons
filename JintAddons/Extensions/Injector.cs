using Jint;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace JintAddons.Extensions
{
    public class Injector
    {
        public void Inject(Engine engine)
        {
            var interval = new Interval();
            engine.SetValue("console", new Console());

            engine.SetValue("parseInt", new Func<string, int>((string val) =>
            {
                return Int32.Parse(val);
            }));
            engine.SetValue("parseFloat", new Func<string, float>((string val) =>
            {
                return float.Parse(val);
            }));
            engine.SetValue("parseDouble", new Func<string, double>((string val) =>
            {
                return double.Parse(val);
            }));

            engine.SetValue("setInterval", new Func<Func<object>, int, Timer>(interval.setInterval));
            engine.SetValue("cleanInterval", new Func<Timer, bool>(interval.cleanInterval));
        }
    }
}
