using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public string Message { get; } = string.Empty;


        protected Result(string? message = null)
        {
            Message = message ?? string.Empty;
        }


        public Result(Exception? exception, string? message = null)
        {
            Exception = exception;
            Message = message ?? string.Empty;
        }


        public Result(Result otherResult)
        {
            Exception = otherResult.Exception;
            Message = otherResult.Message;
        }


        public static readonly Result Ok = new Result();

    }

    public class Result<T> : Result
    {
#if NET5_0_OR_GREATER
        public T? Data { get; }
#else
        public T Data { get; }
#endif

        public Result(T data) : base()
        {
            Data = data;
        }

        public Result(Exception exception) : base(exception)
        {
            Data = default;
        }

        public Result(Result otherResult) : base(otherResult) { }

        public static implicit operator Result<T>(T data) => new Result<T>(data);

    }
}
