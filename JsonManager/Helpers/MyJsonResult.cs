using System.Collections.Generic;

namespace JsonManager.Helpers
{
    public class MyJsonResult : IJsonResult
    {
        public MyJsonResult()
        {
            this.Result = JsonResultFlag.Failed;
            this.SuccessList = new List<string>();
            this.NonSuccessList = new List<string>();
            this.NotFoundList = new List<string>();
            this.KeyValuePair = new Dictionary<string, string>();
        }
        public MyJsonResult(string message = null, string returnUrl = null) 
        {
            this.Result = JsonResultFlag.Failed;
            this.Message = message;
            this.RedirectUrl = returnUrl;

            this.SuccessList = new List<string>();
            this.NonSuccessList = new List<string>();
            this.NotFoundList = new List<string>();
            this.ParamList = new List<string>();
            this.KeyValuePair = new Dictionary<string, string>();

            // this.ResultObjectId = -1; // Causing out of bound index exception in SetResultObject();
            // this.ResultObjectString = string.Empty;
        }
        public T GetResultObject<T>(ResultObject<T> resultObject) where T : class 
        {
            return resultObject.GetItem(this.ResultObjectId);
        }  
        /// <summary>
        /// example SetResultObject(T)(new ResultObject(T)(), T value): where T : class(EntityName) 
        /// such as IdentityResult, Product, StripeCharge etc.!--.
        /// and 'value' is an instance (with attribute values) of class(EntityName) 
        /// </summary>
        /// <param name="resultObject">Type of Object, or instance of Type(Class/Entity): new Object(): class </param>
        /// <param name="value">'value' is an instance (with attribute values) of class(EntityName): new Object() { attr1, attr2, attr3.!--.!--.etc}: class</param>
        public void SetResultObject<T>(ResultObject<T> resultObject, T value) where T : class 
        {
                resultObject.SetItem(this.ResultObjectId, value);
        }
        public int ResultObjectId { get; set; } = 0;
        /// <summary>
        /// Useful to send result string, such strings are validation of string, can be return as accordingly modified (i.e string.empty if whitespace or zero length) from validators
        /// </summary>
        
        public string ResultObjectString { get; set; }
        public string ObjectId { get; set; }
        
        public string Message { get; set; }
        public JsonResultFlag Result { get; set; }
        public IList<string> SuccessList { get; set; }
        public IList<string> NonSuccessList { get; set; }
        public IList<string> NotFoundList { get; set; }
        public IList<string> ParamList { get; set; }
        public string RedirectUrl { get; set; }
        public IDictionary<string,string> KeyValuePair { get; set; }

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
        // ~MyJsonResult() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void System.IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
#endregion

    }
}