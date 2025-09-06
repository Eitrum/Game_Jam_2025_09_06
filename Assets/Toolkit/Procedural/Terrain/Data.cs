using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Toolkit.Procedural.Terrain
{
    public class Data
    {
        #region Variables

        private const float EPSILON_ONE = 1.0005f;
        public const int DEFAULT_DETAIL_DENSITY = 64;
        public const int DEFAULT_DETAIL_MULTIPLIER = 2;

        private Vector2 worldPosition;

        private int width, height;
        private float[,] heightmap;

        private int splatWidth, splatHeight;
        private SplatData[,] splatmap;

        private float heightmapToSplatmapWidth;
        private float heightmapToSplatmapHeight;
        private float heightmapToDetailmapWidth;
        private float heightmapToDetailmapHeight;

        private int detailWidth;
        private int detailHeight;

        private List<TreeData> trees = new List<TreeData>();
        private List<DetailData> details = new List<DetailData>();
        private List<PrefabData> prefabs = new List<PrefabData>();
        private List<IUniqueData> uniqueData = new List<IUniqueData>();

        private System.Random random;
        private int detailDensity;
        private int seed = 0;
        private bool vertexColoured = false;

        #endregion

        #region Properties

        public Vector2 WorldPosition => worldPosition;

        public int Width => width;
        public int Height => height;
        public float[,] Heightmap => heightmap;

        public int SplatWidth => splatWidth;
        public int SplatHeight => splatHeight;
        public SplatData[,] Splatmap => splatmap;

        public int DetailWidth => detailWidth;
        public int DetailHeight => detailHeight;
        public int DetailDensity => detailDensity;

        public List<TreeData> Trees => trees;
        public List<DetailData> Details => details;
        public List<PrefabData> Prefabs => prefabs;
        public List<IUniqueData> UniqueData => uniqueData;

        public System.Random Random => random;
        public int Seed => seed;
        public bool VertexColoured => vertexColoured;

        #endregion

        #region Constructor

        public Data(int size) : this(size, size) { }

        public Data(int width, int height) : this(width, height, Toolkit.Mathematics.Random.Int) { }

        public Data(int width, int height, int seed) {
            this.width = width;
            this.height = height;
            this.heightmap = new float[width, height];

            this.splatWidth = width;
            this.splatHeight = height;
            this.splatmap = new SplatData[splatWidth, splatHeight];

            this.detailWidth = width * DEFAULT_DETAIL_MULTIPLIER;
            this.detailHeight = height * DEFAULT_DETAIL_MULTIPLIER;
            this.detailDensity = DEFAULT_DETAIL_DENSITY;

            this.random = new System.Random(seed);
            this.seed = seed;
            this.vertexColoured = true;

            this.heightmapToSplatmapWidth = splatWidth / (float)width;
            this.heightmapToSplatmapHeight = splatHeight / (float)height;
            this.heightmapToDetailmapWidth = detailWidth / (float)width;
            this.heightmapToDetailmapHeight = detailHeight / (float)height;
        }

        public Data(UnityEngine.Terrain terrain) : this(terrain, Toolkit.Mathematics.Random.Int) { }

        public Data(UnityEngine.Terrain terrain, int seed) {
            var data = terrain.terrainData;
            var size = data.heightmapResolution;
            this.width = size;
            this.height = size;
            this.heightmap = new float[size, size];

            var spltSize = data.alphamapResolution;
            this.splatWidth = spltSize;
            this.splatHeight = spltSize;
            this.splatmap = new SplatData[spltSize, spltSize];

            this.random = new System.Random(seed);
            this.seed = seed;
            var detailSize = data.detailResolution;
            this.detailWidth = detailSize;
            this.detailHeight = detailSize;
            this.detailDensity = DEFAULT_DETAIL_DENSITY;
            this.vertexColoured = false;

            this.heightmapToSplatmapWidth = splatWidth / (float)width;
            this.heightmapToSplatmapHeight = splatHeight / (float)height;
            this.heightmapToDetailmapWidth = detailWidth / (float)width;
            this.heightmapToDetailmapHeight = detailHeight / (float)height;
        }

        #endregion

        #region Conversion

        public Vector2 GetSplatmapPositionFromHeight(float x, float y) {
            return new Vector2(x * heightmapToSplatmapWidth, y * heightmapToSplatmapHeight);
        }

        public Vector2 GetSplatmapPositionFromHeight(Vector2 position) {
            return new Vector2(position.x * heightmapToSplatmapWidth, position.y * heightmapToSplatmapHeight);
        }

        public Vector2 GetHeightmapPositionFromSplatmap(float x, float y) {
            return new Vector2(x / heightmapToSplatmapWidth, y / heightmapToSplatmapHeight);
        }

        public Vector2 GetHeightmapPositionFromSplatmap(Vector2 position) {
            return new Vector2(position.x / heightmapToSplatmapWidth, position.y / heightmapToSplatmapHeight);
        }

        public Vector2 GetDetailmapPositionFromHeightmap(float x, float y) {
            return new Vector2(x * heightmapToDetailmapWidth, y * heightmapToDetailmapHeight);
        }

        public Vector2 GetDetailmapPositionFromHeightmap(Vector2 position) {
            return new Vector2(position.x * heightmapToDetailmapWidth, position.y * heightmapToDetailmapHeight);
        }

        public Vector2 GetHeightmapPositionFromDetailmap(float x, float y) {
            return new Vector2(x / heightmapToDetailmapWidth, y / heightmapToDetailmapHeight);
        }

        public Vector2 GetHeightmapPositionFromDetailmap(Vector2 position) {
            return new Vector2(position.x / heightmapToDetailmapWidth, position.y / heightmapToDetailmapHeight);
        }

        #endregion

        #region Utility

        public float GetBilinearHeight(Vector2 position)
            => GetBilinearHeight(position.x, position.y);

        /// <summary>
        /// Returns the height at the specific point interpolated between heightmap.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public float GetBilinearHeight(float x, float y) {
            // TODO: support nearby tiles.
            x = Mathf.Clamp(x, 0f, width - EPSILON_ONE);
            y = Mathf.Clamp(y, 0f, height - EPSILON_ONE);

            // Round down
            var nodeX = (int)x;
            var nodeY = (int)y;
            // Get modulo values of x and y
            var offsetX = x - nodeX;
            var offsetY = y - nodeY;

            return
                heightmap[nodeX + 0, nodeY + 0] * (1f - offsetY) * (1f - offsetX) +
                heightmap[nodeX + 0, nodeY + 1] * (offsetY) * (1f - offsetX) +
                heightmap[nodeX + 1, nodeY + 0] * (1f - offsetY) * (offsetX) +
                heightmap[nodeX + 1, nodeY + 1] * (offsetY) * (offsetX);
        }

        public SplatData GetBilinearSplat(Vector2 position)
            => GetBilinearSplat(position.x, position.y);

        public SplatData GetBilinearSplat(float x, float y) {
            x = Mathf.Clamp(x, 0f, splatWidth - EPSILON_ONE);
            y = Mathf.Clamp(y, 0f, splatHeight - EPSILON_ONE);

            // Round down
            var nodeX = (int)x;
            var nodeY = (int)y;
            // Get modulo values of x and y
            var offsetX = x - nodeX;
            var offsetY = y - nodeY;

            SplatData data = new SplatData();
            data.Add(splatmap[nodeX + 0, nodeY + 0], (1f - offsetY) * (1f - offsetX));
            data.Add(splatmap[nodeX + 0, nodeY + 1], (offsetY) * (1f - offsetX));
            data.Add(splatmap[nodeX + 1, nodeY + 0], (1f - offsetY) * (offsetX));
            data.Add(splatmap[nodeX + 1, nodeY + 1], (offsetY) * (offsetX));

            return data.Normalized;
        }

        public float GetHeight(Vector2 position) => heightmap[(int)Mathf.Clamp(position.x, 0f, width - 1), (int)Mathf.Clamp(position.y, 0f, height - 1)];
        public float GetHeight(float x, float y) => heightmap[(int)Mathf.Clamp(x, 0f, width - 1), (int)Mathf.Clamp(y, 0f, height - 1)];
        public float GetHeight(Vector2 position, Vector2 size) => GetHeight(position.x, position.y, size.x, size.y);
        public float GetHeight(Rect rect) => GetHeight(rect.x, rect.y, rect.width, rect.height);
        public float GetHeight(float x, float y, float width, float height) {
            float result = 0f;
            for(int ix = 0; ix < width; ix++) {
                for(int iy = 0; iy < height; iy++) {
                    result += GetHeight(x + ix, y + iy);
                }
            }
            return result / (Mathf.Floor(width) * Mathf.Floor(height));
        }

        public Vector2 GetRawSlope(Vector2 position)
            => GetRawSlope(position.x, position.y);

        public Vector2 GetRawSlope(float x, float y) {
            // TODO: support nearby tiles.
            x = Mathf.Clamp(x, 0f, width - EPSILON_ONE);
            y = Mathf.Clamp(y, 0f, height - EPSILON_ONE);
            var nodeX = (int)x;
            var nodeY = (int)y;

            // Get modulo values of x and y
            var offsetX = x - nodeX;
            var offsetY = y - nodeY;

            var slopeX = (heightmap[nodeX + 1, nodeY] - heightmap[nodeX, nodeY]) * (1f - offsetY) + (heightmap[nodeX + 1, nodeY + 1] - heightmap[nodeX, nodeY + 1]) * offsetY;
            var slopeY = (heightmap[nodeX, nodeY + 1] - heightmap[nodeX, nodeY]) * (1f - offsetX) + (heightmap[nodeX + 1, nodeY + 1] - heightmap[nodeX + 1, nodeY]) * offsetX;

            return new Vector2(slopeX, slopeY);
        }

        public float GetSlopeDegrees(Vector2 position)
            => GetSlopeDegrees(position.x, position.y);

        public float GetSlopeDegrees(float x, float y) {
            var s = GetRawSlope(x, y);
            return Mathf.Atan(Mathf.Max(Mathf.Abs(s.x), Mathf.Abs(s.y))) * Mathf.Rad2Deg;
        }

        public Vector3 GetNormal(Vector2 position)
            => GetNormal(position.x, position.y);

        public Vector3 GetNormal(float x, float y) {
            var slope = GetRawSlope(x, y);
            return Vector3.Cross(new Vector3(0, slope.y, 1), new Vector3(1, slope.x, 0)).normalized;
        }

        #endregion

        #region Remove / Clear

        /// <summary>
        /// Clears all values and resets the seed
        /// </summary>
        public void ClearAll() => ClearAll(true);

        public void ClearAll(bool seedReset) {
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    heightmap[x, y] = 0f;
                }
            }
            for(int x = 0; x < splatWidth; x++) {
                for(int y = 0; y < splatHeight; y++) {
                    splatmap[x, y] = new SplatData();
                }
            }

            trees.Clear();
            details.Clear();
            prefabs.Clear();
            if(seedReset)
                random = new System.Random(seed);
        }

        public void ResetRandom() => random = new System.Random(seed);

        public void ClearHeightmap() {
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    heightmap[x, y] = 0f;
                }
            }
        }

        public void ClearSplatmap() {
            for(int x = 0; x < splatWidth; x++) {
                for(int y = 0; y < splatHeight; y++) {
                    splatmap[x, y] = new SplatData();
                }
            }
        }

        public void ClearTrees() => trees.Clear();
        public void ClearDetails() => details.Clear();
        public void ClearPrefabs() => prefabs.Clear();

        #endregion

        #region Position

        public void SetWorldPosition(Vector2 position)
            => this.worldPosition = position;

        #endregion

        #region Resize

        public void Resize(int size)
            => Resize(size, size);

        public void Resize(int width, int height) {
            if(this.width == width && this.height == height)
                return;
            if(details.Count == 0) {
                var dx = this.width / this.detailWidth;
                var dy = this.height / this.detailHeight;
                this.detailWidth = width / dx;
                this.detailHeight = height / dy;
            }
            throw new System.NotImplementedException();
        }

        public void SetDetailPatchCount(int size)
            => SetDetailPatchCount(size, size);

        public void SetDetailPatchCount(int width, int height) {
            if(details.Count > 0) {
                Debug.LogError("Attempting to change the detail patch while there already exists details");
                return;
            }
            this.detailWidth = this.width / width;
            this.detailHeight = this.height / height;
            this.detailDensity = width * height;
        }

        #endregion

        #region Terrain

        public enum TerrainConversion
        {
            None,
            FitTerrain,
            ScaleTerrain,
        }

        public void Apply(UnityEngine.Terrain terrain, TerrainConversion conversion = TerrainConversion.ScaleTerrain)
            => Apply(terrain.terrainData, conversion, terrain.transform);

        public void Apply(TerrainData data, TerrainConversion conversion, Transform prefabParent) {
            if(width != height || width != data.heightmapResolution) {
                throw new System.NotImplementedException("Terrain scaling not implmented");
            }

            // Copy heightmap
            float terrainHeight = data.heightmapScale.y;
            var terrainScale = data.size;
            float[,] copy = new float[height, width];
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    copy[y, x] = heightmap[x, y] / terrainHeight;
                }
            }
            data.SetHeights(0, 0, copy);

            // Copy Splatmap
            float[,,] splatCopy = new float[splatHeight, splatWidth, data.terrainLayers.Length];
            for(int x = 0; x < splatWidth; x++) {
                for(int y = 0; y < splatHeight; y++) {
                    var tempSplat = splatmap[x, y].GetSplatArray();
                    for(int i = 0, length = tempSplat.Length; i < length; i++) {
                        splatCopy[y, x, i] = tempSplat[i];
                    }
                }
            }
            data.SetAlphamaps(0, 0, splatCopy);

            // Copy trees
            var treePrototypes = trees.Select(x => x.TreePrototype).Unique();
            var treeProtoArray = treePrototypes.ToArray();
            data.treePrototypes = treeProtoArray;
            var treeInstances = trees
                .SelectMany(x => {
                    var index = treeProtoArray.IndexOf(x.TreePrototype);
                    if(index == -1)
                        Debug.LogError("INDEX IS -1");

                    return x.Instances
                        .Select(y => y.ToTreeInstance(index, terrainScale));
                })
                .ToArray();
            Debug.Log("Applying " + treeInstances.Length + " trees to the terrain");
            data.SetTreeInstances(treeInstances, true);

            // Copy Details
            var detailPrototypes = details
                .Select(x => x.DetailPrototype)
                .ToArray();
            data.detailPrototypes = detailPrototypes;
            for(int i = 0, length = details.Count; i < length; i++) {
                Debug.Log("Applying details: " + i);
                var amount = details[i].Amount;
                var dcopy = new int[detailHeight, detailWidth];
                for(int x = 0; x < detailWidth; x++) {
                    for(int y = 0; y < detailHeight; y++) {
                        dcopy[y, x] = amount[x, y];
                    }
                }
                data.SetDetailLayer(0, 0, i, dcopy);
            }

            data.SyncHeightmap();

            prefabParent.DestroyAllChildrenImmediate();
            foreach(var pref in prefabs) {
                foreach(var inst in pref.Instances) {
                    var t = GameObject.Instantiate(pref.Prefab, prefabParent).transform;
                    t.localPosition = inst.point;
                    t.localRotation = inst.rotation;
                    t.localScale = inst.scale;
                }
            }

            foreach(var unique in uniqueData)
                unique.Build(prefabParent);
        }

        #endregion
    }
}
