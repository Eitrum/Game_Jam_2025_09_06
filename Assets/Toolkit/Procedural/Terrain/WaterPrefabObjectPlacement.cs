using System.Collections;
using System.Collections.Generic;
using Toolkit.Utility;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    [CreateAssetMenu(fileName = "Water Prefab Object Placement", menuName = "Toolkit/Procedural/Terrain/Details/Water Prefab Objects Placement")]
    public class WaterPrefabObjectPlacement : ScriptableObject, IProceduralTerrainGeneration
    {
        [SerializeField] private float waterHeight = 63.5f;

        [SerializeField] private PrefabCollection prefabCollection = null;
        [SerializeField] private Layer[] layers = { };


        public bool Generate(Data data) {
            var width = data.Width;
            var height = data.Height;

            foreach(var l in layers) {
                var entry = prefabCollection.GetEntry(l.prefabIndex);
                var variantCount = entry.VariantCount;
                var prefabData = new PrefabData[variantCount];
                for(int i = 0; i < variantCount; i++) {
                    var temp = prefabData[i] = new PrefabData(entry.GetVariant(i).Prefab);
                    data.Prefabs.Add(temp);
                }

                var points = PoissonDiscSampling.Simple(l.objectRadius, width, height, data.Random);
                foreach(var p in points) {
                    var h = data.GetBilinearHeight(p);
                    if(h > waterHeight)
                        continue;
                    var s = data.GetSlopeDegrees(p);
                    var splat = data.GetBilinearSplat(data.GetSplatmapPositionFromHeight(p));
                    var amount = l.amount.CalculatAmount(h, s, splat);
                    if(amount > l.threshold) {
                        if(l.useCluster) {
                            var cluster = (amount - l.threshold) / (1f - l.threshold);
                            var clusterStrength = l.clusterStrength.Evaluate(cluster);
                            var cPoints = PoissonDiscSampling.Simple(l.clusterRadius, l.clusterSize, l.clusterSize, data.Random);
                            var count = clusterStrength * cPoints.Count;
                            for(int c = 0; c < count; c++) {
                                var cPos = cPoints[c] - new Vector2(l.clusterSize / 2f, l.clusterSize / 2f);
                                if(l.amount.CalculatAmount(data.GetBilinearHeight(p + cPos), data.GetSlopeDegrees(p + cPos), data.GetBilinearSplat(data.GetSplatmapPositionFromHeight(p + cPos))) > l.threshold) {
                                    AddPrefab(data, entry, prefabData, p + cPos, waterHeight, data.Random);
                                }
                            }
                        }
                        else {
                            AddPrefab(data, entry, prefabData, p, waterHeight, data.Random);
                        }
                    }
                }
            }

            return true;
        }


        private static void AddPrefab(Data data, PrefabCollection.Entry entry, PrefabData[] prefabData, Vector2 pos, float waterHeight, System.Random random)
            => AddPrefab(data, entry, prefabData, pos.x, pos.y, waterHeight, random);

        private static void AddPrefab(Data data, PrefabCollection.Entry entry, PrefabData[] prefabData, float x, float y, float waterHeight, System.Random random) {
            var index = entry.GetRandomVariantIndex(random);

            var variant = entry.GetVariant(index);
            var position = new Vector3(x, waterHeight, y);
            var norm = data.GetNormal(x, y);
            var rotation = variant.CalculateRotation(norm, random);
            position = variant.CalulatePosition(position, norm, rotation);
            var scale = variant.CalculateSize(random);
            prefabData[index].Add(position, rotation, scale);
        }

        [System.Serializable]
        public class Layer
        {
            public int prefabIndex = 0;
            public float objectRadius = 2f;
            public float threshold = 0.05f;
            public Amount amount;
            [Header("Cluster")]
            public bool useCluster = false;
            public AnimationCurve clusterStrength = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            public float clusterRadius = 2f;
            public float clusterSize = 4f;

            [System.NonSerialized] public DetailData dData;
        }

        [System.Serializable]
        public class Amount
        {
            [SerializeField] private MinMax heightRange = new MinMax(68, 200);
            [SerializeField] private AnimationCurve heightModifier = AnimationCurve.Linear(0, 1f, 1f, 1f);
            [SerializeField] private MinMax slopeRange = new MinMax(0, 90);
            [SerializeField] private AnimationCurve slopeModifier = AnimationCurve.Linear(0f, 1f, 1f, 1f);

            [SerializeField] private bool useSplatmap = false;
            [SerializeField] private int splatId = 0;
            [SerializeField] private MinMax splatThreshold = new MinMax(0, 1);
            [SerializeField] private AnimationCurve splatModifier = AnimationCurve.Linear(0f, 1f, 1f, 1f);

            public float CalculatAmount(float h, float s, SplatData splat) {
                var result = 1f;
                result *= heightModifier.Evaluate(heightRange.InverseEvaluate(h));
                result *= slopeModifier.Evaluate(slopeRange.InverseEvaluate(s));
                if(useSplatmap) {
                    result *= splatModifier.Evaluate(splatThreshold.InverseEvaluate(splat.GetTexturePercentage(splatId)));
                }
                return result;
            }
        }
    }
}
