using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    public class GenerationBlending : MonoBehaviour
    {
        [SerializeField] private GenerationPreset presetA;
        [SerializeField] private GenerationPreset presetB;

        [SerializeField] private AnimationCurve blendingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private int seed = 0;


        [ContextMenu("Generate To Current Terrain")]
        public void Generate() {
            if(seed == 0)
                seed = Toolkit.Mathematics.Random.Int;
            var terrain = GetComponent<UnityEngine.Terrain>();
            if(terrain) {
                Generate(terrain, seed);
            }
        }

        public void Generate(UnityEngine.Terrain terrain, int seed) {
            var dataA = presetA.Generate(terrain, seed, false);
            var dataB = presetB.Generate(terrain, seed, false);

            Data data = new Data(terrain, seed);
            // Copy heightmap
            var heightmap = data.Heightmap;
            var heightA = dataA.Heightmap;
            var heightB = dataB.Heightmap;
            for(int x = 0, width = data.Width; x < width; x++) {
                var t = blendingCurve.Evaluate(x / (float)(width - 1f));
                for(int y = 0, depth = data.Height; y < depth; y++) {
                    heightmap[x, y] = Mathf.Lerp(heightA[x, y], heightB[x, y], t);
                }
            }

            // Splatmap
            var splat = data.Splatmap;
            var splatA = dataA.Splatmap;
            var splatB = dataB.Splatmap;
            for(int x = 0, splatWidth = data.SplatWidth; x < splatWidth; x++) {
                var t = blendingCurve.Evaluate(x / (float)(splatWidth - 1f));
                for(int y = 0, splatHeight = data.SplatHeight; y < splatHeight; y++) {
                    splat[x, y] = SplatData.Lerp(splatA[x, y], splatB[x, y], t);
                }
            }

            // Copy trees
            data.Trees.AddRange(dataA.Trees.Select(x => {
                var temp = new TreeData(x.Collection_Prefab);
                var instances = x.Instances;
                for(int i = 0, length = instances.Count; i < length; i++) {
                    var inst = instances[i];
                    var t = blendingCurve.Evaluate(inst.point.x / (data.Width - 1f));
                    if(t < Random.value)
                        temp.AddTree(inst);
                }
                return temp;
            }));
            data.Trees.AddRange(dataB.Trees.Select(x => {
                var temp = new TreeData(x.Collection_Prefab);
                var instances = x.Instances;
                for(int i = 0, length = instances.Count; i < length; i++) {
                    var inst = instances[i];
                    var t = blendingCurve.Evaluate(inst.point.x / (data.Width - 1f));
                    if(t > Random.value)
                        temp.AddTree(inst);
                }
                return temp;
            }));

            // Details
            data.Details.AddRange(dataA.Details.Select(x => {
                var temp = new DetailData(x.Detail, x.Width, x.Height);
                var amount = temp.Amount;
                var amountA = x.Amount;
                for(int w = 0; w < x.Width; w++) {
                    var t = blendingCurve.Evaluate(w / (x.Width - 1f));
                    for(int y = 0; y < x.Height; y++) {
                        amount[w, y] = (int)(amountA[w, y] * (1f - t));
                    }
                }

                return temp;
            }));
            data.Details.AddRange(dataB.Details.Select(x => {
                var temp = new DetailData(x.Detail, x.Width, x.Height);
                var amount = temp.Amount;
                var amountA = x.Amount;
                for(int w = 0; w < x.Width; w++) {
                    var t = blendingCurve.Evaluate(w / (x.Width - 1f));
                    for(int y = 0; y < x.Height; y++) {
                        amount[w, y] = (int)(amountA[w, y] * (t));
                    }
                }

                return temp;
            }));

            // Prefabs
            data.Prefabs.AddRange(dataA.Prefabs.Select(g => {
                var temp = new PrefabData(g.Prefab);
                var instances = g.Instances;
                for(int i = 0, length = instances.Count; i < length; i++) {
                    var inst = instances[i];
                    var t = blendingCurve.Evaluate(inst.point.x / (data.Width - 1f));
                    if(t < Random.value) {
                        var p = inst.point;
                        var r = inst.rotation;
                        var pos = p.To_Vector2_XZ();
                        var extraRot = Quaternion.FromToRotation(dataA.GetNormal(pos), data.GetNormal(pos));
                        p.y = data.GetBilinearHeight(pos);
                        r = r * extraRot;

                        temp.Add(p, r, inst.scale);
                    }
                }

                return temp;
            }));

            data.Prefabs.AddRange(dataB.Prefabs.Select(g => {
                var temp = new PrefabData(g.Prefab);
                var instances = g.Instances;
                for(int i = 0, length = instances.Count; i < length; i++) {
                    var inst = instances[i];
                    var t = blendingCurve.Evaluate(inst.point.x / (data.Width - 1f));
                    if(t > Random.value) {
                        var p = inst.point;
                        var r = inst.rotation;
                        var pos = p.To_Vector2_XZ();
                        var extraRot = Quaternion.FromToRotation(dataB.GetNormal(pos), data.GetNormal(pos));
                        p.y = data.GetBilinearHeight(pos);
                        r = r * extraRot;

                        temp.Add(p, r, inst.scale);
                    }
                }

                return temp;
            }));

            data.Apply(terrain);
        }
    }
}
