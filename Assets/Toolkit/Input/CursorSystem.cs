using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.InputSystem {
    public static class CursorSystem {

        #region Nullable State Stack

        private class NullableStateStack<T> {
            public class Data {
                public readonly INullable Nullable;
                public readonly T Value;
                public readonly int Order;

                public Data(INullable nullable, T value) {
                    this.Nullable = nullable;
                    this.Value = value;
                    Order = globalOrder++;
                }
            }

            #region Variables

            private T defaultValue = default(T);
            private List<Data> data = new List<Data>();

            #endregion

            #region Properties

            public T Default => defaultValue;
            public IReadOnlyList<Data> Values => data;

            #endregion

            #region Constructor

            public NullableStateStack() { }
            public NullableStateStack(T defaultValue) {
                this.defaultValue = defaultValue;
            }

            #endregion

            #region SetDefault

            public void SetDefault(T def) {
                this.defaultValue = def;
            }

            #endregion

            #region Add

            public void Add(INullable nullable, T item) {
                data.Add(new Data(nullable, item));
            }

            #endregion

            #region Remove

            public void Remove(INullable nullable, T item) {
                for(int i = data.Count - 1; i >= 0; i--) {
                    if(data[i].Nullable == nullable && data[i].Value.Equals(item)) {
                        data.RemoveAt(i);
                    }
                }
            }

            public void Remove(INullable nullable) {
                for(int i = data.Count - 1; i >= 0; i--) {
                    if(data[i].Nullable == nullable) {
                        data.RemoveAt(i);
                    }
                }
            }

            public void RemoveAll(System.Predicate<Data> removeFunction) {
                for(int i = data.Count - 1; i >= 0; i--) {
                    if(removeFunction(data[i])) {
                        data.RemoveAt(i);
                    }
                }
            }

            #endregion

            #region Util

            public T Last() {
                for(int i = data.Count - 1; i >= 0; i--)
                    if(data[i].Nullable.IsNull)
                        data.RemoveAt(i);
                if(data.Count == 0)
                    return defaultValue;
                return data[data.Count - 1].Value;
            }

            public void Clear() {
                data.Clear();
            }

            #endregion
        }

        #endregion

        #region Variables

        private static int globalOrder;

        private static bool lockFromChanges = false;
        private static NullableStateStack<CursorLockModeSetting> LockMode = new NullableStateStack<CursorLockModeSetting>();
        private static NullableStateStack<bool> Visible = new NullableStateStack<bool>();
        private static NullableStateStack<CursorObject> Texture = new NullableStateStack<CursorObject>();
        private static UnityObjectNullable lastUnityObjectNullable;

        private static CursorObject nullCursor;
        private static bool isCursorConfinedByDefault = true;

        public static event System.Action OnCursorChanged;

        #endregion

        #region Properties

        public static bool IsCursorConfinedByDefault {
            get => isCursorConfinedByDefault;
            set {
                if(value == isCursorConfinedByDefault)
                    return;
                isCursorConfinedByDefault = value;
                ForceUpdate();
            }
        }

        public static bool LockFromChanges {
            get => lockFromChanges;
            set {
                if(value != lockFromChanges)
                    return;

                value = lockFromChanges;
                if(!value)
                    ForceUpdate();
            }
        }

        public static CursorObject NullCursor {
            get {
                if(nullCursor == null)
                    nullCursor = CursorObject.CreateInstance<CursorObject>();
                return nullCursor;
            }
        }

        public static IEnumerable<CursorLockMode> CurrentLockModeStack {
            get {
                yield return LockMode.Default.ToUnity();
                foreach(var lm in LockMode.Values)
                    yield return lm.Value.ToUnity();
            }
        }

        public static IEnumerable<CursorLockModeSetting> CurrentLockModeSettingsStack {
            get {
                yield return LockMode.Default;
                foreach(var lm in LockMode.Values)
                    yield return lm.Value;
            }
        }

        public static IEnumerable<bool> CurrentVisibilityStack {
            get {
                yield return Visible.Default;
                foreach(var lm in Visible.Values)
                    yield return lm.Value;
            }
        }

        public static IEnumerable<CursorObject> CurrentTextures {
            get {
                yield return Texture.Default;
                foreach(var lm in Texture.Values)
                    yield return lm.Value;
            }
        }

        internal static IEnumerable<(int type, object o, int order, INullable nullable)> GetAll() {
            yield return (0, LockMode.Default, 0, null);
            yield return (1, Visible.Default, 0, null);
            yield return (2, Texture.Default, 0, null);

            foreach(var lm in LockMode.Values)
                yield return (0, lm.Value, lm.Order, lm.Nullable);

            foreach(var lm in Visible.Values)
                yield return (1, lm.Value, lm.Order, lm.Nullable);

            foreach(var lm in Texture.Values)
                yield return (2, lm.Value, lm.Order, lm.Nullable);
        }

        #endregion

        #region Constructor

        static CursorSystem() {
            SetDefaults(CursorLockModeSetting.None, true, null);
            UpdateAll();
        }

        public static void SetDefaults(CursorLockModeSetting lockMode, bool visible, CursorObject cursorObject) {
            LockMode.SetDefault(lockMode);
            Visible.SetDefault(visible);
            Texture.SetDefault(cursorObject);
        }

        #endregion

        #region Update

        public static void ForceUpdate() {
            var lockMode = LockMode.Last();
            var visible = Visible.Last();
            var texture = Texture.Last();

            Cursor.lockState = lockMode.ToUnity();
            Cursor.visible = visible;
            Cursor.SetCursor(texture.Texture, texture.HotSpot, texture.Mode);

            OnCursorChanged?.Invoke();
        }

        public static void UpdateAll() {
            if(lockFromChanges)
                return;
            Cursor.lockState = LockMode.Last().ToUnity();
            Cursor.visible = Visible.Last();
            var texture = Texture.Last() ?? NullCursor;
            Cursor.SetCursor(texture.Texture, texture.HotSpot, texture.Mode);

            OnCursorChanged?.Invoke();
        }

        public static void UpdateLockMode() {
            if(lockFromChanges)
                return;
            Cursor.lockState = LockMode.Last().ToUnity();
            OnCursorChanged?.Invoke();
        }

        public static void UpdateVisibility() {
            if(lockFromChanges)
                return;
            Cursor.visible = Visible.Last();
            OnCursorChanged?.Invoke();
        }

        public static void UpdateTexture() {
            if(lockFromChanges)
                return;
            var texture = Texture.Last();

            if(texture == null)
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            else
                Cursor.SetCursor(texture.Texture, texture.HotSpot, texture.Mode);
            OnCursorChanged?.Invoke();
        }

        #endregion

        #region Util

        public static bool GetCurrent(out CursorLockMode lockMode, out bool visible, out CursorObject cursorObject) {
            lockMode = LockMode.Last().ToUnity();
            visible = Visible.Last();
            cursorObject = Texture.Last();
            if(cursorObject == null)
                cursorObject = NullCursor;

            return true;
        }

        public static void ClearAll(bool forceUpdate = true) {
            LockMode.Clear();
            Visible.Clear();
            Texture.Clear();
            if(forceUpdate)
                ForceUpdate();
        }

        public static CursorLockMode ToUnity(this CursorLockModeSetting setting) {
            switch(setting) {
                case CursorLockModeSetting.Locked: return CursorLockMode.Locked;
                case CursorLockModeSetting.Confined: return CursorLockMode.Confined;
                case CursorLockModeSetting.Free: return CursorLockMode.None;
                case CursorLockModeSetting.None: return CursorLockMode.None;
                case CursorLockModeSetting.UseDefaultPlayerSetting: return isCursorConfinedByDefault ? CursorLockMode.Confined : CursorLockMode.None;
            }
            return CursorLockMode.None;
        }

        private static UnityObjectNullable GetLastOrCreate(UnityEngine.Object obj) {
            if(lastUnityObjectNullable == null)
                lastUnityObjectNullable = new UnityObjectNullable(obj);
            if(lastUnityObjectNullable.obj == obj)
                return lastUnityObjectNullable;
            lastUnityObjectNullable = new UnityObjectNullable(obj);
            return lastUnityObjectNullable;
        }

        #endregion

        #region Remove

        public static void Remove<T>(T nullable) where T : INullable {
            Texture.Remove(nullable);
            LockMode.Remove(nullable);
            Visible.Remove(nullable);
            UpdateTexture();
            UpdateVisibility();
            UpdateLockMode();
        }

        public static void Remove(UnityEngine.Object obj) {
            Texture.RemoveAll((x => x.Nullable is UnityObjectNullable uon && uon.obj == obj));
            LockMode.RemoveAll((x => x.Nullable is UnityObjectNullable uon && uon.obj == obj));
            Visible.RemoveAll((x => x.Nullable is UnityObjectNullable uon && uon.obj == obj));
            UpdateTexture();
            UpdateVisibility();
            UpdateLockMode();
        }

        public static void Remove(RectTransform unityObject)
            => Remove(unityObject as UnityEngine.Object);

        public static void Remove(Transform unityObject)
            => Remove(unityObject as UnityEngine.Object);

        public static void Remove(GameObject unityObject)
            => Remove(unityObject as UnityEngine.Object);

        public static void Remove(MonoBehaviour unityObject)
            => Remove(unityObject as UnityEngine.Object);

        #endregion

        #region Add/Remove Cursor

        public static void Add<T>(T nullable, CursorObject cursor) where T : INullable {
            Texture.Add(nullable, cursor);
            UpdateTexture();
        }

        public static void Add(RectTransform unityObject, CursorObject cursor)
            => Add(GetLastOrCreate(unityObject), cursor);

        public static void Add(Transform unityObject, CursorObject cursor)
            => Add(GetLastOrCreate(unityObject), cursor);

        public static void Add(GameObject unityObject, CursorObject cursor)
            => Add(GetLastOrCreate(unityObject), cursor);

        public static void Add(MonoBehaviour unityObject, CursorObject cursor)
            => Add(GetLastOrCreate(unityObject), cursor);


        public static void Remove<T>(T nullable, CursorObject cursor) where T : INullable {
            Texture.Remove(nullable, cursor);
            UpdateTexture();
        }

        public static void Remove(UnityEngine.Object obj, CursorObject cursor) {
            Texture.RemoveAll((x => x.Nullable is UnityObjectNullable uon && uon.obj == obj && x.Value == cursor));
            UpdateTexture();
        }

        public static void Remove(RectTransform unityObject, CursorObject cursor)
            => Remove(unityObject as UnityEngine.Object, cursor);

        public static void Remove(Transform unityObject, CursorObject cursor)
            => Remove(unityObject as UnityEngine.Object, cursor);

        public static void Remove(GameObject unityObject, CursorObject cursor)
            => Remove(unityObject as UnityEngine.Object, cursor);

        public static void Remove(MonoBehaviour unityObject, CursorObject cursor)
            => Remove(unityObject as UnityEngine.Object, cursor);

        #endregion

        #region Add/Remove Visibility

        public static void Add<T>(T nullable, bool visibility) where T : INullable {
            Visible.Add(nullable, visibility);
            UpdateVisibility();
        }

        public static void Add(RectTransform unityObject, bool visibility)
            => Add(GetLastOrCreate(unityObject), visibility);

        public static void Add(Transform unityObject, bool visibility)
            => Add(GetLastOrCreate(unityObject), visibility);

        public static void Add(GameObject unityObject, bool visibility)
            => Add(GetLastOrCreate(unityObject), visibility);

        public static void Add(MonoBehaviour unityObject, bool visibility)
            => Add(GetLastOrCreate(unityObject), visibility);


        public static void Remove<T>(T nullable, bool visibility) where T : INullable {
            Visible.Remove(nullable, visibility);
            UpdateVisibility();
        }

        public static void Remove(UnityEngine.Object obj, bool visibility) {
            Visible.RemoveAll((x => x.Nullable is UnityObjectNullable uon && uon.obj == obj && x.Value == visibility));
            UpdateVisibility();
        }

        public static void Remove(RectTransform unityObject, bool visibility)
            => Remove(unityObject as UnityEngine.Object, visibility);

        public static void Remove(Transform unityObject, bool visibility)
            => Remove(unityObject as UnityEngine.Object, visibility);

        public static void Remove(GameObject unityObject, bool visibility)
            => Remove(unityObject as UnityEngine.Object, visibility);

        public static void Remove(MonoBehaviour unityObject, bool visibility)
            => Remove(unityObject as UnityEngine.Object, visibility);

        #endregion

        #region Add/Remove LockMode

        public static void Add<T>(T nullable, CursorLockModeSetting lockMode) where T : INullable {
            LockMode.Add(nullable, lockMode);
            UpdateLockMode();
        }

        public static void Add(RectTransform unityObject, CursorLockModeSetting lockMode)
            => Add(GetLastOrCreate(unityObject), lockMode);

        public static void Add(Transform unityObject, CursorLockModeSetting lockMode)
            => Add(GetLastOrCreate(unityObject), lockMode);

        public static void Add(GameObject unityObject, CursorLockModeSetting lockMode)
            => Add(GetLastOrCreate(unityObject), lockMode);

        public static void Add(MonoBehaviour unityObject, CursorLockModeSetting lockMode)
            => Add(GetLastOrCreate(unityObject), lockMode);


        public static void Remove<T>(T nullable, CursorLockModeSetting lockMode) where T : INullable {
            LockMode.Remove(nullable, lockMode);
            UpdateLockMode();
        }

        public static void Remove(UnityEngine.Object obj, CursorLockModeSetting lockMode) {
            LockMode.RemoveAll((x => x.Nullable is UnityObjectNullable uon && uon.obj == obj && x.Value == lockMode));
            UpdateLockMode();
        }

        public static void Remove(RectTransform unityObject, CursorLockModeSetting lockMode)
            => Remove(unityObject as UnityEngine.Object, lockMode);

        public static void Remove(Transform unityObject, CursorLockModeSetting lockMode)
            => Remove(unityObject as UnityEngine.Object, lockMode);

        public static void Remove(GameObject unityObject, CursorLockModeSetting lockMode)
            => Remove(unityObject as UnityEngine.Object, lockMode);

        public static void Remove(MonoBehaviour unityObject, CursorLockModeSetting lockMode)
            => Remove(unityObject as UnityEngine.Object, lockMode);

        #endregion
    }
}
