using System;
using System.Collections.Generic;
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
                    System.Console.WriteLine(content);
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
