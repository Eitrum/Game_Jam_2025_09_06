using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    [CreateAssetMenu(fileName = "Step Heightmap Modifier", menuName = "Toolkit/Procedural/Terrain/Step Heightmap Modifier")]
    public class StepHeightmapModifier : ScriptableObject, IProceduralTerrainGeneration
    {
        [SerializeField] private float stepSize = 0.05f;
        [SerializeField] private MinMax stepRange = new MinMax(0.15f, 1);

        [Header("Smoothing")]
        [SerializeField, RangeEx(0f, 10f, 0.05f)] private float smoothing = 3f;
        [SerializeField] private AnimationCurve smoothingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Header("Connection Map")]
        [SerializeField, Min(4)] private int points = 20;
        [SerializeField] private float connectionWidth = 4f;
        [SerializeField] private float connectionSmoothing = 2f;
        [SerializeField] private Layer connectionPerlinLayer;

        public bool Generate(Data data) {
            var w = data.Width;
            var h = data.Height;
            var map = data.Heightmap;
            var temp = new float[w, h];

            var xyScale = new Vector2(4f / w, 4f / h);
            var halfStep = stepSize / 2f;

            var voronoi = Toolkit.Mathematics.DelaunayTriangulation.GenerateVoronoi(points, w, h, data.Random);
            // Apply step
            for(int x = 0; x < w; x++) {
                for(int y = 0; y < h; y++) {
                    var v = map[x, y];
                    if(stepRange.Contains(v)) {
                        var perlin = Mathf.PerlinNoise(data.WorldPosition.x + x * xyScale.x, data.WorldPosition.y + y * xyScale.y) * halfStep;
                        var t = (v).Snap(stepSize);
                        var dist = ClosestDistance(voronoi, new Vector2(x, y));
                        var conWidth = connectionWidth + connectionPerlinLayer.Calculate(x / (float)w, y / (float)h) * connectionWidth;
                        var distStrength = Mathf.Pow(Mathf.Clamp01(dist / conWidth), connectionSmoothing);
                        var strength = Mathf.Clamp01((Mathf.PerlinNoise(data.WorldPosition.x - x * xyScale.x, data.WorldPosition.y - y * xyScale.y) + 1f) / 2f);
                        var d = Mathf.Clamp01(1f - Mathf.Pow(smoothingCurve.Evaluate(Mathf.Abs(v - t) / halfStep), smoothing));
                        temp[x, y] = Mathf.Lerp(v, t + perlin, strength * d * distStrength);
                    }
                    else
                        temp[x, y] = v;
                }
            }

            // Copy
            for(int x = 0; x < w; x++) {
                for(int y = 0; y < h; y++) {
                    map[x, y] = temp[x, y];
                }
            }

            return true;
        }

        private static float ClosestDistance(List<Mathematics.Line> tree, Vector2 point) {
            var t = tree.Lowest(x => x.GetDistanceToLineClamped(point));
            return t.GetDistanceToLineClamped(point);
        }
    }
}
