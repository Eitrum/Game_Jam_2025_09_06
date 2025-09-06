using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit
{
    [CreateAssetMenu(menuName = "Toolkit/Unit/Experience/Table (Math Expression)")]
    public class ExperienceTableFunction : ScriptableObject, IExperienceTable, ISerializationCallbackReceiver
    {
        #region Variables

        [SerializeField] private string expression = "x*x";
        [SerializeField] private int levels = 99;
        private Mathematics.IMathExpression compiled;
        private double[] experienceTable = { 0, 1 };
        private double[] totalExperience = { };

        #endregion

        #region Properties

        public int Levels => levels;

        public IReadOnlyList<double> Difference => experienceTable;
        public IReadOnlyList<double> TotalExperience => totalExperience;

        #endregion

        #region Utility

        [ContextMenu("Recalculate Experience Table")]
        public void RecalculateExperienceTable() {
            experienceTable = new double[levels + 1];
            for(int i = 2; i <= levels; i++) {
                using(var sv = Mathematics.StoredVariables.Create(i - 1))
                    experienceTable[i] = compiled.Compute(sv);
            }
        }

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

        [ContextMenu("Log")]
        private void PrintLogFunction() {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            compiled.CreateLogTree(sb, 0);
            Debug.Log(sb.ToString());
        }

        #endregion

        #region Serialization Impl

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            compiled = Toolkit.Mathematics.MathematicalExpressionBuilder.Build(expression);
            RecalculateExperienceTable();
            RecalculateTotalExperience();
        }

        #endregion
    }
}
