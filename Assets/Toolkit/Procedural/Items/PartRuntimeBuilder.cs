using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Procedural.Items
{
    public class PartRuntimeBuilder : MonoBehaviour
    {
        #region Variables

        [SerializeField] private PartAssembly assembly = null;
        [SerializeField] private bool applyOnStart = true;
        [SerializeField] private Transform container = null;

        private int prefabChildIndex = 0;

        #endregion

        #region Properties

        public int ChildIndex {
            get => prefabChildIndex;
            set => prefabChildIndex = value;
        }

        #endregion

        #region Init

        private void Awake() {
            if(container == null) {
                container = transform;
                prefabChildIndex = container.childCount - 1;
            }
        }

        void Start() {
            if(applyOnStart) {
                Generate();
            }
        }

        #endregion

        #region Generate

        [ContextMenu("Generate")]
        public void Generate() {
            container.DestroyAllChildrenAfter(prefabChildIndex);
            if(assembly == null)
                return;
            assembly.CreateAssemblyInRoot(container);
        }

        public void Generate(PartBlueprint blueprint) {
            container.DestroyAllChildrenAfter(prefabChildIndex);
            if(assembly == null)
                return;
            assembly.CreateAssemblyInRoot(container, blueprint);
        }

        #endregion
    }
}
