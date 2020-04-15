using Jint;
using JintAddons.Plugins;
using System;
using System.Collections.Generic;
using System.Text;

namespace JintAddons.Plugins
{
    public class Fetch
    {
        private Action<string> callback = null;
        private Action<string> exCallback = null;
      
       public Fetch(string url = null)
        {

            if(url != null)
            {
                using (var client = new System.Net.WebClient())
                {
                    client.DownloadStringAsync(new Uri(url));
                    client.DownloadStringCompleted += (obj, send) =>
                    {
                        if (send.Error != null)
                        {
                            exCallback.Invoke(send.Error.Message);
                        }
                        else
                        {
                            callback.Invoke(send.Result);
                        }
                    };
                }
            }
           
        }

        public static string get(string url)
        {
            using (var client = new System.Net.WebClient())
            {
                return client.DownloadString(new Uri(url));
            }
        }

        public Fetch then(Action<string> action)
        {
            this.callback = action;
            return this;
        }

        public Fetch catchEx(Action<string> action)
        {
            this.exCallback = action;
            return this;
        }

        public class Response
        {
            public string Content { get; set; }
            public bool HasErrors { get; set; }

            public string ToString(IFormatProvider provider)
            {
                return "";
            }

            public object ToType(Type conversionType, IFormatProvider provider)
            {
                return this.GetType();
            }

        }

        public string ToString(IFormatProvider provider)
        {
            return "";
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return this.GetType();
        }
    } 
}
