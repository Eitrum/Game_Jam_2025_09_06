using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    public static partial class AttributesUtility {
        #region Requirement Checks

        public static bool HasRequirements(this IAttributes attr, IAttributeRequirements req) {
            foreach(var r in req.Requirements) {
                if(attr.GetAttribute(r.Type).Total < r.Amount)
                    return false;
            }
            return true;
        }

        public static bool HasRequirements(this IAttributes attr, IAttributeRequirement requirement) {
            return attr.GetAttribute(requirement.Type).Total >= requirement.Amount;
        }

        public static bool HasRequirements(this IAttributes attr, AttributeRequirement req) {
            return attr.GetAttribute(req.Type).Total >= req.Amount;
        }

        public static bool HasRequirements(this IAttributes attr, AttributeType type, float amount) {
            return attr.GetAttribute(type).Total >= amount;
        }

        #endregion


    }
}
