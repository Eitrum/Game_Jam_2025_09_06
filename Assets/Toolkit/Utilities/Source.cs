using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Toolkit.Unit;
using UnityEngine;

namespace Toolkit {
    /// <summary>
    /// A generic class to handle sources
    /// </summary>
    public class Source : IDisposable, INullable {
        #region Pooling

        private static FastPool<Source> POOL = new FastPool<Source>(64, PoolReset);

        private static void PoolReset(Source obj) {
            obj.reference = null;
            obj.type = SourceType.None;
            obj.parent = null;
            obj.children = 0;
            obj.wantToDispose = false;
        }

        #endregion

        #region Variables

        private const string TAG = "[Toolkit.Source] - ";

        [System.NonSerialized] private object reference;
        [System.NonSerialized] private SourceType type;

        [System.NonSerialized] private Source parent;
        [System.NonSerialized] private int children;
        [System.NonSerialized] private bool wantToDispose = false;

        #endregion

        #region Properties

        public object Reference => reference;
        public SourceType Type => type;

        public bool IsValid => type != SourceType.None && !IsNull && reference != null;
        public bool IsNull => type == SourceType.Disposed || reference == null;

        public Source Root {
            get {
#if UNITY_EDITOR
                if(parent != null && parent.type != SourceType.Editor)
                    return parent.Root;
                return this;
#else
                return parent?.Root ?? this;
#endif
            }
        }
        public Source Parent => parent;

        // Types
        public object Generic => reference;

        public GameObject GameObject {
            get {
                switch(type) {
                    case SourceType.GameObject: return reference as GameObject;
                }
                return (reference is Component c) ? c.gameObject : null;
            }
        }

        public ScriptableObject ScriptableObject => reference as ScriptableObject;

        public Transform Transform {
            get {
                switch(type) {
                    case SourceType.Transform: return reference as Transform;
                    case SourceType.GameObject: return (reference is GameObject go) ? go.transform : null;
                }
                return (reference is Component c) ? c.transform : null;
            }
        }

        public Component Component => reference as Component;

        public Rigidbody Rigidbody {
            get {
                switch(type) {
                    case SourceType.Rigidbody: return reference as Rigidbody;
                    case SourceType.GameObject: return (reference is GameObject go) ? go.GetComponent<Rigidbody>() : null;
                }
                return (reference is Component c) ? c.GetComponent<Rigidbody>() : null;
            }
        }

        public Rigidbody2D Rigidbody2D {
            get {
                switch(type) {
                    case SourceType.Rigidbody: return reference as Rigidbody2D;
                    case SourceType.GameObject: return (reference is GameObject go) ? go.GetComponent<Rigidbody2D>() : null;
                }
                return (reference is Component c) ? c.GetComponent<Rigidbody2D>() : null;
            }
        }

        public Entity Entity {
            get {
                switch(type) {
                    case SourceType.Entity: return reference as Entity;
                    case SourceType.GameObject: return (reference is GameObject go) ? go.GetComponent<Entity>() : null;
                }
                return (reference is Component c) ? c.GetComponent<Entity>() : null;
            }
        }

        public Health.IHealth Health {
            get {
                switch(type) {
                    case SourceType.Health: return reference as Health.IHealth;
                    case SourceType.GameObject: return (reference is GameObject go) ? go.GetComponent<Health.IHealth>() : null;
                    case SourceType.ScriptableObject: return (reference is Health.IHealth health) ? health : null;
                }
                return (reference is Component c) ?
                    c.GetComponent<Health.IHealth>() :
                    ((reference is Health.IHealth h) ? h : null);
            }
        }

        internal Trigger.ITrigger Trigger {
            get {
                switch(type) {
                    case SourceType.Trigger: return reference as Trigger.ITrigger;
                    case SourceType.GameObject: return (reference is GameObject go) ? go.GetComponent<Trigger.ITrigger>() : null;
                    case SourceType.ScriptableObject: return (reference is Trigger.ITrigger trigger) ? trigger : null;
                }
                return (reference is Component c) ?
                    c.GetComponent<Trigger.ITrigger>() :
                    ((reference is Trigger.ITrigger h) ? h : null);
            }
        }

        public IUnit Unit {
            get {
                switch(type) {
                    case SourceType.Unit: return reference as IUnit;
                    case SourceType.GameObject: return (reference is GameObject go) ? go.GetComponent<IUnit>() : null;
                }
                return (reference is Component c) ?
                    c.GetComponent<IUnit>() :
                    ((reference is IUnit u) ? u : null);
            }
        }

        #endregion

        #region Constructor

        public Source() { }

        internal Source(object o, SourceType t) {
            reference = o;
            type = t;
        }

        // Generates GC
        public Source(object obj) : this(obj, SourceType.Generic) { }
        public Source(Component component) : this(component, SourceType.Component) { }
        public Source(GameObject gameObject) : this(gameObject, SourceType.GameObject) { }
        public Source(Transform transform) : this(transform, SourceType.Transform) { }
        public Source(Rigidbody body) : this(body, SourceType.Rigidbody) { }
        public Source(Rigidbody2D body) : this(body, SourceType.Rigidbody2D) { }
        public Source(Entity entity) : this(entity, SourceType.Entity) { }
        public Source(Toolkit.Health.IHealth health) : this(health, SourceType.Health) { }
        public Source(Trigger.ITrigger trigger) : this(trigger, SourceType.Trigger) { }
        public Source(IUnit unit) : this(unit, SourceType.Unit) { }

        ~Source() {
            if(type == SourceType.Disposed)
                return;
            // This should not occur to ensure proper GC handling and pooling.
            Debug.LogWarning(TAG + $"Non-Pooled Cleanup: {type} | {(reference is UnityEngine.Object uobj && uobj != null ? uobj.name : "''")} | children ({children}) | wantToDispose ({wantToDispose})");
        }

        #endregion

        #region IDisposable Impl

        public void Dispose() {
            if(type == SourceType.Disposed) {
#if UNITY_EDITOR
                Debug.LogError(TAG + "Attempting to dispose of a source already marked as dispose.");
#endif
                return;
            }
            if(children == 0 && type != SourceType.Disposed) {
                type = SourceType.Disposed;
                POOL.Push(this);
                if(parent != null) {
                    parent.children--;
                    if(parent.children == 0 && parent.wantToDispose)
                        parent.Dispose();
                }
            }
            else
                wantToDispose = true;
        }

        #endregion

        #region Methods

        public bool IsType(SourceType type) {
            return type == this.type;
        }

        public bool IsAny(SourceTypeMask mask) {
            if(this.type == SourceType.None && mask == SourceTypeMask.None)
                return false;
            var tMask = 1 << (this.type.ToInt() - 1);
            return mask.ToInt().HasFlag(tMask);
        }

        #endregion

        #region TryGet Unity

        public bool TryGet(out GameObject value) {
            value = GameObject;
            return value != null;
        }

        public bool TryGet(out Transform transform) {
            transform = Transform;
            return transform != null;
        }

        public bool TryGet(out ScriptableObject scriptableObject) {
            scriptableObject = ScriptableObject;
            return scriptableObject != null;
        }

        public bool TryGet(out Component component) {
            component = Component;
            return component != null;
        }

        public bool TryGet(out Rigidbody body) {
            body = Rigidbody;
            return body != null;
        }

        public bool TryGet(out Rigidbody2D body) {
            body = Rigidbody2D;
            return body != null;
        }

        #endregion

        #region TryGet Toolkit

        public bool TryGet(out Entity entity) {
            entity = Entity;
            return entity != null;
        }

        public bool TryGet(out IUnit unit) {
            unit = Unit;
            return unit != null;
        }

        public bool TryGet(out Trigger.ITrigger trigger) {
            trigger = Trigger;
            return trigger != null;
        }

        #endregion

        #region Add Child

        /// <summary>
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public Source AddChild(Source child) {
#if UNITY_EDITOR
            if(child.parent != null)
                Debug.LogError(TAG + $"Assigning parent when it already exists on {reference}");
#endif
            child.parent = this;
            children++;
            return child;
        }

        public Source AddChild(object generic) => AddChild(Create(generic));
        public Source AddChild(Component component) => AddChild(Create(component));
        public Source AddChild(GameObject gameObject) => AddChild(Create(gameObject));
        public Source AddChild(Transform transform) => AddChild(Create(transform));
        public Source AddChild(Rigidbody body) => AddChild(Create(body));
        public Source AddChild(Rigidbody2D body) => AddChild(Create(body));
        public Source AddChild(ScriptableObject scriptableObject) => AddChild(Create(scriptableObject));

        public Source AddChild(IUnit unit) => AddChild(Create(unit));
        public Source AddChild(Entity entity) => AddChild(Create(entity));
        public Source AddChild(Health.IHealth health) => AddChild(Create(health));
        public Source AddChild(Trigger.ITrigger trigger, bool forceAddExtra = false) {
            if(type == SourceType.Trigger && reference == trigger && !forceAddExtra)
                return this;
            return AddChild(Create(trigger));
        }

        #endregion

        #region Find

        /// <summary>
        /// Attempts to find a specific source type in parents.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Source Find(SourceType type) {
            if(this.type == type)
                return this;
            return parent?.Find(type) ?? null;
        }

        public Source FindRoot(SourceType type) {
            return parent?.FindRoot(type) ?? (this.type == type ? this : null);
        }

        #endregion

        #region Get Chain

        private static List<Source> chainSources = new List<Source>();
        public Source[] GetChain() {
            chainSources.Clear();
            GetChainRecursive(chainSources);
            chainSources.Invert();
            return chainSources.ToArray();
        }

        private void GetChainRecursive(List<Source> sources) {
            sources.Add(this);
            if(this.parent != null)
                this.parent.GetChainRecursive(sources);
        }

        #endregion

        #region Overrides

        public string ToString(bool entireChain) {
            if(!entireChain)
                return ToString();
            StringBuilder sb = new StringBuilder();
            parent?.ToStringChainRecursive(sb);
            sb.Append(ToString());
            return sb.ToString();
        }

        private void ToStringChainRecursive(StringBuilder sb) {
            parent?.ToStringChainRecursive(sb);
            sb.Append($"{ToString()}-");
        }

        public override string ToString() {
            switch(type) {
                case SourceType.Generic: return reference.ToString();
                case SourceType.GameObject: return (reference is GameObject go) ? go.name : "null (gameObject)";
                case SourceType.ScriptableObject: return (reference is ScriptableObject so) ? so.name : "null (scriptableObject)";
                case SourceType.Transform: return (reference is Transform transform) ? transform.name : "null (transform)";
                case SourceType.Component: return (reference is Component component) ? component.name : "null (component)";
                case SourceType.Rigidbody: return (reference is Rigidbody body) ? $"{body.name} (rigidbody)" : "null (rigidbody)";
                case SourceType.Rigidbody2D: return (reference is Rigidbody2D body2d) ? $"{body2d.name} (rigidbody2d)" : "null (rigidbody2d)";

                case SourceType.Entity: return (reference is Entity entity) ? $"{entity.EntityName} (entity)" : "null (entity)";
                case SourceType.Unit: return (reference is IUnit unit) ? $"{unit.Name} (unit)" : "null (unit)";
                case SourceType.Trigger: return (reference is UnityEngine.Object trigger) ? $"{trigger.name} (trigger)" : "null (trigger)";
                case SourceType.Health: return (reference is Component health) ? $"{health.name} (health)" : (reference != null ? reference.ToString() : "null (health)");

#if UNITY_EDITOR
                case SourceType.Editor: return $"Editor";
#endif
            }
            return "missing source";
        }

        #endregion

        #region Static Create

        public static Source Create<T>(T instance) {
            // Toolkit Specific
            if(instance is Health.IHealth health)
                return Create(health);
            if(instance is Entity entity)
                return Create(entity);
            if(instance is IUnit unit)
                return Create(unit);
            if(instance is Trigger.ITrigger trigger)
                return Create(trigger);

            // Unity Components
            if(instance is Rigidbody body)
                return Create(body);
            if(instance is Rigidbody2D body2d)
                return Create(body2d);
            if(instance is Transform transform)
                return Create(transform);
            if(instance is ScriptableObject scriptableObject)
                return Create(scriptableObject);
            if(instance is Component component)
                return Create(component);
            if(instance is GameObject go)
                return Create(go);

            // Create generic reference
            return Create(instance as object);
        }

        public static Source Create(object generic) {
            var s = POOL.Pop();
            s.reference = generic;
            s.type = SourceType.Generic;
            return s;
        }

        public static Source Create(GameObject gameObject) {
            var s = POOL.Pop();
            s.reference = gameObject;
            s.type = SourceType.GameObject;
            return s;
        }

        public static Source Create(Component component) {
            var s = POOL.Pop();
            s.reference = component;
            s.type = SourceType.Component;
            return s;
        }

        public static Source Create(ScriptableObject scriptableObject) {
            var s = POOL.Pop();
            s.reference = scriptableObject;
            s.type = SourceType.ScriptableObject;
            return s;
        }

        public static Source Create(Transform transform) {
            var s = POOL.Pop();
            s.reference = transform;
            s.type = SourceType.Transform;
            return s;
        }

        public static Source Create(Rigidbody2D rigidbody2D) {
            var s = POOL.Pop();
            s.reference = rigidbody2D;
            s.type = SourceType.Rigidbody2D;
            return s;
        }

        public static Source Create(Rigidbody rigidbody) {
            var s = POOL.Pop();
            s.reference = rigidbody;
            s.type = SourceType.Rigidbody;
            return s;
        }

        public static Source Create(Trigger.ITrigger trigger) {
            var s = POOL.Pop();
            s.reference = trigger;
            s.type = SourceType.Trigger;
            return s;
        }

        public static Source Create(IUnit unit) {
            var s = POOL.Pop();
            s.reference = unit;
            s.type = SourceType.Unit;
            return s;
        }

        public static Source Create(Health.IHealth health) {
            var s = POOL.Pop();
            s.reference = health;
            s.type = SourceType.Health;
            return s;
        }

        public static Source Create(Entity entity) {
            var s = POOL.Pop();
            s.reference = entity;
            s.type = SourceType.Entity;
            return s;
        }

        #endregion
    }

    public enum SourceType {
        None = 0,
        Generic = 1,
        GameObject = 2,
        ScriptableObject = 3,
        Transform = 4,
        Component = 5,
        Rigidbody = 6,
        Rigidbody2D = 7,

        Entity = 16,
        Unit = 17,
        Trigger = 18,
        Health = 19,

        Disposed = 31,
        // Editor Only
        Editor = 32,
    }

    [System.Flags]
    public enum SourceTypeMask {
        None = 0,
        Generic = 1 << 0,
        GameObject = 1 << 1,
        ScriptableObject = 1 << 2,
        Transform = 1 << 3,
        Component = 1 << 4,
        Rigidbody = 1 << 5,
        Rigidbody2D = 1 << 6,

        Entity = 1 << 15,
        Unit = 1 << 16,
        Trigger = 1 << 17,
        Health = 1 << 18,

        Disposed = 1 << 30,
        // Editor Only
        Editor = 1 << 31,
    }
}
