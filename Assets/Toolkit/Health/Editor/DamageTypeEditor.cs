using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Toolkit.Health
{
    [InitializeOnLoad]
    [CustomPropertyDrawer(typeof(DamageType))]
    [CustomPropertyDrawer(typeof(DamageTypeAttribute))]
    public class DamageTypeEditor : PropertyDrawer
    {
        #region Variables

        private const string TYPE_SYSTEM_PREFS_PATH = "Toolkit.DamageTypeSystem";

        #endregion

        #region Properties

        public static DamageTypeSystem DamageTypeSystem {
            get => EditorPrefs.GetInt(TYPE_SYSTEM_PREFS_PATH, 0).ToEnum<DamageTypeSystem>();
            set {
                if(value != DamageTypeSystem) {
                    EditorPrefs.SetInt(TYPE_SYSTEM_PREFS_PATH, value.ToInt());
                }
            }
        }

        #endregion

        #region Base OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if(property.propertyType == SerializedPropertyType.Integer || property.propertyType == SerializedPropertyType.Enum) {
                property.intValue = Draw(position, label, property.intValue, (attribute as DamageTypeAttribute)?.useMask ?? true);
            }
            else {
                EditorGUI.LabelField(position, new GUIContent($"Error drawing element '{label.text}'"));
            }
        }

        #endregion

        #region Draw

        public static int Draw(Rect position, int intValue, bool useMask) {
            switch(DamageTypeSystem) {
                case DamageTypeSystem.None:
                    if(useMask)
                        return EditorGUI.EnumFlagsField(position, intValue.ToEnum<DamageType>()).ToInt();
                    else
                        return EditorGUI.EnumPopup(position, intValue.ToEnum<DamageType>()).ToInt();
                case DamageTypeSystem.DungeonsAndDragons:
                    return DrawDnD(position, intValue, useMask);
                case DamageTypeSystem.Custom:
                    if(useMask)
                        return EditorGUI.EnumFlagsField(position, intValue.ToEnum<DamageTypeCustom>()).ToInt();
                    else
                        return EditorGUI.EnumPopup(position, intValue.ToEnum<DamageTypeCustom>()).ToInt();
            }
            return 0;
        }

        public static int Draw(Rect position, GUIContent label, int intValue, bool useMask) {
            switch(DamageTypeSystem) {
                case DamageTypeSystem.None:
                    if(useMask)
                        return EditorGUI.EnumFlagsField(position, label, intValue.ToEnum<DamageType>()).ToInt();
                    else
                        return EditorGUI.EnumPopup(position, label, intValue.ToEnum<DamageType>()).ToInt();
                case DamageTypeSystem.DungeonsAndDragons:
                    return DrawDnD(position, intValue, label, useMask);
                case DamageTypeSystem.Custom:
                    if(useMask)
                        return EditorGUI.EnumFlagsField(position, label, intValue.ToEnum<DamageTypeCustom>()).ToInt();
                    else
                        return EditorGUI.EnumPopup(position, label, intValue.ToEnum<DamageTypeCustom>()).ToInt();
            }
            return 0;
        }

        private static int DrawDnD(Rect pos, int intValue, bool useMask) {
            if(useMask) {
                return EditorGUI.MaskField(pos, intValue, DnD_Paths_Mask);
            }
            else {
                var mask = intValue;
                var index = mask.GetFlagIndex() + 1; // This is to convert -1 -> 31 to also include "none"
                var selected = EditorGUI.Popup(pos, index, DnD_Paths);
                return selected > 0 ? 1 << (selected - 1) : 0;
            }
        }

        private static int DrawDnD(Rect pos, int intValue, GUIContent label, bool useMask) {
            if(useMask) {
                return EditorGUI.MaskField(pos, label, intValue, DnD_Paths_Mask);
            }
            else {
                var mask = intValue;
                var index = mask.GetFlagIndex() + 1; // This is to convert -1 -> 31 to also include "none"
                var selected = EditorGUI.Popup(pos, label, index, DnD_Paths);
                return selected > 0 ? 1 << (selected - 1) : 0;
            }
        }

        #endregion

        #region GUI Contents

        /// <summary>
        /// Direct copy from DnD @ https://roll20.net/compendium/dnd5e/Combat#toc_50
        /// </summary>
        private static GUIContent[] DnD_Paths = new GUIContent[]{
            new GUIContent("None", "For when you do not want to deal any damage type. It's true damage!"),
            new GUIContent("Normal/Bludgeoning", "Blunt force attacks—hammers, Falling, constriction, and the like—deal bludgeoning damage."),
            new GUIContent("Normal/Piercing", "Puncturing and impaling attacks, including spears and monsters’ bites, deal piercing damage."),
            new GUIContent("Normal/Slashing", "Swords, axes, and monsters’ claws deal slashing damage."),
            new GUIContent("Magic/Acid", "The corrosive spray of a black dragon’s breath and the dissolving enzymes secreted by a Black Pudding deal acid damage."),
            new GUIContent("Magic/Cold", "The Infernal chill radiating from an Ice Devil’s spear and the frigid blast of a white dragon’s breath deal cold damage."),
            new GUIContent("Magic/Fire", "Red Dragons breathe fire, and many Spells conjure flames to deal fire damage."),
            new GUIContent("Magic/Force", "Force is pure magical energy focused into a damaging form. Most Effects that deal force damage are Spells, including Magic Missile and Spiritual Weapon."),
            new GUIContent("Magic/Lightning", "A Lightning Bolt spell and a blue dragon’s breath deal lightning damage."),
            new GUIContent("Magic/Necrotic", "Necrotic damage, dealt by certain Undead and a spell such as Chill Touch, withers matter and even the soul."),
            new GUIContent("Magic/Poison", "Venomous stings and the toxic gas of a green dragon’s breath deal poison damage."),
            new GUIContent("Magic/Psychic", "Mental Abilities such as a mind flayer’s psionic blast deal psychic damage."),
            new GUIContent("Magic/Radiant", "Radiant damage, dealt by a cleric’s Flame Strike spell or an angel’s smiting weapon, sears the flesh like fire and overloads the spirit with power."),
            new GUIContent("Magic/Thunder", "A concussive burst of sound, such as the effect of the Thunderwave spell, deals thunder damage.")
        };

        private static string[] DnD_Paths_Mask = new string[]{
            "Normal/Bludgeoning",
            "Normal/Piercing",
            "Normal/Slashing",
            "Magic/Acid",
            "Magic/Cold",
            "Magic/Fire",
            "Magic/Force",
            "Magic/Lightning",
            "Magic/Necrotic",
            "Magic/Poison",
            "Magic/Psychic",
            "Magic/Radiant",
            "Magic/Thunder"
        };

        #endregion
    }
}
