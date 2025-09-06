using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public class MaterialProperty : BaseMaterialProperty, ISerializationCallbackReceiver
    {
        #region Variables

        public const int DEFAULT_RANGE_TO_ARRAY = 16;

        [SerializeField] private MaterialPropertyType type;
        [SerializeField] private UnityEngine.Object[] objectReferences;
        [SerializeField] private float[] valueArray;
        [SerializeField] private Vector4[] vectorArray;

        #endregion

        #region Constructor

        public MaterialProperty(string name, MaterialPropertyType type) : base(name) {
            this.type = type;
            objectReferences = new UnityEngine.Object[0];
            valueArray = new float[0];
            vectorArray = new Vector4[0];
        }

        public MaterialProperty(string name, UnityEngine.Object objectReference) : base(name) {
            if(objectReference is Texture || objectReference is Texture2D) {
                objectReferences = new UnityEngine.Object[] { objectReference };
                valueArray = new float[0];
                vectorArray = new Vector4[0];
                type = MaterialPropertyType.Texture;
            }
            else if(objectReference is Texture2DArray) {
                objectReferences = new UnityEngine.Object[] { objectReference };
                valueArray = new float[0];
                vectorArray = new Vector4[0];
                type = MaterialPropertyType.TextureArray;
            }
            else if(objectReference is Texture3D) {
                objectReferences = new UnityEngine.Object[] { objectReference };
                valueArray = new float[0];
                vectorArray = new Vector4[0];
                type = MaterialPropertyType.Texture3D;
            }
            else {
                objectReferences = new UnityEngine.Object[0];
                valueArray = new float[0];
                vectorArray = new Vector4[0];
                type = MaterialPropertyType.None;
            }
        }

        #endregion

        #region Block

        public override void Apply(MaterialPropertyBlock block) {
            switch(type) {
                case MaterialPropertyType.Float: block.SetFloat(nameId, valueArray[0]); break;
                case MaterialPropertyType.FloatArray: block.SetFloatArray(nameId, valueArray); break;
                case MaterialPropertyType.Texture:
                case MaterialPropertyType.TextureArray:
                case MaterialPropertyType.Texture3D: block.SetTexture(nameId, objectReferences[0] as Texture); break;
                case MaterialPropertyType.Texture3D_Random:
                case MaterialPropertyType.TextureArray_Random:
                case MaterialPropertyType.Texture_Random: block.SetTexture(nameId, objectReferences.RandomElement() as Texture); break;

                case MaterialPropertyType.Color:
                    block.SetColor(nameId, vectorArray[0]);
                    break;

                case MaterialPropertyType.Vector2:
                case MaterialPropertyType.Vector3:
                case MaterialPropertyType.Vector4:
                    block.SetVector(nameId, vectorArray[0]);
                    break;

                case MaterialPropertyType.Vector2_Random:
                case MaterialPropertyType.Vector3_Random:
                case MaterialPropertyType.Vector4_Random:
                    block.SetVector(nameId, vectorArray.RandomElement());
                    break;

                case MaterialPropertyType.Color_Random:
                    block.SetColor(nameId, vectorArray.RandomElement());
                    break;

                case MaterialPropertyType.Vector2_Range:
                case MaterialPropertyType.Vector3_Range:
                case MaterialPropertyType.Vector4_Range:
                    Vector4 result = new Vector4(
                        Mathf.Lerp(vectorArray[0].x, vectorArray[1].x, UnityEngine.Random.value),
                        Mathf.Lerp(vectorArray[0].y, vectorArray[1].y, UnityEngine.Random.value),
                        Mathf.Lerp(vectorArray[0].z, vectorArray[1].z, UnityEngine.Random.value),
                        Mathf.Lerp(vectorArray[0].w, vectorArray[1].w, UnityEngine.Random.value));
                    block.SetVector(nameId, result);
                    break;

                case MaterialPropertyType.Color_Range:
                    Vector4 colResult = new Vector4(
                        Mathf.Lerp(vectorArray[0].x, vectorArray[1].x, UnityEngine.Random.value),
                        Mathf.Lerp(vectorArray[0].y, vectorArray[1].y, UnityEngine.Random.value),
                        Mathf.Lerp(vectorArray[0].z, vectorArray[1].z, UnityEngine.Random.value),
                        Mathf.Lerp(vectorArray[0].w, vectorArray[1].w, UnityEngine.Random.value));
                    block.SetColor(nameId, colResult);
                    break;
            }
        }

        public override void Apply(MaterialPropertyBlock block, System.Random random) {
            switch(type) {
                case MaterialPropertyType.TextureArray_Random:
                case MaterialPropertyType.Texture3D_Random:
                case MaterialPropertyType.Texture_Random: block.SetTexture(nameId, objectReferences.RandomElement(random) as Texture); break;
                case MaterialPropertyType.Vector2_Random:
                case MaterialPropertyType.Vector3_Random:
                case MaterialPropertyType.Vector4_Random: block.SetVector(nameId, vectorArray.RandomElement(random)); break;
                case MaterialPropertyType.Color_Random: block.SetColor(nameId, vectorArray.RandomElement(random)); break;
                case MaterialPropertyType.Float_Random: block.SetFloat(nameId, valueArray.RandomElement(random)); break;
                default: Apply(block); break;
            }
        }

        public override void Apply(Material material) {
            switch(type) {
                case MaterialPropertyType.Float: material.SetFloat(nameId, valueArray[0]); break;
                case MaterialPropertyType.FloatArray: material.SetFloatArray(nameId, valueArray); break;
                case MaterialPropertyType.Texture:
                case MaterialPropertyType.TextureArray:
                case MaterialPropertyType.Texture3D: material.SetTexture(nameId, objectReferences[0] as Texture); break;
                case MaterialPropertyType.Texture3D_Random:
                case MaterialPropertyType.TextureArray_Random:
                case MaterialPropertyType.Texture_Random: material.SetTexture(nameId, objectReferences.RandomElement() as Texture); break;

                case MaterialPropertyType.Color:
                    material.SetColor(nameId, vectorArray[0]);
                    break;

                case MaterialPropertyType.Vector2:
                case MaterialPropertyType.Vector3:
                case MaterialPropertyType.Vector4:
                    material.SetVector(nameId, vectorArray[0]);
                    break;

                case MaterialPropertyType.Vector2_Random:
                case MaterialPropertyType.Vector3_Random:
                case MaterialPropertyType.Vector4_Random:
                    material.SetVector(nameId, vectorArray.RandomElement());
                    break;

                case MaterialPropertyType.Color_Random:
                    material.SetColor(nameId, vectorArray.RandomElement());
                    break;

                case MaterialPropertyType.Vector2_Range:
                case MaterialPropertyType.Vector3_Range:
                case MaterialPropertyType.Vector4_Range:
                    Vector4 result = new Vector4(
                        Mathf.Lerp(vectorArray[0].x, vectorArray[1].x, UnityEngine.Random.value),
                        Mathf.Lerp(vectorArray[0].y, vectorArray[1].y, UnityEngine.Random.value),
                        Mathf.Lerp(vectorArray[0].z, vectorArray[1].z, UnityEngine.Random.value),
                        Mathf.Lerp(vectorArray[0].w, vectorArray[1].w, UnityEngine.Random.value));
                    material.SetVector(nameId, result);
                    break;

                case MaterialPropertyType.Color_Range:
                    material.SetColor(nameId, Color.Lerp(vectorArray[0], vectorArray[1], UnityEngine.Random.value));
                    break;
            }
        }

        public override void Apply(Material material, System.Random random) {
            switch(type) {
                case MaterialPropertyType.TextureArray_Random:
                case MaterialPropertyType.Texture3D_Random:
                case MaterialPropertyType.Texture_Random: material.SetTexture(nameId, objectReferences.RandomElement(random) as Texture); break;
                case MaterialPropertyType.Vector2_Random:
                case MaterialPropertyType.Vector3_Random:
                case MaterialPropertyType.Vector4_Random: material.SetVector(nameId, vectorArray.RandomElement(random)); break;
                case MaterialPropertyType.Color_Random: material.SetColor(nameId, vectorArray.RandomElement(random)); break;
                case MaterialPropertyType.Float_Random: material.SetFloat(nameId, valueArray.RandomElement(random)); break;
                default: Apply(material); break;
            }
        }

        #endregion

        #region Color

        public Color GetColor() {
            switch(type) {
                case MaterialPropertyType.Color: return vectorArray[0];
                case MaterialPropertyType.Color_Random: return vectorArray.RandomElement();
                case MaterialPropertyType.Color_Range: return Color.Lerp(vectorArray[0], vectorArray[1], UnityEngine.Random.value);
            }
            return Color.clear;
        }

        public Color GetColor(System.Random random) {
            switch(type) {
                case MaterialPropertyType.Color: return vectorArray[0];
                case MaterialPropertyType.Color_Random: return vectorArray.RandomElement(random);
                case MaterialPropertyType.Color_Range: return Color.Lerp(vectorArray[0], vectorArray[1], random.NextFloat());
            }
            return Color.clear;
        }

        public bool GetColorRange(out Color _0, out Color _1) {
            switch(type) {
                case MaterialPropertyType.Color:
                    _0 = vectorArray[0];
                    _1 = vectorArray[0];
                    return true;
                case MaterialPropertyType.Color_Range:
                    _0 = vectorArray[0];
                    _1 = vectorArray[1];
                    return true;
            }
            _0 = Color.clear;
            _1 = Color.clear;
            return false;
        }

        public bool GetColorArray(out Color[] colors) {
            switch(type) {
                case MaterialPropertyType.Color:
                    colors = new Color[] { vectorArray[0] };
                    return true;
                case MaterialPropertyType.Color_Random:
                    colors = new Color[vectorArray.Length];
                    for(int i = 0, length = colors.Length; i < length; i++)
                        colors[i] = vectorArray[i];
                    return true;
                case MaterialPropertyType.Color_Range:
                    colors = new Color[DEFAULT_RANGE_TO_ARRAY];
                    for(int i = 0; i < DEFAULT_RANGE_TO_ARRAY; i++)
                        colors[i] = Color.Lerp(vectorArray[0], vectorArray[1], i / (float)(DEFAULT_RANGE_TO_ARRAY - 1));
                    return true;
            }
            colors = new Color[] { Color.clear };
            return false;
        }

        #endregion

        #region Float

        public float GetFloat() {
            switch(type) {
                case MaterialPropertyType.Float: return valueArray[0];
                case MaterialPropertyType.FloatArray:
                case MaterialPropertyType.Float_Random: return valueArray.RandomElement();
                case MaterialPropertyType.Float_Range: return Mathf.Lerp(valueArray[0], valueArray[1], UnityEngine.Random.value);
            }
            return 0f;
        }

        public float GetFloat(System.Random random) {
            switch(type) {
                case MaterialPropertyType.Float: return valueArray[0];
                case MaterialPropertyType.FloatArray:
                case MaterialPropertyType.Float_Random: return valueArray.RandomElement(random);
                case MaterialPropertyType.Float_Range: return Mathf.Lerp(valueArray[0], valueArray[1], random.NextFloat());
            }
            return 0f;
        }

        #endregion
    }

    public static class MaterialPropertyUtility
    {
        public static MaterialPropertyType ToNonRandom(this MaterialPropertyType type) {
            switch(type) {
                case MaterialPropertyType.Texture_Random: return MaterialPropertyType.Texture;
                case MaterialPropertyType.Texture3D_Random: return MaterialPropertyType.Texture3D;
                case MaterialPropertyType.TextureArray_Random: return MaterialPropertyType.TextureArray;
                case MaterialPropertyType.Vector2_Random: return MaterialPropertyType.Vector2;
                case MaterialPropertyType.Vector3_Random: return MaterialPropertyType.Vector3;
                case MaterialPropertyType.Vector4_Random: return MaterialPropertyType.Vector4;
                case MaterialPropertyType.Color_Random: return MaterialPropertyType.Color;
                case MaterialPropertyType.Float_Random: return MaterialPropertyType.Float;
            }
            return type;
        }

        public static MaterialPropertyType ToNonRange(this MaterialPropertyType type) {
            switch(type) {
                case MaterialPropertyType.Float_Range: return MaterialPropertyType.Float;
                case MaterialPropertyType.Vector2_Range: return MaterialPropertyType.Vector2;
                case MaterialPropertyType.Vector3_Range: return MaterialPropertyType.Vector3;
                case MaterialPropertyType.Vector4_Range: return MaterialPropertyType.Vector4;
                case MaterialPropertyType.Color_Range: return MaterialPropertyType.Color;
            }
            return type;
        }
    }

    public enum MaterialPropertyType
    {
        None,
        [InspectorName(" ")] _None,
        Float,
        FloatArray,
        Texture,
        TextureArray,
        Texture3D,
        Color,
        Vector2,
        Vector3,
        Vector4,
        Matrix4x4,
        Matrix4x4Array,

        [InspectorName("  ")] _Range,
        [InspectorName("Float (Range)")] Float_Range,
        [InspectorName("Color (Range)")] Color_Range,
        [InspectorName("Vector2 (Range)")] Vector2_Range,
        [InspectorName("Vector3 (Range)")] Vector3_Range,
        [InspectorName("Vector4 (Range)")] Vector4_Range,

        [InspectorName("   ")] _Random,
        [InspectorName("Float (Random)")] Float_Random,
        [InspectorName("Texture (Random)")] Texture_Random,
        [InspectorName("Texture Array (Random)")] TextureArray_Random,
        [InspectorName("Texture3D (Random)")] Texture3D_Random,
        [InspectorName("Color (Random)")] Color_Random,
        [InspectorName("Vector2 (Random)")] Vector2_Random,
        [InspectorName("Vector3 (Random)")] Vector3_Random,
        [InspectorName("Vector4 (Random)")] Vector4_Random,
    }
}
