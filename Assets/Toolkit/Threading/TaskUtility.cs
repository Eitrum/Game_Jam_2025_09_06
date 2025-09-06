using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Toolkit.Threading
{
    public static class TaskUtility
    {

        public static TaskContainer<T> CreateContainer<T>(Task<T> task) => new TaskContainer<T>(task);
        public static TaskContainer<T> CreateContainer<T>(Func<Task<T>> taskFunction) => new TaskContainer<T>(taskFunction());
        public static TaskContainer<T> CreateContainer<T>(Func<CancellationToken, Task<T>> taskFunction) => new TaskContainer<T>(taskFunction);

    }

    public abstract class TaskContainer
    {
        #region  Variables

        protected CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public CancellationToken Token => cancellationTokenSource.Token;
        public bool IsComplete { get; protected set; } = false;
        public bool IsCanceled { get; protected set; } = false;
        public bool IsFaulty { get; protected set; } = false;

        public System.Exception Exception { get; protected set; } = null;

        #endregion

        #region Cancel

        public void Cancel() {
            if(!cancellationTokenSource.IsCancellationRequested)
                cancellationTokenSource.Cancel();
        }

        public void Cancel(float time) {
            if(!cancellationTokenSource.IsCancellationRequested)
                cancellationTokenSource.CancelAfter(Mathf.RoundToInt(time * 1000f));
        }

        #endregion
    }

    public class TaskContainer<T> : TaskContainer
    {
        #region Variables

        private Task<T> task;
        private Action<T> callback;
        private Action<TaskContainer<T>> onError;
        public T Result => task.Result;

        #endregion

        #region Properties

        public event Action<T> OnComplete {
            add {
                callback += value;
                if(IsComplete)
                    value(Result);
            }
            remove => callback -= value;
        }

        public event Action<TaskContainer<T>> OnError {
            add => onError += value;
            remove => onError -= value;
        }

        #endregion

        #region Constructor

        public TaskContainer(Task<T> task) {
            this.task = task;
            Runner();
        }

        public TaskContainer(System.Func<CancellationToken, Task<T>> function) {
            task = function(Token);
            Runner();
        }

        private async void Runner() {
            try {
                await task;
                IsCanceled = task.IsCanceled;
                IsComplete = task.IsCompleted;
            }
            catch(System.Exception e) {
                Exception = e;
                IsFaulty = true;
                onError?.Invoke(this);
            }
            finally {
                // Debug.Log($"Task ended:\nComplete: {IsComplete}\nCanceled: {IsCanceled}\nFaulty: {IsFaulty}");
                if(IsComplete) {
                    callback?.Invoke(task.Result);
                }
            }
        }

        #endregion
    }

    public class TaskEnumerator<T> : IEnumerator
    {
        #region Variables

        private Task<T> task;
        public T Result { get; private set; }
        public bool IsComplete { get; private set; } = false;

        #endregion

        #region Constructor

        public TaskEnumerator(Task<T> task) {
            this.task = task;
            RunTask(task);
        }

        #endregion

        #region Task runner

        private async void RunTask(Task<T> task) {
            await task;
            Result = task.Result;
            IsComplete = true;
        }

        public void Cancel() {

        }

        #endregion

        #region IEnumerator Impl

        object IEnumerator.Current => null;

        bool IEnumerator.MoveNext() {
            return IsComplete;
        }

        void IEnumerator.Reset() { }

        #endregion
    }
}
