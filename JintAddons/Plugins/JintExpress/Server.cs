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
namespace JintAddons.Plugins.JintExpress
{
    public class Server
    {


        #region  Server internal fields
        private Dictionary<string, Action<Request, Response>> GETS = new Dictionary<string, Action<Request, Response>>();
        private Dictionary<string, Action<Request, Response>> POSTS = new Dictionary<string, Action<Request, Response>>();
        private Dictionary<string, Action<Request, Response>> PUTS = new Dictionary<string, Action<Request, Response>>();
        private Dictionary<string, Action<Request, Response>> PATCHS = new Dictionary<string, Action<Request, Response>>();
        private Dictionary<string, Action<Request, Response>> DELETES = new Dictionary<string, Action<Request, Response>>();
        private List<string> PublicFolders = new List<string>();
        private Dictionary<string, string> ResponseHeaders = new Dictionary<string, string>();
        private HttpListener listener;
        private bool enabled = false;
        private bool UseClientSideRouting = false;
        private IAsyncResult handle;

        #endregion

        #region Server Public fields
        public string hostname = null;
        public int port;
        public string localIp
        {
            get
            {
                return ServerHelpers.GetLocalIp();
            }
        }
        #endregion




        public void listen(int port, Action callback, string host)
        {
            this.hostname = host;
            listen(port);
            callback.Invoke();
        }

        public void listen(int port, Action callback)
        {
         
            listen(port);
            callback.Invoke();
        }
       
        public void listen (int port )
        {
            this.port = port;
            if(hostname == null)
            {
                hostname = "localhost";
            }
            string url = $"http://{hostname}:{port}/";
            this.enabled = true;
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            handle = listener.BeginGetContext(new AsyncCallback(HandleIncomingRequest), listener);


        }

        public void stop()
        {
          
            handle.AsyncWaitHandle.WaitOne(1000);
            this.listener.Abort();
            this.enabled = false;
        }

        private async void HandleIncomingRequest(IAsyncResult res)
        {


            HttpListenerContext ctx = null;
            try
            {
                ctx = listener.EndGetContext(res);
            }
            catch (Exception)
            {
                return;
            }


            Dictionary<string, Action<Request, Response>> routesDictionary = null;

            foreach (var resHeader in ResponseHeaders)
            {
                ctx.Response.Headers.Add(resHeader.Key, resHeader.Value);
            }

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

                        if (ServerHelpers.NormalizeRoute(ctx.Request.RawUrl) == route)
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
                            handle404(ctx);
                        }
                        else
                        {

                            new Response(ctx).file(ServerHelpers.GetFileFromFolders(PublicFolders, "index.html"));
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
            if (enabled)
            {
                handle = listener.BeginGetContext(new AsyncCallback(HandleIncomingRequest), listener);
            }

        }

        private bool IsFromPublicFolder(HttpListenerContext ctx)
        {
            string filename = ServerHelpers.GetFileFromFolders(PublicFolders, ServerHelpers.NormalizeRoute(ctx.Request.RawUrl));
            return File.Exists(filename);
        }

        private void handlePublicResponse(HttpListenerContext ctx)
        {
            string filename = ServerHelpers.GetFileFromFolders(PublicFolders, ServerHelpers.NormalizeRoute(ctx.Request.RawUrl));
            filename = WebUtility.UrlDecode(filename);
            new Response(ctx).file(filename);
        }
        private void handleIndexRedirection(HttpListenerContext ctx)
        {
            if (ctx.Request.RawUrl == "/" && this.GETS.Count > 0)
            {
                if (!this.GETS.ContainsKey("/"))
                {
                    new Response(ctx).redirect("/index.html");
                }
            }
        }

        private async void handle404(HttpListenerContext ctx)
        {
            new Response(ctx).send(StaticTemplates.NoFoundTemplate(ctx.Request.RawUrl), 404, "text/html");
        }

        public Server appendResponseHeader(string key, string value)
        {
            this.ResponseHeaders.Add(key, value);
            return this;
        }

        public void staticFolder(string path)
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
        }

        public Server allowClientSideRouting(bool value)
        {
            this.UseClientSideRouting = value;
            return this;
        }


        public Server get(string route, Action<Request, Response> callback)
        {

            GETS.Add(ServerHelpers.NormalizeRoute(route), callback);
            return this;
        }

        public Server post(string route, Action<Request, Response> callback)
        {

            POSTS.Add(ServerHelpers.NormalizeRoute(route), callback);
            return this;
        }
        public Server put(string route, Action<Request, Response> callback)
        {

            PUTS.Add(ServerHelpers.NormalizeRoute(route), callback);
            return this;
        }
        public Server patch(string route, Action<Request, Response> callback)
        {

            PATCHS.Add(ServerHelpers.NormalizeRoute(route), callback);
            return this;
        }
        public Server delete(string route, Action<Request, Response> callback)
        {

            DELETES.Add(ServerHelpers.NormalizeRoute(route), callback);
            return this;
        }

    }
}
