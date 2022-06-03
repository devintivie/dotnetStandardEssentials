using System;
using System.Collections.Generic;
using System.Text;

namespace dotnetStandardEssentials
{
    public interface INotificationHandler
    {
        List<GeneralMessage> Notifications { get; }
        void AddNotification(string message, GeneralMessageType logType = GeneralMessageType.Info);
        void AddNotification(GeneralMessage logMessage);
        GeneralMessage ShowNotification(int index);
        void DismissNotification(int index);
        void IgnoreMessage(string message);
        void IgnoreTopic(string topic);
        //int IgnoreMessage(int currentIndex, string message);
        void Clear();
    }
}
