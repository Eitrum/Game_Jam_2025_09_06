using UnityEngine;
using System;
namespace Toolkit.Unit {
    [System.Serializable]
    public class Attributes : IAttributes {
        public Toolkit.Stat Might = new Stat(0f, 0f, 0f);
        public Toolkit.Stat Toughness = new Stat(0f, 0f, 0f);
        public Toolkit.Stat Agility = new Stat(0f, 0f, 0f);
        public Toolkit.Stat Intelligence = new Stat(0f, 0f, 0f);
        public Toolkit.Stat Talent = new Stat(0f, 0f, 0f);

        public Toolkit.Stat this[AttributeType type] { get => GetAttribute(type); set => SetAttribute(type, value); }
        public Toolkit.Stat this[int index] { get => GetAttribute((AttributeType)(index - 1)); set => SetAttribute((AttributeType)(index - 1), value); }

        public event OnAttributeChangedDelegate OnAttributeChanged;

        public Toolkit.Stat GetAttribute(AttributeType type) {
            switch(type) {
                case AttributeType.Might: return Might;
                case AttributeType.Agility: return Agility;
                case AttributeType.Toughness: return Toughness;
                case AttributeType.Intelligence: return Intelligence;
                case AttributeType.Talent: return Talent;
            }
            throw new System.Exception("Attribute Type not supported");

        }

        public void SetAttribute(AttributeType type, Toolkit.Stat value) {
            switch(type) {
                case AttributeType.Might:
                    Might = value;
                    break;
                case AttributeType.Agility:
                    Agility = value;
                    break;
                case AttributeType.Toughness:
                    Toughness = value;
                    break;
                case AttributeType.Intelligence:
                    Intelligence = value;
                    break;
                case AttributeType.Talent:
                    Talent = value;
                    break;
            }
            OnAttributeChanged?.Invoke(type);
        }

        public static string GetFullName(AttributeType type) => AttributesUtility.GetFullName(type);
        public static string GetShortName(AttributeType type) => AttributesUtility.GetShortName(type);
    }
}
