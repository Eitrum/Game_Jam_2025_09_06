using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Utility
{
    [CreateAssetMenu(fileName = "Prefab Collection", menuName = "Toolkit/Utility/Prefab Collection")]
    public class PrefabCollection : ScriptableObject
    {
        #region Variables

        [SerializeField] private string overrideName = "";
        [SerializeField] private string category = "";
        [SerializeField] private Entry[] entries = { };

        #endregion

        #region Properties

        public string CollectionName => string.IsNullOrEmpty(overrideName) ? name : overrideName;
        public string Category => category;
        public IReadOnlyList<Entry> Entries => entries;
        public int EntriesCount => entries.Length;
        public Entry this[int index] => entries[index];

        #endregion

        #region Methods

        public Entry GetRandomEntry() => entries.RandomElement(x => x.Weight);
        public Entry GetRandomEntry(System.Random random) => entries.RandomElement(x => x.Weight, random);
        public Entry GetEntry(int index) => entries == null || entries.Length == 0 ? null : entries[index];

        public IEnumerable<Entry> GetEntries(System.Func<Entry, bool> searchFunction) => entries.Where(searchFunction);

        #endregion

        [System.Serializable]
        public class Entry
        {
            #region Variables

            [SerializeField] private string name = "";
            [SerializeField] private float weight = 1f;
            [SerializeField] Variant[] variants = { };

            #endregion

            #region Properties

            public string Name => (!string.IsNullOrEmpty(name)) ? name : (variants == null || variants.Length == 0 ? "null" : variants[0].Name);
            public float Weight => weight;
            public IReadOnlyList<Variant> Variants => variants;
            public int VariantCount => variants.Length;
            public float TotalVariantWeight => variants.Sum(x => x.Weight);
            public Variant this[int index] => variants[index];

            public Rendering.GameObjectRendererInstance DefaultRenderer => variants != null && variants.Length > 0 ? variants[0].Renderer : null;

            #endregion

            #region GetVariation

            public Variant GetRandomVariant() => variants.RandomElement(x => x.Weight);
            public Variant GetRandomVariant(System.Random random) => variants.RandomElement(x => x.Weight, random);
            public int GetRandomVariantIndex() => variants.RandomIndex(x => x.Weight);
            public int GetRandomVariantIndex(System.Random random) => variants.RandomIndex(x => x.Weight, random);
            public Variant GetVariant(int index) => variants[index];

            #endregion
        }

        [System.Serializable]
        public class Variant
        {
            #region Variables

            [SerializeField] private GameObject prefab = null;
            [SerializeField, Min(0.001f)] private float weight = 1f;
            [SerializeField] private Vector3 minimumSize = Vector3.one;
            [SerializeField] private Vector3 maximumSize = Vector3.one;
            [SerializeField] private AnimationCurve sizeWeight = AnimationCurve.Linear(0, 0, 1, 1);
            [SerializeField] private Orientation orientation = Orientation.Default;
            [SerializeField] private Vector3 direction = Vector3.up;
            [SerializeField] private MinMax tiltX = new MinMax(0, 0);
            [SerializeField] private MinMax tiltZ = new MinMax(0, 0);
            [SerializeField] private bool yAxisRotation = true;
            [SerializeField] private float normalOffset = 0f;
            [SerializeField] private Vector3 originOffset = Vector3.zero;

            [System.NonSerialized] private Toolkit.Rendering.GameObjectRendererInstance renderer;
#if UNITY_EDITOR
            [System.NonSerialized] private GameObject renderingPrefab;
#endif

            #endregion

            #region Properties

            public string Name => prefab != null ? prefab.name : "missing prefab";
            public GameObject Prefab => prefab;
            public float Weight => weight;
            public Vector3 MinimumSize => minimumSize;
            public Vector3 MaximumSize => maximumSize;
            public Orientation Orientation => orientation;

            public Rendering.GameObjectRendererInstance Renderer
            {
                get
                {


#if UNITY_EDITOR
                    if (renderer != null && renderingPrefab != prefab)
                    {
                        renderer = prefab == null ? null : new Rendering.GameObjectRendererInstance(prefab);
                        renderingPrefab = prefab;
                    }
                    else if ((renderer == null && prefab != null))
                    {
                        renderer = new Rendering.GameObjectRendererInstance(prefab);
                        renderingPrefab = prefab;
                    }
#else
                    if((renderer == null && prefab != null))
                        renderer = new Rendering.GameObjectRendererInstance(prefab);
#endif

                    return renderer;
                }
            }

            #endregion

            #region Spawn

            public GameObject Spawn(Vector3 point, Vector3 normal)
            {
                var go = GameObject.Instantiate(prefab);
                go.transform.localPosition = point;
                go.transform.localScale = go.transform.localScale.Multiply(CalculateSize());
                go.transform.localRotation = CalculateRotation(normal);
                return go;
            }

            public GameObject Spawn(Vector3 point, Vector3 normal, System.Random random)
            {
                var go = GameObject.Instantiate(prefab);
                go.transform.localPosition = point;
                go.transform.localScale = go.transform.localScale.Multiply(CalculateSize(random));
                go.transform.localRotation = CalculateRotation(normal, random);
                return go;
            }

            #endregion

            #region Calculation

            public Vector3 CalculateSize(System.Random random)
            {
                return Vector3.Lerp(minimumSize, maximumSize, sizeWeight.Evaluate(random.NextFloat()));
            }

            public Vector3 CalculateSize()
            {
                return Vector3.Lerp(minimumSize, maximumSize, sizeWeight.Evaluate(Random.value));
            }

            public Vector3 CalculateSize(float t)
            {
                return Vector3.Lerp(minimumSize, maximumSize, t);
            }

            internal Vector3 CalulatePosition(Vector3 position, Vector3 normal, Quaternion rotation)
            {
                return position + normal * normalOffset + rotation * originOffset;
            }

            internal Quaternion CalculateRotation(Vector3 inNormal, float tXvalue, float tZvalue, float yValue, Vector3 rValue)
            {
                switch (orientation)
                {
                    case Orientation.Normal: return Quaternion.FromToRotation(Vector3.up, inNormal) * Quaternion.Euler(tiltX.Evaluate(tXvalue), yValue * 360f, tiltZ.Evaluate(tZvalue)) * Quaternion.Euler(direction);
                    case Orientation.Direction: return Quaternion.LookRotation(Vector3.forward, direction) * Quaternion.Euler(tiltX.Evaluate(tXvalue), yValue * 360f, tiltZ.Evaluate(tZvalue));
                    case Orientation.Tilt: return Quaternion.Euler(tiltX.Evaluate(tXvalue), yValue * 360f, tiltZ.Evaluate(tZvalue));
                    case Orientation.Random: return Quaternion.Euler(rValue.x * 360f, rValue.y * 360f, rValue.z * 360f);
                }
                return Quaternion.Euler(0, yValue * 360f, 0);
            }

            public Quaternion CalculateRotation(Vector3 inNormal, System.Random random)
            {
                switch (orientation)
                {
                    case Orientation.Normal: return Quaternion.FromToRotation(Vector3.up, inNormal) * Quaternion.Euler(tiltX.Evaluate(random.NextFloat()), yAxisRotation ? random.NextFloat() * 360f : 0f, tiltZ.Evaluate(random.NextFloat())) * Quaternion.Euler(direction);
                    case Orientation.Random: return Quaternion.Euler(random.NextFloat() * 360f, random.NextFloat() * 360f, random.NextFloat() * 360f);
                    case Orientation.Direction: return Quaternion.Euler(direction) * Quaternion.Euler(tiltX.Evaluate(random.NextFloat()), yAxisRotation ? random.NextFloat() * 360f : 0f, tiltZ.Evaluate(random.NextFloat()));
                    case Orientation.Tilt: return Quaternion.Euler(tiltX.Evaluate(random.NextFloat()), yAxisRotation ? random.NextFloat() * 360f : 0f, tiltZ.Evaluate(random.NextFloat()));
                }
                return Quaternion.identity;
            }

            public Quaternion CalculateRotation(Vector3 inNormal)
            {
                switch (orientation)
                {
                    case Orientation.Normal: return Quaternion.FromToRotation(Vector3.up, inNormal) * Quaternion.Euler(direction) * Quaternion.Euler(tiltX.Evaluate(Random.value), yAxisRotation ? Random.value * 360f : 0f, tiltZ.Evaluate(Random.value));
                    case Orientation.Random: return Toolkit.Mathematics.Random.Rotation;
                    case Orientation.Direction: return Quaternion.Euler(direction) * Quaternion.Euler(tiltX.Evaluate(Random.value), yAxisRotation ? Random.value * 360f : 0f, tiltZ.Evaluate(Random.value));
                    case Orientation.Tilt: return Quaternion.Euler(tiltX.Evaluate(Random.value), yAxisRotation ? Random.value * 360f : 0f, tiltZ.Evaluate(Random.value));
                }
                return Quaternion.identity;
            }

            #endregion
        }

        public enum Orientation
        {
            Default = 0,
            Random = 1,
            Direction = 2,
            Normal = 3,
            Tilt = 4,
        }
    }
}
