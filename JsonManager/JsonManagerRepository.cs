using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JsonManager.Helpers;

namespace JsonManager
{
    public class JsonManagerRepository : IJsonManagerRepository
    {

#region Get All Lines from File
        /// <summary>
        /// Get All Lines from the file as key-value pair of string.!--.!--.
        /// </summary>
        /// <param name="fileName">File Name: string</param>
        /// <param name="serverPath">Full path to locate the file: string</param>
        /// <param name="type">Get particular Section from Json File: string</param>
        
        async Task<string[]> IJsonManagerRepository.GetAllLines(string fileName, string serverPath, string type, bool ignoreCase = true) {
            string[] settings = null;
            var file_path = Directory.GetCurrentDirectory() + "\\";
            var file_setting = $"sitesettings.json"; 
            // Create if not found 
            JsonHelper.CreateFileIfNeeded(file_path, file_setting);

            if(File.Exists(Path.Combine(serverPath, fileName))){
                // RemoveLine function reads all lines from file and return the line(s) does not match the key
                settings = await ((IJsonManagerRepository)this).RemoveLine(Path.Combine(serverPath, fileName), "Unknown-$String-$To-$Find", ignoreCase); 
            }
            return settings;
        }
#endregion
#region Get Json Value by Key
        /// <summary>
        /// 'addIfNotFound' is important for adding a 'key', which is in use by program's (such as in controller, in Class or in view etc.!--.!--. )
        /// must add prefix (such as controller_, view_, class_, viewModel_, component_ etc.!--.!--. to identify them and not to delete them
        /// unless required. example of key (controller_checkout_credit_charge, controller_checkout_vat_rate etc.!--.remarks )!--.!--.)
        /// </summary>
        /// <param name="serverPath">Full Path without FileName: string</param>
        /// <param name="fileName">FileName: string</param>
        /// <param name="Key">Key of Key:Value Pair: string</param>
        /// <param name="addIfNotFound">Flag to indicate, if key not found, then will be added to the file: bool</param>
        
        async Task<string> IJsonManagerRepository.GetValueByKey(string serverPath, string fileName, string key, bool addIfNotFound, bool ignoreCase = true) 
        {
            string line = await ((IJsonManagerRepository)this).GetLineByKey(Path.Combine(serverPath, fileName), key, ignoreCase);
            string result = null;
            
            if((string.IsNullOrWhiteSpace(line) || (!string.IsNullOrWhiteSpace(line) && line.Length < 6)) && addIfNotFound == true) { // add the key in the list as Key:"", value ""
                IJsonResult jsonResult = await ((IJsonManagerRepository)this).AddOrUpdateJson(key, null, serverPath, fileName, addIfNotFound, ignoreCase);
                return result;
            }
            if(!(string.IsNullOrWhiteSpace(line)) && (line.IndexOf(':') > 1 && line.LastIndexOf('"') > 5)) { // "": "" format for "Key": "Value"
                result = line.Substring(line.IndexOf(':') + 3, line.LastIndexOf('"') - (line.IndexOf(':') + 3));
                if(result.Length <= 0) { result = null; }
            }
            return result;
        }
#endregion
#region AddOrUpdate Json 
        /// <summary>
        /// Add new Key:Value pair in json file or update existing value found by key
        /// </summary>
        /// <param name="key">Key part of Key:Value pair: string</param>
        /// <param name="value">Value part of Key:Value pair: string</param>
        /// <param name="serverPath">Full path without FileName: string</param>
        /// <param name="fileName">FileName: string</param>
        /// <param name="addExplicitly">Flag to detect a key:value pair is using in any controller,class or view etc., if true then explicitly added the key:value pair, ignoring null value for 'value' field: bool </param>
         
        async Task<IJsonResult> IJsonManagerRepository.AddOrUpdateJson(string key, string value, string serverPath, string fileName, bool addExplicitly, bool ignoreCase = true) {
            IJsonResult jsonResult = new MyJsonResult() { 
                Message = "Failed to update " + $"'{key}':'{value}'", 
                Result = JsonResultFlag.Failed 
            };
            if(string.IsNullOrWhiteSpace(serverPath) || string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(key) || (string.IsNullOrWhiteSpace(value) && (addExplicitly == false) )) { // if explicit path defined for server resource 
                return jsonResult;
            }
            string[] linesToKeep;
            try{
                JsonHelper.CreateFileIfNeeded(serverPath , fileName);
                if(File.Exists(Path.Combine(serverPath, fileName)) == true) {
                    var tempFile = Path.GetTempFileName();
                    linesToKeep = await ((IJsonManagerRepository)this).AddOrUpdate(Path.Combine(serverPath, fileName), key, value, false, ignoreCase);
                    if(linesToKeep != null) {
                        if(linesToKeep.Count() > 2) {
                            linesToKeep[1] = linesToKeep[1].Substring(linesToKeep[1].IndexOf(',') + 1); // important to remove ',' at the beginning of json file
                        }
                        File.WriteAllLines(tempFile, linesToKeep.ToList());
                        File.Delete(Path.Combine(serverPath, fileName));
                        File.Move(tempFile, Path.Combine(serverPath, fileName));
                    }
                }
                jsonResult.Message = "Succeeded to update " + $"'{key}':'{value}'";
                jsonResult.Result = JsonResultFlag.Succeeded;
            }catch(Exception ex) {
                jsonResult.Message = ex.Message;
                jsonResult.Result = JsonResultFlag.DbRetry;
            }
            return jsonResult;

        }
#endregion
#region Remove List of json
        /// <summary>
        /// Remove list[key] from json file.!--.
        /// </summary>
        /// <param name="fileName">Full path with FileName: string</param>
        /// <param name="keyList">List[Key] seperated by ', comma' : string[]</param>
        
        async Task<IJsonResult> IJsonManagerRepository.RemoveListJson(string fileName, string keyList, bool ignoreCase = true) {
            IJsonResult jsonResult = new MyJsonResult() { 
                Message = "Failed to remove " + $"'{keyList}'", 
                Result = JsonResultFlag.Failed 
            };
            if(!(File.Exists(fileName) == true)) {
                return jsonResult;
            }
            // ensure atleast one key 
            if(string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(keyList) || keyList.Length < 1 ){
                return jsonResult;
            }
            try{
                string queryStr  = "";
                string[] lines = File.ReadAllLines(fileName);
                string[] linesToKeep = lines;
                string[] keys = keyList.Split(',');
                var tempFile = Path.GetTempFileName();
                if(linesToKeep.Length > 2){
                    // loop through all elements in keyList==
                    foreach (var key in keys)
                    {
                        queryStr = '"' + key + '"'; // Important to wrap the key with '"'
                        linesToKeep = ignoreCase ? linesToKeep.Where(l => !(l.ToString().ToLower().IndexOf(queryStr.ToLower()) >= 0)).ToArray() : 
                                        linesToKeep.Where(l => !(l.ToString().IndexOf(queryStr) >= 0)).ToArray();
                    }
                    if(linesToKeep != null) {
                        if(linesToKeep.Count() > 2) {
                            linesToKeep[1] = linesToKeep[1].Substring(linesToKeep[1].IndexOf(',') + 1); // important to remove ',' at the beginning of json file
                        }
                        File.WriteAllLines(tempFile, linesToKeep.ToList());
                        File.Delete(fileName);
                        File.Move(tempFile, fileName);
                    }
                }
                jsonResult.Message = "Succeeded to remove " + $"'{keyList}'";
                jsonResult.Result = JsonResultFlag.Succeeded;
            }catch(Exception ex){
                jsonResult.Message = ex.Message;
                jsonResult.Result = JsonResultFlag.DbRetry;
            }
            return jsonResult;
        }
#endregion
#region GetLineByKey
        async Task<string> IJsonManagerRepository.GetLineByKey(string fileName, string key, bool ignoreCase = true) {
            string queryStr = '"' + key + '"';
            string[] lines = File.ReadAllLines(fileName);
            string line = "";
            if(lines.Length > 2){
                line = ignoreCase ? lines.FirstOrDefault(l => (l.ToString().ToLower().IndexOf(queryStr.ToLower()) >= 0)) :
                        lines.FirstOrDefault(l => (l.ToString().IndexOf(queryStr) >= 0));
                return line;
            }else { return null; }
        }
#endregion
#region AddOrUpdate
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">Full Path with FileName: string</param>
        /// <param name="key">Key of Key:Value Pair: string</param>
        /// <param name="value">Value of Key:Value Pair: string</param>
        /// <param name="isClient">
        /// Particularly use for language translation json file(s), where Server resources(Views, Controller, ViewModel, Model etc.) and
        /// Client resources (.js, angular[controller, class. service, module, html etc.!--.] ): bool
        /// </param>
        /// <returns>modified lines for whole file: string[]</returns>
        async Task<string[]> IJsonManagerRepository.AddOrUpdate(string fileName, string key, string value, bool isClient, bool ignoreCase = true) {
            string queryStr = '"' + key + '"';
            string[] lines = File.ReadAllLines(fileName);
            string[] linesToKeep = lines;
            var length = 3;
            string lineToAdd = "";
            
            if(lines.Length <= 2) { 
                // Deprecated! in AspNetCore2Angular5 v2.0, Client translation Key is as it is, not to LowerCase().
                // // translate key to toLower() for client resource-file...as client resource require all language(s) translation file..
                // // including english // so in views (html file) .. all string will be toLowerCase()
                
                /// <summary>
                /// if value not set to null then on calling function cannot determine if valid value returned. if null then on calling function can set its default value
                /// </summary>
                /// <param name="="></param>
                
                // if(value != null) { 
                lineToAdd = '"' + (isClient == true ? ignoreCase ? key.ToLower() : key : key) + '"' + ": " + '"' + value + '"'; 
                //}
                // else{ lineToAdd = '"' + (isClient == true ? key.ToLower() : key) + '"' + ": " + '"' + key + '"'; }

                linesToKeep = new string[length];
                linesToKeep[0] = "{";
                linesToKeep[1] = lineToAdd;
                linesToKeep[2] = "}";
            }
            else{
                linesToKeep = ignoreCase ? lines.Where(l => !(l.ToString().ToLower().IndexOf(queryStr.ToLower()) >= 0)).ToArray() : 
                                lines.Where(l => !(l.ToString().IndexOf(queryStr) >= 0)).ToArray();
                length = linesToKeep.Length;
                /// <summary>
                /// if value not set to null then on calling function cannot determine if valid value returned. if null then on calling function can set its default value
                /// </summary>
                /// <param name="="></param>
                
                // if(value != null) {  
                lineToAdd = (length > 2 ? "," : "") + '"' + (isClient == true ? ignoreCase ? key.ToLower() : key : key) + '"' + ": " + '"' + value + '"'; 
                //}
                //else{ lineToAdd = (length > 2 ? "," : "") + '"' + (isClient == true ? key.ToLower() : key) + '"' + ": " + '"' + key + '"'; }

                Array.Resize(ref linesToKeep, length + 1);
                linesToKeep[length] = linesToKeep[length - 1];
                linesToKeep[length - 1] = lineToAdd;
            }
            return linesToKeep;
        }
        /// <summary>
        /// RemoveLine functions as read all lines from file and return the line(s) does not match the key
        /// Provide an string not findable.!--. to get all lines from the file
        /// </summary>
        /// <param name="fileName">the full path to locate the: string</param>
        /// <param name="key">query string: string</param>
#endregion
#region RemoveLine
        async Task<string[]> IJsonManagerRepository.RemoveLine(string fileName, string key, bool ignoreCase = true) 
        { // has functionality to get all lines from file as well
            string queryStr = '"' + key + '"';
            string[] lines = File.ReadAllLines(fileName);
            string[] linesToKeep = lines;

            if(lines.Length > 2){
                linesToKeep = ignoreCase ? lines.Where(l => !(l.ToString().ToLower().IndexOf(queryStr.ToLower()) >= 0)).ToArray() : 
                                lines.Where(l => !(l.ToString().IndexOf(queryStr) >= 0)).ToArray();
            }
            return linesToKeep;
        }
        /// RemoveLine functions as read all lines from file and return the line(s) does not match the key
        /// Provide an string not findable.!--. to get all lines from the file
        /// </summary>
        /// <param name="fileName">the full path with FileName: string</param>
        /// <param name="key">query string: string</param>
        
#endregion
#region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~JsonManagerRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        
#endregion

    }
}