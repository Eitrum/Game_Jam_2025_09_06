using System;

namespace Toolkit.Threading
{
    public class Job : IMainThreadCallback
    {
        #region Variables

        private Action task;
        private Action onCompleteAnyThread;
        private Action onCompleteUnityThread;
        private bool isComplete;
        private bool isError;

        #endregion

        #region Properties

        public event Action OnComplete {
            add => onCompleteAnyThread += value;
            remove => onCompleteAnyThread -= value;
        }

        public event Action OnCompleteUnityThread {
            add => onCompleteUnityThread += value;
            remove => onCompleteUnityThread -= value;
        }

        public bool IsComplete => isComplete;
        public bool IsError => isError;

        #endregion

        #region Constructor

        public Job(Action task) {
            this.task = task;
            this.task.BeginInvoke(Callback, null);
        }

        public Job(Action task, Action onCompleteAnyThread) {
            this.task = task;
            this.onCompleteAnyThread = onCompleteAnyThread;
            this.task.BeginInvoke(Callback, null);
        }

        public Job(Action task, Action onCompleteAnyThread, Action onCompleteUnityThread) {
            this.task = task;
            this.onCompleteAnyThread = onCompleteAnyThread;
            this.onCompleteUnityThread = onCompleteUnityThread;
            this.task.BeginInvoke(Callback, null);
        }

        #endregion

        #region Callback

        void Callback(IAsyncResult ar) {
            try {
                task.EndInvoke(ar);
            }
            catch(Exception e) {
                isError = true;
                UnityEngine.Debug.LogException(e);
            }
            try {
                onCompleteAnyThread?.Invoke();
            }
            catch(Exception e) {
                isError = true;
                UnityEngine.Debug.LogException(e);
            }
            try {
                if(this.onCompleteUnityThread != null)
                    UnityThreading.AddMainThreadCallback(this);
            }
            catch(Exception e) {
                isError = true;
                UnityEngine.Debug.LogException(e);
            }
            if(this.onCompleteUnityThread == null)
                isComplete = true;
        }

        void IMainThreadCallback.Handle() {
            onCompleteUnityThread?.Invoke();
            isComplete = true;
        }

        #endregion

        #region Static Run

        public static Job Run(Action task) => new Job(task);
        public static Job Run(Action task, Action onCompleteAnyThread) => new Job(task, onCompleteAnyThread);
        public static Job Run(Action task, Action onCompleteAnyThread, Action onCompleteUnityThread) => new Job(task, onCompleteAnyThread, onCompleteUnityThread);
        public static Job Run(Action task, Action onComplete, bool onCompleteIsUnityThread) => onCompleteIsUnityThread ? new Job(task, null, onComplete) : new Job(task, onComplete);

        #endregion

        #region Static Run<TResult>

        public static Job<TResult> Run<TResult>(Func<TResult> task)
            => new Job<TResult>(task);
        public static Job<TResult> Run<TResult>(Func<TResult> task, Action<TResult> onCompleteAnyThread)
            => new Job<TResult>(task, onCompleteAnyThread);
        public static Job<TResult> Run<TResult>(Func<TResult> task, Action<TResult> onCompleteAnyThread, Action<TResult> onCompleteUnityThread)
            => new Job<TResult>(task, onCompleteAnyThread, onCompleteUnityThread);
        public static Job<TResult> Run<TResult>(Func<TResult> task, Action<TResult> onComplete, bool onCompleteIsUnityThread)
            => onCompleteIsUnityThread ? new Job<TResult>(task, null, onComplete) : new Job<TResult>(task, onComplete);

        #endregion

        #region Static Run<T0, TResult>

        public static Job<T0, TResult> Run<T0, TResult>(Func<T0, TResult> task, T0 t0)
            => new Job<T0, TResult>(task, t0);
        public static Job<T0, TResult> Run<T0, TResult>(Func<T0, TResult> task, T0 t0, Action<TResult> onCompleteAnyThread)
            => new Job<T0, TResult>(task, t0, onCompleteAnyThread);
        public static Job<T0, TResult> Run<T0, TResult>(Func<T0, TResult> task, T0 t0, Action<TResult> onCompleteAnyThread, Action<TResult> onCompleteUnityThread)
            => new Job<T0, TResult>(task, t0, onCompleteAnyThread, onCompleteUnityThread);
        public static Job<T0, TResult> Run<T0, TResult>(Func<T0, TResult> task, T0 t0, Action<TResult> onComplete, bool onCompleteIsUnityThread)
            => onCompleteIsUnityThread ? new Job<T0, TResult>(task, t0, null, onComplete) : new Job<T0, TResult>(task, t0, onComplete);

        #endregion

        #region Static Run<T0, T1, TResult>

        public static Job<T0, T1, TResult> Run<T0, T1, TResult>(Func<T0, T1, TResult> task, T0 t0, T1 t1)
            => new Job<T0, T1, TResult>(task, t0, t1);
        public static Job<T0, T1, TResult> Run<T0, T1, TResult>(Func<T0, T1, TResult> task, T0 t0, T1 t1, Action<TResult> onCompleteAnyThread)
            => new Job<T0, T1, TResult>(task, t0, t1, onCompleteAnyThread);
        public static Job<T0, T1, TResult> Run<T0, T1, TResult>(Func<T0, T1, TResult> task, T0 t0, T1 t1, Action<TResult> onCompleteAnyThread, Action<TResult> onCompleteUnityThread)
            => new Job<T0, T1, TResult>(task, t0, t1, onCompleteAnyThread, onCompleteUnityThread);
        public static Job<T0, T1, TResult> Run<T0, T1, TResult>(Func<T0, T1, TResult> task, T0 t0, T1 t1, Action<TResult> onComplete, bool onCompleteIsUnityThread)
            => onCompleteIsUnityThread ? new Job<T0, T1, TResult>(task, t0, t1, null, onComplete) : new Job<T0, T1, TResult>(task, t0, t1, onComplete);

        #endregion

        #region Static Run<T0, T1, T2, TResult>

        public static Job<T0, T1, T2, TResult> Run<T0, T1, T2, TResult>(Func<T0, T1, T2, TResult> task, T0 t0, T1 t1, T2 t2)
            => new Job<T0, T1, T2, TResult>(task, t0, t1, t2);
        public static Job<T0, T1, T2, TResult> Run<T0, T1, T2, TResult>(Func<T0, T1, T2, TResult> task, T0 t0, T1 t1, T2 t2, Action<TResult> onCompleteAnyThread)
            => new Job<T0, T1, T2, TResult>(task, t0, t1, t2, onCompleteAnyThread);
        public static Job<T0, T1, T2, TResult> Run<T0, T1, T2, TResult>(Func<T0, T1, T2, TResult> task, T0 t0, T1 t1, T2 t2, Action<TResult> onCompleteAnyThread, Action<TResult> onCompleteUnityThread)
            => new Job<T0, T1, T2, TResult>(task, t0, t1, t2, onCompleteAnyThread, onCompleteUnityThread);
        public static Job<T0, T1, T2, TResult> Run<T0, T1, T2, TResult>(Func<T0, T1, T2, TResult> task, T0 t0, T1 t1, T2 t2, Action<TResult> onComplete, bool onCompleteIsUnityThread)
            => onCompleteIsUnityThread ? new Job<T0, T1, T2, TResult>(task, t0, t1, t2, null, onComplete) : new Job<T0, T1, T2, TResult>(task, t0, t1, t2, onComplete);

        #endregion

        #region Static Run<T0, T1, T2, T3, TResult>

        public static Job<T0, T1, T2, T3, TResult> Run<T0, T1, T2, T3, TResult>(Func<T0, T1, T2, T3, TResult> task, T0 t0, T1 t1, T2 t2, T3 t3)
            => new Job<T0, T1, T2, T3, TResult>(task, t0, t1, t2, t3);
        public static Job<T0, T1, T2, T3, TResult> Run<T0, T1, T2, T3, TResult>(Func<T0, T1, T2, T3, TResult> task, T0 t0, T1 t1, T2 t2, T3 t3, Action<TResult> onCompleteAnyThread)
            => new Job<T0, T1, T2, T3, TResult>(task, t0, t1, t2, t3, onCompleteAnyThread);
        public static Job<T0, T1, T2, T3, TResult> Run<T0, T1, T2, T3, TResult>(Func<T0, T1, T2, T3, TResult> task, T0 t0, T1 t1, T2 t2, T3 t3, Action<TResult> onCompleteAnyThread, Action<TResult> onCompleteUnityThread)
            => new Job<T0, T1, T2, T3, TResult>(task, t0, t1, t2, t3, onCompleteAnyThread, onCompleteUnityThread);
        public static Job<T0, T1, T2, T3, TResult> Run<T0, T1, T2, T3, TResult>(Func<T0, T1, T2, T3, TResult> task, T0 t0, T1 t1, T2 t2, T3 t3, Action<TResult> onComplete, bool onCompleteIsUnityThread)
            => onCompleteIsUnityThread ? new Job<T0, T1, T2, T3, TResult>(task, t0, t1, t2, t3, null, onComplete) : new Job<T0, T1, T2, T3, TResult>(task, t0, t1, t2, t3, onComplete);

        #endregion

        #region Run Unity Thread

        public static UnityThreadTask RunUnityTask(Action task) => new UnityThreadTask(task);

        public static UnityThreadTask<TResult> RunUnityTask<TResult>(Func<TResult> task) => new UnityThreadTask<TResult>(task);
        public static UnityThreadTask<TResult> RunUnityTask<TResult>(Func<TResult> task, Action<TResult> onComplete) => new UnityThreadTask<TResult>(task, onComplete);
        public static UnityThreadTask<TResult> RunUnityTask<TResult>(Func<TResult> task, Action<TResult> onComplete, bool invokeCompletOnOtherThread) => new UnityThreadTask<TResult>(task, onComplete, invokeCompletOnOtherThread);

        #endregion
    }

    public class UnityThreadTask : IMainThreadCallback
    {
        #region Variables

        private Action task;
        public bool IsDone { get; private set; }

        #endregion

        #region Constructor

        public UnityThreadTask(Action task) {
            this.task = task;
            if(ThreadingUtility.IsMainThread)
                Invoke();
            else
                UnityThreading.AddMainThreadCallback(this);
        }

        #endregion

        #region Invoke

        void IMainThreadCallback.Handle() => Invoke();

        private void Invoke() {
            task();
            IsDone = true;
        }

        #endregion
    }

    public class UnityThreadTask<TResult> : IMainThreadCallback
    {
        #region Variables

        private Func<TResult> task;
        private Action<TResult> onComplete;

        public bool InvokeCompleteOnOtherThread = false;
        public TResult Result { get; private set; }
        public bool IsDone { get; private set; }

        #endregion

        #region Constructor

        public UnityThreadTask(Func<TResult> task) : this(task, null) { }

        public UnityThreadTask(Func<TResult> task, Action<TResult> onComplete) : this(task, onComplete, false) { }

        public UnityThreadTask(Func<TResult> task, Action<TResult> onComplete, bool invokeCompleteOnOtherThread) {
            this.task = task;
            this.onComplete = onComplete;
            this.InvokeCompleteOnOtherThread = invokeCompleteOnOtherThread;
            if(ThreadingUtility.IsMainThread)
                Invoke();
            else
                UnityThreading.AddMainThreadCallback(this);
        }

        #endregion

        #region Invoke

        void IMainThreadCallback.Handle() => Invoke();

        private void Invoke() {
            Result = task.Invoke();
            if(onComplete != null) {
                if(InvokeCompleteOnOtherThread)
                    onComplete.BeginInvoke(Result, Callback, null);
                else
                    onComplete.Invoke(Result);
            }
            IsDone = true;
        }

        private void Callback(IAsyncResult ar) {
            try {
                onComplete.EndInvoke(ar);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
        }

        #endregion
    }

    public class UnityThreadTask<T0, TResult> : IMainThreadCallback
    {
        #region Variables

        private T0 t0;
        private Func<T0, TResult> task;
        private Action<TResult> onComplete;

        public bool InvokeOtherThread = false;
        public TResult Result { get; private set; }
        public bool IsDone { get; private set; }

        #endregion

        #region Constructor

        public UnityThreadTask(Func<T0, TResult> task, T0 t0) : this(task, t0, null) { }

        public UnityThreadTask(Func<T0, TResult> task, T0 t0, Action<TResult> onComplete) : this(task, t0, onComplete, false) { }

        public UnityThreadTask(Func<T0, TResult> task, T0 t0, Action<TResult> onComplete, bool invokeOtherThread) {
            this.t0 = t0;
            this.task = task;
            this.onComplete = onComplete;
            this.InvokeOtherThread = invokeOtherThread;
            if(ThreadingUtility.IsMainThread)
                Invoke();
            else
                UnityThreading.AddMainThreadCallback(this);
        }

        #endregion

        #region Invoke

        void IMainThreadCallback.Handle() => Invoke();

        private void Invoke() {
            Result = task.Invoke(t0);
            if(onComplete != null) {
                if(InvokeOtherThread)
                    onComplete.BeginInvoke(Result, Callback, null);
                else
                    onComplete.Invoke(Result);
            }
            IsDone = true;
        }

        private void Callback(IAsyncResult ar) {
            try {
                onComplete.EndInvoke(ar);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
        }

        #endregion
    }

    public class UnityThreadTask<T0, T1, TResult> : IMainThreadCallback
    {
        #region Variables

        private T0 t0;
        private T1 t1;
        private Func<T0, T1, TResult> task;
        private Action<TResult> onComplete;

        public bool InvokeOtherThread = false;
        public TResult Result { get; private set; }
        public bool IsDone { get; private set; }

        #endregion

        #region Constructor

        public UnityThreadTask(Func<T0, T1, TResult> task, T0 t0, T1 t1) : this(task, t0, t1, null) { }

        public UnityThreadTask(Func<T0, T1, TResult> task, T0 t0, T1 t1, Action<TResult> onComplete) : this(task, t0, t1, onComplete, false) { }

        public UnityThreadTask(Func<T0, T1, TResult> task, T0 t0, T1 t1, Action<TResult> onComplete, bool invokeOtherThread) {
            this.t0 = t0;
            this.t1 = t1;
            this.task = task;
            this.onComplete = onComplete;
            this.InvokeOtherThread = invokeOtherThread;
            if(ThreadingUtility.IsMainThread)
                Invoke();
            else
                UnityThreading.AddMainThreadCallback(this);
        }

        #endregion

        #region Invoke

        void IMainThreadCallback.Handle() => Invoke();

        private void Invoke() {
            Result = task.Invoke(t0, t1);
            if(onComplete != null) {
                if(InvokeOtherThread)
                    onComplete.BeginInvoke(Result, Callback, null);
                else
                    onComplete.Invoke(Result);
            }
            IsDone = true;
        }

        private void Callback(IAsyncResult ar) {
            try {
                onComplete.EndInvoke(ar);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
        }

        #endregion
    }

    public class UnityThreadTask<T0, T1, T2, TResult> : IMainThreadCallback
    {
        #region Variables

        private T0 t0;
        private T1 t1;
        private T2 t2;
        private Func<T0, T1, T2, TResult> task;
        private Action<TResult> onComplete;

        public bool InvokeOtherThread = false;
        public TResult Result { get; private set; }
        public bool IsDone { get; private set; }

        #endregion

        #region Constructor

        public UnityThreadTask(Func<T0, T1, T2, TResult> task, T0 t0, T1 t1, T2 t2) : this(task, t0, t1, t2, null) { }

        public UnityThreadTask(Func<T0, T1, T2, TResult> task, T0 t0, T1 t1, T2 t2, Action<TResult> onComplete) : this(task, t0, t1, t2, onComplete, false) { }

        public UnityThreadTask(Func<T0, T1, T2, TResult> task, T0 t0, T1 t1, T2 t2, Action<TResult> onComplete, bool invokeOtherThread) {
            this.t0 = t0;
            this.t1 = t1;
            this.t2 = t2;
            this.task = task;
            this.onComplete = onComplete;
            this.InvokeOtherThread = invokeOtherThread;
            if(ThreadingUtility.IsMainThread)
                Invoke();
            else
                UnityThreading.AddMainThreadCallback(this);
        }

        #endregion

        #region Invoke

        void IMainThreadCallback.Handle() => Invoke();

        private void Invoke() {
            Result = task.Invoke(t0, t1, t2);
            if(onComplete != null) {
                if(InvokeOtherThread)
                    onComplete.BeginInvoke(Result, Callback, null);
                else
                    onComplete.Invoke(Result);
            }
            IsDone = true;
        }

        private void Callback(IAsyncResult ar) {
            try {
                onComplete.EndInvoke(ar);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
        }

        #endregion
    }

    public class Job<TResult> : IMainThreadCallback
    {
        #region Variables

        private TResult result;
        private Func<TResult> task;
        private Action<TResult> onCompleteAnyThread;
        private Action<TResult> onCompleteUnityThread;

        #endregion

        #region Properties

        public event Action<TResult> OnComplete {
            add => onCompleteAnyThread += value;
            remove => onCompleteAnyThread -= value;
        }

        public event Action<TResult> OnCompleteUnityThread {
            add => onCompleteUnityThread += value;
            remove => onCompleteUnityThread -= value;
        }

        #endregion

        #region Constructor

        public Job(Func<TResult> task) {
            this.task = task;
            this.task.BeginInvoke(Callback, null);
        }

        public Job(Func<TResult> task, Action<TResult> onCompleteAnyThread) {
            this.task = task;
            this.onCompleteAnyThread = onCompleteAnyThread;
            this.task.BeginInvoke(Callback, null);
        }

        public Job(Func<TResult> task, Action<TResult> onCompleteAnyThread, Action<TResult> onCompleteUnityThread) {
            this.task = task;
            this.onCompleteAnyThread = onCompleteAnyThread;
            this.onCompleteUnityThread = onCompleteUnityThread;
            this.task.BeginInvoke(Callback, null);
        }

        #endregion

        #region Callback

        void Callback(IAsyncResult ar) {
            try {
                result = task.EndInvoke(ar);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
            try {
                onCompleteAnyThread?.Invoke(result);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
            try {
                if(this.onCompleteUnityThread != null)
                    UnityThreading.AddMainThreadCallback(this);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
        }

        void IMainThreadCallback.Handle() {
            onCompleteUnityThread?.Invoke(result);
        }

        #endregion
    }

    public class Job<T0, TResult> : IMainThreadCallback
    {
        #region Variables

        private TResult result;
        private Func<T0, TResult> task;
        private Action<TResult> onCompleteAnyThread;
        private Action<TResult> onCompleteUnityThread;

        #endregion

        #region Properties

        public event Action<TResult> OnComplete {
            add => onCompleteAnyThread += value;
            remove => onCompleteAnyThread -= value;
        }

        public event Action<TResult> OnCompleteUnityThread {
            add => onCompleteUnityThread += value;
            remove => onCompleteUnityThread -= value;
        }

        #endregion

        #region Constructor

        public Job(Func<T0, TResult> task, T0 t0) {
            this.task = task;
            this.task.BeginInvoke(t0, Callback, null);
        }

        public Job(Func<T0, TResult> task, T0 t0, Action<TResult> onCompleteAnyThread) {
            this.task = task;
            this.onCompleteAnyThread = onCompleteAnyThread;
            this.task.BeginInvoke(t0, Callback, null);
        }

        public Job(Func<T0, TResult> task, T0 t0, Action<TResult> onCompleteAnyThread, Action<TResult> onCompleteUnityThread) {
            this.task = task;
            this.onCompleteAnyThread = onCompleteAnyThread;
            this.onCompleteUnityThread = onCompleteUnityThread;
            this.task.BeginInvoke(t0, Callback, null);
        }

        #endregion

        #region Callback

        void Callback(IAsyncResult ar) {
            try {
                result = task.EndInvoke(ar);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
            try {
                onCompleteAnyThread?.Invoke(result);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
            try {
                if(this.onCompleteUnityThread != null)
                    UnityThreading.AddMainThreadCallback(this);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
        }

        void IMainThreadCallback.Handle() {
            onCompleteUnityThread?.Invoke(result);
        }

        #endregion
    }

    public class Job<T0, T1, TResult> : IMainThreadCallback
    {
        #region Variables

        private TResult result;
        private Func<T0, T1, TResult> task;
        private Action<TResult> onCompleteAnyThread;
        private Action<TResult> onCompleteUnityThread;

        #endregion

        #region Properties

        public event Action<TResult> OnComplete {
            add => onCompleteAnyThread += value;
            remove => onCompleteAnyThread -= value;
        }

        public event Action<TResult> OnCompleteUnityThread {
            add => onCompleteUnityThread += value;
            remove => onCompleteUnityThread -= value;
        }

        #endregion

        #region Constructor

        public Job(Func<T0, T1, TResult> task, T0 t0, T1 t1) {
            this.task = task;
            this.task.BeginInvoke(t0, t1, Callback, null);
        }

        public Job(Func<T0, T1, TResult> task, T0 t0, T1 t1, Action<TResult> onCompleteAnyThread) {
            this.task = task;
            this.onCompleteAnyThread = onCompleteAnyThread;
            this.task.BeginInvoke(t0, t1, Callback, null);
        }

        public Job(Func<T0, T1, TResult> task, T0 t0, T1 t1, Action<TResult> onCompleteAnyThread, Action<TResult> onCompleteUnityThread) {
            this.task = task;
            this.onCompleteAnyThread = onCompleteAnyThread;
            this.onCompleteUnityThread = onCompleteUnityThread;
            this.task.BeginInvoke(t0, t1, Callback, null);
        }

        #endregion

        #region Callback

        void Callback(IAsyncResult ar) {
            try {
                result = task.EndInvoke(ar);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
            try {
                onCompleteAnyThread?.Invoke(result);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
            try {
                if(this.onCompleteUnityThread != null)
                    UnityThreading.AddMainThreadCallback(this);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
        }

        void IMainThreadCallback.Handle() {
            onCompleteUnityThread?.Invoke(result);
        }

        #endregion
    }

    public class Job<T0, T1, T2, TResult> : IMainThreadCallback
    {
        #region Variables

        private TResult result;
        private Func<T0, T1, T2, TResult> task;
        private Action<TResult> onCompleteAnyThread;
        private Action<TResult> onCompleteUnityThread;

        #endregion

        #region Properties

        public event Action<TResult> OnComplete {
            add => onCompleteAnyThread += value;
            remove => onCompleteAnyThread -= value;
        }

        public event Action<TResult> OnCompleteUnityThread {
            add => onCompleteUnityThread += value;
            remove => onCompleteUnityThread -= value;
        }

        #endregion

        #region Constructor

        public Job(Func<T0, T1, T2, TResult> task, T0 t0, T1 t1, T2 t2) {
            this.task = task;
            this.task.BeginInvoke(t0, t1, t2, Callback, null);
        }

        public Job(Func<T0, T1, T2, TResult> task, T0 t0, T1 t1, T2 t2, Action<TResult> onCompleteAnyThread) {
            this.task = task;
            this.onCompleteAnyThread = onCompleteAnyThread;
            this.task.BeginInvoke(t0, t1, t2, Callback, null);
        }

        public Job(Func<T0, T1, T2, TResult> task, T0 t0, T1 t1, T2 t2, Action<TResult> onCompleteAnyThread, Action<TResult> onCompleteUnityThread) {
            this.task = task;
            this.onCompleteAnyThread = onCompleteAnyThread;
            this.onCompleteUnityThread = onCompleteUnityThread;
            this.task.BeginInvoke(t0, t1, t2, Callback, null);
        }

        #endregion

        #region Callback

        void Callback(IAsyncResult ar) {
            try {
                result = task.EndInvoke(ar);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
            try {
                onCompleteAnyThread?.Invoke(result);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
            try {
                if(this.onCompleteUnityThread != null)
                    UnityThreading.AddMainThreadCallback(this);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
        }

        void IMainThreadCallback.Handle() {
            onCompleteUnityThread?.Invoke(result);
        }

        #endregion
    }

    public class Job<T0, T1, T2, T3, TResult> : IMainThreadCallback
    {
        #region Variables

        private TResult result;
        private Func<T0, T1, T2, T3, TResult> task;
        private Action<TResult> onCompleteAnyThread;
        private Action<TResult> onCompleteUnityThread;

        #endregion

        #region Properties

        public event Action<TResult> OnComplete {
            add => onCompleteAnyThread += value;
            remove => onCompleteAnyThread -= value;
        }

        public event Action<TResult> OnCompleteUnityThread {
            add => onCompleteUnityThread += value;
            remove => onCompleteUnityThread -= value;
        }

        #endregion

        #region Constructor

        public Job(Func<T0, T1, T2, T3, TResult> task, T0 t0, T1 t1, T2 t2, T3 t3) {
            this.task = task;
            this.task.BeginInvoke(t0, t1, t2, t3, Callback, null);
        }

        public Job(Func<T0, T1, T2, T3, TResult> task, T0 t0, T1 t1, T2 t2, T3 t3, Action<TResult> onCompleteAnyThread) {
            this.task = task;
            this.onCompleteAnyThread = onCompleteAnyThread;
            this.task.BeginInvoke(t0, t1, t2, t3, Callback, null);
        }

        public Job(Func<T0, T1, T2, T3, TResult> task, T0 t0, T1 t1, T2 t2, T3 t3, Action<TResult> onCompleteAnyThread, Action<TResult> onCompleteUnityThread) {
            this.task = task;
            this.onCompleteAnyThread = onCompleteAnyThread;
            this.onCompleteUnityThread = onCompleteUnityThread;
            this.task.BeginInvoke(t0, t1, t2, t3, Callback, null);
        }

        #endregion

        #region Callback

        void Callback(IAsyncResult ar) {
            try {
                result = task.EndInvoke(ar);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
            try {
                onCompleteAnyThread?.Invoke(result);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
            try {
                if(this.onCompleteUnityThread != null)
                    UnityThreading.AddMainThreadCallback(this);
            }
            catch(Exception e) {
                UnityEngine.Debug.LogException(e);
            }
        }

        void IMainThreadCallback.Handle() {
            onCompleteUnityThread?.Invoke(result);
        }

        #endregion
    }
}
