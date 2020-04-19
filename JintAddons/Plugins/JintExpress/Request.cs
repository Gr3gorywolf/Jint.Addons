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
        public object @params {get;set;}
        public object query { get; set; }
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
                if(request.ContentType == "multipart/form-data" || request.ContentType == "application/x-www-form-urlencoded")
                {
                    data = ServerHelpers.ParseFormData(input);
                }
                else
                {
                    data = input;
                }
                
            }

            Dictionary<string, object> querys = new Dictionary<string, object>();
            foreach(var qu in request.QueryString.AllKeys)
            {
                querys.Add(qu, request.QueryString.Get(qu));
            }
            this.query = Utils.ObjectUtils.DictionaryToObject(querys);
        }
    }
}
