using System;
using System.Collections.Generic;
using System.Text;

namespace dotnetStandardEssentials
{
    public enum GeneralMessageType
    {
        // Info, Warning and Error match Serilog ErrorType
        Info = 2,
        Warning = 3,
        Error = 4,

        Success = 10
    }

    public enum GeneralMessageCode
    {
        None,
        Ok,
        Success,
        Failed
    }
}
