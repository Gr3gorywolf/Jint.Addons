using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace JintAddons.Extensions
{
    public class Console
    {
        public Action<object> log {

            get
            {
                return (object content) =>
                {
                  
                        System.Console.WriteLine(JsonConvert.SerializeObject(content));
                   
                };
            }
        }

        public Action<object> clear
        {

            get
            {
                return (object content) =>
                {
                    System.Console.Clear();
                };
            }
        }

        public Func<string> read
        {

            get
            {
                return () =>
                {
                    return System.Console.ReadLine();
                };
            }
        }

        /* public Func<string> table
         {

             get
             {
                 return () =>
                 {
                     return System.Console.WriteLine;
                 };
             }
         }*/

        public string ToString(IFormatProvider provider)
        {
            return "";
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return this.GetType();
        }
    }
}
