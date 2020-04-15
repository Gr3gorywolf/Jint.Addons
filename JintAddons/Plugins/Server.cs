using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JintAddons.Plugins
{
    public class Server
    {

        private Dictionary<string, Action<Request, Response>> GETS = new Dictionary<string, Action<Request, Response>>();
        private Dictionary<string, Action<Request, Response>> POSTS = new Dictionary<string, Action<Request, Response>>();
        private Dictionary<string, Action<Request, Response>> PUTS = new Dictionary<string, Action<Request, Response>>();
        private Dictionary<string, Action<Request, Response>> PATCHS = new Dictionary<string, Action<Request, Response>>();
        private Dictionary<string, Action<Request, Response>> DELETES = new Dictionary<string, Action<Request, Response>>();
        public string PublicFolder = "";
        public  HttpListener listener;
        public bool enabled = false;
        Thread handleThread;

        public void start(int port)
        {
            string url = $"http://localhost:{port}/";
            this.enabled = true;
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            handleThread = new Thread(new ThreadStart(HandleRequest));
            handleThread.Start();
        }

        public void stop()
        {
            listener.Stop();
            this.enabled = false;
            handleThread.Abort();
        }


        public async void HandleRequest()
        {
            while (enabled)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();
                var response = new Response(ctx);
                var request = new Request(ctx);
                Dictionary<string, Action<Request, Response>> routesDictionary = null;
                
                switch (ctx.Request.HttpMethod)
                {
                    case "GET":
                        routesDictionary = GETS;
                        break;
                    case "POST":
                        routesDictionary = POSTS;
                        break;
                    case "PUT":
                        routesDictionary = PUTS;
                        break;
                    case "PATCH":
                        routesDictionary = PATCHS;
                        break;
                    case "DELETE":
                        routesDictionary = DELETES;
                        break;
                }
                if (!IsFromPublicFolder(ctx))
                {
                    bool found = false;
                    foreach (var route in routesDictionary.Keys)
                    {
                        if (ctx.Request.RawUrl == route || route.EndsWith("*"))
                        {
                            found = true;
                            routesDictionary[route].Invoke(request, response);
                        }
                    }
                    if (!found)
                    {
                        handle404Async(ctx);
                    }
                }
                else
                {
                    handlePublicResponse(ctx);
                }
             
            }


        }

        private bool IsFromPublicFolder(HttpListenerContext context)
        {
            string filename = PublicFolder + WebUtility.UrlDecode(context.Request.RawUrl);
            return File.Exists(filename);
        }

        private void handlePublicResponse(HttpListenerContext ctx)
        {
            string filename = PublicFolder + ctx.Request.RawUrl;
            filename = WebUtility.UrlDecode(filename);
            string mime;
            ctx.Response.StatusCode = (int)HttpStatusCode.OK;

            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ctx.Response.ContentType = Utils.Server.mimeTypesMap.TryGetValue(Path.GetExtension(filename), out mime) ? mime : "undefined";
                ctx.Response.ContentLength64 = stream.Length;
                ctx.Response.AddHeader("Accept-Ranges", "bytes");
                ctx.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                ctx.Response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(filename).ToString("r"));
                ctx.Response.AddHeader("Content-Range", string.Format("bytes {0}-{1}/{2}", 0, Convert.ToInt32(stream.Length) - 1, Convert.ToInt32(stream.Length)));
                ctx.Response.ContentLength64 = stream.Length;
                stream.CopyTo(ctx.Response.OutputStream);
                stream.Flush();
            }
            ctx.Response.OutputStream.Flush();
            ctx.Response.OutputStream.Close();
            ctx.Response.Close();
        }


        private async Task handle404Async(HttpListenerContext ctx)
        {
            new Response(ctx).send(NoFoundTemplate(ctx.Request.RawUrl), 404, "text/html");
        }


        public class Response
        {
            HttpListenerContext context { get; set; }
            public Response (HttpListenerContext ctx)
            {
                context = ctx;
            }
            public async void send(string data, int statusCode, string contentType)
            {
                var response = context.Response;
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                response.ContentType = contentType;
                response.ContentEncoding = Encoding.UTF8;
                response.ContentLength64 = bytes.LongLength;
                response.StatusCode = statusCode;
                await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                response.Close();
            }

            public async void redirect(string route)
            {
                context.Response.Redirect(route);
            }
        }

        public class Request
        {
            HttpListenerContext context { get; set; }
            public  string data { get; set; }
           public Request(HttpListenerContext ctx)
            {
                var request = ctx.Request;
                StreamReader stream = new StreamReader(request.InputStream);
                this.data =  stream.ReadToEnd();
            }
            
        }



        public Server staticFiles(string path)
        {
            this.PublicFolder = path;
            return this;
        }


        public Server get(string route , Action<Request, Response> callback)
        {

            GETS.Add(route, callback);
            return this;
        }

        public Server post(string route, Action<Request, Response> callback)
        {

            POSTS.Add(route, callback);
            return this;
        }
        public Server put(string route, Action<Request, Response> callback)
        {

            PUTS.Add(route, callback);
            return this;
        }
        public Server patch(string route, Action<Request, Response> callback)
        {

            PATCHS.Add(route, callback);
            return this;
        }
        public Server delete(string route, Action<Request, Response> callback)
        {

            DELETES.Add(route, callback);
            return this;
        }




        public string NoFoundTemplate(string route )
        {
           var _res = $@"
               <!DOCTYPE html>
               <html lang='en'>
               <head>
               <meta charset='UTF-8'>
               <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>No found</title>
                </head>
                 <body>
                  <h1>{route} No found</h1>

                </body>
             </html>
            ";
            return _res;
        }


    }
}
