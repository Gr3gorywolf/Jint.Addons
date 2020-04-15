using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JintAddons.Plugins
{
    public class FileSystem
    {



        public static bool make(string path, string content)
        {
            try
            {
                var file = File.CreateText(path);
                file.Write(content);
                file.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool delete(string path)
        {
            try
            {
                File.Delete(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
           
        }

        public static string read(string path)
        {
            return File.ReadAllText(path);
        }

    }
}
