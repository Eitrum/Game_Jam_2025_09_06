using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Toolkit
{
    public enum ImageComponentType
    {
        None = 0,

        Renderer = 1,
        Material = 2,

        UIImage = 3,
        UIRawImage = 4,

        Light = 5,

        Sprite = 6,
        SpriteMask = 7,
    }
    [System.Serializable, StructLayout(LayoutKind.Explicit)]
    public struct ImageComponent : ISerializationCallbackReceiver
    {
        #region Variables

        [FieldOffset(0), SerializeField] private UnityEngine.Object reference;
        [FieldOffset(0), System.NonSerialized] private UnityEngine.Renderer refRenderer;
        [FieldOffset(0), System.NonSerialized] private UnityEngine.Material refMaterial;
        [FieldOffset(0), System.NonSerialized] private UnityEngine.UI.Image refUIImage;
        [FieldOffset(0), System.NonSerialized] private UnityEngine.UI.RawImage refUIRawImage;
        [FieldOffset(0), System.NonSerialized] private UnityEngine.Light refLight;
        [FieldOffset(0), System.NonSerialized] private UnityEngine.SpriteRenderer refSprite;
        [FieldOffset(0), System.NonSerialized] private UnityEngine.SpriteMask refSpriteMask;
        [FieldOffset(8), System.NonSerialized] private ImageComponentType type;

        #endregion

        #region Properties

        public UnityEngine.Object Reference => reference;
        public bool HasReference => reference != null;

        public bool Enabled {
            get {
                switch(type) {
                    case ImageComponentType.Light: return refLight.enabled;
                    case ImageComponentType.UIImage: return refUIImage.enabled;
                    case ImageComponentType.UIRawImage:  return refUIRawImage.enabled;
                    case ImageComponentType.Sprite:  return refSprite.enabled;
                    case ImageComponentType.SpriteMask: return refSpriteMask.enabled;
                    case ImageComponentType.Renderer:return refRenderer.enabled;
                }
                return false;
            }
            set {
                switch(type) {
                    case ImageComponentType.Light: refLight.enabled = value; break;
                    case ImageComponentType.UIImage: refUIImage.enabled = value; break;
                    case ImageComponentType.UIRawImage: refUIRawImage.enabled = value; break;
                    case ImageComponentType.Sprite: refSprite.enabled = value; break;
                    case ImageComponentType.SpriteMask: refSpriteMask.enabled = value; break;
                    case ImageComponentType.Renderer: refRenderer.enabled = value; break;
                    default:
                        Debug.LogError($"Unable to set enabled/disabled to '{reference.name}' as it is not a supported type.");
                        break;
                }
            }
        }

        public bool IsSprite {
            get {
                switch(type) {
                    case ImageComponentType.UIImage:
                    case ImageComponentType.Sprite:
                    case ImageComponentType.SpriteMask:
                        return true;
                }
                return false;
            }
        }

        public Texture Texture {
            get {
                switch(type) {
                    case ImageComponentType.Renderer: return refRenderer.sharedMaterial.mainTexture;
                    case ImageComponentType.Material: return refMaterial.mainTexture;
                    case ImageComponentType.UIImage: return refUIImage.mainTexture;
                    case ImageComponentType.UIRawImage: return refUIRawImage.mainTexture;
                    case ImageComponentType.Light: return refLight.cookie;
                    case ImageComponentType.Sprite: return refSprite.sprite.texture;
                    case ImageComponentType.SpriteMask: return refSpriteMask.sprite.texture;
                }
                return null;
            }
            set {
                switch(type) {
                    case ImageComponentType.Sprite:
                    case ImageComponentType.SpriteMask:
                    case ImageComponentType.UIImage:
                        Debug.LogError($"Unable to assign texture to '{reference.name}' as it is a sprite, instead use ImageComponent.Sprite");
                        break;
                    case ImageComponentType.Renderer: refRenderer.material.mainTexture = value; break;
                    case ImageComponentType.Material: refMaterial.mainTexture = value; break;
                    case ImageComponentType.UIRawImage: refUIRawImage.texture = value; break;
                    case ImageComponentType.Light: refLight.cookie = value; break;
                    default:
                        Debug.LogError($"Unable to assign texture to '{reference.name}' as it is not a supported type. '{type}'");
                        break;
                }
            }
        }

        public Sprite Sprite {
            get {
                switch(type) {
                    case ImageComponentType.Sprite: return refSprite.sprite;
                    case ImageComponentType.SpriteMask: return refSpriteMask.sprite;
                    case ImageComponentType.UIImage: return refUIImage.sprite;
                }
                return null;
            }
            set {
                switch(type) {
                    case ImageComponentType.Sprite: refSprite.sprite = value; break;
                    case ImageComponentType.SpriteMask: refSpriteMask.sprite = value; break;
                    case ImageComponentType.UIImage: refUIImage.sprite = value; break;
                    case ImageComponentType.Renderer:
                    case ImageComponentType.Material:
                    case ImageComponentType.UIRawImage:
                    case ImageComponentType.Light:
                        Debug.LogError($"Unable to assign sprite to '{reference.name}' as it is not a sprite type, instead use ImageComponent.Texture");
                        break;
                    default:
                        Debug.LogError($"Unable to assign sprite to '{reference.name}' as it is not a sprite type. '{type}'");
                        break;
                }
            }
        }

        public Color Color {
            get {
                switch(type) {
                    case ImageComponentType.Sprite: return refSprite.color;
                    case ImageComponentType.UIImage: return refUIImage.color;
                    case ImageComponentType.UIRawImage: return refUIRawImage.color;
                    case ImageComponentType.Renderer: return refRenderer.material.color;
                    case ImageComponentType.Material: return refMaterial.color;
                    case ImageComponentType.Light: return refLight.color;
                }
                return Color.clear;
            }
            set {
                switch(type) {
                    case ImageComponentType.Sprite: refSprite.color = value; break;
                    case ImageComponentType.UIImage: refUIImage.color = value; break;
                    case ImageComponentType.UIRawImage: refUIRawImage.color = value; break;
                    case ImageComponentType.Renderer: refRenderer.material.color = value; break;
                    case ImageComponentType.Material: refMaterial.color = value; break;
                    case ImageComponentType.Light: refLight.color = value; break;
                    case ImageComponentType.SpriteMask:
                        Debug.LogError($"Unable to assign color to '{reference.name}' as it is not a type that supports color");
                        break;
                    default:
                        Debug.LogError($"Unable to assign color to '{reference.name}' as it is not a type that supports color");
                        break;
                }
            }
        }

        public float Transparency {
            get {
                switch(type) {
                    case ImageComponentType.Sprite: return refSprite.color.a;
                    case ImageComponentType.UIImage: return refUIImage.color.a;
                    case ImageComponentType.UIRawImage: return refUIRawImage.color.a;
                    case ImageComponentType.Renderer: return refRenderer.material.color.a;
                    case ImageComponentType.Material: return refMaterial.color.a;
                    case ImageComponentType.Light: return refLight.color.a;
                }
                return 0f;
            }
            set {
                var col = Color;
                col.a = Mathf.Clamp01(value);
                switch(type) {
                    case ImageComponentType.Sprite: refSprite.color = col; break;
                    case ImageComponentType.UIImage: refUIImage.color = col; break;
                    case ImageComponentType.UIRawImage: refUIRawImage.color = col; break;
                    case ImageComponentType.Renderer: refRenderer.material.color = col; break;
                    case ImageComponentType.Material: refMaterial.color = col; break;
                    case ImageComponentType.Light: refLight.color = col; break;
                    case ImageComponentType.SpriteMask:
                        Debug.LogError($"Unable to assign color to '{reference.name}' as it is not a type that supports color");
                        break;
                    default:
                        Debug.LogError($"Unable to assign color to '{reference.name}' as it is not a type that supports color");
                        break;
                }
            }
        }

        public RectTransform RectTransform {
            get {
                switch(type) {
                    case ImageComponentType.UIImage:
                    case ImageComponentType.UIRawImage:
                        return (reference as UnityEngine.UI.Graphic).rectTransform;
                    default:
                        Debug.LogError("Unable to return a rect transform as reference is not of a supported type!");
                        return null;
                }
            }
        }

        public Transform Transform {
            get {
                switch(type) {
                    case ImageComponentType.Light:
                    case ImageComponentType.Renderer:
                    case ImageComponentType.Sprite:
                    case ImageComponentType.SpriteMask:
                    case ImageComponentType.UIImage:
                    case ImageComponentType.UIRawImage:
                        return (reference as Component).transform;
                    default:
                        Debug.LogError("Unable to return a transform as reference is not of a supported type!");
                        return null;
                }
            }
        }

        #endregion

        #region Constructed

        public ImageComponent(Component component) {
            refRenderer = null;
            refMaterial = null;
            refUIImage = null;
            refUIRawImage = null;
            refLight = null;
            refSprite = null;
            refSpriteMask = null;

            if(reference = component.GetComponent<UnityEngine.Renderer>())
                type = ImageComponentType.Renderer;
            else if(reference = component.GetComponent<UnityEngine.Material>())
                type = ImageComponentType.Material;
            else if(reference = component.GetComponent<UnityEngine.UI.Image>())
                type = ImageComponentType.UIImage;
            else if(reference = component.GetComponent<UnityEngine.UI.RawImage>())
                type = ImageComponentType.UIRawImage;
            else if(reference = component.GetComponent<UnityEngine.Light>())
                type = ImageComponentType.Light;
            else if(reference = component.GetComponent<UnityEngine.SpriteRenderer>())
                type = ImageComponentType.Sprite;
            else if(reference = component.GetComponent<UnityEngine.SpriteMask>())
                type = ImageComponentType.SpriteMask;
            else
                type = ImageComponentType.None;
        }

        public ImageComponent(UnityEngine.Renderer component) {
            refRenderer = component;
            refMaterial = null;
            refUIImage = null;
            refUIRawImage = null;
            refLight = null;
            refSprite = null;
            refSpriteMask = null;
            reference = component;
            type = component == null ? ImageComponentType.None : ImageComponentType.Renderer;
        }

        public ImageComponent(UnityEngine.Material component) {
            refRenderer = null;
            refMaterial = component;
            refUIImage = null;
            refUIRawImage = null;
            refLight = null;
            refSprite = null;
            refSpriteMask = null;
            reference = component;
            type = component == null ? ImageComponentType.None : ImageComponentType.Material;
        }

        public ImageComponent(UnityEngine.UI.Image component) {
            refRenderer = null;
            refMaterial = null;
            refUIImage = component;
            refUIRawImage = null;
            refLight = null;
            refSprite = null;
            refSpriteMask = null;
            reference = component;
            type = component == null ? ImageComponentType.None : ImageComponentType.UIImage;
        }

        public ImageComponent(UnityEngine.UI.RawImage component) {
            refRenderer = null;
            refMaterial = null;
            refUIImage = null;
            refUIRawImage = component;
            refLight = null;
            refSprite = null;
            refSpriteMask = null;
            reference = component;
            type = component == null ? ImageComponentType.None : ImageComponentType.UIRawImage;
        }

        public ImageComponent(UnityEngine.Light component) {
            refRenderer = null;
            refMaterial = null;
            refUIImage = null;
            refUIRawImage = null;
            refLight = component;
            refSprite = null;
            refSpriteMask = null;
            reference = component;
            type = component == null ? ImageComponentType.None : ImageComponentType.Light;
        }

        public ImageComponent(UnityEngine.SpriteRenderer component) {
            refRenderer = null;
            refMaterial = null;
            refUIImage = null;
            refUIRawImage = null;
            refLight = null;
            refSprite = component;
            refSpriteMask = null;
            reference = component;
            type = component == null ? ImageComponentType.None : ImageComponentType.Sprite;
        }

        public ImageComponent(UnityEngine.SpriteMask component) {
            refRenderer = null;
            refMaterial = null;
            refUIImage = null;
            refUIRawImage = null;
            refLight = null;
            refSprite = null;
            refSpriteMask = component;
            reference = component;
            type = component == null ? ImageComponentType.None : ImageComponentType.SpriteMask;
        }

        #endregion

        #region Static Find

        public static ImageComponent Find<T>(T component) where T : Component {
            UnityEngine.Object reference;
            if(reference = component.GetComponent<UnityEngine.Renderer>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.Renderer };
            else if(reference = component.GetComponent<UnityEngine.UI.Image>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.UIImage };
            else if(reference = component.GetComponent<UnityEngine.UI.RawImage>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.UIRawImage };
            else if(reference = component.GetComponent<UnityEngine.Light>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.Light };
            else if(reference = component.GetComponent<UnityEngine.SpriteRenderer>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.Sprite };
            else if(reference = component.GetComponent<UnityEngine.SpriteMask>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.SpriteMask };
            else
                return default;
        }

        public static ImageComponent Find(GameObject gameObject) {
            UnityEngine.Object reference;
            if(reference = gameObject.GetComponent<UnityEngine.Renderer>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.Renderer };
            else if(reference = gameObject.GetComponent<UnityEngine.UI.Image>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.UIImage };
            else if(reference = gameObject.GetComponent<UnityEngine.UI.RawImage>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.UIRawImage };
            else if(reference = gameObject.GetComponent<UnityEngine.Light>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.Light };
            else if(reference = gameObject.GetComponent<UnityEngine.SpriteRenderer>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.Sprite };
            else if(reference = gameObject.GetComponent<UnityEngine.SpriteMask>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.SpriteMask };
            else
                return default;
        }

        public static ImageComponent FindInChildren<T>(T component) where T : Component {
            UnityEngine.Object reference;
            if(reference = component.GetComponentInChildren<UnityEngine.Renderer>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.Renderer };
            else if(reference = component.GetComponentInChildren<UnityEngine.UI.Image>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.UIImage };
            else if(reference = component.GetComponentInChildren<UnityEngine.UI.RawImage>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.UIRawImage };
            else if(reference = component.GetComponentInChildren<UnityEngine.Light>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.Light };
            else if(reference = component.GetComponentInChildren<UnityEngine.SpriteRenderer>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.Sprite };
            else if(reference = component.GetComponentInChildren<UnityEngine.SpriteMask>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.SpriteMask };
            else
                return default;
        }

        public static ImageComponent FindInChildren(GameObject gameObject) {
            UnityEngine.Object reference;
            if(reference = gameObject.GetComponentInChildren<UnityEngine.Renderer>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.Renderer };
            else if(reference = gameObject.GetComponentInChildren<UnityEngine.UI.Image>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.UIImage };
            else if(reference = gameObject.GetComponentInChildren<UnityEngine.UI.RawImage>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.UIRawImage };
            else if(reference = gameObject.GetComponentInChildren<UnityEngine.Light>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.Light };
            else if(reference = gameObject.GetComponentInChildren<UnityEngine.SpriteRenderer>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.Sprite };
            else if(reference = gameObject.GetComponentInChildren<UnityEngine.SpriteMask>())
                return new ImageComponent() { reference = reference, type = ImageComponentType.SpriteMask };
            else
                return default;
        }

        public static ImageComponent[] FindAllInChildren<T>(T component) where T : Component {
            List<ImageComponent> imageComponents = new List<ImageComponent>();
            imageComponents.AddRange(component.GetComponentsInChildren<UnityEngine.Renderer>().Select(x => new ImageComponent() { reference = x, type = ImageComponentType.Renderer }));
            imageComponents.AddRange(component.GetComponentsInChildren<UnityEngine.UI.Image>().Select(x => new ImageComponent() { reference = x, type = ImageComponentType.UIImage }));
            imageComponents.AddRange(component.GetComponentsInChildren<UnityEngine.UI.RawImage>().Select(x => new ImageComponent() { reference = x, type = ImageComponentType.UIRawImage }));
            imageComponents.AddRange(component.GetComponentsInChildren<UnityEngine.Light>().Select(x => new ImageComponent() { reference = x, type = ImageComponentType.Light }));
            imageComponents.AddRange(component.GetComponentsInChildren<UnityEngine.SpriteRenderer>().Select(x => new ImageComponent() { reference = x, type = ImageComponentType.Sprite }));
            imageComponents.AddRange(component.GetComponentsInChildren<UnityEngine.SpriteMask>().Select(x => new ImageComponent() { reference = x, type = ImageComponentType.SpriteMask }));
            return imageComponents.ToArray();
        }

        public static void FindAllInChildren<T>(T component, List<ImageComponent> list) where T : Component {
            list.Clear();
            list.AddRange(component.GetComponentsInChildren<UnityEngine.Renderer>().Select(x => new ImageComponent() { reference = x, type = ImageComponentType.Renderer }));
            list.AddRange(component.GetComponentsInChildren<UnityEngine.UI.Image>().Select(x => new ImageComponent() { reference = x, type = ImageComponentType.UIImage }));
            list.AddRange(component.GetComponentsInChildren<UnityEngine.UI.RawImage>().Select(x => new ImageComponent() { reference = x, type = ImageComponentType.UIRawImage }));
            list.AddRange(component.GetComponentsInChildren<UnityEngine.Light>().Select(x => new ImageComponent() { reference = x, type = ImageComponentType.Light }));
            list.AddRange(component.GetComponentsInChildren<UnityEngine.SpriteRenderer>().Select(x => new ImageComponent() { reference = x, type = ImageComponentType.Sprite }));
            list.AddRange(component.GetComponentsInChildren<UnityEngine.SpriteMask>().Select(x => new ImageComponent() { reference = x, type = ImageComponentType.SpriteMask }));
        }

        #endregion

        #region ISerializationCallback Impl

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            if(reference is UnityEngine.Renderer)
                type = ImageComponentType.Renderer;
            else if(reference is UnityEngine.Material)
                type = ImageComponentType.Material;
            else if(reference is UnityEngine.UI.Image)
                type = ImageComponentType.UIImage;
            else if(reference is UnityEngine.UI.RawImage)
                type = ImageComponentType.UIRawImage;
            else if(reference is UnityEngine.Light)
                type = ImageComponentType.Light;
            else if(reference is UnityEngine.SpriteRenderer)
                type = ImageComponentType.Sprite;
            else if(reference is UnityEngine.SpriteMask)
                type = ImageComponentType.SpriteMask;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
        }

        #endregion

        #region Overloads

        public static implicit operator bool (ImageComponent comp) => comp.HasReference;

        #endregion
    }
}
