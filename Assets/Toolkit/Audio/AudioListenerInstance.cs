using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Audio
{
    [AddComponentMenu("Toolkit/Audio/Audio Listener Instance")]
    [RequireComponent(typeof(AudioListener))]
    public class AudioListenerInstance : MonoBehaviour, INullable
    {
        public delegate void ListenerActiveCallback(bool active);

        #region Variables

        [SerializeField] private int priority = 0;
        private ListenerActiveCallback onActive;
        private AudioListener listener;

        private static List<AudioListenerInstance> instances = new List<AudioListenerInstance>();
        private static AudioListenerInstance activeInstance;

        #endregion

        #region Properties

        public int Priority => priority;
        bool INullable.IsNull => this == null;
        public event ListenerActiveCallback OnActive {
            add => onActive += value;
            remove => onActive -= value;
        }

        public static AudioListenerInstance ActiveInstance => activeInstance;
        public static IReadOnlyList<AudioListenerInstance> Instances => instances;

        #endregion

        #region Unity Methods

        private void Awake() {
            listener = GetComponent<AudioListener>();
            listener.enabled = false;
        }

        private void OnEnable() {
            for(int i = 0, length = instances.Count; i < length; i++) {
                if(instances[i].priority < priority) {
                    instances.Insert(i, this);
                    if(i == 0)
                        SetActive();
                    return;
                }
            }
            instances.Add(this);
            if(activeInstance == null)
                SetActive();
        }

        private void OnDisable() {
            instances.Remove(this);
            if(activeInstance == this) {
                SetInactive();
            }
        }

        #endregion

        #region Active Inactive Methods

        public void SetActive() {
            if(activeInstance != null)
                activeInstance.SetActive(false);
            activeInstance = this;
            activeInstance.SetActive(true);
            onActive?.Invoke(true);
        }

        public void SetInactive() {
            if(activeInstance != this) {
                return;
            }
            if(activeInstance != null) {
                activeInstance.SetActive(false);
                activeInstance = null;
                onActive?.Invoke(false);
            }
            if(instances.Count > 0) {
                instances[0].SetActive();
            }
        }

        private void SetActive(bool active) => listener.enabled = active;

        #endregion

        #region Public Static

        public static void SetClosestListenerActive(Vector3 position) {
            if(instances.Count > 0)
                return;
            int index = 0;
            float dist = float.MaxValue;
            for(int i = 0, length = instances.Count; i < length; i++) {
                var d = Vector3.Distance(instances[i].transform.position, position);
                if(d < dist) {
                    index = i;
                    dist = d;
                }
            }
            instances[index].SetActive();
        }

        #endregion
    }
}
