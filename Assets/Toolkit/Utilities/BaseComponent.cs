using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    public abstract class BaseComponent : MonoBehaviour, IEarlyUpdate, IUpdate, ILateUpdate, IPostUpdate, IFixedUpdate, IOnBeforeRender, IOnGUI
    {
        #region Variables

        private TLinkedListNode<IEarlyUpdate> earlyUpdateNode;
        private TLinkedListNode<IUpdate> updateNode;
        private TLinkedListNode<ILateUpdate> lateUpdateNode;
        private TLinkedListNode<IPostUpdate> postUpdateNode;
        private TLinkedListNode<IFixedUpdate> fixedUpdateNode;
        private TLinkedListNode<IOnBeforeRender> onBeforeRenderNode;
        private TLinkedListNode<IOnGUI> onGUINode;

        private Entity entityReference;

        #endregion

        #region Properties

        public Entity Entity {
            get {
                if(entityReference == null) {
                    entityReference = this.GetOrAddComponent<Entity>();
                }
                return entityReference;
            }
        }

        #endregion

        #region Updates

        protected virtual void EarlyUpdateComponent(float dt) { }
        protected virtual void UpdateComponent(float dt) { }
        protected virtual void LateUpdateComponent(float dt) { }
        protected virtual void PostUpdateComponent(float dt) { }
        protected virtual void FixedUpdateComponent(float dt) { }
        protected virtual void OnBeforeRenderComponent(float dt) { }
        protected virtual void OnGUIComponent(float dt) { }

        bool INullable.IsNull => this == null;
        void IEarlyUpdate.EarlyUpdate(float dt) => EarlyUpdateComponent(dt);
        void IUpdate.Update(float dt) => UpdateComponent(dt);
        void ILateUpdate.LateUpdate(float dt) => LateUpdateComponent(dt);
        void IPostUpdate.PostUpdate(float dt) => PostUpdateComponent(dt);
        void IFixedUpdate.FixedUpdate(float dt) => FixedUpdateComponent(dt);
        void IOnBeforeRender.OnBeforeRender(float dt) => OnBeforeRenderComponent(dt);
        void IOnGUI.OnGUI(float dt) => OnGUIComponent(dt);

        #endregion

        #region Update System subscription

        protected void Subscribe(UpdateModeMask mode) {
            if(mode.HasFlag(UpdateModeMask.EarlyUpdate)) SubscribeEarlyUpdate();
            if(mode.HasFlag(UpdateModeMask.Update)) SubscribeUpdate();
            if(mode.HasFlag(UpdateModeMask.LateUpdate)) SubscribeLateUpdate();
            if(mode.HasFlag(UpdateModeMask.PostUpdate)) SubscribePostUpdate();
            if(mode.HasFlag(UpdateModeMask.FixedUpdate)) SubscribeFixedUpdate();
            if(mode.HasFlag(UpdateModeMask.OnBeforeRender)) SubscribeOnBeforeRender();
        }

        protected void Subscribe(UpdateMode mode) {
            switch(mode) {
                case UpdateMode.EarlyUpdate: SubscribeEarlyUpdate(); break;
                case UpdateMode.Update: SubscribeUpdate(); break;
                case UpdateMode.LateUpdate: SubscribeLateUpdate(); break;
                case UpdateMode.PostUpdate: SubscribePostUpdate(); break;
                case UpdateMode.FixedUpdate: SubscribeFixedUpdate(); break;
                case UpdateMode.OnBeforeRender: SubscribeOnBeforeRender(); break;
            }
        }

        protected void SubscribeEarlyUpdate() {
            if(earlyUpdateNode == null)
                earlyUpdateNode = UpdateSystem.Subscribe(this as IEarlyUpdate);
        }

        protected void SubscribeUpdate() {
            if(updateNode == null)
                updateNode = UpdateSystem.Subscribe(this as IUpdate);
        }

        protected void SubscribeLateUpdate() {
            if(lateUpdateNode == null)
                lateUpdateNode = UpdateSystem.Subscribe(this as ILateUpdate);
        }

        protected void SubscribePostUpdate() {
            if(postUpdateNode == null)
                postUpdateNode = UpdateSystem.Subscribe(this as IPostUpdate);
        }

        protected void SubscribeFixedUpdate() {
            if(fixedUpdateNode == null)
                fixedUpdateNode = UpdateSystem.Subscribe(this as IFixedUpdate);
        }

        protected void SubscribeOnBeforeRender() {
            if(onBeforeRenderNode == null)
                onBeforeRenderNode = UpdateSystem.Subscribe(this as IOnBeforeRender);
        }

        /*protected void SubscribeOnGUI() {
            if(onGUINode == null)
                onGUINode = UpdateSystem.Subscribe(this as IOnGUI);
        }*/

        protected void SubscribeTimer(float delay, OnTimerUpdateCallback callback) => UpdateSystem.Subscribe(this, delay, callback);

        protected void Unsubscribe(UpdateModeMask mode) {
            if(mode.HasFlag(UpdateModeMask.EarlyUpdate)) UnsubscribeEarlyUpdate();
            if(mode.HasFlag(UpdateModeMask.Update)) UnsubscribeUpdate();
            if(mode.HasFlag(UpdateModeMask.LateUpdate)) UnsubscribeLateUpdate();
            if(mode.HasFlag(UpdateModeMask.PostUpdate)) UnsubscribePostUpdate();
            if(mode.HasFlag(UpdateModeMask.FixedUpdate)) UnsubscribeFixedUpdate();
            if(mode.HasFlag(UpdateModeMask.OnBeforeRender)) UnsubscribeOnBeforeRender();
        }

        protected void Unsubscribe(UpdateMode mode) {
            switch(mode) {
                case UpdateMode.EarlyUpdate: UnsubscribeEarlyUpdate(); break;
                case UpdateMode.Update: UnsubscribeUpdate(); break;
                case UpdateMode.LateUpdate: UnsubscribeLateUpdate(); break;
                case UpdateMode.PostUpdate: UnsubscribePostUpdate(); break;
                case UpdateMode.FixedUpdate: UnsubscribeFixedUpdate(); break;
                case UpdateMode.OnBeforeRender: UnsubscribeOnBeforeRender(); break;
            }
        }

        protected void UnsubscribeEarlyUpdate() {
            if(earlyUpdateNode != null)
                UpdateSystem.Unsubscribe(earlyUpdateNode);
            earlyUpdateNode = null;
        }

        protected void UnsubscribeUpdate() {
            if(updateNode != null)
                UpdateSystem.Unsubscribe(updateNode);
            updateNode = null;
        }

        protected void UnsubscribeLateUpdate() {
            if(lateUpdateNode != null)
                UpdateSystem.Unsubscribe(lateUpdateNode);
            lateUpdateNode = null;
        }

        protected void UnsubscribePostUpdate() {
            if(postUpdateNode != null)
                UpdateSystem.Unsubscribe(postUpdateNode);
            postUpdateNode = null;
        }

        protected void UnsubscribeFixedUpdate() {
            if(fixedUpdateNode != null)
                UpdateSystem.Unsubscribe(fixedUpdateNode);
            fixedUpdateNode = null;
        }

        protected void UnsubscribeOnBeforeRender() {
            if(onBeforeRenderNode != null)
                UpdateSystem.Unsubscribe(onBeforeRenderNode);
            onBeforeRenderNode = null;
        }

        protected void UnsuscribeOnGUI() {
            if(onGUINode != null)
                UpdateSystem.Unsubscribe(onGUINode);
            onGUINode = null;
        }

        protected void UnsubscribeTimer(OnTimerUpdateCallback callback) => UpdateSystem.Unsubscribe(this, callback);

        #endregion

        #region Message

        protected void Subscribe<T>(System.Action<T> method) {
            Message.Subscribe(this, method);
        }

        protected void Unsubscribe<T>(System.Action<T> method) {
            Message.Unsubscribe<T>(this, method);
        }

        protected void Publish<T>(T message) where T : class {
            Message.Publish(message);
        }

        #endregion
    }
}
