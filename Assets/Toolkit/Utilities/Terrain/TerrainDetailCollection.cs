using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Toolkit.Utility
{
    [CreateAssetMenu(fileName = "Terrain Detail Collection", menuName = "Toolkit/Terrain/Terrain Detail Collection")]
    public class TerrainDetailCollection : ScriptableObject
    {
        [SerializeField] private Detail[] details = null;

        public IReadOnlyList<Detail> Details => details;
        public DetailPrototype[] GetDetailPrototypes() => details.Select(x => x.GetDetailPrototype()).ToArray();

        [System.Serializable]
        public class Detail
        {
            #region Variables

            [SerializeField] private Texture2D texture;
            [SerializeField] private GameObject prefab;
            [SerializeField] private MinMax width = new MinMax(0.5f, 1f);
            [SerializeField] private MinMax height = new MinMax(0.5f, 1f);
            [SerializeField, Min(0)] private float noiseSpread = 0.1f;
            [SerializeField] private Color healthyColor = ColorTable.LawnGreen;
            [SerializeField] private Color dryColor = ColorTable.SandyBrown;
            [SerializeField] private bool billboard = false;

            #endregion

            #region Properties

            public bool IsPrefab => prefab != null;
            public DetailRenderMode RenderMode => prefab != null ? DetailRenderMode.VertexLit : (billboard ? DetailRenderMode.GrassBillboard : DetailRenderMode.Grass);
            public bool Billboard => billboard;
            public Color HealthyColor => healthyColor;
            public Color DryColor => dryColor;
            public float NoiseSpread => noiseSpread;
            public MinMax Width => width;
            public MinMax Height => height;
            public Texture2D Texture => texture;
            public GameObject Prefab => prefab;
            public int Verticies {
                get {
                    if(IsPrefab) {
                        int result = 0;
                        foreach(var mf in prefab.GetComponentsInChildren<MeshFilter>())
                            result += mf.sharedMesh.vertexCount;
                        foreach(var smr in prefab.GetComponentsInChildren<SkinnedMeshRenderer>())
                            result += smr.sharedMesh.vertexCount;
                        return result;
                    }
                    else {
                        return 4;
                    }
                }
            }

            #endregion

            #region Constructor

            internal Detail() {
                healthyColor = ColorTable.LawnGreen;
                dryColor = ColorTable.SandyBrown;
                width = new MinMax(0.5f, 1f);
                height = new MinMax(0.5f, 1f);
            }

            public Detail(Texture2D texture) : this() {
                this.texture = texture;
            }

            public Detail(Texture2D texture, MinMax width, MinMax height) {
                this.texture = texture;
                this.width = width;
                this.height = height;
            }

            public Detail(Texture2D texture, Color healthyColor, Color dryColor) {
                this.texture = texture;
                this.healthyColor = healthyColor;
                this.dryColor = dryColor;
                width = new MinMax(0.5f, 1f);
                height = new MinMax(0.5f, 1f);
            }

            public Detail(Texture2D texture, MinMax width, MinMax height, Color healthyColor, Color dryColor) {
                this.texture = texture;
                this.width = width;
                this.height = height;
                this.healthyColor = healthyColor;
                this.dryColor = dryColor;
            }

            public Detail(DetailPrototype prototype) {
                texture = prototype.prototypeTexture;
                prefab = prototype.prototype;
                billboard = prototype.renderMode == DetailRenderMode.GrassBillboard;
                healthyColor = prototype.healthyColor;
                dryColor = prototype.dryColor;
                width = new MinMax(prototype.minWidth, prototype.maxWidth);
                height = new MinMax(prototype.minHeight, prototype.maxHeight);
                noiseSpread = prototype.noiseSpread;
            }

            #endregion

            #region Conversion

            public DetailPrototype GetDetailPrototype() {
                return new DetailPrototype() {
                    healthyColor = healthyColor,
                    dryColor = dryColor,
                    minWidth = width.min,
                    maxWidth = width.max,
                    minHeight = height.min,
                    maxHeight = height.max,
                    usePrototypeMesh = IsPrefab,
                    renderMode = RenderMode,
                    prototype = prefab,
                    prototypeTexture = texture,
                    noiseSpread = noiseSpread,
                };
            }

            public static implicit operator DetailPrototype(Detail detail) => detail.GetDetailPrototype();

            #endregion
        }
    }
}
