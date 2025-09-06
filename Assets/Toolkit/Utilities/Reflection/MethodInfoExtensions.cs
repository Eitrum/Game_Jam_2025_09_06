using System;
using System.Reflection;
using UnityEngine;

namespace Toolkit {
    public static class MethodInfoExtensions {
        #region Variables

        private static FastPoolArray<object> cached1 = new FastPoolArray<object>(1, 8);
        private static FastPoolArray<object> cached2 = new FastPoolArray<object>(2, 8);
        private static FastPoolArray<object> cached3 = new FastPoolArray<object>(3, 8);
        private static FastPoolArray<object> cached4 = new FastPoolArray<object>(4, 8);

        private static FastPoolArray<object> cached5 = new FastPoolArray<object>(5, 8);
        private static FastPoolArray<object> cached6 = new FastPoolArray<object>(6, 8);
        private static FastPoolArray<object> cached7 = new FastPoolArray<object>(7, 8);
        private static FastPoolArray<object> cached8 = new FastPoolArray<object>(8, 8);

        #endregion

        #region Invoke Static

        public static void InvokeStatic(this MethodInfo method) {
            try {
                var obj = method?.Invoke(null, null);
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        public static void InvokeStatic<T>(this MethodInfo method, T item) {
            try {
                using(var s = cached1.GetScope()) {
                    s.Value[0] = item;
                    var obj = method?.Invoke(null, s.Value);
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        public static void InvokeStatic<T0, T1>(this MethodInfo method, T0 item0, T1 item1) {
            try {
                using(var s = cached2.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    var obj = method?.Invoke(null, s.Value);
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        public static void InvokeStatic<T0, T1, T2>(this MethodInfo method, T0 item0, T1 item1, T2 item2) {
            try {
                using(var s = cached3.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    var obj = method?.Invoke(null, s.Value);
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        public static void InvokeStatic<T0, T1, T2, T3>(this MethodInfo method, T0 item0, T1 item1, T2 item2, T3 item3) {
            try {
                using(var s = cached4.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    var obj = method?.Invoke(null, s.Value);
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        public static void InvokeStatic<T0, T1, T2, T3, T4>(this MethodInfo method, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4) {
            try {
                using(var s = cached5.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    s.Value[4] = item4;
                    var obj = method?.Invoke(null, s.Value);
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        public static void InvokeStatic<T0, T1, T2, T3, T4, T5>(this MethodInfo method, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) {
            try {
                using(var s = cached6.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    s.Value[4] = item4;
                    s.Value[5] = item5;
                    var obj = method?.Invoke(null, s.Value);
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        public static void InvokeStatic<T0, T1, T2, T3, T4, T5, T6>(this MethodInfo method, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) {
            try {
                using(var s = cached7.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    s.Value[4] = item4;
                    s.Value[5] = item5;
                    s.Value[6] = item6;
                    var obj = method?.Invoke(null, s.Value);
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        public static void InvokeStatic<T0, T1, T2, T3, T4, T5, T6, T7>(this MethodInfo method, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) {
            try {
                using(var s = cached8.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    s.Value[4] = item4;
                    s.Value[5] = item5;
                    s.Value[6] = item6;
                    s.Value[7] = item7;
                    var obj = method?.Invoke(null, s.Value);
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        #endregion

        #region Invoke Static (Return)

        public static TReturn InvokeStatic<TReturn>(this MethodInfo method) {
            try {
                var obj = method?.Invoke(null, null);
                if(obj is TReturn tret)
                    return tret;
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            return default(TReturn);
        }

        public static TReturn InvokeStatic<T, TReturn>(this MethodInfo method, T item) {
            try {
                using(var s = cached1.GetScope()) {
                    s.Value[0] = item;
                    var obj = method?.Invoke(null, s.Value);
                    if(obj is TReturn tret)
                        return tret;
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            return default(TReturn);
        }

        public static TReturn InvokeStatic<T0, T1, TReturn>(this MethodInfo method, T0 item0, T1 item1) {
            try {
                using(var s = cached2.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    var obj = method?.Invoke(null, s.Value);
                    if(obj is TReturn tret)
                        return tret;
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            return default(TReturn);
        }

        public static TReturn InvokeStaticTryGet<TIn, TOut, TReturn>(this MethodInfo method, TIn item0, out TOut item1) {
            try {
                using(var s = cached2.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = default(TOut);
                    var obj = method?.Invoke(null, s.Value);
                    item1 = (TOut)s.Value[1];
                    if(obj is TReturn tret)
                        return tret;
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            item1 = default(TOut);
            return default(TReturn);
        }

        public static TReturn InvokeStatic<T0, T1, T2, TReturn>(this MethodInfo method, T0 item0, T1 item1, T2 item2) {
            try {
                using(var s = cached3.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    var obj = method?.Invoke(null, s.Value);
                    if(obj is TReturn tret)
                        return tret;
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            return default(TReturn);
        }

        public static TReturn InvokeStatic<T0, T1, T2, T3, TReturn>(this MethodInfo method, T0 item0, T1 item1, T2 item2, T3 item3) {
            try {
                using(var s = cached4.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    var obj = method?.Invoke(null, s.Value);
                    if(obj is TReturn tret)
                        return tret;
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            return default(TReturn);
        }

        public static TReturn InvokeStatic<T0, T1, T2, T3, T4, TReturn>(this MethodInfo method, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4) {
            try {
                using(var s = cached5.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    s.Value[4] = item4;
                    var obj = method?.Invoke(null, s.Value);
                    if(obj is TReturn tret)
                        return tret;
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            return default(TReturn);
        }

        public static TReturn InvokeStatic<T0, T1, T2, T3, T4, T5, TReturn>(this MethodInfo method, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) {
            try {
                using(var s = cached6.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    s.Value[4] = item4;
                    s.Value[5] = item5;
                    var obj = method?.Invoke(null, s.Value);
                    if(obj is TReturn tret)
                        return tret;
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            return default(TReturn);
        }

        public static TReturn InvokeStatic<T0, T1, T2, T3, T4, T5, T6, TReturn>(this MethodInfo method, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) {
            try {
                using(var s = cached7.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    s.Value[4] = item4;
                    s.Value[5] = item5;
                    s.Value[6] = item6;
                    var obj = method?.Invoke(null, s.Value);
                    if(obj is TReturn tret)
                        return tret;
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            return default(TReturn);
        }

        public static TReturn InvokeStatic<T0, T1, T2, T3, T4, T5, T6, T7, TReturn>(this MethodInfo method, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) {
            try {
                using(var s = cached8.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    s.Value[4] = item4;
                    s.Value[5] = item5;
                    s.Value[6] = item6;
                    s.Value[7] = item7;
                    var obj = method?.Invoke(null, s.Value);
                    if(obj is TReturn tret)
                        return tret;
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            return default(TReturn);
        }

        #endregion


        #region Invoke On Instance

        public static void InvokeOnInstance<TInstance>(this MethodInfo method, TInstance instance) {
            try {
                var obj = method?.Invoke(instance, null);
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        public static void InvokeOnInstance<TInstance, T>(this MethodInfo method, TInstance instance, T item) {
            try {
                using(var s = cached1.GetScope()) {
                    s.Value[0] = item;
                    var obj = method?.Invoke(instance, s.Value);
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        public static void InvokeOnInstance<TInstance, T0, T1>(this MethodInfo method, TInstance instance, T0 item0, T1 item1) {
            try {
                using(var s = cached2.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    var obj = method?.Invoke(instance, s.Value);
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        public static void InvokeOnInstance<TInstance, T0, T1, T2>(this MethodInfo method, TInstance instance, T0 item0, T1 item1, T2 item2) {
            try {
                using(var s = cached3.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    var obj = method?.Invoke(instance, s.Value);
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        public static void InvokeOnInstance<TInstance, T0, T1, T2, T3>(this MethodInfo method, TInstance instance, T0 item0, T1 item1, T2 item2, T3 item3) {
            try {
                using(var s = cached4.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    var obj = method?.Invoke(instance, s.Value);
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        public static void InvokeOnInstance<TInstance, T0, T1, T2, T3, T4>(this MethodInfo method, TInstance instance, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4) {
            try {
                using(var s = cached5.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    s.Value[4] = item4;
                    var obj = method?.Invoke(instance, s.Value);
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        public static void InvokeOnInstance<TInstance, T0, T1, T2, T3, T4, T5>(this MethodInfo method, TInstance instance, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) {
            try {
                using(var s = cached6.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    s.Value[4] = item4;
                    s.Value[5] = item5;
                    var obj = method?.Invoke(instance, s.Value);
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        public static void InvokeOnInstance<TInstance, T0, T1, T2, T3, T4, T5, T6>(this MethodInfo method, TInstance instance, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) {
            try {
                using(var s = cached7.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    s.Value[4] = item4;
                    s.Value[5] = item5;
                    s.Value[6] = item6;
                    var obj = method?.Invoke(instance, s.Value);
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        public static void InvokeOnInstance<TInstance, T0, T1, T2, T3, T4, T5, T6, T7>(this MethodInfo method, TInstance instance, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) {
            try {
                using(var s = cached8.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    s.Value[4] = item4;
                    s.Value[5] = item5;
                    s.Value[6] = item6;
                    s.Value[7] = item7;
                    var obj = method?.Invoke(instance, s.Value);
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        #endregion

        #region Invoke On Instance (Return)

        public static TReturn InvokeOnInstance<TInstance, TReturn>(this MethodInfo method, TInstance instance) {
            try {
                var obj = method?.Invoke(instance, null);
                if(obj is TReturn tret)
                    return tret;
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            return default(TReturn);
        }

        public static TReturn InvokeOnInstance<TInstance, T, TReturn>(this MethodInfo method, TInstance instance, T item) {
            try {
                using(var s = cached1.GetScope()) {
                    s.Value[0] = item;
                    var obj = method?.Invoke(instance, s.Value);
                    if(obj is TReturn tret)
                        return tret;
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            return default(TReturn);
        }

        public static TReturn InvokeOnInstance<TInstance, T0, T1, TReturn>(this MethodInfo method, TInstance instance, T0 item0, T1 item1) {
            try {
                using(var s = cached2.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    var obj = method?.Invoke(instance, s.Value);
                    if(obj is TReturn tret)
                        return tret;
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            return default(TReturn);
        }

        public static TReturn InvokeTryGetOnInstance<TInstance, TIn, TOut, TReturn>(this MethodInfo method, TInstance instance, TIn item0, out TOut item1) {
            try {
                using(var s = cached2.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = default(TOut);
                    var obj = method?.Invoke(instance, s.Value);
                    item1 = (TOut)s.Value[1];
                    if(obj is TReturn tret)
                        return tret;
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            item1 = default(TOut);
            return default(TReturn);
        }

        public static TReturn InvokeOnInstance<TInstance, T0, T1, T2, TReturn>(this MethodInfo method, TInstance instance, T0 item0, T1 item1, T2 item2) {
            try {
                using(var s = cached3.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    var obj = method?.Invoke(instance, s.Value);
                    if(obj is TReturn tret)
                        return tret;
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            return default(TReturn);
        }

        public static TReturn InvokeOnInstance<TInstance, T0, T1, T2, T3, TReturn>(this MethodInfo method, TInstance instance, T0 item0, T1 item1, T2 item2, T3 item3) {
            try {
                using(var s = cached4.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    var obj = method?.Invoke(instance, s.Value);
                    if(obj is TReturn tret)
                        return tret;
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            return default(TReturn);
        }

        public static TReturn InvokeOnInstance<TInstance, T0, T1, T2, T3, T4, TReturn>(this MethodInfo method, TInstance instance, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4) {
            try {
                using(var s = cached5.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    s.Value[4] = item4;
                    var obj = method?.Invoke(instance, s.Value);
                    if(obj is TReturn tret)
                        return tret;
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            return default(TReturn);
        }

        public static TReturn InvokeOnInstance<TInstance, T0, T1, T2, T3, T4, T5, TReturn>(this MethodInfo method, TInstance instance, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) {
            try {
                using(var s = cached6.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    s.Value[4] = item4;
                    s.Value[5] = item5;
                    var obj = method?.Invoke(instance, s.Value);
                    if(obj is TReturn tret)
                        return tret;
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            return default(TReturn);
        }

        public static TReturn InvokeOnInstance<TInstance, T0, T1, T2, T3, T4, T5, T6, TReturn>(this MethodInfo method, TInstance instance, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) {
            try {
                using(var s = cached7.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    s.Value[4] = item4;
                    s.Value[5] = item5;
                    s.Value[6] = item6;
                    var obj = method?.Invoke(instance, s.Value);
                    if(obj is TReturn tret)
                        return tret;
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            return default(TReturn);
        }

        public static TReturn InvokeOnInstance<TInstance, T0, T1, T2, T3, T4, T5, T6, T7, TReturn>(this MethodInfo method, TInstance instance, T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) {
            try {
                using(var s = cached8.GetScope()) {
                    s.Value[0] = item0;
                    s.Value[1] = item1;
                    s.Value[2] = item2;
                    s.Value[3] = item3;
                    s.Value[4] = item4;
                    s.Value[5] = item5;
                    s.Value[6] = item6;
                    s.Value[7] = item7;
                    var obj = method?.Invoke(instance, s.Value);
                    if(obj is TReturn tret)
                        return tret;
                }
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
            return default(TReturn);
        }

        #endregion
    }
}
