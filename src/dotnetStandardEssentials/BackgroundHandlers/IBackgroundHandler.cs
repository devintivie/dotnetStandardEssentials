﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetStandardEssentials
{
    public interface IBackgroundHandler
    {
        #region Messenger
        void RegisterMessage<T>(object recipient, Action<T> action);
        void SendMessage<T>(T message);
        void SendMessage<T>(T message, object context);
        void UnregisterMessages(object recipient);
        void SetApplicationStatus(string statusMessage);
        #endregion

        #region Logger
        void Log(string message, GeneralMessageType logType = GeneralMessageType.Info);
        void Log(GeneralMessage log);
        #endregion

        #region ErrorHandler
        bool IsStartupFinished { get; set; }
        bool BuildMessagesLogOnly { get; set; }
        GeneralMessage CurrentNotification { get; }
        int NotificationIndex { get; }
        int NotificationCount { get; }
        bool HasNotifications { get; }
        void Notify(string message, GeneralMessageType logType = GeneralMessageType.Warning, bool saveToLog = true);
        void Notify(Exception exception, GeneralMessageType logType = GeneralMessageType.Warning, bool saveToLog = true);
        void Notify(GeneralMessage message, bool saveToLog = true);
        void NotifyRange(IEnumerable<GeneralMessage> messages, bool saveToLog = true);
        void ControlledNotify(string message, GeneralMessageType messageType, double delaySeconds = 10.0, bool saveToLog = true);
        //Task<bool> ConfirmAsync(string message);//, Action<bool> action);
        Task DismissCurrentMessage();
        Task DismissMessage(int index);
        Task ShowMessage(int index);
        Task ShowNextMessage();
        Task ShowPreviousMessage();
        Task IgnoreAllMessagesWithCurrentMessage();
        void IgnoreAllMessagesWithSpecificTopic(string topic);
        Task ClearAsync();
        #endregion

    }
}
