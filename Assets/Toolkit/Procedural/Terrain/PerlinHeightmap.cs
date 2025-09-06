using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    [CreateAssetMenu(fileName = "Perlin Heightmap", menuName = "Toolkit/Procedural/Terrain/Perlin Heightmap")]
    public class PerlinHeightmap : ScriptableObject, IProceduralTerrainGeneration
    {
        [SerializeField] private WorldPositionMode randomizeWorldPosition = WorldPositionMode.ChunkOffset;
        [SerializeField] private bool clearExistingHeightmap = true;
        [SerializeField] private bool useWorldPosition = true;
        [Header("Scale")]
        [SerializeField, Range(0f, 4096f)] private float scale = 512f;
        [SerializeField, Range(0f, 5f)] private float scalePerLayer = 1f;
        [Header("Strength")]
        [SerializeField] AnimationCurve strengthMultiplier = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        [SerializeField] private float flatStrengthMuliplier = 1f;
        [Header("Layers")]
        [SerializeField] private Layer[] layers = { };

        public bool Generate(Data data) {
            var width = data.Width;
            var height = data.Height;
            if(clearExistingHeightmap)
                data.ClearHeightmap();
            switch(randomizeWorldPosition) {
                case WorldPositionMode.ChunkOffset:
                    data.SetWorldPosition(new Vector2(width * data.Random.Next(-1024, 1025), height * data.Random.Next(-1024, 1025)));
                    break;
                case WorldPositionMode.Float:
                    data.SetWorldPosition(new Vector2(Random.Range(-65536f, 65536f), Random.Range(-65536f, 65536f)));
                    break;
            }
            var arr = data.Heightmap;
            var ox = useWorldPosition ? data.WorldPosition.x : 0f;
            var oy = useWorldPosition ? data.WorldPosition.y : 0f;
            for(int i = 0, length = layers.Length; i < length; i++) {
                var tscale = Mathf.Pow(2f, i * scalePerLayer);
                var lay = layers[i];
                var str = flatStrengthMuliplier * strengthMultiplier.Evaluate(i / Mathf.Max(1f, (float)(length - 1f)));

                for(int x = 0; x < width; x++) {
                    for(int y = 0; y < height; y++) {
                        arr[x, y] += lay.Calculate((ox + x) / scale, (oy + y) / scale, tscale, str);
                    }
                }
            }

            return true;
        }

        public enum WorldPositionMode
        {
            None,
            [InspectorName("Float, -65536 -> 65536"), Tooltip("A random value between these numbers")]
            Float,
            [InspectorName("Chunk Offset"), Tooltip("Offsets the world position with a random chunk")]
            ChunkOffset,
        }
    }
}
