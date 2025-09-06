using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Toolkit {
    public interface ISelectable {
        void OnSelect();
        void OnDeselect();
    }

    public class TSelection { // Figure out naming and how to make it work with UnityEditor.Selection

        #region Variables

        private TLinkedList<object> selectedObjects = new TLinkedList<object>();
        public event Action<object> OnSelected;
        public event Action<object> OnDeselected;

        #endregion

        #region Properties

        public int SelectedCount => selectedObjects.Count;
        public TLinkedListEnumerator<object> Enumerator => selectedObjects.GetEnumerator();

        public object[] Objects {
            get => GetAll();
            set => Set(value);
        }

        #endregion

        #region Constructor

        public TSelection() { }
        
        ~TSelection() {
            Clear();
        }

        #endregion

        #region Utility

        public bool IsSelected<T>(T item) => ((T)selectedObjects.Find(item)?.Value) != null;

        #endregion

        #region Add

        public void Add(Transform t) => Add(t.gameObject);
        public void Add(GameObject go) {
            if(!selectedObjects.Contains(go)) {
                selectedObjects.Add(go);
                var iSels = go.GetComponents<ISelectable>();
                for(int i = 0, length = iSels.Length; i < length; i++) {
                    iSels[i].OnSelect();
                }
                OnSelected?.Invoke(go);
            }
        }

        public void Add<T>(T obj) where T : ISelectable {
            if(!selectedObjects.Contains(obj)) {
                selectedObjects.Add(obj);
                obj.OnSelect();
                OnSelected?.Invoke(obj);
            }
        }

        public void Add(object obj) {
            if(!selectedObjects.Contains(obj)) {
                selectedObjects.Add(obj);
                if(obj is ISelectable iSel)
                    iSel.OnSelect();
                OnSelected?.Invoke(obj);
            }
        }

        public void Add<T>(IList<T> range) where T : ISelectable {
            for(int i = 0, length = range.Count; i < length; i++) {
                Add(range[i]);
            }
        }

        public void Add(IList<GameObject> gameObjects) {
            for(int i = 0, length = gameObjects.Count; i < length; i++) {
                Add(gameObjects[i]);
            }
        }

        public void Add(IList<Transform> transforms) {
            for(int i = 0, length = transforms.Count; i < length; i++) {
                Add(transforms[i]);
            }
        }

        public void Add(IList<object> objs) {
            for(int i = 0, length = objs.Count; i < length; i++) {
                Add(objs[i]);
            }
        }

        #endregion

        #region Remove

        public void Remove(Transform t) => Remove(t.gameObject);
        public void Remove(GameObject go) {
            if(selectedObjects.Remove(go)) {
                var iSels = go.GetComponents<ISelectable>();
                for(int i = 0, length = iSels.Length; i < length; i++) {
                    iSels[i].OnDeselect();
                }
                OnDeselected?.Invoke(go);
            }
        }

        public void Remove<T>(T obj) where T : ISelectable {
            if(selectedObjects.Remove(obj)) {
                obj.OnDeselect();
                OnDeselected?.Invoke(obj);
            }
        }

        public void Remove(object o) {
            if(selectedObjects.Remove(o)) {
                if(o is ISelectable iSel)
                    iSel.OnDeselect();
                OnDeselected?.Invoke(o);
            }
        }

        public void Remove<T>(IList<T> range) where T : ISelectable {
            for(int i = 0, length = range.Count; i < length; i++) {
                Remove(range[i]);
            }
        }

        public void Remove(IList<GameObject> range) {
            for(int i = 0, length = range.Count; i < length; i++) {
                Remove(range[i]);
            }
        }

        public void Remove(IList<Transform> range) {
            for(int i = 0, length = range.Count; i < length; i++) {
                Remove(range[i]);
            }
        }

        public void Remove(IList<object> range) {
            for(int i = 0, length = range.Count; i < length; i++) {
                Remove(range[i]);
            }
        }

        #endregion

        #region Set

        public void Set<T>(T obj) where T : ISelectable {
            Clear();
            Add(obj);
        }

        public void Set(GameObject go) {
            Clear();
            Add(go);
        }

        public void Set(Transform transform) {
            Clear();
            Add(transform);
        }

        public void Set(object o) {
            Clear();
            Add(o);
        }

        public void Set<T>(IList<T> array) where T : ISelectable {
            Clear();
            Add(array);
        }

        public void Set(IList<GameObject> array) {
            Clear();
            Add(array);
        }

        public void Set(IList<Transform> array) {
            Clear();
            Add(array);
        }

        public void Set(IList<object> array) {
            Clear();
            Add(array);
        }

        #endregion

        #region Clear

        public void Clear() {
            var iterator = selectedObjects.GetEnumerator();
            while(iterator.MoveNext(out object o)) {
                if(o is ISelectable iSel) {
                    iSel.OnDeselect();
                }
                else if(o is GameObject go) {
                    var iSels = go.GetComponents<ISelectable>();
                    for(int i = 0, length = iSels.Length; i < length; i++) {
                        iSels[i].OnDeselect();
                    }
                }
                else if(o is Transform transform) {
                    var iSels = transform.GetComponents<ISelectable>();
                    for(int i = 0, length = iSels.Length; i < length; i++) {
                        iSels[i].OnDeselect();
                    }
                }
                OnDeselected?.Invoke(o);
            }
            selectedObjects.Clear();
        }

        public void ClearNulls() {
            var iterator = selectedObjects.GetEnumerator();
            while(iterator.MoveNext(out object o)) {
                if(o == null) {
                    iterator.DestroyCurrent();
                    continue;
                }
                if(o is UnityEngine.Object uObj && uObj == null) {
                    iterator.DestroyCurrent();
                }
            }
        }

        #endregion

        #region Getters

        public T[] GetAll<T>() => selectedObjects.Where(x => x is T).Select(x => (T)x).ToArray();
        public object[] GetAll() => selectedObjects.ToArray();

        public int GetAllNonAlloc<T>(IList<T> array) {
            var iterator = Enumerator;
            int index = 0;
            while(iterator.MoveNext(out object o)) {
                if(o is T t)
                    array[index++] = t;
            }
            return index;
        }

        public int GetAllNonAlloc(IList<object> array) {
            var iterator = Enumerator;
            int index = 0;
            while(iterator.MoveNext(out object o)) {
                array[index++] = o;
            }
            return index;
        }

        #endregion
    }
}
