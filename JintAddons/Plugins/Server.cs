﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        public string PublicFolder = "";
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
            listener.Stop();
            this.enabled = false;
            handleThread.Abort();
        }


        public async void HandleRequest()
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

                            new Response(ctx).file(PublicFolder + "/index.html");
                        }

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
            new Response(ctx).file(filename);


        }
        private void handleIndexRedirection(HttpListenerContext ctx)
        {
            if (ctx.Request.RawUrl == "/" && this.PublicFolder == "")
            {
                if (!this.GETS.ContainsKey("/"))
                {
                    new Response(ctx).file(this.PublicFolder + "/index.html");
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
                        Console.WriteLine(ex.Message + " " + ex.StackTrace);
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
                        Console.WriteLine(ex.Message + " " + ex.StackTrace);
                    }
                    send(Server.OopsTemplate(ex.Message, ex.StackTrace), 500, "text/html");
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
                        Console.WriteLine(ex.Message + " " + ex.StackTrace);
                    }
                    send(Server.OopsTemplate(ex.Message, ex.StackTrace), 500, "text/html");
                }

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
                        Console.WriteLine(ex.Message + " " +  ex.StackTrace);
                    }
                    send(Server.OopsTemplate(ex.Message, ex.StackTrace), 500, "text/html");
                }
            }
        }

        public class Request
        {
            HttpListenerContext context { get; set; }
            public string data { get; set; }
            public Request(HttpListenerContext ctx)
            {
                context = ctx;
                var request = ctx.Request;
                StreamReader stream = new StreamReader(request.InputStream);
                this.data = stream.ReadToEnd();
            }

        }



        public Server staticFiles(string path)
        {
            this.PublicFolder = path;
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





        public static string OopsTemplate(string error, string trace)
        {

            if (!JintAddons.debug)
            {
                error = "an error has occurred";
                trace = "";
            }

            var _res = $@"

              <h1>Oooops</h1><br>
             <h3>{error}</h3>
             <h6 style='color:red'>{trace}</h6>
                  
             ";
            return _res;
        }

        public static string NoFoundTemplate(string route)
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
