using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    [CreateAssetMenu(menuName = "Toolkit/Physics/Layer Settings")]
    public class PhysicsLayerSettings : ScriptableObject
    {
        #region Variables

        [SerializeField] private string[] layerNameOverrides = new string[32];
        [SerializeField] private int[] collisonMask = new int[32];

        #endregion

        #region Properties

        public IReadOnlyList<string> LayerNames => layerNameOverrides;
        public IReadOnlyList<int> LayerIgnoreMasks => collisonMask;

        #endregion

        #region Get / Set

        public bool IsIgnore(int layer0, int layer1) {
            var m = 1 << layer1;
            return (collisonMask[layer0] & m) == m;
        }

        public void SetIgnore(int layer0, int layer1, bool ignore) {
            var m = 1 << layer1;
            if(ignore)
                collisonMask[layer0] |= m;
            else
                collisonMask[layer0] &= ~m;
        }

        #endregion

        #region Baking

        [ContextMenu("Get Current")]
        private void Bake() {
            for(int x = 0; x < 32; x++) {
                int result = 0;
                for(int y = 0; y < 32; y++) {
                    if(Physics.GetIgnoreLayerCollision(x, y))
                        result |= 1 << y;
                }
                collisonMask[x] = result;
            }
        }

        #endregion

        #region Apply

        [ContextMenu("Apply")]
        public void Apply() {
            for(int x = 0; x < 32; x++) {
                var layer = collisonMask[x];
                for(int y = x; y < 32; y++) {
                    var m = 1 << y;
                    Physics.IgnoreLayerCollision(x, y, (layer & m) == m);
                }
            }
        }

        #endregion
    }
}
