using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetStandardEssentials.Results
{
    public class DictionaryResult<T, U> : Result
    {
        public readonly Dictionary<T, U> Data;

        public DictionaryResult(Dictionary<T, U> data)
        {
            if (data is null)
            {
                Data = new Dictionary<T, U>();
            }
            else
            {
                Data = data;
            }
        }

        public DictionaryResult(Exception? exception) : base(exception)
        {
            Data = new Dictionary<T, U>();
        }

        public DictionaryResult(Result otherResult) : this(otherResult.Exception) { }
    }
}
