using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JsonManager
{
    public static class JsonHelper
    {
        public static string JsonSerialize(object obj)
        {
            return JsonConvert.SerializeObject(obj,
                        new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            StringEscapeHandling =  StringEscapeHandling.EscapeHtml,
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });
        }
        /// <summary>
        /// Create the directory path if not exists
        /// </summary>
        /// <param name="path">the path of location: string</param>
        /// <returns>bool</returns>
        public static bool CreateFolderIfNeeded(string path)
        {
            if (Directory.Exists(path))
                return true;
            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception)
            {
                /*TODO: You must process this exception.*/
                return false;
            }
        }
        /// <summary>
        /// Create the file if not exists
        /// </summary>
        /// <param name="path">the path locate the file : string</param>
        /// <param name="fileName">name of the file with extension: string</param>
        /// <returns>bool</returns>
        public static bool CreateFileIfNeeded(string path, string fileName)
        {
            JsonHelper.CreateFolderIfNeeded(path);
            if (File.Exists(Path.Combine(path , fileName) ) )
            {
                return true;
            }
            try
            {
                /// <summary>
                /// .Close() is not supporting in all TargetFrameworks.
                /// </summary>
                File.Create(Path.Combine(path , fileName)).Dispose(); // Important to close...
                return true;
            }
            catch (Exception)
            {
                /*TODO: You must process this exception.*/
                return false;
            }
        }
    }
}