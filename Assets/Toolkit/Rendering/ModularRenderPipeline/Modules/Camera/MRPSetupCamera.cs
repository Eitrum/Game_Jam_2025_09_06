using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline.Modules {
    [NSOFile("Camera/Setup Camera", typeof(MRPModule))]
    public class MRPSetupCamera : MRPModule {

        #region Variables

        [Header("Settings")]
        [SerializeField] private bool matrices = true;
        [SerializeField] private bool cameraLocation = true;
        [SerializeField] private bool projectionParams = true;
        [SerializeField] private bool screenParams = true;

        [Header("Unity")]
        [SerializeField] private bool setupUnityVariables = false;
        [SerializeField] private bool setupDefaultUnity = false;

        #endregion

        #region MRPModule Impl

        protected override void Run(in ScriptableRenderContext context, MRPContext data) {
            var c = data.TargetCamera;
            if(c == null) {
                if(debug)
                    Debug.LogError(this.FormatLog("Target Camera is null!"));
                return;
            }

            if(matrices)
                Matrices(c, setupUnityVariables);
            if(cameraLocation)
                CameraLocation(c);
            if(projectionParams)
                ProjectionParams(c);
            if(screenParams)
                ScreenParams(c);

            if(setupDefaultUnity) {
                context.SetupCameraProperties(c, false, 0);
            }
            else {
                var buffer = GetCommandBuffer("Setup Camera");
                buffer.SetViewport(c.pixelRect);
                buffer.SetViewProjectionMatrices(c.worldToCameraMatrix, c.projectionMatrix);

                context.ExecuteCommandBuffer(buffer);
                context.Submit();
                Release(buffer);
            }
        }

        public override void Dispose() { }

        #endregion

        #region Setup

        public static void Matrices(Camera c, bool setupUnityVariables) {
            var viewMatrix = c.worldToCameraMatrix; // Used to be cameraToWorldMatrix
            var projMatrix = c.projectionMatrix;
            var viewProjMatrix = projMatrix * viewMatrix;

            Shader.SetGlobalMatrix(MRPPropertyIDs.ViewMatrix, viewMatrix);
            Shader.SetGlobalMatrix(MRPPropertyIDs.ProjectionMatrix, projMatrix);
            Shader.SetGlobalMatrix(MRPPropertyIDs.ViewProjMatrix, viewProjMatrix);

            if(setupUnityVariables) {
                Shader.SetGlobalMatrix(MRPPropertyIDs.Unity.UNITY_MATRIX_V, viewMatrix);
                Shader.SetGlobalMatrix(MRPPropertyIDs.Unity.UNITY_MATRIX_P, projMatrix);
                Shader.SetGlobalMatrix(MRPPropertyIDs.Unity.UNITY_MATRIX_VP, viewProjMatrix);
            }
        }

        public static void CameraLocation(Camera c) {
            var worldSpacePosition = c.transform.position;
            var worldSpaceDirection = c.transform.forward;
            Shader.SetGlobalVector(MRPPropertyIDs.WorldCameraPosition, worldSpacePosition);
            Shader.SetGlobalVector(MRPPropertyIDs.WorldCameraDirection, worldSpaceDirection);
        }

        public static void ProjectionParams(Camera c) {
            Vector4 projectionParams = new Vector4();

            projectionParams.x = 1.0f; // Flip ???
            projectionParams.y = c.nearClipPlane;
            projectionParams.z = c.farClipPlane;
            projectionParams.w = 1.0f / c.farClipPlane;

            Shader.SetGlobalVector(MRPPropertyIDs.ProjectionParams, projectionParams);
        }

        public static void ScreenParams(Camera c) {
            Vector4 screenParams = new Vector4();
            screenParams.x = Screen.width;
            screenParams.y = Screen.height;
            screenParams.z = 1f / screenParams.x;
            screenParams.w = 1f / screenParams.y;

            Shader.SetGlobalVector(MRPPropertyIDs.ScreenParams, screenParams);
        }

        #endregion
    }
}
