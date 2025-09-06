using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit
{
    public interface IExperienceTable
    {
        int Levels { get; }

        IReadOnlyList<double> TotalExperience { get; }
        IReadOnlyList<double> Difference { get; }
    }

    [System.Serializable]
    public class SimpleExperienceTable : IExperienceTable, ISerializationCallbackReceiver
    {
        #region Variables

        [SerializeField] private double[] difference;
        private double[] totalExperience;

        #endregion

        #region Properties

        public int Levels => difference.Length - 1;
        public IReadOnlyList<double> Difference => difference;
        public IReadOnlyList<double> TotalExperience => totalExperience;

        #endregion

        #region Constructor

        public SimpleExperienceTable() { }
        public SimpleExperienceTable(double[] difference) {
            this.difference = difference;
            LoadTotalExperienceTable();
        }

        #endregion

        #region ISerialization Impl

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }
        void ISerializationCallbackReceiver.OnAfterDeserialize() => LoadTotalExperienceTable();

        void LoadTotalExperienceTable() {
            int length = difference.Length;
            totalExperience = new double[length];
            if(length == 0)
                return;
            double total = difference[0];
            for(int i = 1; i < length; i++)
                totalExperience[i] = (total += difference[i]);
        }

        #endregion
    }
}
