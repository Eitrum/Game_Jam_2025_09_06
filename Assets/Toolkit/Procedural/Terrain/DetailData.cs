using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    public class DetailData
    {
        #region Variables

        private Utility.TerrainDetailCollection.Detail detail;
        private int[,] amount;
        private int width, height;

        #endregion

        #region Properties

        public Utility.TerrainDetailCollection.Detail Detail => detail;
        public DetailPrototype DetailPrototype => detail.GetDetailPrototype();
        public bool IsMesh => detail.IsPrefab;
        public int Verticies => detail.Verticies;
        public int[,] Amount => amount;
        public int Width => width;
        public int Height => height;

        #endregion

        #region Constructor

        public DetailData(Utility.TerrainDetailCollection.Detail detail, int width, int height) {
            this.detail = detail;
            this.amount = new int[width, height];
            this.width = width;
            this.height = height;
        }

        #endregion
    }
}
