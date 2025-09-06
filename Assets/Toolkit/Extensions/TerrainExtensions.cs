using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Toolkit
{
    public static class TerrainExtensions
    {
        #region Splat / Alphamap

        public static int GetSplatIdAt(this Terrain terrain, Vector3 position) {
            var relativePos = terrain.GetPosition() - position;
            var size = terrain.terrainData.size;
            float x = relativePos.x / size.x;
            float y = relativePos.z / size.z;
            if(x < 0f || x > 1f || y < 0f || y > 1f)
                return -1;
            x *= terrain.terrainData.alphamapWidth;
            y *= terrain.terrainData.alphamapHeight;

            var alphamap = terrain.terrainData.GetAlphamaps((int)x, (int)y, 1, 1);
            return Highest(alphamap);
        }

        private static int Highest(float[,,] alphamap) {
            float highest = 0f;
            int index = -1;
            for(int i = 0, length = alphamap.GetLength(2); i < length; i++) {
                if(alphamap[0, 0, i] > highest) {
                    index = i;
                    highest = alphamap[0, 0, i];
                }
            }
            return index;
        }

        #endregion
    }
}
