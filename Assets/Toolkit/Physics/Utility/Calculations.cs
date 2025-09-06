using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.PhysicEx
{
    public static class Calculations
    {
        #region Magnus Effect

        public static Vector3 MagnusEffect(Vector3 velocity, Vector3 angularVelocity, float radius, float fluidDensity) {
            var direction = Vector3.Cross(angularVelocity, velocity);
            var magnitude = 1.33333333333333f * Mathf.PI * Mathf.Pow(fluidDensity * 1000f, 0.33333333f) * 0.001f * radius * radius * radius;
            return direction * magnitude;
        }

        #endregion

        #region Tennis

        /// <summary>
        /// Returns the bounce multiplier for a tennis ball impact
        /// </summary>
        public static float Tennis_CoefficientOfRestitution(float velocity) {
            return 1f / ((velocity * 0.15f) + 1f);
        }

        // Should in theory be slightly more accurate and handle low velocity hits better (0 velocity is 92% efficient)
        public static float Tennis_CoefficientOfRestitutionAlternative(float velocity) {
            return (7.7f / (velocity + 1.5f + 7.7f)) * 1.1f;
        }

        // https://isjos.org/JoP/vol1/Papers/JoPv1i1-2Tennis.pdf
        public static float Tennis_CoefficientOfRestitutionAlternative2(float velocity) {
            return 1f - (0.18f * Mathf.Pow(velocity, 0.5f));
        }

        #endregion

    }
}
