using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace JintAddons.Plugins.JintExpress
{
   public class Request
    {

        HttpListenerContext context { get; set; }
        public object data { get; set; }
        public Request(HttpListenerContext ctx)
        {
            context = ctx;
            var request = ctx.Request;
            StreamReader stream = new StreamReader(request.InputStream);
            var input = stream.ReadToEnd();
            try
            {
                data = JsonConvert.DeserializeObject(input);
            }
            catch (Exception)
            {
                data = input;
            }



        }
    }
}
