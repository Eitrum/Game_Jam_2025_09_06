using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit {
    [CustomPropertyDrawer(typeof(LineDecoAttribute))]
    public class LineDecoAttributeDrawer : DecoratorDrawer {
        public override float GetHeight() {
            return (attribute as LineDecoAttribute).Thickness;
        }

        public override void OnGUI(Rect position) {
            var ld = (attribute as LineDecoAttribute);
            if(ld.Width > Mathf.Epsilon)
                position.width = Mathf.Min(ld.Width, position.width);
            EditorGUI.DrawRect(position, ld.Color);
        }
    }
}
