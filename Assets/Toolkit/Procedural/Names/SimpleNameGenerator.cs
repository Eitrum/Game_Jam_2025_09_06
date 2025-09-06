using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Toolkit.Procedural.Names
{
    [CreateAssetMenu(fileName = "Name Generator", menuName = "Toolkit/Procedural/Names/Name Generator (Simple)")]
    public class SimpleNameGenerator : ScriptableObject, INameGenerator, INameGenerator<int>
    {
        #region Variables

        [SerializeField] private float prefixProbability = 0f;
        [SerializeField] private string[] prefixNames = { };
        [SerializeField] private bool firstName = false;
        [SerializeField] private string[] firstNames = { };
        [SerializeField] private MinMaxInt middleNameCount = new MinMaxInt(0, 0);
        [SerializeField] private AnimationCurve middleNameCountWeight = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private string[] middleNames = { };
        [SerializeField] private MinMaxInt lastNameCount = new MinMaxInt(0, 0);
        [SerializeField] private AnimationCurve lastNameCountWeight = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private string[] lastNames = { };
        [SerializeField] private float suffixProbability = 0f;
        [SerializeField] private string[] suffixNames = { };

        #endregion

        #region Generation

        public string Generate() {
            StringBuilder sb = new StringBuilder();
            if(Random.value < prefixProbability) {
                sb.Append($"{prefixNames.RandomElement()} ");
            }
            if(firstName) {
                sb.Append($"{firstNames.RandomElement()} ");
            }
            if(middleNameCount.min > 0) {
                var mnc = middleNameCount.min != middleNameCount.max ? middleNameCount.Evaluate(middleNameCountWeight.Evaluate(Random.value)) : middleNameCount.min;
                for(int i = 0; i < mnc; i++) {
                    sb.Append($"{middleNames.RandomElement()} ");
                }
            }
            if(lastNameCount.min > 0) {
                var lnc = lastNameCount.min != lastNameCount.max ? lastNameCount.Evaluate(lastNameCountWeight.Evaluate(Random.value)) : lastNameCount.min;
                for(int i = 0; i < lnc; i++) {
                    sb.Append($"{lastNames.RandomElement()} ");
                }
            }
            if(Random.value < suffixProbability) {
                sb.Append($"{suffixNames.RandomElement()}");
            }

            return sb.ToString();
        }

        public string Generate(int seed) {
            System.Random random = new System.Random(seed);
            StringBuilder sb = new StringBuilder();
            if(random.NextDouble() < prefixProbability) {
                sb.Append($"{prefixNames.RandomElement(random)} ");
            }
            if(firstName) {
                sb.Append($"{firstNames.RandomElement(random)} ");
            }
            if(middleNameCount.min > 0) {
                var mnc = middleNameCount.min != middleNameCount.max ? middleNameCount.Evaluate(middleNameCountWeight.Evaluate((float)random.NextDouble())) : middleNameCount.min;
                for(int i = 0; i < mnc; i++) {
                    sb.Append($"{middleNames.RandomElement(random)} ");
                }
            }
            if(lastNameCount.min > 0) {
                var lnc = lastNameCount.min != lastNameCount.max ? lastNameCount.Evaluate(lastNameCountWeight.Evaluate((float)random.NextDouble())) : lastNameCount.min;
                for(int i = 0; i < lnc; i++) {
                    sb.Append($"{lastNames.RandomElement(random)} ");
                }
            }
            if(random.NextDouble() < suffixProbability) {
                sb.Append($"{suffixNames.RandomElement(random)}");
            }

            return sb.ToString().TrimEnd();
        }

        #endregion
    }
}
