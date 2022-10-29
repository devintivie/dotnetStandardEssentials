using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetStandardEssentials.Results
{
    public class Result : IResult
    {
        public bool Success => Exception == null;
        public bool Failure => !Success;
        public Exception? Exception { get; }

        protected Result()
        {
        }

        public Result(Exception? exception)
        {
            Exception = exception;
        }

        public Result(Result otherResult)
        {
            Exception = otherResult.Exception;
        }

        public static readonly Result Ok = new Result();
    }

    public class Result<T> : Result
    {
#if NET5_0_OR_GREATER
        public readonly T? Data;
#else
        public readonly T Data;
#endif
        public Result(T data) : base()
        {
            Data = data;
        }

        public Result(Exception exception) : base(exception)
        {
            Data = default(T);
        }
        public Result(Result otherResult) : base(otherResult) { }


        public static implicit operator Result<T>(T data) => new Result<T>(data);
        public static implicit operator T(Result<T> otherResult) => otherResult.Data;
    }
}
