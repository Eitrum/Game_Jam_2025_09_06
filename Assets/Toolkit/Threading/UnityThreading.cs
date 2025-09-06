using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Threading
{
    public class UnityThreading : MonoSingleton<UnityThreading>
    {
        public delegate void OnCloseCallback();

        #region Singleton

        [RuntimeInitializeOnLoadMethod]
        private static void Create() { var inst = Instance; }
        protected override bool KeepAlive { get => true; set => base.KeepAlive = value; }

        #endregion

        #region Variables

        private static OnCloseCallback onClose;
        private static bool isClosing = false;
        private TLinkedList<IMainThreadCallback> mainThreadCallbacks = new TLinkedList<IMainThreadCallback>();

        #endregion

        #region Properties

        public static event OnCloseCallback OnClose {
            add {
                if(isClosing) {
                    value();
                    return;
                }
                onClose += value;
            }
            remove {
                if(isClosing) {
                    return;
                }
                onClose -= value;
            }
        }

        #endregion

        #region Update

        private void Update() {
            if(mainThreadCallbacks.Count == 0)
                return;
            var temp = mainThreadCallbacks.GetEnumerator();
            lock(mainThreadCallbacks) {
                mainThreadCallbacks = new TLinkedList<IMainThreadCallback>();
            }
            IMainThreadCallback obj;
            while(temp.MoveNext(out obj)) {
                obj.Handle();
            }
        }

        #endregion

        #region Subscribe

        public static void AddMainThreadCallback(IMainThreadCallback callback) {
            lock(Instance.mainThreadCallbacks)
                Instance.mainThreadCallbacks.Add(callback);
        }

        #endregion

        #region OnDestroy

        private void OnDestroy() {
            isClosing = true;
            onClose?.Invoke();
            onClose = null;
        }

        #endregion
    }
}
