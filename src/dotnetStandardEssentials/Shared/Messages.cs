using System;
using System.Collections.Generic;
using System.Text;

namespace dotnetStandardEssentials
{
    public class ExitMessage
    {
        public ExitMessage()
        {

        }
    }

    public class LogUpdateMessage
    {
        public GeneralMessage Message { get; set; }

        public LogUpdateMessage(GeneralMessage message)
        {
            Message = message;
        }
    }

    public class StringMessage : IMessage
    {
        public string Message { get; }

        public StringMessage(string message)
        {
            Message = message;
        }
    }

    public class UpdateViewMessage
    {

    }
    public class UpdateDataMessage
    {

    }

    public class NotifyMessage
    {
        public GeneralMessage LogMessage { get; set; }
        public NotifyMessage(GeneralMessage message)
        {
            LogMessage = message;
        }
        public NotifyMessage(string message, GeneralMessageType logType)
        {
            LogMessage = new GeneralMessage(message, logType);
        }
    }

    public struct ApplicationStatusMessage
    {
        public string Message { get; }
        public ApplicationStatusMessage(string message)
        {
            Message = message;
        }
    }

    public class UpdateErrorMessage
    {

    }

    public class NotifyBuildMessage : NotifyMessage
    {
        public NotifyBuildMessage(GeneralMessage message) : base(message)
        {
        }
    }

    public class ClearViewMessage
    {

    }

    public class ViewUnloadedMessage
    {

    }
}
