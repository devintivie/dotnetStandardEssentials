using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetStandardEssentials.Results
{
    public class DictionaryResult<TKey, TValue> : Result where TKey : notnull
    {

        private readonly Dictionary<TKey, TValue> _data = new Dictionary<TKey, TValue>();


        public Dictionary<TKey, TValue> Data => _data;


        public DictionaryResult(Dictionary<TKey, TValue> data)
        {
            if (data is null)
            {
                _data = new Dictionary<TKey, TValue>();
            }
            else
            {
                _data = data;
            }
        }


        public DictionaryResult(Exception? exception) 
            : base(exception)
        {
            _data = new Dictionary<TKey, TValue>();
        }


        public DictionaryResult(Result otherResult) 
            : this(otherResult.Exception) 
        { 

        }
    }
}
