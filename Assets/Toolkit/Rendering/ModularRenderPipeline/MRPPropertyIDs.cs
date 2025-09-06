using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Toolkit.Rendering.ModularRenderPipeline {
    public static class MRPPropertyIDs {

        #region Camera

        // Matrix4x4
        public static readonly int ViewMatrix;
        // Matrix4x4
        public static readonly int ProjectionMatrix;
        // Matrix4x4
        public static readonly int ViewProjMatrix;

        // Vector3
        public static readonly int WorldCameraPosition;
        // Vector3
        public static readonly int WorldCameraDirection;

        /// <summary>
        /// Vector4
        /// x = projection flipping
        /// y = camera near clip plane
        /// z = camera far clip plane
        /// w = 1.0 / camera far clip plane
        /// </summary>
        public static readonly int ProjectionParams;

        /// <summary>
        /// Vector4
        /// x = screen width
        /// y = screen height
        /// z = 1.0 / width
        /// w = 1.0 / height
        /// </summary>
        public static readonly int ScreenParams;

        public static readonly int CameraCullingMatrix;

        #endregion

        static MRPPropertyIDs() {
            ViewMatrix = Shader.PropertyToID("_ViewMatrix");
            ProjectionMatrix = Shader.PropertyToID("_ProjectionMatrix");
            ViewProjMatrix = Shader.PropertyToID("_ViewProjMatrix");
            WorldCameraPosition = Shader.PropertyToID("_WorldSpaceCameraPos");
            WorldCameraDirection = Shader.PropertyToID("_WorldSpaceCameraDir");
            ProjectionParams = Shader.PropertyToID("_ProjectionParams");
            ScreenParams = Shader.PropertyToID("_ScreenParams");
            CameraCullingMatrix = Shader.PropertyToID("_CameraCullingMatrix");
        }

        public static class Unity {
            #region Matrices

            // Matrix4x4
            public static readonly int UNITY_MATRIX_V;
            // Matrix4x4
            public static readonly int UNITY_MATRIX_P;
            // Matrix4x4
            public static readonly int UNITY_MATRIX_VP;

            #endregion

            static Unity() {
                UNITY_MATRIX_V = Shader.PropertyToID("UNITY_MATRIX_V");
                UNITY_MATRIX_P = Shader.PropertyToID("UNITY_MATRIX_P");
                UNITY_MATRIX_VP = Shader.PropertyToID("UNITY_MATRIX_VP");
            }
        }
    }

}
