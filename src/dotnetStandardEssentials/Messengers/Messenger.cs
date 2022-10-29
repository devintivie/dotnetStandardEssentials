using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetStandardEssentials
{
    public class Messenger : IMessenger
    {
        #region Fields
        private static readonly ConcurrentDictionary<MessengerKey, object> Dictionary = new ConcurrentDictionary<MessengerKey, object>();

        #endregion

        #region Properties

        #endregion

        #region Constructors

        #endregion

        #region Methods


        /// <summary>
        /// Registers a recipient for a type of message T. The action parameter will be executed
        /// when a corresponding message is sent.
        /// </summary>
        /// <typeparam name=""T""></typeparam>
        /// <param name=""recipient""></param>
        /// <param name=""action""></param>
        public void Register<T>(object recipient, Action<T> action)
        {
            Register(recipient, action, typeof(T));
        }
        /// <summary>
        /// Registers a recipient for a type of message T and a matching context. The action parameter will be executed
        /// when a corresponding message is sent.
        /// </summary>
        /// <typeparam name=""T""></typeparam>
        /// <param name=""recipient""></param>
        /// <param name=""action""></param>
        /// <param name=""context""></param>
        public void Register<T>(object recipient, Action<T> action, object context)
        {
            var key = new MessengerKey(recipient, context);
            bool success = Dictionary.TryAdd(key, action);
        }

        /// <summary>
        /// Unregisters a messenger recipient completely. After this method is executed, the recipient will
        /// no longer receive any messages.
        /// </summary>
        /// <param name=""recipient""></param>
        public void Unregister(object recipient)
        {
            Unregister(recipient, null);
        }

        /// <summary>
        /// Unregisters a messenger recipient with a matching context completely. After this method is executed, the recipient will
        /// no longer receive any messages.
        /// </summary>
        /// <param name=""recipient""></param>
        /// <param name=""context""></param>
        public void Unregister(object recipient, object? context)
        {
            object action;
            if (context != null)
            {
                var key = new MessengerKey(recipient, context);
                Dictionary.TryRemove(key, out _);
                //Debug.WriteLine($"action {action} removed");
            }
            else
            {
                var keys = Dictionary.Keys;
                foreach (var key in keys)
                {
                    if (key.Recipient.Equals(recipient))
                    {
                        Dictionary.TryRemove(key, out action);
                        //Debug.WriteLine($"action {action} removed");
                    }
                }
            }


        }

        /// <summary>
        /// Sends a message to registered recipients. The message will reach all recipients that are
        /// registered for this message type.
        /// </summary>
        /// <typeparam name=""T""></typeparam>
        /// <param name=""message""></param>
        public void Send<T>(T message)
        {
            Send(message, typeof(T));
        }

        /// <summary>
        /// Sends a message to registered recipients. The message will reach all recipients that are
        /// registered for this message type and matching context.
        /// </summary>
        /// <typeparam name=""T""></typeparam>
        /// <param name=""message""></param>
        /// <param name=""context""></param>
        public void Send<T>(T message, object context)
        {
            IEnumerable<KeyValuePair<MessengerKey, object>> result;

            if (context == null)
            {
                // Get all recipients where the context is null.
                result = from r in Dictionary where r.Key.Context == null select r;
            }
            else
            {
                // Get all recipients where the context is matching.
                result = from r in Dictionary where r.Key.Context != null && r.Key.Context.Equals(context) select r;
            }

            foreach (Action<T> action in result.Select(x => x.Value).OfType<Action<T>>())
            {
                // Send the message to all recipients.
                action(message);
            }
        }



        #endregion

        protected class MessengerKey
        {
            #region Properties
            public object Recipient { get; private set; }
            public object? Context { get; private set; }
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the MessengerKey class.
            /// </summary>
            /// <param name=""recipient""></param>
            /// <param name=""context""></param>
            public MessengerKey(object recipient, object? context)
            {
                Recipient = recipient;
                Context = context;
            }
            #endregion

            #region Methods
            /// <summary>
            /// Determines whether the specified MessengerKey is equal to the current MessengerKey.
            /// </summary>
            /// <param name=""other""></param>
            /// <returns></returns>
            protected bool Equals(MessengerKey other)
            {
                return Equals(Recipient, other.Recipient) && Equals(Context, other.Context);
            }

            /// <summary>
            /// Determines whether the specified MessengerKey is equal to the current MessengerKey.
            /// </summary>
            /// <param name=""obj""></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;

                return Equals((MessengerKey)obj);
            }

            /// <summary>
            /// Serves as a hash function for a particular type. 
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Recipient != null ? Recipient.GetHashCode() : 0) * 397) ^ (Context != null ? Context.GetHashCode() : 0);
                }
            }


            #endregion
        }
    }
}
