using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    public static partial class AttributesUtility {
        public static string GetFullName(AttributeType type) {
            switch(type) {
                case AttributeType.Might: return "Might";
                case AttributeType.Intelligence: return "Intelligence";
                case AttributeType.Agility: return "Agility";
                case AttributeType.Toughness: return "Toughness";
                case AttributeType.Talent: return "Talent";
            }
            return "";
        }

        public static string GetShortName(AttributeType type) {
            switch(type) {
                case AttributeType.Might: return "Mgt";
                case AttributeType.Intelligence: return "Int";
                case AttributeType.Agility: return "Agi";
                case AttributeType.Toughness: return "Tgh";
                case AttributeType.Talent: return "Tal";
            }
            return "";
        }
    }
}
