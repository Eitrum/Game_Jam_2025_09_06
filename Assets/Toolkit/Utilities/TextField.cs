#define TEXTMESHPRO
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Toolkit
{
    public enum TextType
    {
        None = 0,
        TextMesh = 1,
        UIText = 2,

#if TEXTMESHPRO
        TextMeshPro = 3
#else
        TextMeshProUnavailable = 3
#endif
    }

    [System.Serializable, StructLayout(LayoutKind.Explicit)]
    public struct TextField : ISerializationCallbackReceiver
    {
        #region Const Supported Properties

        public const bool TextMeshSupported = true;
        public const bool UITextSupported = true;
#if TEXTMESHPRO
        public const bool TextMeshProSupported = true;
#else
        public const bool TextMeshProSupported = false;
#endif

        #endregion

        #region Variables

        private const string TAG = "[TextField] - ";

        [FieldOffset(0), SerializeField] private UnityEngine.Object reference;
        [FieldOffset(0), System.NonSerialized] private UnityEngine.TextMesh refTextMesh;
        [FieldOffset(0), System.NonSerialized] private UnityEngine.UI.Text refUIText;
#if TEXTMESHPRO
        [FieldOffset(0), System.NonSerialized] private TMPro.TMP_Text refTMP;
#endif
        [FieldOffset(8), System.NonSerialized] private TextType type;

        #endregion

        #region Text Properties

        public TextType TextType => type;

        public string Text {
            get {
                switch(type) {
                    case TextType.TextMesh:
                        return refTextMesh.text;
                    case TextType.UIText:
                        return refUIText.text;
#if TEXTMESHPRO
                    case TextType.TextMeshPro:
                        return refTMP.text;
#endif
                }
                throw new System.Exception("Text Type is not of a valid type");
            }
            set {
#if UNITY_EDITOR
                for(int i = 0, length = value.Length; i < length; i++) {
                    if(value[i] == '\r') {
                        if(!(i >= length - 1 || value[i + 1] == '\n'))
                            Debug.LogWarning($"[TextField] - Contains return character (13) '{value}'\nNew Copy: '{value.Replace("\n", "")}'");
                    }
                }
#endif
                switch(type) {
                    case TextType.TextMesh:
                        refTextMesh.text = value;
                        break;
                    case TextType.UIText:
                        refUIText.text = value;
                        break;
#if TEXTMESHPRO
                    case TextType.TextMeshPro:
                        refTMP.text = value;
                        break;
#endif
                    default:
                        throw new System.Exception("Text Type is not of a valid type");
                }
            }
        }

        public int FontSize {
            get {
                switch(type) {
                    case TextType.TextMesh:
                        return refTextMesh.fontSize;
                    case TextType.UIText:
                        return refUIText.fontSize;
#if TEXTMESHPRO
                    case TextType.TextMeshPro:
                        return Mathf.RoundToInt(refTMP.fontSize);
#endif
                }
                throw new System.Exception("Text Type is not of a valid type");
            }
            set {
                switch(type) {
                    case TextType.TextMesh:
                        refTextMesh.fontSize = value;
                        break;
                    case TextType.UIText:
                        refUIText.fontSize = value;
                        break;
#if TEXTMESHPRO
                    case TextType.TextMeshPro:
                        refTMP.fontSize = value;
                        break;
#endif
                    default:
                        throw new System.Exception("Text Type is not of a valid type");
                }
            }
        }

        public bool RichText {
            get {
                switch(type) {
                    case TextType.TextMesh:
                        return refTextMesh.richText;
                    case TextType.UIText:
                        return refUIText.supportRichText;
#if TEXTMESHPRO
                    case TextType.TextMeshPro:
                        return refTMP.richText;
#endif
                }
                throw new System.Exception("Text Type is not of a valid type");
            }
            set {
                switch(type) {
                    case TextType.TextMesh:
                        refTextMesh.richText = value;
                        break;
                    case TextType.UIText:
                        refUIText.supportRichText = value;
                        break;
#if TEXTMESHPRO
                    case TextType.TextMeshPro:
                        refTMP.richText = value;
                        break;
#endif
                    default:
                        throw new System.Exception("Text Type is not of a valid type");
                }
            }
        }

        public Color Color {
            get {
                switch(type) {
                    case TextType.TextMesh:
                        return refTextMesh.color;
                    case TextType.UIText:
                        return refUIText.color;
#if TEXTMESHPRO
                    case TextType.TextMeshPro:
                        return refTMP.color;
#endif
                }
                throw new System.Exception("Text Type is not of a valid type");
            }
            set {
                switch(type) {
                    case TextType.TextMesh:
                        refTextMesh.color = value;
                        break;
                    case TextType.UIText:
                        refUIText.color = value;
                        break;
#if TEXTMESHPRO
                    case TextType.TextMeshPro:
                        refTMP.color = value;
                        break;
#endif
                    default:
                        throw new System.Exception("Text Type is not of a valid type");
                }
            }
        }

        public Font Font {
            get {
                switch(type) {
                    case TextType.TextMesh:
                        return refTextMesh.font;
                    case TextType.UIText:
                        return refUIText.font;
#if TEXTMESHPRO
                    case TextType.TextMeshPro:
                        throw new System.Exception("TextMesh Pro uses other font types than default, please refer to TextField.TMP_Font");
#endif
                }
                throw new System.Exception("Text Type is not of a valid type");
            }
            set {
                switch(type) {
                    case TextType.TextMesh:
                        refTextMesh.font = value;
                        break;
                    case TextType.UIText:
                        refUIText.font = value;
                        break;
#if TEXTMESHPRO
                    case TextType.TextMeshPro:
                        throw new System.Exception("TextMesh Pro uses other font types than default, please refer to TextField.TMP_Font");
#endif
                    default:
                        throw new System.Exception("Text Type is not of a valid type");
                }
            }
        }

#if TEXTMESHPRO
        public TMPro.TMP_FontAsset TMP_Font {
            get {
                switch(type) {
                    case TextType.TextMeshPro:
                        return refTMP.font;
                    case TextType.TextMesh:
                    case TextType.UIText:
                        throw new System.Exception("TMP_Font only supports TextMesh Pro TextFields");
                    default:
                        throw new System.Exception("Text Type is not of a valid type");
                }
            }
            set {
                switch(type) {
                    case TextType.TextMeshPro:
                        refTMP.font = value;
                        break;
                    case TextType.TextMesh:
                    case TextType.UIText:
                        throw new System.Exception("TMP_Font only supports TextMesh Pro TextFields");
                    default:
                        throw new System.Exception("Text Type is not of a valid type");
                }
            }
        }
#endif

        public float LineSpacing {
            get {
                switch(type) {
                    case TextType.TextMesh:
                        return refTextMesh.lineSpacing;
                    case TextType.UIText:
                        return refUIText.lineSpacing;
#if TEXTMESHPRO
                    case TextType.TextMeshPro:
                        return refTMP.lineSpacing;
#endif
                }
                throw new System.Exception("Text Type is not of a valid type");
            }
            set {
                switch(type) {
                    case TextType.TextMesh:
                        refTextMesh.lineSpacing = value;
                        break;
                    case TextType.UIText:
                        refUIText.lineSpacing = value;
                        break;
#if TEXTMESHPRO
                    case TextType.TextMeshPro:
                        refTMP.lineSpacing = value;
                        break;
#endif
                    default:
                        throw new System.Exception("Text Type is not of a valid type");
                }
            }
        }

        public bool IsValid {
            get {
                switch(type) {
                    case TextType.TextMesh:
                        return refTextMesh != null;
                    case TextType.UIText:
                        return refUIText != null;
#if TEXTMESHPRO
                    case TextType.TextMeshPro:
                        return refTMP != null;
#endif
                }
                return false;
            }
        }

        public bool Enabled {
            get {
                switch(type) {
                    case TextType.TextMesh:
                        return refTextMesh.gameObject.activeSelf;
                    case TextType.UIText:
                        return refUIText.enabled;
#if TEXTMESHPRO
                    case TextType.TextMeshPro:
                        return refTMP.enabled;
#endif
                }
                throw new System.Exception("Text Type is not of a valid type");
            }
            set {
                switch(type) {
                    case TextType.TextMesh:
                        refTextMesh.gameObject.SetActive(value);
                        break;
                    case TextType.UIText:
                        refUIText.enabled = value;
                        break;
#if TEXTMESHPRO
                    case TextType.TextMeshPro:
                        refTMP.enabled = value;
                        break;
#endif
                    default:
                        throw new System.Exception("Text Type is not of a valid type");
                }
            }
        }

        public Transform Transform {
            get {
                switch(type) {
                    case TextType.TextMesh:
                        return refTextMesh.transform;
                    case TextType.UIText:
                        return refUIText.transform;
#if TEXTMESHPRO
                    case TextType.TextMeshPro:
                        return refTMP.transform;
#endif
                }
                throw new System.Exception("Text Type is not of a valid type");
            }
        }

        public RectTransform RectTransform {
            get {
                switch(type) {
                    case TextType.UIText:
                        return refUIText.rectTransform;
#if TEXTMESHPRO
                    case TextType.TextMeshPro:
                        return refTMP.rectTransform;
#endif
                }
                throw new System.Exception("Text Type is not of a valid type");
            }
        }

        public UnityEngine.Object Reference {
            get {
                return reference;
            }
            set {
                if(value is UnityEngine.UI.Text) {
                    reference = value;
                    type = TextType.UIText;
                }
                else if(value is TextMesh) {
                    reference = value;
                    type = TextType.TextMesh;
                }
#if TEXTMESHPRO
                else if(value is TMPro.TMP_Text) {
                    reference = value;
                    type = TextType.TextMeshPro;
                }
#endif
                else {
                    type = TextType.None;
                    throw new System.Exception("TextField Reference provided is not of a supported type");
                }
            }
        }

        #endregion

        #region Constructed

        public TextField(Component component) {
            refTextMesh = null;
            refUIText = null;
#if TEXTMESHPRO
            refTMP = null;
#endif

            if(reference = component.GetComponent<UnityEngine.UI.Text>()) {
                type = TextType.UIText;
            }
            else if(reference = component.GetComponent<UnityEngine.TextMesh>()) {
                type = TextType.TextMesh;
            }
#if TEXTMESHPRO
            else if(reference = component.GetComponent<TMPro.TMP_Text>()) {
                type = TextType.TextMeshPro;
            }
#endif
            else {
                type = TextType.None;
            }
        }

        #endregion

        #region Static Find

        public static TextField Find<T>(T component) where T : Component {
            UnityEngine.Object reference;

            if(reference = component.GetComponent<UnityEngine.UI.Text>()) {
                return new TextField() { reference = reference, type = TextType.UIText };
            }
            else if(reference = component.GetComponent<TextMesh>()) {
                return new TextField() { reference = reference, type = TextType.TextMesh };
            }
#if TEXTMESHPRO
            else if(reference = component.GetComponent<TMPro.TMP_Text>()) {
                return new TextField() { reference = reference, type = TextType.TextMeshPro };
            }
#endif
            else {
                return default;
            }
        }

        public static TextField FindInChildren<T>(T component) where T : Component {
            UnityEngine.Object reference;

            if(reference = component.GetComponentInChildren<UnityEngine.UI.Text>()) {
                return new TextField() { reference = reference, type = TextType.UIText };
            }
            else if(reference = component.GetComponentInChildren<TextMesh>()) {
                return new TextField() { reference = reference, type = TextType.TextMesh };
            }
#if TEXTMESHPRO
            else if(reference = component.GetComponentInChildren<TMPro.TMP_Text>()) {
                return new TextField() { reference = reference, type = TextType.TextMeshPro };
            }
#endif
            else {
                return default;
            }
        }

        public static TextField[] FindAllInChildren<T>(T component) where T : Component {
            List<TextField> textFields = new List<TextField>();
            textFields.AddRange(component.GetComponentsInChildren<UnityEngine.UI.Text>().Select(x => new TextField() { reference = x, type = TextType.UIText }));
            textFields.AddRange(component.GetComponentsInChildren<TextMesh>().Select(x => new TextField() { reference = x, type = TextType.TextMesh }));
#if TEXTMESHPRO
            textFields.AddRange(component.GetComponentsInChildren<TMPro.TMP_Text>().Select(x => new TextField() { reference = x, type = TextType.TextMeshPro }));
#endif
            return textFields.ToArray();
        }

        public static void FindAllInChildren<T>(T component, List<TextField> list) where T : Component {
            list.Clear();
            list.AddRange(component.GetComponentsInChildren<UnityEngine.UI.Text>().Select(x => new TextField() { reference = x, type = TextType.UIText }));
            list.AddRange(component.GetComponentsInChildren<TextMesh>().Select(x => new TextField() { reference = x, type = TextType.TextMesh }));
#if TEXTMESHPRO
            list.AddRange(component.GetComponentsInChildren<TMPro.TMP_Text>().Select(x => new TextField() { reference = x, type = TextType.TextMeshPro }));
#endif
        }

        #endregion

        #region Conversion

        public static implicit operator bool(TextField tf) => tf.reference != null;

        public static explicit operator TextField(UnityEngine.TextMesh text) => new TextField(text);
        public static implicit operator UnityEngine.TextMesh(TextField tf) {
#if UNITY_EDITOR
            if(tf.type != TextType.TextMesh) {
                Debug.LogError(TAG + "Conversion to text mesh is not possible");
                return null;
            }
#endif
            return tf.refTextMesh;
        }

        public static explicit operator TextField(UnityEngine.UI.Text text) => new TextField(text);
        public static implicit operator UnityEngine.UI.Text(TextField tf) {
#if UNITY_EDITOR
            if(tf.type != TextType.UIText) {
                Debug.LogError(TAG + "Conversion to UI text is not possible");
                return null;
            }
#endif
            return tf.refUIText;
        }

#if TEXTMESHPRO
        public static explicit operator TextField(TMPro.TMP_Text tmp) => new TextField(tmp);
        public static implicit operator TMPro.TMP_Text(TextField tf) {
#if UNITY_EDITOR
            if(tf.type != TextType.TextMeshPro) {
                Debug.LogError(TAG + "Conversion to text mesh PRO is not possible");
                return null;
            }
#endif
            return tf.refTMP;
        }
#endif

        #endregion

        #region ISerializationCallbackReceiver Impl

        void ISerializationCallbackReceiver.OnBeforeSerialize() {

        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            if(reference is UnityEngine.UI.Text) {
                type = TextType.UIText;
            }
            else if(reference is TextMesh) {
                type = TextType.TextMesh;
            }
#if TEXTMESHPRO
            else if(reference is TMPro.TMP_Text) {
                type = TextType.TextMeshPro;
            }
#endif
            else {
                type = TextType.None;
            }
        }

        #endregion
    }
}
