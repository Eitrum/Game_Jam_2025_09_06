using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolkit.Unit;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Toolkit.Inventory
{
    [CreateAssetMenu(fileName = "Simple drop table", menuName = "Toolkit/Inventory/Drop Table")]
    public class SimpleDropTable : ScriptableObject, IDropTable, IVerify
    {
        #region Variables

        [SerializeField] private string overrideName = "";
        [SerializeField] private string description = "";
        [SerializeField] private SimpleDropData[] commonDropTable = null;

        [SerializeField] private MinMaxInt rareDropTableAmount = new MinMaxInt(0, 1);
        [SerializeField] private AnimationCurve rareDropTableWeight = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private bool uniqueDrops = true;
        [SerializeField] private SimpleDropData[] rareDropTable = null;
        [SerializeField] private bool disableAdvancedDropTable = false;

        #endregion

        #region IDropTable Impl

        public string Name => string.IsNullOrEmpty(overrideName) ? name : overrideName;
        public string Description => description;

        public IDropData[] GetDrop(IUnit source) {
            List<IDropData> drops = new List<IDropData>();
            foreach(var dd in commonDropTable) {
                if(dd.Percentage >= Random.value)
                    drops.Add(dd);
            }

            var rareCount = rareDropTableAmount.Evaluate(rareDropTableWeight.Evaluate(Random.value));
            if(rareCount > 0 && rareDropTable.Length > 0) {
                drops.AddRange(rareDropTable.RandomElements(rareCount, uniqueDrops, x => x.Weight));
            }

            return drops.ToArray();
        }

        public IDropData[][] GetDropAdvanced(IUnit source) { // TEMPORARY Spaghetti
            List<IDropData[]> drops = new List<IDropData[]>();
            foreach(var dd in commonDropTable) {
                if(dd.Percentage >= Random.value)
                    drops.Add(new IDropData[] { dd });
            }

            var level = source?.Experience.Level ?? 1;
            var maximumProbability = 0.25f + (level / 20f) * (level / 25f);
            var minimumProbability = (level / 45f) * (level / 45f);
            var range = new MinMax(minimumProbability, maximumProbability);

            var rareCount = rareDropTableAmount.Evaluate(rareDropTableWeight.Evaluate(Random.value));
            if(rareDropTable.Length > 0) {
                for(int i = 0; i < rareCount; i++) {
                    var tCount = MinMaxInt.Lerp(1, 5, range.Evaluate(Random.value));
                    var t = rareDropTable.RandomElement(x => x.Weight);

                    if(disableAdvancedDropTable || t.Drop.Type == ItemType.Resource)
                        drops.Add(new IDropData[] { t });
                    else
                        drops.Add(rareDropTable.RandomElements(Mathf.Clamp(tCount, 1, rareDropTable.Length), uniqueDrops, x => (x.Drop.Type == ItemType.Resource ? 0f : x.Weight)));
                }
            }

            return drops.ToArray();
        }

        #endregion

        #region Verify

        public bool Verify(out string error) {
            foreach(var d in commonDropTable)
                if(d.Object == null) {
                    error = "common drop table has missing object";
                    return false;
                }

            foreach(var d in rareDropTable)
                if(d.Object == null) {
                    error = "rare drop table has missing object";
                    return false;
                }

            error = null;
            return true;
        }

        #endregion
    }

    [System.Serializable]
    public class SimpleDropData : IDropData
    {
        #region Variables

        [SerializeField, Min(0f)] private float percentage = 0f;
        [SerializeField] private MinMaxInt range = new MinMaxInt(1, 1);
        [SerializeField] private AnimationCurve rangeWeight = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField, TypeFilter(typeof(IItem))] private UnityEngine.Object item = null;

        #endregion

        #region Properties

        public float Percentage => percentage;
        public float Weight => percentage;
        public MinMaxInt Range => range;
        public AnimationCurve RangeWeight => rangeWeight;
        public UnityEngine.Object Object => item;
        public IItem Drop => item is IItem drop ? drop : (item is GameObject go ? go.GetComponent<IItem>() : null);
        public bool IsStackable => item is IStackable || (item is GameObject go && go.GetComponent<IStackable>() != null);

        #endregion

        #region Methods

        public int GetAmount(IUnit source) {
            return range.Evaluate(rangeWeight.Evaluate(UnityEngine.Random.value));
        }

        #endregion
    }
}
