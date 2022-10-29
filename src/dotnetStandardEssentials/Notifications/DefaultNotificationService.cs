using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetStandardEssentials
{
    public class DefaultNotificationService : INotificationHandler
    {
        #region Fields
        IMessenger _messenger;
        #endregion

        #region Properties
        /// <summary>
        /// List of active notifications
        /// </summary>
        public List<GeneralMessage> Notifications { get; private set; } = new List<GeneralMessage>();

        /// <summary>
        /// List of current messages that will be ignored if they come up again
        /// </summary>
        public List<string> IgnoredMessages { get; set; } = new List<string>();

        /// <summary>
        /// List of topics that will be ignored if they come up at all, usually provided in config file
        /// </summary>
        public List<string> IgnoredTopics { get; set; } = new List<string>();
        #endregion

        #region Constructors
        /// <summary>
        /// <see cref="SimpleNotificationService" constructor/>
        /// </summary>
        /// <param name="messenger">Required: Instance of <see cref="IMessenger"/></param>
        public DefaultNotificationService(IMessenger messenger)
        {
            _messenger = messenger;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Start instance fresh, clear all collections
        /// </summary>
        public void Clear()
        {
            Notifications.Clear();
            IgnoredMessages.Clear();
            IgnoredTopics.Clear();
        }

        /// <summary>
        /// Create <see cref="GenericMessage"/> and add it to notification collection
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logType"></param>
        public void AddNotification(string message, GeneralMessageType logType = GeneralMessageType.Info)
        {
            var logMessage = new GeneralMessage(message, logType);
            AddNotification(logMessage);
        }

        /// <summary>
        /// Add <see cref="GenericMessage"/> to notification collection
        /// </summary>
        /// <param name="logMessage"></param>
        public void AddNotification(GeneralMessage logMessage)
        {
            string message = logMessage.Message;
            if (IsTopicInMessageIgnored(message))
            {
                return;
            }

            if (!IgnoredMessages.Contains(message))
            {
                Notifications.Add(logMessage);
                _messenger.Send(new UpdateErrorMessage());
            }
        }

        /// <summary>
        /// check if logMessage contains an ignored topic
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool IsTopicInMessageIgnored(string message)
        {
            foreach (var topic in IgnoredTopics)
            {
                //returns -1 if topic is not found in message
                if (message.IndexOf(topic, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Shot specific notification
        /// </summary>
        /// <param name="index">Index of the notification</param>
        /// <returns></returns>
        public GeneralMessage ShowNotification(int index)
        {
            //CurrentIndex = index;
            if (Notifications.Count != 0 && Notifications.Count > index)
            {
                return Notifications[index];
            }

            return new GeneralMessage(string.Empty);

        }


        /// <summary>
        /// Remove notification from notification collection
        /// </summary>
        /// <param name="index"></param>
        public void DismissNotification(int index)
        {
            Notifications.RemoveAt(index);
        }

        /// <summary>
        /// Ignore future message with same string if they appear again
        /// </summary>
        /// <param name="message"></param>
        public void IgnoreMessage(string message)
        {
            //Remove all current notifications containing message
            if (!IgnoredMessages.Contains(message))
            {
                IgnoredMessages.Add(message);
                Notifications.RemoveAll(IgnoredMessage);
            }
        }

        /// <summary>
        /// Ignore all messages containing specified topic
        /// </summary>
        /// <param name="topic">string that reference topic to be ignored</param>
        public void IgnoreTopic(string topic)
        {
            if (!IgnoredTopics.Contains(topic))
            {
                IgnoredTopics.Add(topic);
            }
        }

        /// <summary>
        /// Check if <see cref="GenericMessage" message is ignored/>
        /// </summary>
        /// <param name="logMessage"></param>
        /// <returns></returns>
        private bool IgnoredMessage(GeneralMessage logMessage)
        {
            if (IgnoredMessages.Contains(logMessage.Message))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
