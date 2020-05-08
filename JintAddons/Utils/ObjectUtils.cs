using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace JintAddons.Utils
{
    public static class ObjectUtils
    {

        public static dynamic DictionaryToObject(IDictionary<String, Object> dictionary)
        {
            var expandoObj = new ExpandoObject();
            var expandoObjCollection = (ICollection<KeyValuePair<String, Object>>)expandoObj;

            foreach (var keyValuePair in dictionary)
            {
                expandoObjCollection.Add(keyValuePair);
            }
            dynamic eoDynamic = expandoObj;
            return eoDynamic;
        }



        public static bool HasDiffs(object obj1, object obj2, string path = "")
        {
            string path1 = string.IsNullOrEmpty(path) ? "" : path + ": ";
            if (obj1 == null && obj2 != null)
                return true;
            else if (obj2 == null && obj1 != null)
                return true;
            else if (obj1 == null && obj2 == null)
                return false;

            if (!obj1.GetType().Equals(obj2.GetType()))
                return true;

            Type type = obj1.GetType();
            if (path == "")
                path = type.Name;

            if (type.IsPrimitive || typeof(string).Equals(type))
            {
                if (!obj1.Equals(obj2))
                    return true;
                return false;
            }
            if (type.IsArray)
            {
                Array first = obj1 as Array;
                Array second = obj2 as Array;
                if (first.Length != second.Length)
                    return true;

                var en = first.GetEnumerator();
                int i = 0;
                while (en.MoveNext())
                {
                    bool res = HasDiffs(en.Current, second.GetValue(i), path);
                    if (res != false)
                        return true;
                    i++;
                }
            }
            else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
            {
                System.Collections.IEnumerable first = obj1 as System.Collections.IEnumerable;
                System.Collections.IEnumerable second = obj2 as System.Collections.IEnumerable;

                var en = first.GetEnumerator();
                var en2 = second.GetEnumerator();
                int i = 0;
                while (en.MoveNext())
                {
                    if (!en2.MoveNext())
                        return true;

                    bool res = HasDiffs(en.Current, en2.Current, path);
                    if (res != false)
                        return true;
                    i++;
                }
            }
            else
            {
                foreach (PropertyInfo pi in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
                {
                    try
                    {
                        var val = pi.GetValue(obj1);
                        var tval = pi.GetValue(obj2);
                        if (path.EndsWith("." + pi.Name))
                            return false;
                        var pathNew = (path.Length == 0 ? "" : path + ".") + pi.Name;
                        bool res = HasDiffs(val, tval, pathNew);
                        if (res != false)
                            return true;
                    }
                    catch (TargetParameterCountException)
                    {
                        //index property
                    }
                }
                foreach (FieldInfo fi in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
                {
                    var val = fi.GetValue(obj1);
                    var tval = fi.GetValue(obj2);
                    if (path.EndsWith("." + fi.Name))
                        return false;
                    var pathNew = (path.Length == 0 ? "" : path + ".") + fi.Name;
                    bool res = HasDiffs(val, tval, pathNew);
                    if (res != false)
                        return true;
                }
            }
            return false;
        }

    }
     
} 
