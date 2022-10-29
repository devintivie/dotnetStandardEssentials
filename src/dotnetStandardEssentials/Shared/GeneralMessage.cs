using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetStandardEssentials
{
    public class GeneralMessage
    {
        #region Fields

        #endregion

        #region Properties
        public string Message { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public GeneralMessageType MessageType { get; set; }
        #endregion

        #region Constructors
        public GeneralMessage() : this("") { }

        public GeneralMessage(string command, GeneralMessageType messageType = GeneralMessageType.Info)
        {
            Message = command;
            MessageType = messageType;
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"{MessageType}: {Message}";
        }

        protected virtual bool Equals(GeneralMessage other)
        {
            bool messageEquals = Equals(Message, other.Message);
            bool typeEquals = Equals(MessageType, other.MessageType);
            return messageEquals && typeEquals;
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

            return Equals((GeneralMessage)obj);

        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Message, MessageType);
        }
        #endregion

    }
}
