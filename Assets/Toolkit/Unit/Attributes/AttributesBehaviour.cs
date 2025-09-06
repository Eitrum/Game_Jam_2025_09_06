using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    [AddComponentMenu("Toolkit/Unit/Attributes")]
    public class AttributesBehaviour : MonoBehaviour, IAttributes {
        #region Variables

        private const string TAG = "[Toolkit.AttributesBehaviour] - ";

        [SerializeField] private Stat might = new Stat(0f, 0f, 0f);
        [SerializeField] private Stat toughness = new Stat(0f, 0f, 0f);
        [SerializeField] private Stat agility = new Stat(0f, 0f, 0f);
        [SerializeField] private Stat intelligence = new Stat(0f, 0f, 0f);
        [SerializeField] private Stat talent = new Stat(0f, 0f, 0f);

        public event OnAttributeChangedDelegate OnAttributeChanged;

        #endregion

        #region Properties

        public Stat this[AttributeType type] { get => GetAttribute(type); set => SetAttribute(type, value); }
        public Stat this[int index] { get => GetAttribute((AttributeType)(index - 1)); set => SetAttribute((AttributeType)(index - 1), value); }

        #endregion

        #region IAttributes Impl

        public Stat GetAttribute(AttributeType type) {
            switch(type) {
                case AttributeType.Might: return might;
                case AttributeType.Agility: return agility;
                case AttributeType.Toughness: return toughness;
                case AttributeType.Intelligence: return intelligence;
                case AttributeType.Talent: return talent;
            }
            throw new System.Exception(TAG + "Attribute Type not supported");

        }

        public void SetAttribute(AttributeType type, Stat value) {
            switch(type) {
                case AttributeType.Might:
                    might = value;
                    break;
                case AttributeType.Agility:
                    agility = value;
                    break;
                case AttributeType.Toughness:
                    toughness = value;
                    break;
                case AttributeType.Intelligence:
                    intelligence = value;
                    break;
                case AttributeType.Talent:
                    talent = value;
                    break;
                default:
                    throw new System.Exception(TAG + "Attribute Type not supported");
            }
            OnAttributeChanged?.Invoke(type);
        }

        #endregion
    }
}
