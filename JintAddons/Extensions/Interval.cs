using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace JintAddons.Extensions
{
    public class Interval
    {
        public Timer setInterval(Func<object> callback, int interval)
        {
            var timer = new Timer();
            timer.Elapsed += delegate
            {
                callback.Invoke();
            };
            timer.Interval = interval;
            timer.Start();
            return timer;
        }

        public bool cleanInterval(Timer timer)
        {
                timer.Stop();
                return true;
        }

    }
}
