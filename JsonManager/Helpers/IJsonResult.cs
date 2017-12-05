using System;
using System.Collections.Generic;

namespace JsonManager.Helpers
{
    public enum JsonResultFlag
    {
        Succeeded = 0,
        Failed = 1,
        Error = 2,
        NotFound = 3,
        DbRetry = 4,
        Existed = 5,
        Forbidden = 6,
        Empty = 7,
        TaskCompleted = 8,
        Redirect = 9,
    };
    public interface IJsonResult: IDisposable
    {
        int ResultObjectId { get; set; }
        /// <summary>
        /// Useful to send result string, such strings are validation of string, can be return as accordingly modified (i.e string.empty if whitespace or zero length) from validators
        /// </summary>
        
        string ResultObjectString { get; set; }
        string ObjectId { get; set; }
        string Message { get; set; }
        JsonResultFlag Result { get; set; }
        IList<string> SuccessList { get; set; }
        IList<string> NonSuccessList { get; set; }
        IList<string> NotFoundList { get; set; }
        IList<string> ParamList { get; set; }
        IDictionary<string,string> KeyValuePair { get; set; }
        string RedirectUrl { get; set; }
        T GetResultObject<T>(ResultObject<T> resultObject) where T : class;
        void SetResultObject<T>(ResultObject<T> resultObject, T value) where T : class;
    }
}