using System;

namespace Toolkit
{
    public interface ICallback
    {
        bool Has<T>() where T : Delegate;
        T Get<T>() where T : Delegate;
        void Add<T>(T callback, int order) where T : Delegate;
        void Remove<T>(T callback) where T : Delegate;
    }

    public static class CallbackUtility
    {
        public static void Add<T>(this ICallback inter, T callback) where T : Delegate
            => inter.Add(callback, InvokeOrder.GetOrder(callback));
    }
}
