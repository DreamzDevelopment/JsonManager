using System.Collections.Generic;
using System.Linq;

namespace JsonManager.Helpers
{
    public class ResultObject<T> {
        private List<T> ObjectList;
        public ResultObject(){
            this.ObjectList = new List<T>();
        }
        public T GetItem(int index) {
            return ObjectList.ElementAtOrDefault(index);
        }
        public void SetItem(int index, T value){
            this.ObjectList.Insert(index, value);
        }
    }
    public class ResultObjectList<T> {
        // 1
        Dictionary<string, List<T>> _dictionary = new Dictionary<string, List<T>>();
        // 2
        public void Add(string key, T value)
        {
            List<T> list;
            if (this._dictionary.TryGetValue(key, out list))
            {
                // 2A.
                list.Add(value);
            }
            else
            {
                // 2B.
                list = new List<T>();
                list.Add(value);
                this._dictionary[key] = list;
            }
        }
        
        // 3
        public IEnumerable<string> Keys
        {
            get
            {
                return this._dictionary.Keys;
            }
        }
        // 4
        public List<T> this[string key]
        {
            get
            {
                List<T> list;
                if (!this._dictionary.TryGetValue(key, out list))
                {
                list = new List<T>();
                this._dictionary[key] = list;
                }
                return list;
            }
        }
    }
        
}