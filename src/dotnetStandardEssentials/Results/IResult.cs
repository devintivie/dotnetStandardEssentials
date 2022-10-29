using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetStandardEssentials.Results
{
    public interface IResult
    {
        bool Success { get; }
        bool Failure { get; }
        Exception Exception { get; }
    }
}
