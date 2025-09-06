using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    [CreateAssetMenu(fileName = "Crude Tree Placement", menuName = "Toolkit/Procedural/Terrain/Tree/Crude Tree Placement")]
    public class CrudeTreePlacement : ScriptableObject, IProceduralTerrainGeneration
    {
        [SerializeField] private Utility.TerrainTreeCollection treeCollection;
        [SerializeField] private Layer[] treeLayers = { };

        public bool Generate(Data data) {
            foreach(var l in treeLayers)
                l.GenerateTrees(data, treeCollection);
            return true;
        }

        [System.Serializable]
        public class Layer
        {
            [SerializeField] private float treeRadius = 3f;
            [SerializeField] private Tree[] trees;
            [SerializeField] private Rule[] rules;

            public void GenerateTrees(Data data, Utility.TerrainTreeCollection collection) {
                foreach(var t in trees) {
                    t.treePrefab = collection.GetTree(t.index);
                    t.treeData = new TreeData(t.treePrefab);
                    data.Trees.Add(t.treeData);
                }

                var points = PoissonDiscSampling.Simple(treeRadius, data.Width, data.Height, data.Random);
                foreach(var p in points) {
                    foreach(var r in rules) {
                        if(r.IsAccepted(
                            data.GetBilinearHeight(p),
                            data.GetSlopeDegrees(p),
                            data.GetBilinearSplat(data.GetSplatmapPositionFromHeight(p)))) {

                            var treeConfig = trees.RandomElement(x => x.weight);
                            treeConfig.treeData.AddTree(
                                new TreeData.Instance(
                                    p.ToVector3_XZ(data.GetBilinearHeight(p)),
                                    data.Random.NextFloat() * Mathf.PI * 2f,
                                    treeConfig.size.Evaluate(treeConfig.sizeWeight.Evaluate(data.Random.NextFloat()))));

                            break;
                        }
                    }
                }
            }
        }

        [System.Serializable]
        public class Tree
        {
            public int index = 0;
            public float weight = 1f;
            public MinMax size = new MinMax(1, 2);
            public AnimationCurve sizeWeight = AnimationCurve.Linear(0f, 0f, 1f, 1f);

            [System.NonSerialized] public Utility.TerrainTreeCollection.TreePrefab treePrefab;
            [System.NonSerialized] public TreeData treeData;
        }

        [System.Serializable]
        public class Rule
        {
            [Header("Heightmap")]
            [SerializeField] MinMax slope = new MinMax(0f, 35f);
            [SerializeField] MinMax height = new MinMax(0f, 500f);
            [Header("Splat")]
            [SerializeField] private bool useSplatmap = false;
            [SerializeField] private int splatId = 0;
            [SerializeField] private MinMax splatThreshold = new MinMax(0, 1);

            public bool IsAccepted(float h, float s, SplatData splat) {
                // Handle height and slope
                if(!height.Contains(h))
                    return false;
                if(!slope.Contains(s))
                    return false;

                // Handle splatmap
                if(useSplatmap) {
                    var percentage = splat.GetTexturePercentage(splatId);
                    if(!splatThreshold.Contains(percentage))
                        return false;
                }
                return true;
            }
        }
    }
}
