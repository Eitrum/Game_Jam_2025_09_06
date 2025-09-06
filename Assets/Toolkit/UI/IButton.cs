using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.UI
{

    public delegate void OnButtonStateChangeCallback(ButtonState newState);
    public delegate void OnClickCallback();
    
    [System.Flags]
    public enum ButtonHighlightMode
    {
        None = 0,
        Color = 1,
        Sprite = 2,
        Texture = 4,
    }

    public enum ButtonState
    {
        Normal,
        Hover,
        Pressed,
        Disabled,
    }

    public enum ButtonMode
    {
        [InspectorName("Default (Pointer Up)"), Tooltip("Activate 'OnClick' when pointer is released. (Cancelable)")]
        Default = 0,
        [InspectorName("Pointer Down"), Tooltip("Activate 'OnClick' when pointer is pressed down. (Uncancellable)")]
        PointerDown = 1,
    }

    public interface IButton
    {
        bool Interactable { get; set; }
        ButtonState CurrentState { get; }
        ButtonMode Mode { get; set; }

        event OnClickCallback OnClick;
        event OnButtonStateChangeCallback OnStateChange;
        void Activate();
    }
}
