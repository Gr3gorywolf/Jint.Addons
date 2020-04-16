using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog.Fluent;
using Nustache.Core;
namespace JintAddons.Plugins
{
    public class Server
    {

        private Dictionary<string, Action<Request, Response>> GETS = new Dictionary<string, Action<Request, Response>>();
        private Dictionary<string, Action<Request, Response>> POSTS = new Dictionary<string, Action<Request, Response>>();
        private Dictionary<string, Action<Request, Response>> PUTS = new Dictionary<string, Action<Request, Response>>();
        private Dictionary<string, Action<Request, Response>> PATCHS = new Dictionary<string, Action<Request, Response>>();
        private Dictionary<string, Action<Request, Response>> DELETES = new Dictionary<string, Action<Request, Response>>();
        public List<string> PublicFolders = new List<string>();
        public HttpListener listener;
        public bool enabled = false;
        public bool UseClientSideRouting = false;
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

            this.enabled = false;
            listener.Close();
            try
            {
                handleThread.Abort();
            }
            catch (Exception)
            {

            }
        }


        private async void HandleRequest()
        {

          
            while (enabled)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();
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
                try
                {
                    handleIndexRedirection(ctx);
                if (!IsFromPublicFolder(ctx))
                {
                    bool found = false;
                    foreach (var route in routesDictionary.Keys)
                    {
                        if (ctx.Request.RawUrl == route)
                        {
                            found = true;
                            var response = new Response(ctx);
                            var request = new Request(ctx);
                            routesDictionary[route].Invoke(request, response);
                        }
                    }
                    if (!found)
                    {

                        if (!UseClientSideRouting)
                        {
                            handle404Async(ctx);
                        }
                        else
                        {

                            new Response(ctx).file(Utils.Server.GetFileFromFolders(PublicFolders, "index.html"));
                        }

                    }
                }
                else
                {
                    handlePublicResponse(ctx);
                }

            
            }
                catch (Exception ex)
                {
                    if (JintAddons.debug)
                    {
                        Log.Error(ex.Message + " " + ex.StackTrace);
                    }
                }
            }
        }

        private bool IsFromPublicFolder(HttpListenerContext context)
        {
            string filename = Utils.Server.GetFileFromFolders(PublicFolders, context.Request.RawUrl);
            return File.Exists(filename);
        }

        private void handlePublicResponse(HttpListenerContext ctx)
        {
            string filename = Utils.Server.GetFileFromFolders(PublicFolders,ctx.Request.RawUrl);
            filename = WebUtility.UrlDecode(filename);
            new Response(ctx).file(filename);


        }
        private void handleIndexRedirection(HttpListenerContext ctx)
        {
            if (ctx.Request.RawUrl == "/" && this.GETS.Count>0)
            {
                if (!this.GETS.ContainsKey("/"))
                {
                    new Response(ctx).redirect("/index.html");
                }
            }
        }

        private async Task handle404Async(HttpListenerContext ctx)
        {
            new Response(ctx).send(NoFoundTemplate(ctx.Request.RawUrl), 404, "text/html");
        }


         


        public class Response
        {
            HttpListenerContext context { get; set; }
            public Response(HttpListenerContext ctx)
            {
                context = ctx;
            }


            public async void dd(object data)
            {
                send(DDTemplate(data), 200, "text/html");
            }

            public async void file(string filePath)
            {
                if (!File.Exists(filePath))
                {
                    send(NoFoundTemplate(filePath), 404, "text/html");
                    return;
                }
                try
                {
                    HttpListenerResponse response = context.Response;
                    response.KeepAlive = true;
                    response.SendChunked = true;
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    string mime;
                    var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    response.ContentType = Utils.Server.mimeTypesMap.TryGetValue(Path.GetExtension(filePath), out mime) ? mime : "undefined";
                    response.ContentLength64 = stream.Length;
                    response.AddHeader("Accept-Ranges", "bytes");
                    response.AddHeader("Date", DateTime.Now.ToString("r"));
                    response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(filePath).ToString("r"));
                    response.AddHeader("Content-Range", string.Format("bytes {0}-{1}/{2}", 0, Convert.ToInt32(stream.Length) - 1, Convert.ToInt32(stream.Length)));
                    response.ContentLength64 = stream.Length;
                    stream.CopyTo(context.Response.OutputStream);
                    stream.Flush();
                    response.Close();
                }
                catch (Exception ex) {
                    if (JintAddons.debug)
                    {
                        Log.Error(ex.Message + " " + ex.StackTrace);
                    }
                }


            }


            public async void view(string viewPath, object data)
            {
                try
                {
                    var html = Render.FileToString(viewPath, data);
                    send(html, 200, "text/html");
                }
                catch (Exception ex)
                {
                    if (JintAddons.debug)
                    {
                        Log.Error(ex.Message + " " + ex.StackTrace);
                    }
                    send(Server.OopsTemplate(ex), 500, "text/html");
                }


            }
            public async void send(string data, int statusCode, string contentType)
            {
                try
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
                catch (Exception ex)
                {

                    if (JintAddons.debug)
                    {
                        Log.Error(ex.Message + " " + ex.StackTrace);
                    }
                    send(Server.OopsTemplate(ex), 500, "text/html");
                }

            }

            public async void send(object data, int statusCode)
            {
                   send(JsonConvert.SerializeObject(data), statusCode, "application/json");
            }

            public async void redirect(string route)
            {
                try
                {
                    context.Response.Redirect(route);
                }
                catch (Exception ex)
                {
                    if (JintAddons.debug)
                    {
                        Log.Error(ex.Message + " " + ex.StackTrace);
                    }
                    send(Server.OopsTemplate(ex), 500, "text/html");
                }
            }
        }

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



        public Server staticFolder(string path)
        {
            if (Directory.Exists(path))
            {
                this.PublicFolders.Add(path);
            }
            else
            {
                if (JintAddons.debug)
                {
                    Log.Error("Folder doesnt exist");
                }
            }
            return this;
        }

        public Server clientSideRouting(bool value)
        {
            this.UseClientSideRouting = value;
            return this;
        }


        public Server get(string route, Action<Request, Response> callback)
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







        public static string DDTemplate(object json)
        {
            if (!JintAddons.debug)
            {
                json = new
                {
                    error="Wrong mode",
                    message="Debug mode must be on"
                };
            }
            return Utils.Server.getTemplate("DD.mustache", JsonConvert.SerializeObject(json));
        }


        public static string OopsTemplate(Exception ex)
        {
            string error = null;
            if (JintAddons.debug)
            {
                error = ex.Message+" --> "+ex.StackTrace;
            }
            return Utils.Server.getTemplate("OopsTemplate.mustache", new { error });
        }

        public static string NoFoundTemplate(string route)
        {
             
            return Utils.Server.getTemplate("404.mustache",new {route});
        }


    }
}
