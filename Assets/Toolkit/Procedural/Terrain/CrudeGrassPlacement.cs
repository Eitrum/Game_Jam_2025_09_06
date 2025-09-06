using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    [CreateAssetMenu(fileName = "Crude Grass Placement", menuName = "Toolkit/Procedural/Terrain/Details/Crude Grass Placement")]
    public class CrudeGrassPlacement : ScriptableObject, IProceduralTerrainGeneration
    {
        [SerializeField] private Utility.TerrainDetailCollection detailCollection;
        [SerializeField] private Layer[] layers = { };


        public bool Generate(Data data) {
            int width = data.DetailWidth;
            int height = data.DetailHeight;

            foreach(var l in layers) {
                l.dData = new DetailData(detailCollection.Details[l.detailCollectionIndex], width, height);
                data.Details.Add(l.dData);
            }

            foreach(var l in layers) {
                var arr = l.dData.Amount;
                for(int x = 0; x < width; x++) {
                    for(int y = 0; y < height; y++) {
                        var hp = data.GetHeightmapPositionFromDetailmap(x, y);
                        var h = data.GetBilinearHeight(hp);
                        var s = data.GetSlopeDegrees(hp);
                        var splat = data.GetBilinearSplat(data.GetSplatmapPositionFromHeight(hp));
                        var amount = l.strength * l.amount.CalculatAmount(h, s, splat);
                        arr[x, y] = Mathf.RoundToInt(amount * data.DetailDensity);
                    }
                }
            }

            return true;
        }

        [System.Serializable]
        public class Layer
        {
            public int detailCollectionIndex = 0;
            [Range(0f, 1f)] public float strength = 0.25f;
            public Amount amount;

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
