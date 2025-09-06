using System;
using System.Collections;
using UnityEngine;

namespace Toolkit {
    public static class Timer {
        #region Singleton

        private static TimerBehaviour Instance => TimerBehaviour.Instance;

        #endregion

        #region Once

        // ----------- return Coroutine --------------

        public static Coroutine Once(float time, Action action) {
            return Instance.Once(time, action);
        }

        public static Coroutine Once<T0>(float time, Action<T0> action, T0 t0) {
            return Instance.Once(time, action, t0);
        }

        public static Coroutine Once<T0, T1>(float time, Action<T0, T1> action, T0 t0, T1 t1) {
            return Instance.Once(time, action, t0, t1);
        }

        public static Coroutine Once<T0, T1, T2>(float time, Action<T0, T1, T2> action, T0 t0, T1 t1, T2 t2) {
            return Instance.Once(time, action, t0, t1, t2);
        }

        public static Coroutine Once<T0, T1, T2, T3>(float time, Action<T0, T1, T2, T3> action, T0 t0, T1 t1, T2 t2, T3 t3) {
            return Instance.Once(time, action, t0, t1, t2, t3);
        }

        // ----------- ref Coroutine --------------

        public static void Once(float time, Action action, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.Once(time, action);
        }

        public static void Once<T0>(float time, Action<T0> action, T0 t0, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.Once(time, action, t0);
        }

        public static void Once<T0, T1>(float time, Action<T0, T1> action, T0 t0, T1 t1, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.Once(time, action, t0, t1);
        }

        public static void Once<T0, T1, T2>(float time, Action<T0, T1, T2> action, T0 t0, T1 t1, T2 t2, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.Once(time, action, t0, t1, t2);
        }

        public static void Once<T0, T1, T2, T3>(float time, Action<T0, T1, T2, T3> action, T0 t0, T1 t1, T2 t2, T3 t3, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.Once(time, action, t0, t1, t2, t3);
        }

        #endregion

        #region Repeat

        // ----------- return Coroutine --------------

        public static Coroutine Repeat(float stepTime, int itterations, Action action) {
            return Instance.Repeat(stepTime, itterations, action);
        }

        public static Coroutine Repeat<T>(float stepTime, int itterations, Action<T> action, T data) {
            return Instance.Repeat<T>(stepTime, itterations, action, data);
        }

        public static Coroutine Repeat(float stepTime, int itterations, Action<int> action) {
            return Instance.Repeat(stepTime, itterations, action);
        }

        public static Coroutine Repeat<T>(float stepTime, int itterations, Action<int, T> action, T data) {
            return Instance.Repeat<T>(stepTime, itterations, action, data);
        }

        // ----------- ref Coroutine --------------

        public static void Repeat(float stepTime, int itterations, Action action, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.Repeat(stepTime, itterations, action);
        }

        public static void Repeat<T>(float stepTime, int itterations, Action<T> action, T data, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.Repeat<T>(stepTime, itterations, action, data);
        }

        public static void Repeat(float stepTime, int itterations, Action<int> action, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.Repeat(stepTime, itterations, action);
        }

        public static void Repeat<T>(float stepTime, int itterations, Action<int, T> action, T data, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.Repeat<T>(stepTime, itterations, action, data);
        }

        #endregion

        #region RepeatAfter

        // ----------- return Coroutine --------------

        public static Coroutine RepeatAfter(float stepTime, int itterations, Action action) {
            return Instance.RepeatAfter(stepTime, itterations, action);
        }

        public static Coroutine RepeatAfter<T>(float stepTime, int itterations, Action<T> action, T data) {
            return Instance.RepeatAfter<T>(stepTime, itterations, action, data);
        }

        public static Coroutine RepeatAfter(float stepTime, int itterations, Action<int> action) {
            return Instance.RepeatAfter(stepTime, itterations, action);
        }

        public static Coroutine RepeatAfter<T>(float stepTime, int itterations, Action<int, T> action, T data) {
            return Instance.RepeatAfter<T>(stepTime, itterations, action, data);
        }

        // ----------- ref Coroutine --------------

        public static void RepeatAfter(float stepTime, int itterations, Action action, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.RepeatAfter(stepTime, itterations, action);
        }

        public static void RepeatAfter<T>(float stepTime, int itterations, Action<T> action, T data, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.RepeatAfter<T>(stepTime, itterations, action, data);
        }

        public static void RepeatAfter(float stepTime, int itterations, Action<int> action, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.RepeatAfter(stepTime, itterations, action);
        }

        public static void RepeatAfter<T>(float stepTime, int itterations, Action<int, T> action, T data, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.RepeatAfter<T>(stepTime, itterations, action, data);
        }

        #endregion

        #region Animate

        // ------------- return Coroutine --------------

        public static Coroutine Animate(float duration, Action<float> operation) {
            return Instance.Animate(duration, operation);
        }

        public static Coroutine Animate<T>(float duration, Action<float, T> operation, T animationData) {
            return Instance.Animate<T>(duration, operation, animationData);
        }

        public static Coroutine Animate(float duration, Action<float> operation, Action onDone) {
            return Instance.Animate(duration, operation, onDone);
        }

        public static Coroutine Animate<T>(float duration, Action<float, T> operation, T animationData, Action onDone) {
            return Instance.Animate<T>(duration, operation, animationData, onDone);
        }

        public static Coroutine Animate<T>(float duration, Action<float, T> operation, T animationData, Action<T> onDone) {
            return Instance.Animate<T>(duration, operation, animationData, onDone);
        }

        // ------------- ref Coroutine --------------

        public static void Animate(float duration, Action<float> operation, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.Animate(duration, operation);
        }

        public static void Animate<T>(float duration, Action<float, T> operation, T animationData, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.Animate<T>(duration, operation, animationData);
        }

        public static void Animate(float duration, Action<float> operation, Action onDone, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.Animate(duration, operation, onDone);
        }

        public static void Animate<T>(float duration, Action<float, T> operation, T animationData, Action onDone, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.Animate<T>(duration, operation, animationData, onDone);
        }

        public static void Animate<T>(float duration, Action<float, T> operation, T animationData, Action<T> onDone, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.Animate<T>(duration, operation, animationData, onDone);
        }

        // ------------- return Coroutine ease function --------------

        public static Coroutine Animate(float duration, Action<float> operation, Func<float, float> easeFunction) {
            return Instance.Animate(duration, operation, easeFunction);
        }

        public static Coroutine Animate<T>(float duration, Action<float, T> operation, T animationData, Func<float, float> easeFunction) {
            return Instance.Animate<T>(duration, operation, animationData, easeFunction);
        }

        public static Coroutine Animate(float duration, Action<float> operation, Func<float, float> easeFunction, Action onDone) {
            return Instance.Animate(duration, operation, easeFunction, onDone);
        }

        public static Coroutine Animate<T>(float duration, Action<float, T> operation, T animationData, Func<float, float> easeFunction, Action onDone) {
            return Instance.Animate<T>(duration, operation, animationData, easeFunction, onDone);
        }

        public static Coroutine Animate<T>(float duration, Action<float, T> operation, T animationData, Func<float, float> easeFunction, Action<T> onDone) {
            return Instance.Animate<T>(duration, operation, animationData, easeFunction, onDone);
        }

        // ------------- ref Coroutine ease function --------------

        public static void Animate(float duration, Action<float> operation, Func<float, float> easeFunction, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.Animate(duration, operation, easeFunction);
        }

        public static void Animate<T>(float duration, Action<float, T> operation, T animationData, Func<float, float> easeFunction, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.Animate<T>(duration, operation, animationData, easeFunction);
        }

        public static void Animate(float duration, Action<float> operation, Func<float, float> easeFunction, Action onDone, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.Animate(duration, operation, easeFunction, onDone);
        }

        public static void Animate<T>(float duration, Action<float, T> operation, T animationData, Func<float, float> easeFunction, Action onDone, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.Animate<T>(duration, operation, animationData, easeFunction, onDone);
        }

        public static void Animate<T>(float duration, Action<float, T> operation, T animationData, Func<float, float> easeFunction, Action<T> onDone, ref Coroutine coroutine) {
            Stop(coroutine);
            coroutine = Instance.Animate<T>(duration, operation, animationData, easeFunction, onDone);
        }

        #endregion

        #region Frameskip

        public static Coroutine NextFrame(Action method) {
            return Instance.NextFrame(method);
        }

        public static void NextFrame(Action method, ref Coroutine routine) {
            Stop(routine);
            routine = Instance.NextFrame(method);
        }

        public static Coroutine NextFrame(Action method, int frames) {
            return Instance.NextFrame(method, frames);
        }

        public static void NextFrame(Action method, int frames, ref Coroutine routine) {
            Stop(routine);
            routine = Instance.NextFrame(method, frames);
        }

        #endregion

        #region WaitUntil

        public static Coroutine WaitUntil(System.Func<bool> wait, Action method) {
            return Instance.WaitUntil(wait, method);
        }

        public static void WaitUntil(System.Func<bool> wait, Action method, ref Coroutine routine) {
            Stop(routine);
            routine = Instance.WaitUntil(wait, method);
        }

        /// <summary>
        /// Makes the callback be on same thread
        /// </summary>
        public static async void WaitUntil<T>(System.Threading.Tasks.Task<T> task, Action method) {
            await task;
            method();
        }

        /// <summary>
        /// Makes the callback be on same thread
        /// </summary>
        public static async void WaitUntil(System.Threading.Tasks.Task task, Action method) {
            await task;
            method();
        }

        #endregion

        #region Stop

        public static void Stop(Coroutine coroutine) {
            Instance.Stop(coroutine);
        }

        public static void StopAll() {
            Instance.StopAllCoroutines();
        }

        #endregion
    }

    internal class TimerBehaviour : MonoSingleton<TimerBehaviour> {
        protected override bool KeepAlive { get => true; set => base.KeepAlive = value; }

        #region Once

        public Coroutine Once(float time, Action action) {
            return StartCoroutine(EOnce(time, action));
        }

        public Coroutine Once<T0>(float time, Action<T0> action, T0 t0) {
            return StartCoroutine(EOnce(time, action, t0));
        }

        public Coroutine Once<T0, T1>(float time, Action<T0, T1> action, T0 t0, T1 t1) {
            return StartCoroutine(EOnce(time, action, t0, t1));
        }

        public Coroutine Once<T0, T1, T2>(float time, Action<T0, T1, T2> action, T0 t0, T1 t1, T2 t2) {
            return StartCoroutine(EOnce(time, action, t0, t1, t2));
        }

        public Coroutine Once<T0, T1, T2, T3>(float time, Action<T0, T1, T2, T3> action, T0 t0, T1 t1, T2 t2, T3 t3) {
            return StartCoroutine(EOnce(time, action, t0, t1, t2, t3));
        }

        IEnumerator EOnce(float time, Action action) {
            yield return new WaitForSeconds(time);
            action();
        }

        IEnumerator EOnce<T>(float time, Action<T> action, T value) {
            yield return new WaitForSeconds(time);
            action(value);
        }

        IEnumerator EOnce<T0, T1>(float time, Action<T0, T1> action, T0 value0, T1 value1) {
            yield return new WaitForSeconds(time);
            action(value0, value1);
        }

        IEnumerator EOnce<T0, T1, T2>(float time, Action<T0, T1, T2> action, T0 value0, T1 value1, T2 value2) {
            yield return new WaitForSeconds(time);
            action(value0, value1, value2);
        }

        IEnumerator EOnce<T0, T1, T2, T3>(float time, Action<T0, T1, T2, T3> action, T0 value0, T1 value1, T2 value2, T3 value3) {
            yield return new WaitForSeconds(time);
            action(value0, value1, value2, value3);
        }

        #endregion

        #region Repeat

        public Coroutine Repeat(float stepTime, int itterations, Action action) {
            return StartCoroutine(ERepeat(stepTime, itterations, action));
        }

        public Coroutine Repeat<T>(float stepTime, int itterations, Action<T> action, T data) {
            return StartCoroutine(ERepeat(stepTime, itterations, action, data));
        }

        public Coroutine Repeat(float stepTime, int itterations, Action<int> action) {
            return StartCoroutine(ERepeat(stepTime, itterations, action));
        }

        public Coroutine Repeat<T>(float stepTime, int itterations, Action<int, T> action, T data) {
            return StartCoroutine(ERepeat(stepTime, itterations, action, data));
        }

        IEnumerator ERepeat(float stepTime, int itterations, Action action) {
            var time = new WaitForSeconds(stepTime);
            for(int i = 0; i < itterations; i++) {
                yield return time;
                action();
            }
        }

        IEnumerator ERepeat<T>(float stepTime, int itterations, Action<T> action, T data) {
            var time = new WaitForSeconds(stepTime);
            for(int i = 0; i < itterations; i++) {
                yield return time;
                action(data);
            }
        }

        IEnumerator ERepeat(float stepTime, int itterations, Action<int> action) {
            var time = new WaitForSeconds(stepTime);
            for(int i = 0; i < itterations; i++) {
                yield return time;
                action(i);
            }
        }

        IEnumerator ERepeat<T>(float stepTime, int itterations, Action<int, T> action, T data) {
            var time = new WaitForSeconds(stepTime);
            for(int i = 0; i < itterations; i++) {
                yield return time;
                action(i, data);
            }
        }

        #endregion

        #region RepeatAfter

        public Coroutine RepeatAfter(float stepTime, int itterations, Action action) {
            return StartCoroutine(ERepeatAfter(stepTime, itterations, action));
        }

        public Coroutine RepeatAfter<T>(float stepTime, int itterations, Action<T> action, T data) {
            return StartCoroutine(ERepeatAfter(stepTime, itterations, action, data));
        }

        public Coroutine RepeatAfter(float stepTime, int itterations, Action<int> action) {
            return StartCoroutine(ERepeatAfter(stepTime, itterations, action));
        }

        public Coroutine RepeatAfter<T>(float stepTime, int itterations, Action<int, T> action, T data) {
            return StartCoroutine(ERepeatAfter(stepTime, itterations, action, data));
        }

        IEnumerator ERepeatAfter(float stepTime, int itterations, Action action) {
            var time = new WaitForSeconds(stepTime);
            for(int i = 0; i < itterations; i++) {
                action();
                yield return time;
            }
        }

        IEnumerator ERepeatAfter<T>(float stepTime, int itterations, Action<T> action, T data) {
            var time = new WaitForSeconds(stepTime);
            for(int i = 0; i < itterations; i++) {
                action(data);
                yield return time;
            }
        }

        IEnumerator ERepeatAfter(float stepTime, int itterations, Action<int> action) {
            var time = new WaitForSeconds(stepTime);
            for(int i = 0; i < itterations; i++) {
                action(i);
                yield return time;
            }
        }

        IEnumerator ERepeatAfter<T>(float stepTime, int itterations, Action<int, T> action, T data) {
            var time = new WaitForSeconds(stepTime);
            for(int i = 0; i < itterations; i++) {
                action(i, data);
                yield return time;
            }
        }

        #endregion

        #region Animate

        public Coroutine Animate(float duration, Action<float> operation) {
            return StartCoroutine(EAnimate(duration, operation));
        }

        public Coroutine Animate<T>(float duration, Action<float, T> operation, T animationData) {
            return StartCoroutine(EAnimate<T>(duration, operation, animationData));
        }

        public Coroutine Animate(float duration, Action<float> operation, Action onDone) {
            return StartCoroutine(EAnimate(duration, operation, onDone));
        }

        public Coroutine Animate<T>(float duration, Action<float, T> operation, T animationData, Action onDone) {
            return StartCoroutine(EAnimate<T>(duration, operation, animationData, onDone));
        }

        public Coroutine Animate<T>(float duration, Action<float, T> operation, T animationData, Action<T> onDone) {
            return StartCoroutine(EAnimate<T>(duration, operation, animationData, onDone));
        }

        // ------------- ease functions

        public Coroutine Animate(float duration, Action<float> operation, Func<float, float> easeFunction) {
            return StartCoroutine(EAnimate(duration, operation, easeFunction));
        }

        public Coroutine Animate<T>(float duration, Action<float, T> operation, T animationData, Func<float, float> easeFunction) {
            return StartCoroutine(EAnimate<T>(duration, operation, animationData, easeFunction));
        }

        public Coroutine Animate(float duration, Action<float> operation, Func<float, float> easeFunction, Action onDone) {
            return StartCoroutine(EAnimate(duration, operation, easeFunction, onDone));
        }

        public Coroutine Animate<T>(float duration, Action<float, T> operation, T animationData, Func<float, float> easeFunction, Action onDone) {
            return StartCoroutine(EAnimate<T>(duration, operation, animationData, easeFunction, onDone));
        }

        public Coroutine Animate<T>(float duration, Action<float, T> operation, T animationData, Func<float, float> easeFunction, Action<T> onDone) {
            return StartCoroutine(EAnimate<T>(duration, operation, animationData, easeFunction, onDone));
        }

        IEnumerator EAnimate(float duration, Action<float> operation) {
            var timer = 0f;
            operation(timer);
            while(timer < 1f) {
                yield return null;
                timer += Time.unscaledDeltaTime / duration;
                operation(System.Math.Min(timer, 1f));
            }
        }

        IEnumerator EAnimate<T>(float duration, Action<float, T> operation, T animationData) {
            var timer = 0f;
            operation(timer, animationData);
            while(timer < 1f) {
                yield return null;
                timer += Time.unscaledDeltaTime / duration;
                operation(System.Math.Min(timer, 1f), animationData);
            }
        }

        IEnumerator EAnimate(float duration, Action<float> operation, Action onDone) {
            var timer = 0f;
            operation(timer);
            while(timer < 1f) {
                yield return null;
                timer += Time.unscaledDeltaTime / duration;
                operation(System.Math.Min(timer, 1f));
            }
            onDone();
        }

        IEnumerator EAnimate<T>(float duration, Action<float, T> operation, T animationData, Action onDone) {
            var timer = 0f;
            operation(timer, animationData);
            while(timer < 1f) {
                yield return null;
                timer += Time.unscaledDeltaTime / duration;
                operation(System.Math.Min(timer, 1f), animationData);
            }
            onDone();
        }

        IEnumerator EAnimate<T>(float duration, Action<float, T> operation, T animationData, Action<T> onDone) {
            var timer = 0f;
            operation(timer, animationData);
            while(timer < 1f) {
                yield return null;
                timer += Time.unscaledDeltaTime / duration;
                operation(System.Math.Min(timer, 1f), animationData);
            }
            onDone(animationData);
        }

        //----------- Ease Functions

        IEnumerator EAnimate(float duration, Action<float> operation, Func<float, float> easeFunction) {
            var timer = 0f;
            operation(easeFunction(timer));
            while(timer < 1f) {
                yield return null;
                timer += Time.unscaledDeltaTime / duration;
                operation(easeFunction(System.Math.Min(timer, 1f)));
            }
        }

        IEnumerator EAnimate<T>(float duration, Action<float, T> operation, T animationData, Func<float, float> easeFunction) {
            var timer = 0f;
            operation(easeFunction(timer), animationData);
            while(timer < 1f) {
                yield return null;
                timer += Time.unscaledDeltaTime / duration;
                operation(easeFunction(System.Math.Min(timer, 1f)), animationData);
            }
        }

        IEnumerator EAnimate(float duration, Action<float> operation, Func<float, float> easeFunction, Action onDone) {
            var timer = 0f;
            operation(easeFunction(timer));
            while(timer < 1f) {
                yield return null;
                timer += Time.unscaledDeltaTime / duration;
                operation(easeFunction(System.Math.Min(timer, 1f)));
            }
            onDone();
        }

        IEnumerator EAnimate<T>(float duration, Action<float, T> operation, T animationData, Func<float, float> easeFunction, Action onDone) {
            var timer = 0f;
            operation(easeFunction(timer), animationData);
            while(timer < 1f) {
                yield return null;
                timer += Time.unscaledDeltaTime / duration;
                operation(easeFunction(System.Math.Min(timer, 1f)), animationData);
            }
            onDone();
        }

        IEnumerator EAnimate<T>(float duration, Action<float, T> operation, T animationData, Func<float, float> easeFunction, Action<T> onDone) {
            var timer = 0f;
            operation(easeFunction(timer), animationData);
            while(timer < 1f) {
                yield return null;
                timer += Time.unscaledDeltaTime / duration;
                operation(easeFunction(System.Math.Min(timer, 1f)), animationData);
            }
            onDone(animationData);
        }

        #endregion

        #region Frameskip

        public Coroutine NextFrame(Action method) {
            return StartCoroutine(ENextFrame(method));
        }

        public Coroutine NextFrame(Action method, int frames) {
            return StartCoroutine(ENextFrame(method, frames));
        }

        IEnumerator ENextFrame(Action method) {
            yield return 0;
            method();
        }

        IEnumerator ENextFrame(Action method, int frames) {
            for(int i = 0; i < frames; i++) {
                yield return 0;
            }
            method();
        }

        #endregion

        #region WaitUntil

        public Coroutine WaitUntil(Func<bool> wait, Action method) {
            return StartCoroutine(EWaitUntil(wait, method));
        }

        IEnumerator EWaitUntil(Func<bool> wait, Action method) {
            yield return new WaitUntil(wait);
            method();
        }

        #endregion

        #region Stop

        public void Stop(Coroutine coroutine) {
            if(coroutine != null)
                StopCoroutine(coroutine);
        }

        #endregion
    }
}
