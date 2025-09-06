using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Toolkit {
    public interface IPromise {
        Promise.State CurrentState { get; }
        void Complete();
        void Error();
        void Error(string msg);
        void Cancel();
        void Timeout();
    }

    public interface IPromise<T> : IPromise {
        void Complete(T result);
    }

    public sealed class Promise : IPromise, INotifyCompletion, ICriticalNotifyCompletion {

        public delegate void OnCompleteDelegate();
        public delegate void OnErrorDelegate(string error);

        public enum State {
            None,
            Success,
            Cancelled,
            Timeout,
            Error,
        }

        public struct Result {
            #region Variables

            private Promise promise;

            #endregion

            #region Properties

            public Promise.State State => promise?.state ?? Promise.State.None;

            public bool IsCompleted => State != Promise.State.None;
            public bool IsSuccess => State == Promise.State.Success;
            public bool IsError => Promise.IsError(State);

            public string Error => promise?.ErrorMessage ?? string.Empty;

            #endregion

            #region Constructor

            public Result(Promise promise) { this.promise = promise; }

            #endregion

            #region Operators

            public static implicit operator Promise.State(Result result) => result.State;

            #endregion
        }

        #region Variables

        private const string TAG = "[Promise] - ";
        public const int DEFAULT_TIMEOUT_IN_SECONDS = 5;

        private State state = State.None;
        private string errorMessage;
        private Coroutine routine;

        private OnCompleteDelegate onComplete;
        private OnErrorDelegate onError;

        private Action asyncOperation;

        private long initialized;
        private long completed;
        private System.Threading.CancellationTokenSource threadedCancellationSource;
        private bool isThreadedOrEditor;

        #endregion

        #region Properties

        public string ErrorMessage => errorMessage;

        public bool IsCompleted { get { return state != State.None; } }
        public State CurrentState => state;
        public Result GetResult() { return new Result(this); }

        public event OnCompleteDelegate OnComplete{
            add {
                onComplete += value;
                if(IsCompleted && IsSuccess(state))
                    value?.Invoke();
            }
            remove => onComplete -= value;
        }

        public event OnErrorDelegate OnError{
            add {
                onError += value;
                if(IsCompleted && IsError(state))
                    value?.Invoke(errorMessage);
            }
            remove => onError -= value;
        }

        public long ElapsedTicks => completed - initialized;
        public TimeSpan ElapsedTime => PerformanceUtility.GetElapsedTimeSpan(initialized, completed);

        #endregion

        #region Constructor

        public Promise() {
            initialized = System.Diagnostics.Stopwatch.GetTimestamp();
            SetTimeout(DEFAULT_TIMEOUT_IN_SECONDS);
        }

        public Promise(bool useTimeout) {
            initialized = System.Diagnostics.Stopwatch.GetTimestamp();
            if(useTimeout)
                SetTimeout(DEFAULT_TIMEOUT_IN_SECONDS);
        }

        public Promise(OnCompleteDelegate onComplete, OnErrorDelegate onError) {
            initialized = System.Diagnostics.Stopwatch.GetTimestamp();
            this.onComplete = onComplete;
            this.onError = onError;
            SetTimeout(DEFAULT_TIMEOUT_IN_SECONDS);
        }

        public Promise(OnCompleteDelegate onComplete, OnErrorDelegate onError, bool useTimeout) {
            initialized = System.Diagnostics.Stopwatch.GetTimestamp();
            this.onComplete = onComplete;
            this.onError = onError;
            if(useTimeout)
                SetTimeout(DEFAULT_TIMEOUT_IN_SECONDS);
        }

        public Promise(float timeout) {
            initialized = System.Diagnostics.Stopwatch.GetTimestamp();
            SetTimeout(timeout);
        }

        public Promise(OnCompleteDelegate onComplete, OnErrorDelegate onError, float timeout) {
            initialized = System.Diagnostics.Stopwatch.GetTimestamp();
            this.onComplete = onComplete;
            this.onError = onError;
            SetTimeout(timeout);
        }

        public Promise(OnCompleteDelegate onComplete, OnErrorDelegate onError, System.TimeSpan timeout) {
            initialized = System.Diagnostics.Stopwatch.GetTimestamp();
            this.onComplete = onComplete;
            this.onError = onError;
            SetTimeout((float)timeout.TotalSeconds);
        }

        #endregion

        #region Static Constructor

        private Promise(Promise.State state) {
            initialized = System.Diagnostics.Stopwatch.GetTimestamp();
            completed = initialized;
            this.state = state;
            asyncOperation?.Invoke();
        }

        private Promise(string error) {
            initialized = System.Diagnostics.Stopwatch.GetTimestamp();
            completed = initialized;
            this.errorMessage = error;
            this.state = Promise.State.Error;
            asyncOperation?.Invoke();
        }

        public static Promise GetPromiseCompleted() {
            return new Promise(Promise.State.Success);
        }

        public static Promise GetPromiseError(string error) {
            return new Promise(error);
        }

        #endregion

        #region SetCallbacks

        public Promise AddOnComplete(OnCompleteDelegate onComplete) {
            this.OnComplete += onComplete;
            return this;
        }

        public Promise AddOnError(OnErrorDelegate onError) {
            this.OnError += onError;
            return this;
        }

        public Promise SetOnComplete(OnCompleteDelegate onComplete) {
            this.onComplete = onComplete;
            if(IsCompleted && IsSuccess(state))
                onComplete?.Invoke();
            return this;
        }

        public Promise SetOnError(OnErrorDelegate onError) {
            this.onError = onError;
            if(IsCompleted && IsError(state))
                onError?.Invoke(errorMessage);
            return this;
        }

        #endregion

        #region Methods

        void IPromise.Complete()
            => Complete();

        public Promise Complete() {
            try {
                if(IsCompleted)
                    return this;
                if(!isThreadedOrEditor && !Toolkit.Threading.ThreadingUtility.IsMainThread) {
                    Debug.LogError(TAG + "Callback on non-main thread!");
                }
                completed = System.Diagnostics.Stopwatch.GetTimestamp();
                state = State.Success;
                onComplete?.Invoke();
                asyncOperation?.Invoke();
                DisableTimeout();
                return this;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return this;
            }
        }

        void IPromise.Cancel()
            => Cancel();

        public Promise Cancel() {
            try {
                if(IsCompleted)
                    return this;
                if(!isThreadedOrEditor && !Toolkit.Threading.ThreadingUtility.IsMainThread) {
                    Debug.LogError(TAG + "Callback on non-main thread!");
                }
                completed = System.Diagnostics.Stopwatch.GetTimestamp();
                state = State.Cancelled;
                errorMessage = "Cancelled";
                onError?.Invoke(errorMessage);
                asyncOperation?.Invoke();
                DisableTimeout();
                return this;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return this;
            }
        }

        void IPromise.Error()
            => Error();

        public Promise Error() {
            try {
                if(IsCompleted)
                    return this;
                if(!isThreadedOrEditor && !Toolkit.Threading.ThreadingUtility.IsMainThread) {
                    Debug.LogError(TAG + "Callback on non-main thread!");
                }
                completed = System.Diagnostics.Stopwatch.GetTimestamp();
                state = State.Error;
                errorMessage = "unknown";
                onError?.Invoke(errorMessage);
                asyncOperation?.Invoke();
                DisableTimeout();
                return this;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return this;
            }
        }

        void IPromise.Error(string message)
            => Error(message);

        public Promise Error(string message) {
            try {
                if(IsCompleted)
                    return this;
                if(!isThreadedOrEditor && !Toolkit.Threading.ThreadingUtility.IsMainThread) {
                    Debug.LogError(TAG + "Callback on non-main thread!");
                }
                completed = System.Diagnostics.Stopwatch.GetTimestamp();
                state = State.Error;
                this.errorMessage = message;
                onError?.Invoke(errorMessage);
                asyncOperation?.Invoke();
                DisableTimeout();
                return this;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return this;
            }
        }

        public void Timeout() {
            try {
                if(IsCompleted)
                    return;
                if(!isThreadedOrEditor && !Toolkit.Threading.ThreadingUtility.IsMainThread) {
                    Debug.LogError(TAG + "Callback on non-main thread!");
                }
                completed = System.Diagnostics.Stopwatch.GetTimestamp();
                state = State.Timeout;
                errorMessage = "Timeout";
                onError?.Invoke(errorMessage);
                asyncOperation?.Invoke();
                DisableTimeout();
            }
            catch(System.Exception e) {
                Debug.LogException(e);
            }
        }

        #endregion

        #region Util

        public Promise SetTimeout() {
            Timer.Once(DEFAULT_TIMEOUT_IN_SECONDS, TimeoutMethod, ref routine);
            return this;
        }

        public Promise SetTimeout(System.TimeSpan span) {
            return SetTimeout((float)span.TotalSeconds);
        }

        public Promise SetTimeout(float seconds) {
            if(seconds <= 0f)
                return DisableTimeout();
            if(Toolkit.Threading.ThreadingUtility.IsMainThread && Application.isPlaying) {
                Timer.Once(seconds, Timeout, ref routine);
            }
            else {
                isThreadedOrEditor = true;
                threadedCancellationSource?.Cancel();
                var tempRef = threadedCancellationSource = new System.Threading.CancellationTokenSource();
                System.Threading.Tasks.Task.Run(async () => {
                    await System.Threading.Tasks.Task.Delay(Mathf.RoundToInt(seconds * 1000f), tempRef.Token);
                    if(!tempRef.IsCancellationRequested)
                        Timeout();
                }, tempRef.Token);
            }
            return this;
        }

        public Promise DisableTimeout() {
            threadedCancellationSource?.Cancel();
            if(Toolkit.Threading.ThreadingUtility.IsMainThread && Application.isPlaying)
                Timer.Stop(routine);
            return this;
        }

        private void TimeoutMethod() => Timeout();

        public static bool IsSuccess(State state) => state == State.Success;

        public static bool IsError(State state) {
            switch(state) {
                case State.Cancelled:
                case State.Timeout:
                case State.Error:
                    return true;
            }
            return false;
        }

        public static bool IsSuccess(Promise p) => IsSuccess(p.state);

        public static bool IsError(Promise p) => IsError(p.state);

        public Promise AllowMultiThreadResponse(){
            isThreadedOrEditor = true;
            return this;
        }

        #endregion

        #region Async

        public Promise GetAwaiter() => this;

        void INotifyCompletion.OnCompleted(Action continuation) {
            asyncOperation += continuation;
            if(IsCompleted)
                continuation?.Invoke();
        }

        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation) {
            asyncOperation += continuation;
            if(IsCompleted)
                continuation?.Invoke();
        }

        #endregion
    }

    public sealed class Promise<T> : IPromise<T>, INotifyCompletion, ICriticalNotifyCompletion {

        public delegate void OnCompleteDelegate(T value);

        public struct Result {
            #region Variables

            private Promise<T> promise;

            #endregion

            #region Properties

            public Promise.State State => promise?.state ?? Promise.State.None;
            public T Value => promise != null ? promise.Value : default(T);

            public bool IsCompleted => State != Promise.State.None;
            public bool IsSuccess => State == Promise.State.Success;
            public bool IsError => Promise.IsError(State);

            public string Error => promise?.ErrorMessage ?? string.Empty;

            #endregion

            #region Constructor

            public Result(Promise<T> promise) { this.promise = promise; }

            #endregion

            #region Operators

            public static implicit operator Promise.State(Result result) => result.State;

            #endregion
        }

        #region Variables

        private const string TAG = "[Promise] - ";
        public const int DEFAULT_TIMEOUT_IN_SECONDS = 5;

        private T value;
        private Promise.State state = Promise.State.None;
        private string errorMessage;
        private Coroutine routine;

        private OnCompleteDelegate onComplete;
        private Promise.OnErrorDelegate onError;

        private Action asyncOperation;

        private long initialized;
        private long completed;
        private System.Threading.CancellationTokenSource threadedCancellationSource;
        private bool isThreadedOrEditor;

        #endregion

        #region Properties

        public T Value => value;

        public string ErrorMessage => errorMessage;

        public bool IsCompleted { get { return state != Promise.State.None; } }
        public Promise.State CurrentState => state;
        public Result GetResult() { return new Result(this); }

        public event OnCompleteDelegate OnComplete {
            add {
                onComplete += value;
                if(IsCompleted && Promise.IsSuccess(state))
                    value?.Invoke(this.value);
            }
            remove => onComplete -= value;
        }

        public event Promise.OnErrorDelegate OnError{
            add {
                onError += value;
                if(IsCompleted && Promise.IsError(state))
                    value?.Invoke(errorMessage);
            }
            remove => onError -= value;
        }

        public long ElapsedTicks => completed - initialized;
        public TimeSpan ElapsedTime => PerformanceUtility.GetElapsedTimeSpan(initialized, completed);

        #endregion

        #region Constructor

        public Promise() {
            initialized = System.Diagnostics.Stopwatch.GetTimestamp();
            SetTimeout(DEFAULT_TIMEOUT_IN_SECONDS);
        }

        public Promise(bool useTimeout) {
            initialized = System.Diagnostics.Stopwatch.GetTimestamp();
            if(useTimeout)
                SetTimeout(DEFAULT_TIMEOUT_IN_SECONDS);
        }

        public Promise(float timeout) {
            initialized = System.Diagnostics.Stopwatch.GetTimestamp();
            SetTimeout(timeout);
        }

        public Promise(OnCompleteDelegate onComplete, Promise.OnErrorDelegate onError) {
            initialized = System.Diagnostics.Stopwatch.GetTimestamp();
            this.onComplete = onComplete;
            this.onError = onError;
            SetTimeout(DEFAULT_TIMEOUT_IN_SECONDS);
        }

        public Promise(OnCompleteDelegate onComplete, Promise.OnErrorDelegate onError, bool useTimeout) {
            initialized = System.Diagnostics.Stopwatch.GetTimestamp();
            this.onComplete = onComplete;
            this.onError = onError;
            if(useTimeout)
                SetTimeout(DEFAULT_TIMEOUT_IN_SECONDS);
        }

        public Promise(OnCompleteDelegate onComplete, Promise.OnErrorDelegate onError, float timeout) {
            initialized = System.Diagnostics.Stopwatch.GetTimestamp();
            this.onComplete = onComplete;
            this.onError = onError;
            SetTimeout(timeout);
        }

        public Promise(OnCompleteDelegate onComplete, Promise.OnErrorDelegate onError, System.TimeSpan timeout) {
            initialized = System.Diagnostics.Stopwatch.GetTimestamp();
            this.onComplete = onComplete;
            this.onError = onError;
            SetTimeout(timeout);
        }

        #endregion

        #region Static Constructor

        private Promise(T value, Promise.State state) {
            initialized = System.Diagnostics.Stopwatch.GetTimestamp();
            completed = initialized;
            this.value = value;
            this.state = state;
            asyncOperation?.Invoke();
        }

        private Promise(string error) {
            initialized = System.Diagnostics.Stopwatch.GetTimestamp();
            completed = initialized;
            this.errorMessage = error;
            this.state = Promise.State.Error;
            asyncOperation?.Invoke();
        }

        public static Promise<T> GetPromiseCompleted(T value) {
            return new Promise<T>(value, Promise.State.Success);
        }

        public static Promise<T> GetPromiseError(string error) {
            return new Promise<T>(error);
        }

        #endregion

        #region SetCallbacks

        public Promise<T> AddOnComplete(OnCompleteDelegate onComplete) {
            this.OnComplete += onComplete;
            return this;
        }

        public Promise<T> AddOnError(Promise.OnErrorDelegate onError) {
            this.OnError += onError;
            return this;
        }

        public Promise<T> SetOnComplete(OnCompleteDelegate onComplete) {
            this.onComplete = onComplete;
            if(IsCompleted && Promise.IsSuccess(state))
                onComplete?.Invoke(this.value);
            return this;
        }

        public Promise<T> SetOnError(Promise.OnErrorDelegate onError) {
            this.onError = onError;
            if(IsCompleted && Promise.IsError(state))
                onError?.Invoke(errorMessage);
            return this;
        }

        #endregion

        #region Methods

        void IPromise.Complete()
            => Complete(default);

        void IPromise<T>.Complete(T value)
            => Complete(value);

        public Promise<T> Complete(T value) {
            try {
                if(IsCompleted)
                    return this;
                if(!isThreadedOrEditor && !Toolkit.Threading.ThreadingUtility.IsMainThread) {
                    Debug.LogError(TAG + "Callback on non-main thread!");
                }
                completed = System.Diagnostics.Stopwatch.GetTimestamp();
                this.value = value;
                state = Promise.State.Success;
                onComplete?.Invoke(value);
                asyncOperation?.Invoke();
                DisableTimeout();
                return this;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return this;
            }
        }

        void IPromise.Cancel()
            => Cancel();

        public Promise<T> Cancel() {
            try {
                if(IsCompleted)
                    return this;
                if(!isThreadedOrEditor && !Toolkit.Threading.ThreadingUtility.IsMainThread) {
                    Debug.LogError(TAG + "Callback on non-main thread!");
                }
                completed = System.Diagnostics.Stopwatch.GetTimestamp();
                state = Promise.State.Cancelled;
                errorMessage = "Cancelled";
                onError?.Invoke(errorMessage);
                asyncOperation?.Invoke();
                DisableTimeout();
                return this;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return this;
            }
        }

        void IPromise.Error()
            => Error();

        public Promise<T> Error() {
            try {
                if(IsCompleted)
                    return this;
                if(!isThreadedOrEditor && !Toolkit.Threading.ThreadingUtility.IsMainThread) {
                    Debug.LogError(TAG + "Callback on non-main thread!");
                }
                completed = System.Diagnostics.Stopwatch.GetTimestamp();
                state = Promise.State.Error;
                errorMessage = "unknown";
                onError?.Invoke(errorMessage);
                asyncOperation?.Invoke();
                DisableTimeout();
                return this;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return this;
            }
        }

        void IPromise.Error(string msg)
            => Error(msg);

        public Promise<T> Error(string message) {
            try {
                if(IsCompleted)
                    return this;
                if(!isThreadedOrEditor && !Toolkit.Threading.ThreadingUtility.IsMainThread) {
                    Debug.LogError(TAG + "Callback on non-main thread!");
                }
                completed = System.Diagnostics.Stopwatch.GetTimestamp();
                state = Promise.State.Error;
                this.errorMessage = message;
                onError?.Invoke(errorMessage);
                asyncOperation?.Invoke();
                DisableTimeout();
                return this;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return this;
            }
        }

        public void Timeout() {
            try {
                if(IsCompleted)
                    return;
                if(!isThreadedOrEditor && !Toolkit.Threading.ThreadingUtility.IsMainThread) {
                    Debug.LogError(TAG + "Callback on non-main thread!");
                }
                completed = System.Diagnostics.Stopwatch.GetTimestamp();
                state = Promise.State.Timeout;
                errorMessage = "Timeout";
                onError?.Invoke(errorMessage);
                asyncOperation?.Invoke();
                DisableTimeout();
            }
            catch(System.Exception e) {
                Debug.LogException(e);
            }
        }

        #endregion

        #region Util

        public Promise<T> SetTimeout() {
            return SetTimeout(DEFAULT_TIMEOUT_IN_SECONDS);
        }

        public Promise<T> SetTimeout(System.TimeSpan span) {
            return SetTimeout((float)span.TotalSeconds);
        }

        public Promise<T> SetTimeout(float seconds) {
            if(seconds <= 0f)
                return DisableTimeout();
            if(Toolkit.Threading.ThreadingUtility.IsMainThread && Application.isPlaying) {
                Timer.Once(seconds, Timeout, ref routine);
            }
            else {
                isThreadedOrEditor = true;
                threadedCancellationSource?.Cancel();
                var tempRef = threadedCancellationSource = new System.Threading.CancellationTokenSource();
                System.Threading.Tasks.Task.Run(async () => {
                    await System.Threading.Tasks.Task.Delay(Mathf.RoundToInt(seconds * 1000f), tempRef.Token);
                    if(!tempRef.IsCancellationRequested)
                        Timeout();
                }, tempRef.Token);
            }
            return this;
        }

        public Promise<T> DisableTimeout() {
            threadedCancellationSource?.Cancel();
            if(Toolkit.Threading.ThreadingUtility.IsMainThread && Application.isPlaying)
                Timer.Stop(routine);
            return this;
        }

        #endregion

        #region Async

        public Promise<T> GetAwaiter() => this;

        void INotifyCompletion.OnCompleted(Action continuation) {
            asyncOperation += continuation;
            if(IsCompleted)
                continuation?.Invoke();
        }

        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation) {
            asyncOperation += continuation;
            if(IsCompleted)
                continuation?.Invoke();
        }

        #endregion
    }

    public static class PromiseExtensions {

        #region Return

        public static Promise ErrorAndReturn(this Promise promise, string message) {
            promise.Error(message);
            return promise;
        }

        public static Promise<T> ErrorAndReturn<T>(this Promise<T> promise, string message) {
            promise.Error(message);
            return promise;
        }

        #endregion

    }
}
