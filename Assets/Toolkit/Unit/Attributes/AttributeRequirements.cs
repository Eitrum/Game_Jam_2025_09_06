using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    [System.Serializable]
    public class AttributeRequirements : IAttributeRequirements {

        #region variables
        private const string TAG = "[Toolkit.AttributeRequirements] - ";

        [SerializeField] private List<AttributeRequirement> requirements = new List<AttributeRequirement>();

        #endregion

        #region Properties

        public IReadOnlyList<AttributeRequirement> Requirements => requirements;

        #endregion

        #region Constructor

        public AttributeRequirements() { }

        public AttributeRequirements(AttributeType type, float amount) {
            requirements.Add(new AttributeRequirement(type, amount));
        }

        public AttributeRequirements(AttributeRequirement requirement) {
            requirements.Add(requirement);
        }

        public AttributeRequirements(AttributeRequirement req0, AttributeRequirement req1) {
            requirements.Add(req0);
            requirements.Add(req1);
        }

        public AttributeRequirements(AttributeRequirement req0, AttributeRequirement req1, AttributeRequirement req2) {
            requirements.Add(req0);
            requirements.Add(req1);
            requirements.Add(req2);
        }

        public AttributeRequirements(params AttributeRequirement[] requirements) {
            this.requirements.AddRange(requirements);
        }

        #endregion

        #region Add / Remove

        public void Add(AttributeType type, float amount) {
            Add(new AttributeRequirement(type, amount), false);
        }

        public void Add(AttributeType type, float amount, bool overrideValues) {
            Add(new AttributeRequirement(type, amount), overrideValues);
        }

        public void Add(AttributeRequirement req) {
            Add(req, false);
        }

        public void Add(AttributeRequirement req, bool overrideValues) {
            var t = req.Type;
            for(int i = 0, length = requirements.Count; i < length; i++) {
                if(requirements[i].Type == t) {
                    if(overrideValues) {
                        requirements[i] = req;
                        return;
                    }
                    else {
                        Debug.LogError(TAG + "Already have a requirement of type");
                        return;
                    }
                }
            }
            this.requirements.Add(req);
        }

        public bool Remove(AttributeType type) {
            for(int i = requirements.Count - 1; i >= 0; i--) {
                if(requirements[i].Type == type) {
                    requirements.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public bool Remove(AttributeRequirement req) {
            var t = req.Type;
            for(int i = requirements.Count - 1; i >= 0; i--) {
                if(requirements[i].Type == t) {
                    requirements.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Util

        public void Clear() {
            requirements.Clear();
        }

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
