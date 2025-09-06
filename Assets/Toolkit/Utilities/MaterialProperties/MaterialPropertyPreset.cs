using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    [CreateAssetMenu(fileName = "Material Property Preset", menuName = "Toolkit/Utility/Material Property Preset")]
    public class MaterialPropertyPreset : ScriptableObject
    {
        #region Variables

        [SerializeField] private MaterialProperty[] properties;

        #endregion

        #region Properties

        public IReadOnlyList<MaterialProperty> Properties => properties;
        public int Count => properties.Length;
        public MaterialProperty this[int index] => properties[index];

        public IEnumerable<MaterialProperty> Color {
            get {
                for(int i = 0, length = Count; i < length; i++) {
                    switch(properties[i].Type) {
                        case MaterialPropertyType.Color:
                        case MaterialPropertyType.Color_Random:
                        case MaterialPropertyType.Color_Range:
                            yield return properties[i];
                            break;
                    }
                }
            }
        }

        public IEnumerable<MaterialProperty> Vector2 {
            get {
                for(int i = 0, length = Count; i < length; i++) {
                    switch(properties[i].Type) {
                        case MaterialPropertyType.Vector2:
                        case MaterialPropertyType.Vector2_Random:
                        case MaterialPropertyType.Vector2_Range:
                            yield return properties[i];
                            break;
                    }
                }
            }
        }

        public IEnumerable<MaterialProperty> Vector3 {
            get {
                for(int i = 0, length = Count; i < length; i++) {
                    switch(properties[i].Type) {
                        case MaterialPropertyType.Vector3:
                        case MaterialPropertyType.Vector3_Random:
                        case MaterialPropertyType.Vector3_Range:
                            yield return properties[i];
                            break;
                    }
                }
            }
        }

        public IEnumerable<MaterialProperty> Vector4 {
            get {
                for(int i = 0, length = Count; i < length; i++) {
                    switch(properties[i].Type) {
                        case MaterialPropertyType.Vector4:
                        case MaterialPropertyType.Vector4_Random:
                        case MaterialPropertyType.Vector4_Range:
                            yield return properties[i];
                            break;
                    }
                }
            }
        }

        public IEnumerable<MaterialProperty> Float {
            get {
                for(int i = 0, length = Count; i < length; i++) {
                    switch(properties[i].Type) {
                        case MaterialPropertyType.Float:
                        case MaterialPropertyType.Float_Random:
                        case MaterialPropertyType.Float_Range:
                            yield return properties[i];
                            break;
                    }
                }
            }
        }

        #endregion

        #region Apply

        public void Apply(Material material) {
            for(int i = 0, length = Count; i < length; i++) {
                properties[i].Apply(material);
            }
        }

        public void Apply(Material material, System.Random random) {
            for(int i = 0, length = Count; i < length; i++) {
                properties[i].Apply(material, random);
            }
        }

        public void Apply(MaterialPropertyBlock block) {
            for(int i = 0, length = Count; i < length; i++) {
                properties[i].Apply(block);
            }
        }

        public void Apply(MaterialPropertyBlock block, System.Random random) {
            for(int i = 0, length = Count; i < length; i++) {
                properties[i].Apply(block, random);
            }
        }

        #endregion
    }
}
