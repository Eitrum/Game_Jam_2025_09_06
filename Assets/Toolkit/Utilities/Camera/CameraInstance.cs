using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Toolkit/Camera/Camera Instance")]
    [DefaultExecutionOrder(-500)]
    public class CameraInstance : MonoBehaviour
    {
        #region Variables

        public delegate void OnCameraInstanceCallback(CameraInstance instance);
        private static List<CameraInstance> instances = new List<CameraInstance>();

        public static event OnCameraInstanceCallback OnCameraAdded;
        public static event OnCameraInstanceCallback OnCameraRemoved;

        [SerializeField] private bool isMain = false;
        private Camera cameraReference = null;

        #endregion

        #region Properties

        public Camera Camera => cameraReference;
        public bool IsMain => isMain;

        public static IReadOnlyList<CameraInstance> Instances => instances;
        public static CameraInstance MainCameraInstance => instances.FirstOrDefault(x => x.isMain);
        public static Camera MainCamera => MainCameraInstance != null ? MainCameraInstance.cameraReference : null;

        #endregion

        #region Enable Disable

        private void Awake() {
            cameraReference = GetComponent<Camera>();
        }

        private void OnEnable() {
            instances.Add(this);
            OnCameraAdded?.Invoke(this);
        }

        private void OnDisable() {
            instances.Remove(this);
            OnCameraRemoved?.Invoke(this);
        }

        #endregion
    }
}
