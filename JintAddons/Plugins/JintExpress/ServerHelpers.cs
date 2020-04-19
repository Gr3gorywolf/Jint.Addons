using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Nustache.Core;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;

namespace JintAddons.Plugins.JintExpress
{
    public class ServerHelpers
    {

        public static IDictionary<string, string> mimeTypesMap = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
        #region extension to MIME type list
        {".asf", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".avi", "video/x-msvideo"},
        {".bin", "application/octet-stream"},
        {".cco", "application/x-cocoa"},
        {".crt", "application/x-x509-ca-cert"},
        {".css", "text/css"}, 
        {".deb", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dll", "application/octet-stream"},
        {".dmg", "application/octet-stream"},
        {".ear", "application/java-archive"},
        {".eot", "application/octet-stream"},
        {".exe", "application/octet-stream"},
        {".flv", "video/x-flv"},
        {".gif", "image/gif"},
        {".hqx", "application/mac-binhex40"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".ico", "image/x-icon"},
        {".img", "application/octet-stream"},
        {".iso", "application/octet-stream"},
        {".jar", "application/java-archive"},
        {".jardiff", "application/x-java-archive-diff"},
        {".jng", "image/x-jng"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".json", "application/json"},
        {".mml", "text/mathml"},
        {".mng", "video/x-mng"},
        {".mov", "video/quicktime"},
        {".mp3", "audio/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpg", "video/mpeg"},
        {".msi", "application/octet-stream"},
        {".msm", "application/octet-stream"},
        {".msp", "application/octet-stream"},
        {".pdb", "application/x-pilot"},
        {".pdf", "application/pdf"},
        {".pem", "application/x-x509-ca-cert"},
        {".pl", "application/x-perl"},
        {".pm", "application/x-perl"},
        {".png", "image/png"},
        {".prc", "application/x-pilot"},
        {".ra", "audio/x-realaudio"},
        {".rar", "application/x-rar-compressed"},
        {".rpm", "application/x-redhat-package-manager"},
        {".rss", "text/xml"},
        {".run", "application/x-makeself"},
        {".sea", "application/x-sea"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".swf", "application/x-shockwave-flash"},
        {".tcl", "application/x-tcl"},
        {".tk", "application/x-tcl"},
        {".txt", "text/plain"},
        {".war", "application/java-archive"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wmv", "video/x-ms-wmv"},
        {".xml", "text/xml"},
        {".xpi", "application/x-xpinstall"},
        {".zip", "application/zip"},
        {".mp4", "video/mpeg"},
            #endregion
        };


        
      
        /// <summary>
        /// Removes the query strings and self complete the specified route 
        /// </summary>
        /// <returns></returns>
        public static string NormalizeRoute(string route)
        {
            route =  (route.StartsWith("/") ? route : $"/{route}");
            if (route.Contains("?"))
            {
                return route.Split('?')[0];
            }
            return route;
        }

        public static string GetTemplate(string templateFullName,object data)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName =assembly.GetManifestResourceNames()
                                    .Single(str => str.EndsWith(templateFullName));
            string result = "";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                 result = reader.ReadToEnd();
            }
            string template = "";
            try
            {
                 template = Render.StringToString(result, data);
            }
            catch (Exception)
            {
                template = "<h5 style='color:red'>Template rendering failed</h5>";
            }
            return template;
        }


        public static object ParseFormData(string formData)
        {

            Dictionary<string, object> form = new Dictionary<string, object>();
            if (formData.Contains("&"))
            {
                foreach (var pair in formData.Split('&'))
                {
                    if (pair.Contains('='))
                    {
                        var pairData = pair.Split('=');
                        form.Add(pairData[0], pairData[1]);
                    }

                }
            }
            return Utils.ObjectUtils.DictionaryToObject(form);
        }


        public static string GetLocalIp()
        {
            string ipAddress = "";
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in localIPs)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipAddress = ip.ToString();

                }
            }
            return ipAddress;
        }

        public static string RemoveIllegalPathCharacters(string path)
        {
            string regexSearch = new string(System.IO.Path.GetInvalidFileNameChars()) + new string(System.IO.Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(path, "");
        }

        public static string GetFileFromFolders(List<string> folders, string fileName)
        {
            fileName = System.Web.HttpUtility.UrlDecode(fileName);
            if (fileName.StartsWith("/"))
            {
               fileName =  fileName.Remove(0, 1);
            }
            var file = "";
            foreach (var fold in folders)
            {
                var filePath = Path.Combine(Path.GetFullPath(fold), fileName);
                if (File.Exists(filePath))
                {
                    return filePath;
                }
            }
            return file;
        }
    }
}
