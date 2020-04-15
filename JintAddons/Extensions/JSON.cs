using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace JintAddons.Extensions
{
    public class JSON
    {
        public static string stringify(object data)
        {
           return JsonConvert.SerializeObject(data);
        }

        public static object parse(string value)
        {
            return JsonConvert.DeserializeObject(value);
        }

    }
}
