using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit
{
    [CreateAssetMenu(menuName = "Toolkit/Unit/Experience/Table")]
    public class ExpereinceTable : ScriptableObject, IExperienceTable, ISerializationCallbackReceiver
    {
        #region Variables

        [SerializeField] private double[] experienceTable = { 0, 0 };
        private double[] totalExperience = { };

        #endregion

        #region Properties

        public int Levels => experienceTable.Length - 1;

        public IReadOnlyList<double> Difference => experienceTable;
        public IReadOnlyList<double> TotalExperience => totalExperience;

        #endregion

        #region Utility

        [ContextMenu("Recalculate Total Experience")]
        public void RecalculateTotalExperience() {
            int length = experienceTable.Length;
            totalExperience = new double[length];
            if(length == 0)
                return;
            double total = experienceTable[0];
            for(int i = 1; i < length; i++)
                totalExperience[i] = (total += experienceTable[i]);
        }

        #endregion

        #region Serialization Impl

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            RecalculateTotalExperience();
        }

        #endregion
    }
}
