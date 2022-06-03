using System;
using System.Collections.Generic;
using System.Text;

namespace dotnetStandardEssentials
{
    public interface IMessenger
    {
        void Register<T>(object recipient, Action<T> action);
        void Register<T>(object recipient, Action<T> action, object context);
        void Send<T>(T message);
        void Send<T>(T message, object context);
        void Unregister(object recipient);
        void Unregister(object recipient, object context);
    }
}
