using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JintAddons.Plugins.JintExpress
{
    class StaticTemplates
    {
        public static string DDTemplate(object json)
        {
            if (!JintAddons.debug)
            {
                json = new
                {
                    error = "Wrong mode",
                    message = "Debug mode must be on"
                };
            }
            return ServerHelpers.getTemplate("DD.mustache", JsonConvert.SerializeObject(json));
        }

        public static string OopsTemplate(Exception ex)
        {
            string error = null;
            if (JintAddons.debug)
            {
                error = ex.Message + " --> " + ex.StackTrace;
            }
            return ServerHelpers.getTemplate("OopsTemplate.mustache", new { error });
        }

        public static string NoFoundTemplate(string route)
        {

            return ServerHelpers.getTemplate("404.mustache", new { route });
        }


    }
}
