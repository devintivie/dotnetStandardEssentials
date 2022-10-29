using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetStandardEssentials.Results
{
    public class EnumerableResult<T> : Result, IEnumerable<T>
    {
        public readonly IEnumerable<T> Data;
        public EnumerableResult(T data) : base()
        {
            Data = new List<T> { data }.AsReadOnly();
        }

        public EnumerableResult(IEnumerable<T>? dataEnumerable) : base()
        {
            if (dataEnumerable is null)
            {
                Data = Enumerable.Empty<T>();
            }
            else
            {
                Data = dataEnumerable;
            }
            
        }

        public EnumerableResult(Exception? exception) : base(exception)
        {
            Data = Enumerable.Empty<T>();
        }
        public EnumerableResult(Result otherResult) : this(otherResult.Exception) { }

        public IEnumerator<T> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        public static implicit operator EnumerableResult<T>(T data) => new EnumerableResult<T>(data);
    }
}
