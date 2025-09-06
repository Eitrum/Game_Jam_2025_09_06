using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    [CreateAssetMenu(fileName = "Splatmap Smoothing", menuName = "Toolkit/Procedural/Terrain/Splatmap/Smoothing")]
    public class SplatmapSmoothing : ScriptableObject, IProceduralTerrainGeneration
    {
        #region Variables

        [SerializeField, Min(1)] int smoothingRadius = 1;
        [SerializeField, Range(0f, 4f)] private float smoothingMultiplier = 1f;

        #endregion

        #region Generate

        public bool Generate(Data data) {
            var w = data.SplatWidth;
            var h = data.SplatHeight;
            var splat = data.Splatmap;
            var copy = new SplatData[w, h];

            for(int x = 0; x < w; x++) {
                for(int y = 0; y < h; y++) {
                    copy[x, y] = Smooth(splat, x, y, w, h, smoothingRadius, smoothingMultiplier);
                }
            }

            for(int x = 0; x < w; x++) {
                for(int y = 0; y < h; y++) {
                    splat[x, y] = copy[x, y];
                }
            }

            return true;
        }

        private static SplatData Smooth(SplatData[,] splatmap, int x, int y, int width, int height, int smoothingRadius, float smoothingMultiplier) {
            SplatData splat = new SplatData();

            for(int ix = -smoothingRadius; ix <= smoothingRadius; ix++) {
                for(int iy = -smoothingRadius; iy <= smoothingRadius; iy++) {
                    if(x + ix < 0 || x + ix >= width || y + iy < 0 || y + iy >= height)
                        continue;
                    splat.Add(splatmap[x + ix, y + iy], 1f / Mathf.Pow(2f, Mathf.Sqrt(ix * ix + iy * iy) * smoothingMultiplier));
                }
            }
            return splat;
        }

        #endregion
    }
}
