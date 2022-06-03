using System;
using System.Collections.Generic;
using System.Text;

namespace dotnetStandardEssentials
{
    public class LogMessage : GeneralMessage
    {
        #region Fields

        #endregion

        #region Properties

        public int MessageId { get; set; }
        public DateTime Timestamp { get; set; }
        #endregion

        #region Constructors
        public LogMessage(string command, GeneralMessageType messageType = GeneralMessageType.Info) : base(command, messageType)
        {
            MessageId = 0;
            Timestamp = DateTime.Now;
        }
        #endregion

        #region Methods
        protected bool Equals(LogMessage other)
        {
            bool timeEquals = Equals(Timestamp, other.Timestamp);
            return timeEquals && base.Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
            { 
                return false; 
            }
            if (ReferenceEquals(this, obj)) 
            { 
                return true; 
            }
            if (obj.GetType() != GetType()) 
            { 
                return false; 
            }

            return Equals((LogMessage)obj);

        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Message, MessageType, Timestamp);
        }
        #endregion

    }
}
