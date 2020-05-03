using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace JintAddons.Utils
{
    class CatchingHelper
    {
        public static long GetRemoteFileSize(string uriPath)
        {

            var webRequest = HttpWebRequest.Create(uriPath);
            webRequest.Method = "HEAD";
            try {
            using (var webResponse = webRequest.GetResponse())
            {
                var fileSize = webResponse.Headers.Get("Content-Length");
                var fileSizeInMegaByte = Convert.ToInt64(fileSize);
                return fileSizeInMegaByte;
            }
            }catch(Exception ex)
            {
                return -1;
            }
        }
    }
}
