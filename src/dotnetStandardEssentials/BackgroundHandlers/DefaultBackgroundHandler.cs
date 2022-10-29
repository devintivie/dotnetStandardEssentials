using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNetStandardEssentials
{
    /// <summary>
    /// Default implementation of <see cref="IBackgroundHandler"/>
    /// </summary>
    public class DefaultBackgroundHandler : IBackgroundHandler
    {
        #region Fields
        private readonly IMessenger _messenger;
        private readonly ILogger _logger;
        private readonly INotificationHandler _notificationHandler;
        private List<GeneralMessage> _notifications => _notificationHandler.Notifications;
        private Dictionary<string, DateTime> _errorTimestamps = new Dictionary<string, DateTime>();
        private const long TICKS_PER_MILLISECOND = 10000;
        private const long TICKS_PER_SECOND = (long)10e6;
        private const double DEFAULT_CONTROLLED_NOTIFY_DELAY = 10.0;
        #endregion

        #region Properties
        /// <summary>
        /// Property to show that system startup is completed
        /// </summary>
        public bool IsStartupFinished { get; set; } = false;

        /// <summary>
        /// Redirect messages from startup to log instead of UI and log
        /// </summary>
        public bool BuildMessagesLogOnly { get; set; } = false;

        /// <summary>
        /// Property stating if this instance has any notifications
        /// </summary>
        public bool HasNotifications => _notifications.Count > 0;

        /// <summary>
        /// Currently displayed notification
        /// </summary>
        public GeneralMessage CurrentNotification { get; private set; } = new GeneralMessage(string.Empty);

        /// <summary>
        /// Current Notification index
        /// </summary>
        private int notificationIndex = -1;
        public int NotificationIndex
        {
            get { return notificationIndex; }
            set
            {
                if (notificationIndex != value)
                {
                    notificationIndex = value;
                    _messenger.Send(new UpdateViewMessage());
                }
            }
        }

        /// <summary>
        /// Number of Notifications in instance/>
        /// </summary>
        public int NotificationCount => _notifications.Count;

        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="messenger">Instance of IMessenger</param>
        /// <param name="notificationHandler">Instance of INotificationHandler</param>
        /// <param name="factory">Instance Serilog ILoggerFactory</param>
        public DefaultBackgroundHandler(IMessenger messenger
            , INotificationHandler notificationHandler
            , ILoggerFactory factory)
        {
            _messenger = messenger;
            _notificationHandler = notificationHandler;
            _logger = factory.CreateLogger("test");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Register message using underlying <see cref="IMessenger"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="recipient">Object registering</param>
        /// <param name="action">Action registering to</param>
        public void RegisterMessage<T>(object recipient, Action<T> action)
        {
            _messenger.Register(recipient, action);
        }

        /// <summary>
        /// Send message using underlying <see cref="IMessenger"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="recipient">Message object</param>
        /// <param name="action">Action registering to</param>
        public void SendMessage<T>(T message, object context)
        {
            _messenger.Send(message, context);
        }

        public void SendMessage<T>(T message)
        {
            _messenger.Send(message);
        }

        public void UnregisterMessages(object recipient)
        {
            _messenger.Unregister(recipient);
        }

        public void Log(GeneralMessage log)
        {
            var logLevel = (LogLevel)log.MessageType;
            _logger.Log(logLevel, log.Message);
        }

        public void Log(string message, GeneralMessageType logType = GeneralMessageType.Info)
        {
            _logger.Log((LogLevel)logType, message);
        }


        #endregion

        #region NotificationHandler Methods
        //public void Notify(string message, LogType logType, bool saveToLog = true)
        //{
        //    var logMessage = new LogMessage(message, logType);
        //    if (!BuildMessagesLogOnly || (BuildMessagesLogOnly && IsSystemBuilt))
        //    {
        //        if (logType != LogType.INFO)
        //        {
        //            _notificationHandler.AddNotification(logMessage.ToString());
        //        }
        //    }

        //    _messenger.Send(new NotifyMessage(logMessage));

        //    if (saveToLog)
        //    {
        //        Log(message, logType);
        //    }

        //    ProcessNotification();
        //    //if (Notifications.Count == 1)
        //    //{
        //    //    NotificationIndex = 0;
        //    //    UpdateNotification();
        //    //}
        //    //_messenger.Send(new UpdateViewMessage());
        //}

        public void Notify(string message, GeneralMessageType logType = GeneralMessageType.Warning, bool saveToLog = true)
        {
            var logMessage = new GeneralMessage(message, logType);
            Notify(logMessage);
        }


        public void Notify(GeneralMessage logMessage, bool saveToLog = true)
        {
            try
            {
                if (!BuildMessagesLogOnly || (BuildMessagesLogOnly && IsStartupFinished))
                {
                    if (logMessage.MessageType != GeneralMessageType.Info)
                    {
                        _notificationHandler.AddNotification(logMessage);
                    }
                }

                if (!IsStartupFinished)
                {
                    _messenger.Send(new NotifyBuildMessage(logMessage));
                }

                _messenger.Send(new NotifyMessage(logMessage));

                if (saveToLog)
                {
                    Log(logMessage);
                }

                ProcessNotification();
            }
            catch(Exception ex)
            {

            }
            
        }

        public void ControlledNotify(string message, GeneralMessageType messageType, double delaySeconds = DEFAULT_CONTROLLED_NOTIFY_DELAY, bool saveToLog = true)
        {
            var now = DateTime.Now;
            TimeSpan delayTimespan = new TimeSpan(ticks: (long)(delaySeconds * TICKS_PER_SECOND));
            if (_errorTimestamps.TryGetValue(message, out DateTime previousMessageTime))
            {
                if (now - previousMessageTime > delayTimespan)
                {
                    Notify(message, messageType, saveToLog);
                    _errorTimestamps[message] = now;
                }
            }
            else
            {
                _errorTimestamps.TryAdd(message, now);
                Notify(message, messageType, saveToLog);
            }
        }

        public void NotifyRange(IEnumerable<GeneralMessage> messages, bool saveToLog = true)
        {
            if (messages == null)
            {
                return;
            }

            foreach (GeneralMessage msg in messages)
            {
                Notify(msg);
            }
        }

        private void ProcessNotification()
        {
            if (_notifications.Count == 1)
            {
                NotificationIndex = 0;
                UpdateNotification();
            }
            _messenger.Send(new UpdateViewMessage());
        }

        private void UpdateNotification()
        {
            if (NotificationCount > 0)
            {
                if (NotificationIndex >= NotificationCount)
                {
                    NotificationIndex = NotificationCount - 1;
                }

                CurrentNotification = _notificationHandler.ShowNotification(NotificationIndex);
            }
            else
            {
                NotificationIndex = -1;
                CurrentNotification = new GeneralMessage(string.Empty);
            }
        }

        public Task DismissCurrentMessage()
        {
            int oldIndex = NotificationIndex;
            _notificationHandler.DismissNotification(NotificationIndex);
            if (_notifications.Count == 0)
            {
                NotificationIndex = 0;
            }
            else if (oldIndex >= NotificationCount)
            {
                NotificationIndex = _notifications.Count - 1;
            }
            else
            {
                NotificationIndex = oldIndex--;
            }
            UpdateNotification();
            _messenger.Send(new UpdateViewMessage());
            return Task.CompletedTask;
        }

        public Task ClearAsync()
        {
            _notificationHandler.Clear();
            UpdateNotification();
            return Task.CompletedTask;
        }

        public string GetCurrentMessage(int index)
        {
            return _notificationHandler.Notifications[index].ToString();
        }

        public Task ShowNextMessage()
        {
            if (NotificationIndex < NotificationCount - 1)
            {
                NotificationIndex++;
            }
            UpdateNotification();
            _messenger.Send(new UpdateErrorMessage());
            return Task.CompletedTask;
        }

        public Task ShowMessage(int index)
        {
            if (index < NotificationCount)
            {
                NotificationIndex = index;
            }
            UpdateNotification();
            _messenger.Send(new UpdateErrorMessage());
            return Task.CompletedTask;
        }

        public Task ShowPreviousMessage()
        {
            if (NotificationIndex != 0)
            {
                NotificationIndex--;
            }
            UpdateNotification();
            _messenger.Send(new UpdateErrorMessage());
            return Task.CompletedTask;
        }

        public Task IgnoreAllMessagesWithCurrentMessage()
        {
            int currentIndex = NotificationIndex;
            string msg = GetCurrentMessage(currentIndex);

            _notificationHandler.IgnoreMessage(msg);
            UpdateNotification();
            return Task.CompletedTask;
        }

        public void IgnoreAllMessagesWithSpecificTopic(string topic)
        {
            _notificationHandler.IgnoreTopic(topic);
        }

        public async Task DismissMessage(int index)
        {
            NotificationIndex = index;

            await DismissCurrentMessage().ConfigureAwait(false);
        }

        public void SetApplicationStatus(string statusMessage)
        {
            SendMessage(new ApplicationStatusMessage(statusMessage));
        }
        #endregion

    }
}
