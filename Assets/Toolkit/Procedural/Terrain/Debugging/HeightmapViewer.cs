using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    public class HeightmapViewer : MonoBehaviour
    {
        [SerializeField] private int seed = 0;
        [SerializeField, Min(4)] private int width = 128;
        [SerializeField] private bool autoUpdate = false;

        [SerializeField, TypeFilter(typeof(IProceduralTerrainGeneration))] private UnityEngine.Object[] generationSteps = { };
        private Data data;

        [ContextMenu("Update Heightmap")]
        public void UpdateData() {
            if(data == null || data.Width != width || data.Seed != seed)
                data = new Data(width, width, seed);

            data.ClearAll();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach(var step in generationSteps)
                if(step is IProceduralTerrainGeneration ptg)
                    ptg.Generate(data);
            if(sw.ElapsedMilliseconds > 500) {
                autoUpdate = false;
            }
        }

        private void OnDrawGizmos() {
            if(data == null || data.Width != width || data.Seed != seed)
                data = new Data(width, width, seed);

            if(autoUpdate) {
                data.ClearAll();
                Stopwatch sw = new Stopwatch();
                sw.Start();
                foreach(var step in generationSteps)
                    if(step is IProceduralTerrainGeneration ptg)
                        ptg.Generate(data);
                if(sw.ElapsedMilliseconds > 500) {
                    autoUpdate = false;
                }
            }

            GizmosUtility.DrawHeightmap(data);
        }
    }
}
