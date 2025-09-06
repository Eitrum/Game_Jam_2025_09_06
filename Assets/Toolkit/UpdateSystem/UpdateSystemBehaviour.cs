using System;
using UnityEngine;

namespace Toolkit
{
    public class UpdateSystemBehaviour : MonoSingleton<UpdateSystemBehaviour>
    {
        #region Singleton

        protected override bool KeepAlive { get => true; set => base.KeepAlive = value; }

        #endregion

        #region Variables

        public TLinkedList<IEarlyUpdate> earlyUpdates = new TLinkedList<IEarlyUpdate>();
        public TLinkedList<IUpdate> updates = new TLinkedList<IUpdate>();
        public TLinkedList<ILateUpdate> lateUpdates = new TLinkedList<ILateUpdate>();
        public TLinkedList<IPostUpdate> postUpdates = new TLinkedList<IPostUpdate>();
        public TLinkedList<IFixedUpdate> fixedUpdates = new TLinkedList<IFixedUpdate>();
        public TLinkedList<IOnBeforeRender> onBeforeRender = new TLinkedList<IOnBeforeRender>();
        public TLinkedList<IOnGUI> onGUIs = new TLinkedList<IOnGUI>();

#if !DISABLE_SCENE_LOADING_CHECK
        private bool isLoadingScene = false;
#endif

        #endregion

        #region On Disable

        private void OnEnable() {
            Application.onBeforeRender += OnBeforeRender;
        }

        private void OnDisable() {
            Application.onBeforeRender -= OnBeforeRender;
#if !UNITY_EDITOR
            Debug.LogError($"<color=cyan>[UpdateSystem]</color> - Somehow got disabled, this will crash a lot of update loops in the game!!!");
#endif
        }

        #endregion

        #region Update Functions

        private void Update() {
#if (!DISABLE_SCENE_LOADING_CHECK) && (!DISABLE_UPDATE_SYSTEM_AUTOMATIC_DESTROY)
            isLoadingScene = SceneManagement.SceneUtility.IsLoading;
#endif
            var dt = Time.deltaTime;

            // Early Update
            using(var earlyUpdateEnumerator = earlyUpdates.GetEnumerator()) {
                TLinkedListNode<IEarlyUpdate> earlyUpdateNode;
                while(earlyUpdateEnumerator.MoveNext(out earlyUpdateNode)) {
                    if(earlyUpdateNode.Value.IsNull) {
#if !DISABLE_UPDATE_SYSTEM_AUTOMATIC_DESTROY
#if !DISABLE_SCENE_LOADING_CHECK
                        if(!isLoadingScene)
#endif
                            earlyUpdateEnumerator.DestroyCurrent();
#endif
                    }
                    else
                        earlyUpdateNode.Value.EarlyUpdate(dt);
                }
            }

            // Update
            using(var updateEnumerator = updates.GetEnumerator()) {
                TLinkedListNode<IUpdate> updateNode;
                while(updateEnumerator.MoveNext(out updateNode)) {
                    if(updateNode.Value.IsNull) {
#if !DISABLE_UPDATE_SYSTEM_AUTOMATIC_DESTROY
#if !DISABLE_SCENE_LOADING_CHECK
                        if(!isLoadingScene)
#endif
                            updateEnumerator.DestroyCurrent();
#endif
                    }
                    else
                        updateNode.Value.Update(dt);
                }
            }
        }

        private void LateUpdate() {
#if (!DISABLE_SCENE_LOADING_CHECK) && (!DISABLE_UPDATE_SYSTEM_AUTOMATIC_DESTROY)
            isLoadingScene = SceneManagement.SceneUtility.IsLoading;
#endif
            var dt = Time.deltaTime;
            // Late Update
            using(var lateUpdateEnumerator = lateUpdates.GetEnumerator()) {
                TLinkedListNode<ILateUpdate> lateUpdateNode;
                while(lateUpdateEnumerator.MoveNext(out lateUpdateNode)) {
                    if(lateUpdateNode.Value.IsNull) {
#if !DISABLE_UPDATE_SYSTEM_AUTOMATIC_DESTROY
#if !DISABLE_SCENE_LOADING_CHECK
                        if(!isLoadingScene)
#endif
                            lateUpdateEnumerator.DestroyCurrent();
#endif
                    }
                    else
                        lateUpdateNode.Value.LateUpdate(dt);
                }
            }

            // Post Update
            using(var postUpdateEnumerator = postUpdates.GetEnumerator()) {
                TLinkedListNode<IPostUpdate> postUpdateNode;
                while(postUpdateEnumerator.MoveNext(out postUpdateNode)) {
                    if(postUpdateNode.Value.IsNull) {
#if !DISABLE_UPDATE_SYSTEM_AUTOMATIC_DESTROY
#if !DISABLE_SCENE_LOADING_CHECK
                        if(!isLoadingScene)
#endif
                            postUpdateEnumerator.DestroyCurrent();
#endif
                    }
                    else
                        postUpdateNode.Value.PostUpdate(dt);
                }
            }
        }

        private void FixedUpdate() {
#if (!DISABLE_SCENE_LOADING_CHECK) && (!DISABLE_UPDATE_SYSTEM_AUTOMATIC_DESTROY)
            isLoadingScene = SceneManagement.SceneUtility.IsLoading;
#endif
            var dt = Time.fixedDeltaTime;
            using(var fixedUpdateEnumerator = fixedUpdates.GetEnumerator()) {
                TLinkedListNode<IFixedUpdate> fixedUpdateNode;
                while(fixedUpdateEnumerator.MoveNext(out fixedUpdateNode)) {
                    if(fixedUpdateNode.Value.IsNull) {
#if !DISABLE_UPDATE_SYSTEM_AUTOMATIC_DESTROY
#if !DISABLE_SCENE_LOADING_CHECK
                        if(!isLoadingScene)
#endif
                            fixedUpdateEnumerator.DestroyCurrent();
#endif
                    }
                    else
                        fixedUpdateNode.Value.FixedUpdate(dt);
                }
            }
        }

        private void OnBeforeRender() {
#if (!DISABLE_SCENE_LOADING_CHECK) && (!DISABLE_UPDATE_SYSTEM_AUTOMATIC_DESTROY)
            isLoadingScene = SceneManagement.SceneUtility.IsLoading;
#endif
            var dt = Time.deltaTime;
            using(var onRenderObjectsEnumerator = onBeforeRender.GetEnumerator()) {
                TLinkedListNode<IOnBeforeRender> onRenderObjectsNode;
                while(onRenderObjectsEnumerator.MoveNext(out onRenderObjectsNode)) {
                    if(onRenderObjectsNode.Value.IsNull) {
#if !DISABLE_UPDATE_SYSTEM_AUTOMATIC_DESTROY
#if !DISABLE_SCENE_LOADING_CHECK
                        if(!isLoadingScene)
#endif
                            onRenderObjectsEnumerator.DestroyCurrent();
#endif
                    }
                    else
                        onRenderObjectsNode.Value.OnBeforeRender(dt);
                }
            }
        }
        /* CAUSES GC
        private void OnGUI() {
            var dt = Time.deltaTime;
            using(var onGUIEnumerator = onGUIs.GetEnumerator()) {
                TLinkedListNode<IOnGUI> onGUINode;
                while(onGUIEnumerator.MoveNext(out onGUINode)) {
                    if(onGUINode.Value.IsNull) {
#if !DISABLE_UPDATE_SYSTEM_AUTOMATIC_DESTROY
#if !DISABLE_SCENE_LOADING_CHECK
                        if(!isLoadingScene)
#endif
                            onGUIEnumerator.DestroyCurrent();
#endif
                    }
                    else
                        onGUINode.Value.OnGUI(dt);
                }
            }
        }*/

        #endregion
    }
}
