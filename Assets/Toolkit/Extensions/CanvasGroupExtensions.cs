using System;
using UnityEngine;

namespace Toolkit
{
    public static class CanvasGroupExtensions
    {
        #region Show / Hide

        public static void Show(this CanvasGroup group) => group.alpha = 1f;

        public static Coroutine Show(this CanvasGroup group, float duration) {
            return Timer.Animate(duration, new TransitionContainer(group, true, null).Update);
        }

        public static Coroutine Show(this CanvasGroup group, float duration, Toolkit.Mathematics.Ease.EaseFunction function) {
            return Timer.Animate(duration, new TransitionContainer(group, true, function).Update);
        }

        public static Coroutine Show(this CanvasGroup group, float duration, Toolkit.Mathematics.Ease.Function function, Toolkit.Mathematics.Ease.Type type) {
            return Timer.Animate(duration, new TransitionContainer(group, true, Toolkit.Mathematics.Ease.GetEaseFunction(function, type)).Update);
        }

        public static Coroutine Show(this CanvasGroup group, float duration, Action onComplete) {
            return Timer.Animate(duration, new TransitionContainer(group, true, null, onComplete).Update);
        }

        public static Coroutine Show(this CanvasGroup group, float duration, Toolkit.Mathematics.Ease.EaseFunction function, Action onComplete) {
            return Timer.Animate(duration, new TransitionContainer(group, true, function, onComplete).Update);
        }

        public static Coroutine Show(this CanvasGroup group, float duration, Toolkit.Mathematics.Ease.Function function, Toolkit.Mathematics.Ease.Type type, Action onComplete) {
            return Timer.Animate(duration, new TransitionContainer(group, true, Toolkit.Mathematics.Ease.GetEaseFunction(function, type), onComplete).Update);
        }

        public static void Show(this CanvasGroup group, float duration, ref Coroutine routine) {
            Timer.Animate(duration, new TransitionContainer(group, true, null).Update, ref routine);
        }

        public static void Show(this CanvasGroup group, float duration, Toolkit.Mathematics.Ease.EaseFunction function, ref Coroutine routine) {
            Timer.Animate(duration, new TransitionContainer(group, true, function).Update, ref routine);
        }

        public static void Show(this CanvasGroup group, float duration, Toolkit.Mathematics.Ease.Function function, Toolkit.Mathematics.Ease.Type type, ref Coroutine routine) {
            Timer.Animate(duration, new TransitionContainer(group, true, Toolkit.Mathematics.Ease.GetEaseFunction(function, type)).Update, ref routine);
        }

        public static void Show(this CanvasGroup group, float duration, Action onComplete, ref Coroutine routine) {
            Timer.Animate(duration, new TransitionContainer(group, true, null, onComplete).Update, ref routine);
        }

        public static void Show(this CanvasGroup group, float duration, Toolkit.Mathematics.Ease.EaseFunction function, Action onComplete, ref Coroutine routine) {
            Timer.Animate(duration, new TransitionContainer(group, true, function, onComplete).Update, ref routine);
        }

        public static void Show(this CanvasGroup group, float duration, Toolkit.Mathematics.Ease.Function function, Toolkit.Mathematics.Ease.Type type, Action onComplete, ref Coroutine routine) {
            Timer.Animate(duration, new TransitionContainer(group, true, Toolkit.Mathematics.Ease.GetEaseFunction(function, type), onComplete).Update, ref routine);
        }

        public static void Hide(this CanvasGroup group) => group.alpha = 0f;

        public static Coroutine Hide(this CanvasGroup group, float duration) {
            return Timer.Animate(duration, new TransitionContainer(group, false, null).Update);
        }

        public static Coroutine Hide(this CanvasGroup group, float duration, Toolkit.Mathematics.Ease.EaseFunction function) {
            return Timer.Animate(duration, new TransitionContainer(group, false, function).Update);
        }

        public static Coroutine Hide(this CanvasGroup group, float duration, Toolkit.Mathematics.Ease.Function function, Toolkit.Mathematics.Ease.Type type) {
            return Timer.Animate(duration, new TransitionContainer(group, false, Toolkit.Mathematics.Ease.GetEaseFunction(function, type)).Update);
        }

        public static Coroutine Hide(this CanvasGroup group, float duration, Action onComplete) {
            return Timer.Animate(duration, new TransitionContainer(group, false, null, onComplete).Update);
        }

        public static Coroutine Hide(this CanvasGroup group, float duration, Toolkit.Mathematics.Ease.EaseFunction function, Action onComplete) {
            return Timer.Animate(duration, new TransitionContainer(group, false, function, onComplete).Update);
        }

        public static Coroutine Hide(this CanvasGroup group, float duration, Toolkit.Mathematics.Ease.Function function, Toolkit.Mathematics.Ease.Type type, Action onComplete) {
            return Timer.Animate(duration, new TransitionContainer(group, false, Toolkit.Mathematics.Ease.GetEaseFunction(function, type), onComplete).Update);
        }

        public static void Hide(this CanvasGroup group, float duration, ref Coroutine routine) {
            Timer.Animate(duration, new TransitionContainer(group, false, null).Update, ref routine);
        }

        public static void Hide(this CanvasGroup group, float duration, Action onComplete, ref Coroutine routine) {
            Timer.Animate(duration, new TransitionContainer(group, false, null, onComplete).Update, ref routine);
        }

        public static void Hide(this CanvasGroup group, float duration, Toolkit.Mathematics.Ease.EaseFunction function, ref Coroutine routine) {
            Timer.Animate(duration, new TransitionContainer(group, false, function).Update, ref routine);
        }

        public static void Hide(this CanvasGroup group, float duration, Toolkit.Mathematics.Ease.EaseFunction function, Action onComplete, ref Coroutine routine) {
            Timer.Animate(duration, new TransitionContainer(group, false, function, onComplete).Update, ref routine);
        }

        public static void Hide(this CanvasGroup group, float duration, Toolkit.Mathematics.Ease.Function function, Toolkit.Mathematics.Ease.Type type, ref Coroutine routine) {
            Timer.Animate(duration, new TransitionContainer(group, false, Toolkit.Mathematics.Ease.GetEaseFunction(function, type)).Update, ref routine);
        }

        public static void Hide(this CanvasGroup group, float duration, Toolkit.Mathematics.Ease.Function function, Toolkit.Mathematics.Ease.Type type, Action onComplete, ref Coroutine routine) {
            Timer.Animate(duration, new TransitionContainer(group, false, Toolkit.Mathematics.Ease.GetEaseFunction(function, type), onComplete).Update, ref routine);
        }

        private class TransitionContainer
        {
            private CanvasGroup group;
            private bool active;
            private Toolkit.Mathematics.Ease.EaseFunction function;
            private Action onComplete;

            public void Update(float t) {
                if(!group)
                    return;
                group.alpha = active ? function(t) : 1f - function(t);
                if(t >= 1f)
                    onComplete?.Invoke();
            }

            public TransitionContainer(CanvasGroup group, bool active, Toolkit.Mathematics.Ease.EaseFunction function) {
                this.group = group;
                this.active = active;
                if(function == null)
                    this.function = Linear;
                else
                    this.function = function;
            }
            public TransitionContainer(CanvasGroup group, bool active, Toolkit.Mathematics.Ease.EaseFunction function, Action onComplete) {
                this.group = group;
                this.active = active;
                if(function == null)
                    this.function = Linear;
                else
                    this.function = function;
                this.onComplete = onComplete;
            }

            private static float Linear(float t) => t;
        }

        #endregion
    }
}
