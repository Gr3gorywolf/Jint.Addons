using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Nustache.Core;
using System.IO;
using System.Text.RegularExpressions;

namespace JintAddons.Utils
{
    public class Server
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


        public static string getTemplate(string templateFullName,object data)
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
            string template = "<h5 style='color:red'>Template rendering failed</h5>";
            try
            {
                 template = Render.StringToString(result, data);
            }
            catch (Exception)
            {
                template = null;
            }
            return template;
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


                 /*   foreach (var fls in Directory.GetFiles(fold, "*.*", SearchOption.AllDirectories))
                    {
                        var normalizedPath = fls.Replace(@"\", "/");
                        if (normalizedPath.EndsWith(fileName))
                        {
                            file = fls;
                            break;
                        }
                    }*/
            }
            return file;
        }
    }
}
