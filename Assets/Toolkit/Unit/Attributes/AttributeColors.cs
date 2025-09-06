using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    public static partial class AttributesUtility {
        public static Color GetColor(AttributeType type) {
            switch(type) {
                case AttributeType.Might: return Color.red;
                case AttributeType.Agility: return Color.green;
                case AttributeType.Intelligence: return Color.blue;
                case AttributeType.Toughness: return Color.gray;
                case AttributeType.Talent: return Color.yellow;
            }
            return Color.white;
        }
    }
}
