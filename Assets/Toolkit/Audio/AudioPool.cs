using UnityEngine;

namespace Toolkit.Audio
{
    public class AudioPool : NullableBehaviour
    {
        #region Variables

        public const int MAX_POOL_COUNT = 64;

        private PoolMode mode = PoolMode.None;
        private int size = 0;
        private IAudioSourceSettings settings = null;

        private int id = 0;

        private TLinkedList<AudioPoolObject> freeObjects = new TLinkedList<AudioPoolObject>();
        private TLinkedList<AudioPoolObject> activeObjects = new TLinkedList<AudioPoolObject>();

        #endregion

        #region Properties

        public PoolMode Mode {
            get => mode;
            set {
                mode = value;
                UpdatePool();
            }
        }

        public int Size {
            get => size;
            set {
                size = Mathf.Clamp(value, 1, MAX_POOL_COUNT);
                UpdatePool();
            }
        }

        public IAudioSourceSettings Settings {
            get => settings;
            set {
                this.settings = value;
                UpdatePool();
            }
        }

        public TLinkedListEnumerator<AudioPoolObject> FreeObjects => freeObjects.GetEnumerator();
        public TLinkedListEnumerator<AudioPoolObject> ActiveObjects => activeObjects.GetEnumerator();

        #endregion

        #region Initialize

        private void UpdatePool() {
            ClearNullObjects();
            switch(mode) {
                case PoolMode.None:
                    DestroyAllInactive();
                    DestroyAllFree();
                    break;
                case PoolMode.Circular: {
                        if(activeObjects.Count > 0)
                            DestroyAllActive();
                        while(freeObjects.Count > size) {
                            var first = freeObjects.FirstNode;
                            first.Value.Destroy();
                            freeObjects.Remove(first);
                        }
                        while(freeObjects.Count < size)
                            freeObjects.AddFirst(CreateObject());
                    }
                    break;
                case PoolMode.Dynamic: {
                        MoveInactiveObjectsToFree();
                        var total = freeObjects.Count + activeObjects.Count;
                        if(total < size)
                            for(int i = 0, length = size - total; i < length; i++)
                                freeObjects.Add(CreateObject());
                        while(freeObjects.Count > size) {
                            var last = freeObjects.LastNode;
                            last.Value.Destroy();
                            freeObjects.Remove(last);
                        }
                    }
                    break;
                case PoolMode.Static: {
                        MoveInactiveObjectsToFree();
                        var total = freeObjects.Count + activeObjects.Count;
                        var diff = size - total;
                        while(diff < 0 && freeObjects.Count > 0) {
                            diff++;
                            var last = freeObjects.LastNode;
                            last.Value.Destroy();
                            freeObjects.Remove(last);
                        }
                        while(diff > 0) {
                            diff--;
                            freeObjects.Add(CreateObject());
                        }
                    }
                    break;
            }
        }

        private void Initialize(IAudioSourceSettings settings, PoolMode mode, int size) {
            this.settings = settings;
            this.mode = mode;
            this.size = Mathf.Clamp(size, 1, MAX_POOL_COUNT);
            UpdatePool();
        }

        public static AudioPool Create(string name, IAudioSourceSettings settings, PoolMode mode, int size) {
            var go = new GameObject(name);
            AudioUtility.AudioScene.AddGameObject(go);
            var ap = go.AddComponent<AudioPool>();
            ap.Initialize(settings, mode, size);
            return ap;
        }

        #endregion

        #region Get Source

        public bool GetAudioObject(out AudioPoolObject obj) {
            switch(mode) {
                case PoolMode.Dynamic: return GetAudioObject_Dynamic(out obj);
                case PoolMode.Static: return GetAudioObject_Static(out obj);
                case PoolMode.Circular: return GetAudioObject_Circular(out obj);
                case PoolMode.None: return GetAudioObject_None(out obj);
            }
            obj = null;
            return false;
        }

        private bool GetAudioObject_None(out AudioPoolObject obj) {
            DestroyAllInactive();
            obj = CreateObject();
            activeObjects.Add(obj);
            OnPoolCreate(obj);
            return true;
        }

        private bool GetAudioObject_Static(out AudioPoolObject obj) {
            if(freeObjects.Count == 0) {
                UpdatePool();
            }
            if(freeObjects.Count == 0) {
                obj = null;
                return false;
            }
            var first = freeObjects.FirstNode;
            var value = first.Value;
            freeObjects.Remove(first);
            if(value == null)
                value = CreateObject();
            activeObjects.Add(value);
            OnPoolCreate(value);
            obj = value;
            return true;
        }

        private bool GetAudioObject_Dynamic(out AudioPoolObject obj) {
            if(freeObjects.Count == 0) {
                UpdatePool();
            }
            if(freeObjects.Count == 0) {
                obj = CreateObject();
                activeObjects.Add(obj);
                OnPoolCreate(obj);
                return true;
            }
            var first = freeObjects.FirstNode;
            var value = first.Value;
            freeObjects.Remove(first);
            if(value == null)
                value = CreateObject();
            activeObjects.Add(value);
            OnPoolCreate(value);
            obj = value;
            return true;
        }

        private bool GetAudioObject_Circular(out AudioPoolObject obj) {
            if(freeObjects.Count == 0)
                UpdatePool();
            var first = freeObjects.FirstNode;
            var value = first.Value;
            freeObjects.Remove(first);
            if(value == null)
                value = CreateObject();
            freeObjects.Add(value);
            OnPoolCreate(value);
            obj = value;
            return true;
        }

        #endregion

        #region Utility

        private AudioPoolObject CreateObject() {
            var go = new GameObject($"{name} : {id++}");
            go.transform.SetParent(this.transform);
            var apo = go.AddComponent<AudioPoolObject>();
            apo.ApplySettings(this, settings);
            return apo;
        }

        private void DestroyAllActive() {
            var enu = activeObjects.GetEnumerator();
            while(enu.MoveNext(out AudioPoolObject apo)) {
                if(apo != null) {
                    apo.Destroy();
                }
            }
            activeObjects.Clear();
        }

        private void DestroyAllFree() {
            var enu = freeObjects.GetEnumerator();
            while(enu.MoveNext(out AudioPoolObject apo)) {
                if(apo != null) {
                    apo.Destroy();
                }
            }
            freeObjects.Clear();
        }

        private void DestroyAllInactive() {
            var enu = activeObjects.GetEnumerator();
            while(enu.MoveNext(out AudioPoolObject apo)) {
                if(apo != null) {
                    if(apo.IsFree) {
                        apo.Destroy();
                        enu.DestroyCurrent();
                    }
                }
                else
                    enu.DestroyCurrent();
            }
        }

        private void ClearNullObjects() {
            var free = freeObjects.GetEnumerator();
            while(free.MoveNext(out AudioPoolObject obj))
                if(obj == null)
                    free.DestroyCurrent();

            var active = activeObjects.GetEnumerator();
            while(active.MoveNext(out AudioPoolObject obj))
                if(obj == null)
                    active.DestroyCurrent();
        }

        private void MoveInactiveObjectsToFree() {
            var active = activeObjects.GetEnumerator();
            while(active.MoveNext(out AudioPoolObject obj))
                if(obj.IsFree) {
                    OnPoolDestroy(obj);
                    active.DestroyCurrent();
                    freeObjects.Add(obj);
                }
        }

        private void OnPoolCreate(AudioPoolObject apo) => (apo as IPoolable).OnPoolInitialize();
        private void OnPoolDestroy(AudioPoolObject apo) => (apo as IPoolable).OnPoolDestroy();

        #endregion
    }
}
