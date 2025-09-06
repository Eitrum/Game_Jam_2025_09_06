using System;
using UnityEngine;

namespace Toolkit {
    public static class UpdateSystem {
        #region Variables

        private const string TAG = "[Toolkit.UpdateSystem] - ";
        private static UpdateSystemBehaviour Instance => UpdateSystemBehaviour.Instance;

        #endregion

        #region Subscribe

        public static void Subscribe<T>(T obj) where T : INullable {
            if(obj is IEarlyUpdate earlyUpdate) Subscribe(earlyUpdate);
            if(obj is IUpdate update) Subscribe(update);
            if(obj is ILateUpdate lateUpdate) Subscribe(lateUpdate);
            if(obj is IPostUpdate postUpdate) Subscribe(postUpdate);
            if(obj is IFixedUpdate fixedUpdate) Subscribe(fixedUpdate);
            if(obj is IOnBeforeRender onBeforeRender) Subscribe(onBeforeRender);
        }

        public static void Subscribe<T>(T obj, UpdateModeMask mode) where T : INullable {
            if(mode.HasFlag(UpdateModeMask.EarlyUpdate) && obj is IEarlyUpdate earlyUpdate) Subscribe(earlyUpdate);
            if(mode.HasFlag(UpdateModeMask.Update) && obj is IUpdate update) Subscribe(update);
            if(mode.HasFlag(UpdateModeMask.LateUpdate) && obj is ILateUpdate lateUpdate) Subscribe(lateUpdate);
            if(mode.HasFlag(UpdateModeMask.PostUpdate) && obj is IPostUpdate postUpdate) Subscribe(postUpdate);
            if(mode.HasFlag(UpdateModeMask.FixedUpdate) && obj is IFixedUpdate fixedUpdate) Subscribe(fixedUpdate);
            if(mode.HasFlag(UpdateModeMask.OnBeforeRender) && obj is IOnBeforeRender onBeforeRender) Subscribe(onBeforeRender);
        }

        public static void Subscribe<T>(T obj, UpdateMode mode) where T : INullable {
            switch(mode) {
                case UpdateMode.EarlyUpdate: if(obj is IEarlyUpdate earlyUpdate) Subscribe(earlyUpdate); break;
                case UpdateMode.Update: if(obj is IUpdate update) Subscribe(update); break;
                case UpdateMode.LateUpdate: if(obj is ILateUpdate lateUpdate) Subscribe(lateUpdate); break;
                case UpdateMode.PostUpdate: if(obj is IPostUpdate postUpdate) Subscribe(postUpdate); break;
                case UpdateMode.FixedUpdate: if(obj is IFixedUpdate fixedUpdate) Subscribe(fixedUpdate); break;
                case UpdateMode.OnBeforeRender: if(obj is IOnBeforeRender onBeforeRender) Subscribe(onBeforeRender); break;
            }
        }

        public static TLinkedListNode<IEarlyUpdate> Subscribe(IEarlyUpdate earlyUpdate) => Instance.earlyUpdates.Add(earlyUpdate);
        public static TLinkedListNode<IUpdate> Subscribe(IUpdate update) => Instance.updates.Add(update);
        public static TLinkedListNode<ILateUpdate> Subscribe(ILateUpdate lateUpdate) => Instance.lateUpdates.Add(lateUpdate);
        public static TLinkedListNode<IPostUpdate> Subscribe(IPostUpdate postUpdate) => Instance.postUpdates.Add(postUpdate);
        public static TLinkedListNode<IFixedUpdate> Subscribe(IFixedUpdate fixedUpdate) => Instance.fixedUpdates.Add(fixedUpdate);
        public static TLinkedListNode<IOnBeforeRender> Subscribe(IOnBeforeRender onBeforeRender) => Instance.onBeforeRender.Add(onBeforeRender);
        public static TLinkedListNode<IOnGUI> Subscribe(IOnGUI onGUI) => Instance.onGUIs.Add(onGUI);

        public static void Subscribe<T>(T nullable, float delay, OnTimerUpdateCallback callback) where T : INullable => TimerUpdateSystemBehaviour.Subscribe<T>(nullable, delay, callback);

        #endregion

        #region Unsubscribe

        public static void Unsubscribe<T>(T obj) where T : INullable {
            if(obj is IEarlyUpdate earlyUpdate) Unsubscribe(earlyUpdate);
            if(obj is IUpdate update) Unsubscribe(update);
            if(obj is ILateUpdate lateUpdate) Unsubscribe(lateUpdate);
            if(obj is IPostUpdate postUpdate) Unsubscribe(postUpdate);
            if(obj is IFixedUpdate fixedUpdate) Unsubscribe(fixedUpdate);
            if(obj is IOnBeforeRender onBeforeRender) Unsubscribe(onBeforeRender);
        }

        public static void Unsubscribe<T>(T obj, UpdateModeMask mode) where T : INullable {
            if(mode.HasFlag(UpdateModeMask.EarlyUpdate) && obj is IEarlyUpdate earlyUpdate) Unsubscribe(earlyUpdate);
            if(mode.HasFlag(UpdateModeMask.Update) && obj is IUpdate update) Unsubscribe(update);
            if(mode.HasFlag(UpdateModeMask.LateUpdate) && obj is ILateUpdate lateUpdate) Unsubscribe(lateUpdate);
            if(mode.HasFlag(UpdateModeMask.PostUpdate) && obj is IPostUpdate postUpdate) Unsubscribe(postUpdate);
            if(mode.HasFlag(UpdateModeMask.FixedUpdate) && obj is IFixedUpdate fixedUpdate) Unsubscribe(fixedUpdate);
            if(mode.HasFlag(UpdateModeMask.OnBeforeRender) && obj is IOnBeforeRender onBeforeRender) Unsubscribe(onBeforeRender);
        }

        public static void Unsubscribe<T>(T obj, UpdateMode mode) where T : INullable {
            switch(mode) {
                case UpdateMode.EarlyUpdate: if(obj is IEarlyUpdate earlyUpdate) Unsubscribe(earlyUpdate); break;
                case UpdateMode.Update: if(obj is IUpdate update) Unsubscribe(update); break;
                case UpdateMode.LateUpdate: if(obj is ILateUpdate lateUpdate) Unsubscribe(lateUpdate); break;
                case UpdateMode.PostUpdate: if(obj is IPostUpdate postUpdate) Unsubscribe(postUpdate); break;
                case UpdateMode.FixedUpdate: if(obj is IFixedUpdate fixedUpdate) Unsubscribe(fixedUpdate); break;
                case UpdateMode.OnBeforeRender: if(obj is IOnBeforeRender onBeforeRender) Unsubscribe(onBeforeRender); break;
            }
        }

        public static void Unsubscribe(IEarlyUpdate earlyUpdate) => Instance.earlyUpdates.Remove(earlyUpdate);
        public static void Unsubscribe(TLinkedListNode<IEarlyUpdate> earlyUpdate) => Instance.earlyUpdates.Remove(earlyUpdate);

        public static void Unsubscribe(IUpdate update) => Instance.updates.Remove(update);
        public static bool Unsubscribe(TLinkedListNode<IUpdate> update) => Instance.updates.Remove(update);

        public static void Unsubscribe(ILateUpdate lateUpdate) => Instance.lateUpdates.Remove(lateUpdate);
        public static void Unsubscribe(TLinkedListNode<ILateUpdate> lateUpdate) => Instance.lateUpdates.Remove(lateUpdate);

        public static void Unsubscribe(IPostUpdate postUpdate) => Instance.postUpdates.Remove(postUpdate);
        public static void Unsubscribe(TLinkedListNode<IPostUpdate> postUpdate) => Instance.postUpdates.Remove(postUpdate);

        public static void Unsubscribe(IFixedUpdate fixedUpdate) => Instance.fixedUpdates.Remove(fixedUpdate);
        public static void Unsubscribe(TLinkedListNode<IFixedUpdate> fixedUpdate) => Instance.fixedUpdates.Remove(fixedUpdate);

        public static void Unsubscribe(IOnBeforeRender onRenderObject) => Instance.onBeforeRender.Remove(onRenderObject);
        public static void Unsubscribe(TLinkedListNode<IOnBeforeRender> onBeforeRender) => Instance.onBeforeRender.Remove(onBeforeRender);

        public static void Unsubscribe(IOnGUI onGUI) => Instance.onGUIs.Remove(onGUI);
        public static void Unsubscribe(TLinkedListNode<IOnGUI> onGUI) => Instance.onGUIs.Remove(onGUI);


        public static void Unsubscribe<T>(T nullable, OnTimerUpdateCallback callback) where T : INullable => TimerUpdateSystemBehaviour.Unsubscribe<T>(nullable, callback);

        #endregion
    }
}
