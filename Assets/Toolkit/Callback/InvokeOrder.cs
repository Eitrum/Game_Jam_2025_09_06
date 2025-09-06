using System;
using System.Reflection;

namespace Toolkit
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InvokeOrder : Attribute
    {
        public int order = 0;
        public InvokeOrder(int order) => this.order = order;

        public static int GetOrder<T>(T callback) where T : Delegate {
            return GetOrder(callback.Method);
        }

        public static int GetOrder(MethodInfo methodInfo) {
            return methodInfo?.GetCustomAttribute<InvokeOrder>()?.order ?? 0;
        }
    }
}
