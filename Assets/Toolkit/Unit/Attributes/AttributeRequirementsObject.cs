using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    [CreateAssetMenu(menuName = "Toolkit/Unit/Attributes/Attribute Requirements")]
    public class AttributeRequirementsObject : ScriptableObject, IAttributeRequirements {
        #region variables
        private const string TAG = "[Toolkit.AttributeRequirementsObject] - ";

        [SerializeField] private List<AttributeRequirement> requirements = new List<AttributeRequirement>();

        #endregion

        #region Properties

        public IReadOnlyList<AttributeRequirement> Requirements => requirements;

        #endregion

        #region Util

        public bool HasRequirementType(AttributeType type) {
            foreach(var r in requirements)
                if(r.Type == type) return true;
            return false;
        }

        public bool HasRequirementType(AttributeType type, out float amount) {
            foreach(var r in requirements) {
                if(r.Type == type) {
                    amount = r.Amount;
                    return true;
                }
            }
            amount = 0f;
            return false;
        }

        #endregion
    }
}
