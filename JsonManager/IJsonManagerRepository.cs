using System;
using System.Threading.Tasks;
using JsonManager.Helpers;

namespace JsonManager
{
    public interface IJsonManagerRepository : IDisposable
    {
        /// <summary>
        /// Get All Lines from the file as key-value pair of string.!--.!--.
        /// </summary>
        /// <param name="fileName">File Name: string</param>
        /// <param name="serverPath">Full path to locate the file: string</param>
        /// <param name="type">Get particular Section from Json File: string</param>
        
        Task<string[]> GetAllLines(string fileName, string serverPath, string type, bool ignoreCase);
        /// <summary>
        /// 'addIfNotFound' is important for adding a 'key', which is in use by program's (such as in controller, in Class or in view etc.!--.!--. )
        /// must add prefix (such as controller_, view_, class_, viewModel_, component_ etc.!--.!--. to identify them and not to delete them
        /// unless required. example of key (controller_checkout_credit_charge, controller_checkout_vat_rate etc.!--.remarks )!--.!--.)
        /// </summary>
        /// <param name="serverPath">Full Path without FileName: string</param>
        /// <param name="fileName">FileName: string</param>
        /// <param name="Key">Key of Key:Value Pair: string</param>
        /// <param name="addIfNotFound">Flag to indicate, if key not found, then will be added to the file: bool</param>
        
        Task<string> GetValueByKey(string serverPath, string fileName, string key, bool addIfNotFound, bool ignoreCase);
        /// <summary>
        /// RemoveLine functions as read all lines from file and return the line(s) does not match the key
        /// Provide an string not findable.!--. to get all lines from the file
        /// </summary>
        /// <param name="fileName">the full path to locate the: string</param>
        /// <param name="key">query string: string</param>
        
        Task<string[]> RemoveLine(string fileName, string key, bool ignoreCase);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">Full Path with FileName: string</param>
        /// <param name="Key">Key of Key:Value Pair: string</param>
        /// <param name="value">Value of Key:Value Pair: string</param>
        /// <param name="isClient">
        /// Particularly use for language translation json file(s), where Server resources(Views, Controller, ViewModel, Model etc.) and
        /// Client resources (.js, angular[controller, class. service, module, html etc.!--.] ): bool
        /// </param>
        /// <returns>modified lines for whole file: string[]</returns>
        Task<string[]> AddOrUpdate(string fileName, string key, string value, bool isClient, bool ignoreCase);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">Full Path with FileName: string</param>
        /// <param name="Key">Key of Key:Value Pair: string</param>
        
        Task<string> GetLineByKey(string fileName, string Key, bool ignoreCase);
        /// <summary>
        /// Remove list[key] from json file.!--.
        /// </summary>
        /// <param name="fileName">Full path with FileName: string</param>
        /// <param name="keyList">List[Key]: string[]</param>
        
        Task<IJsonResult> RemoveListJson(string fileName, string keyList, bool ignoreCase);
        /// <summary>
        /// Add new Key:Value pair in json file or update existing value found by key
        /// </summary>
        /// <param name="key">Key part of Key:Value pair: string</param>
        /// <param name="value">Value part of Key:Value pair: string</param>
        /// <param name="serverPath">Full path without FileName: string</param>
        /// <param name="fileName">FileName: string</param>
        /// <param name="addExplicitly">Flag to detect a key:value pair is using in any controller,class or view etc., if true then explicitly added the key:value pair, ignoring null value for 'value' field: bool </param>
         
        Task<IJsonResult> AddOrUpdateJson(string key, string value, string serverPath, string fileName, bool addExplicitly, bool ignoreCase);
    }
}