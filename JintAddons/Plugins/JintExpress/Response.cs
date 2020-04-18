using Newtonsoft.Json;
using NLog.Fluent;
using Nustache.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace JintAddons.Plugins.JintExpress
{
    public class Response
    {

        HttpListenerContext context { get; set; }


        public Response(HttpListenerContext ctx)
        {
            context = ctx;

        }


        public async void dd(object data)
        {
            send(StaticTemplates.DDTemplate(data), 200, "text/html");
        }

        public async void file(string filePath)
        {
            if (!File.Exists(filePath))
            {
                send(StaticTemplates.NoFoundTemplate(filePath), 404, "text/html");
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
                response.ContentType = ServerHelpers.mimeTypesMap.TryGetValue(Path.GetExtension(filePath), out mime) ? mime : "undefined";
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
            catch (Exception ex)
            {
                if (JintAddons.debug)
                {
                    Log.Error(ex.Message + " " + ex.StackTrace);
                }
            }

        }


        public async void render(string viewPath, object data)
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
                send(StaticTemplates.OopsTemplate(ex), 500, "text/html");
            }


        }


        public async void send(string data)
        {
            send(data,200,"text/plain");
        }
        public async void send(string data, int statusCode)
        {
            send(data, statusCode, "text/plain");
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
                send(StaticTemplates.OopsTemplate(ex), 500, "text/html");
            }

        }


        public async void json(object data)
        {
            json(data, 200);
        }
        public async void json(object data, int statusCode)
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
                send(StaticTemplates.OopsTemplate(ex), 500, "text/html");
            }
        }
    }
}
