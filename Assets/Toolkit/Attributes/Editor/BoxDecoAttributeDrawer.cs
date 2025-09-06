using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit {
    [CustomPropertyDrawer(typeof(BoxDecoAttribute))]
    public class BoxDecoAttributeDrawer : DecoratorDrawer {
        public override float GetHeight() {
            return 0f;
        }

        public override void OnGUI(Rect position) {
            var bd = attribute as BoxDecoAttribute;
            position.height = EditorGUIUtility.singleLineHeight * bd.Elements;
            position.ShrinkRef(-2f);
            var col = bd.Color;
            col.a = 0.2f;
            EditorGUI.DrawRect(position, col);
        }
    }
}
