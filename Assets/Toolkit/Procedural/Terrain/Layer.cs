using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    [System.Serializable]
    public class Layer
    {
        #region Variables

        [SerializeField] private Texture2D texture;
        [SerializeField] private bool useTexture = false;
        [SerializeField, Range(0f, 1f)] private float offsetX = 0f;
        [SerializeField, Range(0f, 1f)] private float offsetY = 0f;
        [SerializeField] private float scaleX = 1;
        [SerializeField] private float scaleY = 1;
        [SerializeField, DegToRad(0, 360f, 1f)] private float rotation = 0f;
        [SerializeField] private float strength = 1f;

        #endregion

        #region Calulate Methods

        public float Calculate(float x, float y) {
            x *= scaleX;
            y *= scaleY;
            if(useTexture) {
                return strength * texture.GetPixelBilinear(
                    offsetX + Mathf.Cos(rotation) * x - Mathf.Sin(rotation) * y,
                    offsetY + Mathf.Sin(rotation) * x + Mathf.Cos(rotation) * y).r;
            }
            else {
                return strength * Mathf.PerlinNoise(
                   offsetX + Mathf.Cos(rotation) * x - Mathf.Sin(rotation) * y,
                   offsetY + Mathf.Sin(rotation) * x + Mathf.Cos(rotation) * y);
            }
        }

        public float Calculate(float x, float y, float scale) {
            x *= scaleX * scale;
            y *= scaleY * scale;
            if(useTexture) {
                return strength * texture.GetPixelBilinear(
                    offsetX + Mathf.Cos(rotation) * x - Mathf.Sin(rotation) * y,
                    offsetY + Mathf.Sin(rotation) * x + Mathf.Cos(rotation) * y).r;
            }
            else {
                return strength * Mathf.PerlinNoise(
                   offsetX + Mathf.Cos(rotation) * x - Mathf.Sin(rotation) * y,
                   offsetY + Mathf.Sin(rotation) * x + Mathf.Cos(rotation) * y);
            }
        }

        public float Calculate(float x, float y, float scale, float layerStrength) {
            x *= scaleX * scale;
            y *= scaleY * scale;
            if(useTexture) {
                return layerStrength * strength * texture.GetPixelBilinear(
                    offsetX + Mathf.Cos(rotation) * x - Mathf.Sin(rotation) * y,
                    offsetY + Mathf.Sin(rotation) * x + Mathf.Cos(rotation) * y).r;
            }
            else {
                return layerStrength * strength * Mathf.PerlinNoise(
                   offsetX + Mathf.Cos(rotation) * x - Mathf.Sin(rotation) * y,
                   offsetY + Mathf.Sin(rotation) * x + Mathf.Cos(rotation) * y);
            }
        }

        #endregion
    }
}
