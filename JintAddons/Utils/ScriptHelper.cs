using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace JintAddons.Utils
{
   public class ScriptHelper
    {
        public static string NormalizeScript(string content)
        {

            content = content.Replace("/*@jint", "").Replace("@end-jint*/","");
            content = Regex.Replace(content, @"//@jint-ignore(.|\n)*?//@end-jint-ignore", "");
            return content;

        }


        /*@jint
         
        lsaldhkjhlklsahkjdhlkjsalhkjdkjwhkjdkjqwkjdkjqwkjdkjqwkjdkjqw
        jsandnojqwdonowdonjqwdoonqwd
        sadnlksadnmolknkjdlonljdqwonljdonjqwdonjqwd
         
         @end-jint*/

        //@jint-ignore
      
        //@end-jint-ignore
       
    }
}
