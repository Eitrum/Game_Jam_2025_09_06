using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    public abstract class BaseMaterialProperty : ISerializationCallbackReceiver
    {
        #region Variables

        [SerializeField] protected string name = "";
        [System.NonSerialized] protected int nameId = 0;

        #endregion

        #region Properties

        public string Name => name;
        public int NameId => nameId;
        public virtual MaterialPropertyType Type => MaterialPropertyType.None;

        #endregion

        #region Constructor

        public BaseMaterialProperty() { }
        public BaseMaterialProperty(string name) {
            this.name = name;
            this.nameId = Shader.PropertyToID(name);
        }

        #endregion

        #region Apply

        public abstract void Apply(Material material);
        public virtual void Apply(Material material, System.Random random) => Apply(material);
        public abstract void Apply(MaterialPropertyBlock block);
        public virtual void Apply(MaterialPropertyBlock block, System.Random random) => Apply(block);

        #endregion

        #region ISerializationCallbackReceiver Impl

        void ISerializationCallbackReceiver.OnAfterDeserialize() => nameId = Shader.PropertyToID(name);
        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        #endregion
    }

    public sealed class MaterialPropertyFloat : BaseMaterialProperty
    {
        #region Variables / Properties

        [SerializeField] private float value;

        public float Value {
            get => value;
            set => this.value = value;
        }
        public override MaterialPropertyType Type => MaterialPropertyType.Float;

        #endregion

        #region Constructor

        public MaterialPropertyFloat() : base() { }
        public MaterialPropertyFloat(string name) : base(name) { }
        public MaterialPropertyFloat(string name, float value) : base(name) {
            this.value = value;
        }

        #endregion

        #region Apply

        public override void Apply(MaterialPropertyBlock block) => block.SetFloat(nameId, value);
        public override void Apply(Material material) => material.SetFloat(nameId, value);

        #endregion
    }

    public sealed class MaterialPropertyVector2 : BaseMaterialProperty
    {
        #region Variables / Properties

        [SerializeField] private Vector2 value;

        public Vector2 Value {
            get => value;
            set => this.value = value;
        }
        public override MaterialPropertyType Type => MaterialPropertyType.Vector2;

        #endregion

        #region Constructor

        public MaterialPropertyVector2() : base() { }
        public MaterialPropertyVector2(string name) : base(name) { }
        public MaterialPropertyVector2(string name, Vector2 value) : base(name) {
            this.value = value;
        }

        #endregion

        #region Apply

        public override void Apply(MaterialPropertyBlock block) => block.SetVector(nameId, value);
        public override void Apply(Material material) => material.SetVector(nameId, value);

        #endregion
    }

    public sealed class MaterialPropertyVector3 : BaseMaterialProperty
    {
        #region Variables / Properties

        [SerializeField] private Vector3 value;

        public Vector3 Value {
            get => value;
            set => this.value = value;
        }
        public override MaterialPropertyType Type => MaterialPropertyType.Vector3;

        #endregion

        #region Constructor

        public MaterialPropertyVector3() : base() { }
        public MaterialPropertyVector3(string name) : base(name) { }
        public MaterialPropertyVector3(string name, Vector3 value) : base(name) {
            this.value = value;
        }

        #endregion

        #region Apply

        public override void Apply(MaterialPropertyBlock block) => block.SetVector(nameId, value);
        public override void Apply(Material material) => material.SetVector(nameId, value);

        #endregion
    }

    public sealed class MaterialPropertyVector4 : BaseMaterialProperty
    {
        #region Variables

        [SerializeField] private Vector4 value;

        public Vector4 Value {
            get => value;
            set => this.value = value;
        }
        public override MaterialPropertyType Type => MaterialPropertyType.Vector4;

        #endregion

        #region Constructor

        public MaterialPropertyVector4() : base() { }
        public MaterialPropertyVector4(string name) : base(name) { }
        public MaterialPropertyVector4(string name, Vector4 value) : base(name) {
            this.value = value;
        }

        #endregion

        #region Apply

        public override void Apply(MaterialPropertyBlock block) => block.SetVector(nameId, value);
        public override void Apply(Material material) => material.SetVector(nameId, value);

        #endregion
    }

    public sealed class MaterialPropertyColor : BaseMaterialProperty
    {
        #region Variables

        [SerializeField] private Color value;

        public Color Value {
            get => value;
            set => this.value = value;
        }
        public override MaterialPropertyType Type => MaterialPropertyType.Vector4;

        #endregion

        #region Constructor

        public MaterialPropertyColor() : base() { }
        public MaterialPropertyColor(string name) : base(name) { }
        public MaterialPropertyColor(string name, Color value) : base(name) {
            this.value = value;
        }

        #endregion

        #region Apply

        public override void Apply(MaterialPropertyBlock block) => block.SetColor(nameId, value);
        public override void Apply(Material material) => material.SetColor(nameId, value);

        #endregion
    }

    public sealed class MaterialPropertyFloatRange : BaseMaterialProperty
    {
        #region Variables / Properties

        [SerializeField] private MinMax value;

        public float Min {
            get => value.min;
            set => this.value.min = value;
        }

        public float Max {
            get => value.max;
            set => this.value.max = value;
        }

        public MinMax Value {
            get => value;
            set => this.value = value;
        }

        public override MaterialPropertyType Type => MaterialPropertyType.Float_Range;

        #endregion

        #region Constructor

        public MaterialPropertyFloatRange() : base() { }
        public MaterialPropertyFloatRange(string name) : base(name) { }
        public MaterialPropertyFloatRange(string name, MinMax value) : base(name) {
            this.value = value;
        }

        public MaterialPropertyFloatRange(string name, float min, float max) : base(name) {
            this.value = new MinMax(min, max);
        }

        #endregion

        #region Apply

        public override void Apply(MaterialPropertyBlock block) => block.SetFloat(nameId, value.Random);
        public override void Apply(MaterialPropertyBlock block, System.Random random) => block.SetFloat(nameId, value.Evaluate(random.NextFloat()));
        public override void Apply(Material material) => material.SetFloat(nameId, value.Random);
        public override void Apply(Material material, System.Random random) => material.SetFloat(nameId, value.Evaluate(random.NextFloat()));

        #endregion
    }

    public sealed class MaterialPropertyVector2Range : BaseMaterialProperty
    {
        #region Variables

        [SerializeField] private MinMaxVector2 value;

        #endregion

        #region Properties

        public Vector2 Min {
            get => value.min;
            set => this.value.min = value;
        }

        public Vector2 Max {
            get => value.max;
            set => this.value.max = value;
        }

        public MinMaxVector2 Value {
            get => value;
            set => this.value = value;
        }

        public Vector2 Random => value.Random;
        public override MaterialPropertyType Type => MaterialPropertyType.Vector2_Range;

        #endregion

        #region Constructor

        public MaterialPropertyVector2Range() { }
        public MaterialPropertyVector2Range(string name) : base(name) { }
        public MaterialPropertyVector2Range(string name, Vector2 min, Vector2 max) : base(name) {
            this.value = new MinMaxVector2(min, max);
        }
        public MaterialPropertyVector2Range(string name, MinMaxVector2 range) : base(name) {
            this.value = range;
        }

        #endregion

        #region Apply

        public override void Apply(MaterialPropertyBlock block) => block.SetVector(nameId, value.GetRandom());
        public override void Apply(MaterialPropertyBlock block, System.Random random) => block.SetVector(nameId, value.GetRandom(random));
        public override void Apply(Material material) => material.SetVector(nameId, value.GetRandom());
        public override void Apply(Material material, System.Random random) => material.SetVector(nameId, value.GetRandom(random));

        #endregion
    }

    public sealed class MaterialPropertyVector3Range : BaseMaterialProperty
    {
        #region Variables

        [SerializeField] MinMaxVector3 value;

        #endregion

        #region Properties

        public Vector3 Min {
            get => value.min;
            set => this.value.min = value;
        }
        public Vector3 Max {
            get => value.max;
            set => this.value.max = value;
        }
        public MinMaxVector3 Value {
            get => value;
            set => this.value = value;
        }
        public Vector3 Random => value.Random;
        public override MaterialPropertyType Type => MaterialPropertyType.Vector3_Range;

        #endregion

        #region Constructor

        public MaterialPropertyVector3Range() { }
        public MaterialPropertyVector3Range(string name) : base(name) { }
        public MaterialPropertyVector3Range(string name, Vector3 min, Vector3 max) : base(name) {
            this.value = new MinMaxVector3(min, max);
        }
        public MaterialPropertyVector3Range(string name, MinMaxVector3 range) : base(name) {
            this.value = range;
        }

        #endregion

        #region Apply

        public override void Apply(MaterialPropertyBlock block) => block.SetVector(nameId, value.GetRandom());
        public override void Apply(MaterialPropertyBlock block, System.Random random) => block.SetVector(nameId, value.GetRandom(random));
        public override void Apply(Material material) => material.SetVector(nameId, value.GetRandom());
        public override void Apply(Material material, System.Random random) => material.SetVector(nameId, value.GetRandom(random));

        #endregion
    }

    public sealed class MaterialPropertyVector4Range : BaseMaterialProperty
    {
        #region Variables

        [SerializeField] MinMaxVector4 value;

        #endregion

        #region Proprties

        public Vector4 Min {
            get => value.min;
            set => this.value.min = value;
        }
        public Vector4 Max {
            get => value.max;
            set => this.value.max = value;
        }
        public MinMaxVector4 Value {
            get => value;
            set => this.value = value;
        }
        public Vector4 Random => value.Random;
        public override MaterialPropertyType Type => MaterialPropertyType.Vector4_Range;

        #endregion

        #region Constructor

        public MaterialPropertyVector4Range() { }
        public MaterialPropertyVector4Range(string name) : base(name) { }
        public MaterialPropertyVector4Range(string name, Vector4 min, Vector4 max) : base(name) {
            this.value = new MinMaxVector4(min, max);
        }
        public MaterialPropertyVector4Range(string name, MinMaxVector4 range) : base(name) {
            this.value = range;
        }

        #endregion

        #region Apply

        public override void Apply(MaterialPropertyBlock block) => block.SetVector(nameId, value.GetRandom());
        public override void Apply(MaterialPropertyBlock block, System.Random random) => block.SetVector(nameId, value.GetRandom(random));
        public override void Apply(Material material) => material.SetVector(nameId, value.GetRandom());
        public override void Apply(Material material, System.Random random) => material.SetVector(nameId, value.GetRandom(random));

        #endregion
    }
}
