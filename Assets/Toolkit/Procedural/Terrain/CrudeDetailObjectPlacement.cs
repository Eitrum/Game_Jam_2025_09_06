using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    [CreateAssetMenu(fileName = "Crude Detail Object Placement", menuName = "Toolkit/Procedural/Terrain/Details/Crude Detail Object Placement")]
    public class CrudeDetailObjectPlacement : ScriptableObject, IProceduralTerrainGeneration
    {
        [SerializeField] private Utility.TerrainDetailCollection detailCollection;
        [SerializeField] private Layer[] layers = { };


        public bool Generate(Data data) {
            var width = data.DetailWidth;
            var height = data.DetailHeight;
            var hwidth = data.DetailWidth;
            var hheight = data.DetailHeight;

            foreach(var l in layers) {
                l.dData = new DetailData(detailCollection.Details[l.detailCollectionIndex], width, height);
                data.Details.Add(l.dData);

                var points = PoissonDiscSampling.Simple(l.detailRadius, hwidth, hheight, data.Random);
                var arr = l.dData.Amount;

                foreach(var p in points) {
                    var h = data.GetBilinearHeight(p);
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
                                    AddDetail(arr, data.GetDetailmapPositionFromHeightmap(p + cPos), width, height);
                                }
                            }
                        }
                        else {
                            AddDetail(arr, data.GetDetailmapPositionFromHeightmap(p), width, height);
                        }
                    }
                }
            }

            return true;
        }

        private static void AddDetail(int[,] array, Vector2 pos, int w, int h) {
            AddDetail(array, (int)pos.x, (int)pos.y, w, h);
        }

        private static void AddDetail(int[,] array, int x, int y, int w, int h) {
            if(x < 0 || y < 0 || x >= w || y >= h)
                return;
            array[x, y]++;
        }


        [System.Serializable]
        public class Layer
        {
            public int detailCollectionIndex = 0;
            public float detailRadius = 2f;
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
