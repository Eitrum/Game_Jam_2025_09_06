using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Rendering.ModularRenderPipeline {
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class MRPCamera : MonoBehaviour {
        #region Varibles

        [SerializeField, UseObjectName] private string identifier;
        private new Camera camera;

        private static List<MRPCamera> allInstances = new List<MRPCamera>();
        private static List<MRPCamera> activeInstances = new List<MRPCamera>();

        #endregion

        #region Properties

        public string Identifier => string.IsNullOrEmpty(identifier) ? name : identifier;
        public Camera Camera {
            get {
                if(!camera)
                    camera = GetComponent<Camera>();
                return camera;
            }
        }

        public static IReadOnlyList<MRPCamera> AllInstances => allInstances;
        public static IReadOnlyList<MRPCamera> ActiveInstances => activeInstances;

        #endregion

        #region Init

        private void Awake() {
            allInstances.Add(this);
            if(!camera)
                camera = GetComponent<Camera>();
        }

        void OnEnable() {
            if(!camera)
                camera = GetComponent<Camera>();
            if(!allInstances.Contains(this))
                allInstances.Add(this);
            activeInstances.Add(this);
        }

        void OnDisable() {
            activeInstances.Remove(this);
        }

        private void OnDestroy() {
            allInstances.Remove(this);
        }

        #endregion

        #region Find

        public static bool Exists(string identifier)
            => Exists(identifier, false);

        public static bool Exists(string identifier, bool includeInactive) {
            if(includeInactive) {
                for(int i = allInstances.Count - 1; i >= 0; i--) {
                    if(allInstances[i].Identifier == identifier) {
                        return true;
                    }
                }
            }
            else {
                for(int i = activeInstances.Count - 1; i >= 0; i--) {
                    if(activeInstances[i].Identifier == identifier) {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool TryGet(string identifier, out MRPCamera instance)
            => TryGet(identifier, out instance, false);

        public static bool TryGet(string identifier, out MRPCamera instance, bool includeInactive) {
            if(includeInactive) {
                for(int i = allInstances.Count - 1; i >= 0; i--) {
                    if(allInstances[i].Identifier == identifier) {
                        instance = allInstances[i];
                        return true;
                    }
                }
            }
            else {
                for(int i = activeInstances.Count - 1; i >= 0; i--) {
                    if(activeInstances[i].Identifier == identifier) {
                        instance = activeInstances[i];
                        return true;
                    }
                }
            }
            instance = default;
            return false;
        }

        #endregion

        #region Overrides

        public static implicit operator Camera(MRPCamera instance) => instance.Camera;

        #endregion
    }
}
