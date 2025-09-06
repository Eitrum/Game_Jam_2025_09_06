using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Toolkit.UI
{
    public class Button : MonoBehaviour, IButton, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler, IDragHandler
    {
        #region Graphics

        [System.Serializable]
        public class GraphicsReference
        {
            #region Variables

            [SerializeField] private UnityEngine.UI.Graphic graphic = null;
            [SerializeField] private UnityEngine.UI.Image imageReference = null;
            [SerializeField] private UnityEngine.UI.RawImage rawImageReference = null;
            [SerializeField] private bool ignore = false;

            private Texture defaultTexture;
            private Color defaultColor;

            [SerializeField] private ButtonHighlightMode highlightMode = ButtonHighlightMode.Color;

            [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 1f);
            [SerializeField] private Color hoverColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            [SerializeField] private Color pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
            [SerializeField] private Color disabledColor = new Color(0.8f, 0.8f, 0.8f, 0.6f);

            [SerializeField] private Sprite normalSprite = null;
            [SerializeField] private Sprite hoverSprite = null;
            [SerializeField] private Sprite pressedSprite = null;
            [SerializeField] private Sprite disabledSprite = null;

            [SerializeField] private Texture normalTexture = null;
            [SerializeField] private Texture hoverTexture = null;
            [SerializeField] private Texture pressedTexture = null;
            [SerializeField] private Texture disabledTexture = null;

            #endregion

            #region Properties

            public UnityEngine.UI.Graphic Graphic {
                get => graphic;
                set {
                    if(graphic != value) {
                        graphic = value;
                        if(value != null) {
                            imageReference = value.GetComponent<UnityEngine.UI.Image>();
                            rawImageReference = value.GetComponent<UnityEngine.UI.RawImage>();
                            if(rawImageReference)
                                defaultTexture = rawImageReference.texture;
                        }
                    }
                }
            }
            public bool Ignore { get => ignore; set => ignore = value; }
            public ButtonHighlightMode HighlightMode {
                get => highlightMode;
                set => highlightMode = value;
            }

            #endregion

            #region Setup

            public void Setup() {
                if(rawImageReference)
                    defaultTexture = rawImageReference.texture;
                if(highlightMode.HasFlag(ButtonHighlightMode.Color))
                    defaultColor = graphic.color;
            }

            public void SetColors(Color normal, Color hover, Color pressed, Color disabled) {
                this.normalColor = normal;
                this.hoverColor = hover;
                this.pressedColor = pressed;
                this.disabledColor = disabled;
            }

            public void SetSprites(Sprite normal, Sprite hover, Sprite pressed, Sprite disabled) {
                this.normalSprite = normal;
                this.hoverSprite = hover;
                this.pressedSprite = pressed;
                this.disabledSprite = disabled;
            }

            public void SetTextures(Texture normal, Texture hover, Texture pressed, Texture disabled) {
                this.normalTexture = normal;
                this.hoverTexture = hover;
                this.pressedTexture = pressed;
                this.disabledTexture = disabled;
            }

            #endregion

            #region Update

            public void Update(ButtonState state) {
                if(ignore)
                    return;
                switch(state) {
                    case ButtonState.Normal:
                        if(highlightMode.HasFlag(ButtonHighlightMode.Color))
                            graphic.color = normalColor;
                        if(highlightMode.HasFlag(ButtonHighlightMode.Sprite))
                            imageReference.overrideSprite = normalSprite;
                        if(highlightMode.HasFlag(ButtonHighlightMode.Texture))
                            rawImageReference.texture = normalTexture == null ? defaultTexture : normalTexture;
                        break;

                    case ButtonState.Hover:
                        if(highlightMode.HasFlag(ButtonHighlightMode.Color))
                            graphic.color = hoverColor;
                        if(highlightMode.HasFlag(ButtonHighlightMode.Sprite))
                            imageReference.overrideSprite = hoverSprite;
                        if(highlightMode.HasFlag(ButtonHighlightMode.Texture))
                            rawImageReference.texture = hoverTexture == null ? defaultTexture : hoverTexture;
                        break;

                    case ButtonState.Pressed:
                        if(highlightMode.HasFlag(ButtonHighlightMode.Color))
                            graphic.color = pressedColor;
                        if(highlightMode.HasFlag(ButtonHighlightMode.Sprite))
                            imageReference.overrideSprite = pressedSprite;
                        if(highlightMode.HasFlag(ButtonHighlightMode.Texture))
                            rawImageReference.texture = pressedTexture == null ? defaultTexture : pressedTexture;
                        break;

                    case ButtonState.Disabled:
                        if(highlightMode.HasFlag(ButtonHighlightMode.Color))
                            graphic.color = disabledColor;
                        if(highlightMode.HasFlag(ButtonHighlightMode.Sprite))
                            imageReference.overrideSprite = disabledSprite;
                        if(highlightMode.HasFlag(ButtonHighlightMode.Texture))
                            rawImageReference.texture = disabledTexture == null ? defaultTexture : disabledTexture;
                        break;
                }
            }

            #endregion
        }

        #endregion

        #region Variables

        private const string TAG = "<color=brown>[Button]</color> - ";

        [SerializeField] private List<GraphicsReference> graphics = new List<GraphicsReference>();
        [SerializeField] private Animator animatorReference = null;
        [SerializeField] private Animation animationReference = null;

        [SerializeField] private AnimationClip normalClip = null;
        [SerializeField] private AnimationClip hoverClip = null;
        [SerializeField] private AnimationClip pressedClip = null;
        [SerializeField] private AnimationClip disabledClip = null;

        [SerializeField] private string normalTrigger = null;
        [SerializeField] private string hoverTrigger = null;
        [SerializeField] private string pressedTrigger = null;
        [SerializeField] private string disabledTrigger = null;

        [Header("Settings")]
        [SerializeField] private ButtonMode mode = ButtonMode.Default;
        [SerializeField] private bool interactable = true;
        [SerializeField] private bool dragPassthrough = true;
        [SerializeField] private UnityEngine.Events.UnityEvent unityOnClick;
        [SerializeField] private bool debug = false;

        private ButtonState state = ButtonState.Normal;
        private OnClickCallback onClick;
        private OnButtonStateChangeCallback onStateChange;

        #endregion

        #region Properties

        public bool Interactable {
            get => interactable;
            set {
                DebugLog($"Updating interactability from '{interactable}' to '{value}'.");
                interactable = value;
                if(!value)
                    state = ButtonState.Disabled;
                else if(state == ButtonState.Disabled)
                    state = ButtonState.Normal;
                UpdateVisuals();
            }
        }
        public bool IsPressed => state == ButtonState.Pressed;

        public ButtonState CurrentState { get => state; }
        public ButtonMode Mode {
            get => mode;
            set => mode = value;
        }

        public event OnClickCallback OnClick {
            add => onClick += value;
            remove => onClick -= value;
        }

        public event OnButtonStateChangeCallback OnStateChange {
            add => onStateChange += value;
            remove => onStateChange -= value;
        }

        #endregion

        #region Enable/Disable

        void Awake() {
            for(int i = 0, length = graphics.Count; i < length; i++) {
                graphics[i].Setup();
            }

            // Setup legacy animation clips if they do not exist on animation component.
            if(animationReference) {
                if(!animationReference.GetClip(normalClip.name))
                    animationReference.AddClip(normalClip, normalClip.name);
                if(!animationReference.GetClip(hoverClip.name))
                    animationReference.AddClip(hoverClip, hoverClip.name);
                if(!animationReference.GetClip(pressedClip.name))
                    animationReference.AddClip(pressedClip, pressedClip.name);
                if(!animationReference.GetClip(disabledClip.name))
                    animationReference.AddClip(disabledClip, disabledClip.name);
            }
            state = interactable ? ButtonState.Normal : ButtonState.Disabled;
        }

        void Start() {
            UpdateVisuals();
        }

        void OnEnable() {
            SetState(ButtonState.Normal);
        }

        void OnDisable() {
            SetState(ButtonState.Normal);
        }

        #endregion

        #region Utility Methods

        public void Activate() {
            DebugLog($"Activated '{this.name}'!");
            onClick?.Invoke();
            unityOnClick.Invoke();
        }

        private void SetState(ButtonState state) {
            if(!interactable) {
                DebugLogWarning($"Unable to change state as '{this.name}' is disabled/non-interactable.");
                return;
            }
            if(state != this.state) {
                DebugLog($"Changing state from '{this.state}' to '{state}' on object '{this.name}'");
                this.state = state;
                UpdateVisuals();
                onStateChange?.Invoke(state);
            }
        }

        private void UpdateVisuals() {
            // Handle Graphics
            DebugLog($"Update graphics objects on '{this.name}' with state '{state}'");
            for(int i = 0, length = graphics.Count; i < length; i++) {
                graphics[i].Update(state);
            }

            // Handle Animations
            switch(state) {
                case ButtonState.Normal:
                    if(animationReference && normalClip)
                        animationReference.Play(normalClip.name, PlayMode.StopAll);
                    if(animatorReference && !string.IsNullOrEmpty(normalTrigger))
                        animatorReference.SetTrigger(normalTrigger);
                    break;
                case ButtonState.Hover:
                    if(animationReference && hoverClip)
                        animationReference.Play(hoverClip.name, PlayMode.StopAll);
                    if(animatorReference && !string.IsNullOrEmpty(hoverTrigger))
                        animatorReference.SetTrigger(hoverTrigger);
                    break;
                case ButtonState.Pressed:
                    if(animationReference && pressedClip)
                        animationReference.Play(pressedClip.name, PlayMode.StopAll);
                    if(animatorReference && !string.IsNullOrEmpty(pressedTrigger))
                        animatorReference.SetTrigger(pressedTrigger);
                    break;
                case ButtonState.Disabled:
                    if(animationReference && disabledClip)
                        animationReference.Play(disabledClip.name, PlayMode.StopAll);
                    if(animatorReference && !string.IsNullOrEmpty(disabledTrigger))
                        animatorReference.SetTrigger(disabledTrigger);
                    break;
            }
        }

        #endregion

        #region Pointer Handlers

        void IDragHandler.OnDrag(PointerEventData eventData) {
            if(transform is RectTransform rt && RectTransformUtility.RectangleContainsScreenPoint(rt, eventData.position))
                SetState(ButtonState.Hover);
            if(dragPassthrough) {
                var pDragHandler = transform.parent.GetComponentInParent<IDragHandler>();
                if(pDragHandler != null)
                    pDragHandler.OnDrag(eventData);
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            if(interactable) {
                DebugLog($"OnPointerDown '{this.name}' and object is currently interactable with mode '{mode}'.");
                SetState(ButtonState.Pressed);
                if(mode == ButtonMode.PointerDown)
                    Activate();
            }
            else {
                DebugLog($"OnPointerDown '{this.name}' and object is currently not interactable.");
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            SetState(ButtonState.Hover);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            SetState(ButtonState.Normal);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            var isPressed = state == ButtonState.Pressed;
            var raycaster = eventData.pointerCurrentRaycast;
            if(raycaster.isValid && raycaster.gameObject.GetComponentInParent<IPointerUpHandler>() == (this as IPointerUpHandler)) {
                DebugLog($"OnPointerUp inside the button object '{this.name}' and object is currently {(isPressed ? "pressed" : "not pressed")}.");
                SetState(ButtonState.Hover);
            }
            else {
                DebugLog($"OnPointerUp outside the button object '{this.name}' and object is currently {(isPressed ? "pressed" : "not pressed")}.");
                SetState(ButtonState.Normal);
                isPressed = false;
            }
            if(isPressed && mode == ButtonMode.Default)
                Activate();
        }

        #endregion

        #region Debugging

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void DebugLog(object message) {
            if(debug)
                Debug.Log(TAG + message, this);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void DebugLogWarning(object message) {
            if(debug)
                Debug.LogWarning(TAG + message, this);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void DebugLogError(object message) {
            if(debug)
                Debug.LogError(TAG + message, this);
        }

        #endregion
    }
}
