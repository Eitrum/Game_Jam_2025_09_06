using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Toolkit.Procedural.Names
{
    [CreateAssetMenu(fileName = "Fantasy Name Generator", menuName = "Toolkit/Procedural/Names/Fantasy Name Generator")]
    public class FantasyNameGenerator : ScriptableObject, INameGenerator, INameGenerator<int>
    {
        #region Variables

        [SerializeField] private string[] nm1;
        [SerializeField] private string[] nm2;
        [SerializeField] private string[] nm3;
        [SerializeField] private string[] nm4;

        #endregion

        #region Generate

        public string Generate() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nm1.RandomElement());
            sb.Append(nm2.RandomElement());
            sb.Append(" ");
            sb.Append(nm3.RandomElement());
            sb.Append(nm4.RandomElement());
            return sb.ToString();
        }

        public string Generate(int seed) {
            System.Random random = new System.Random(seed);
            StringBuilder sb = new StringBuilder();
            sb.Append(nm1.RandomElement(random));
            sb.Append(nm2.RandomElement(random));
            sb.Append(" ");
            sb.Append(nm3.RandomElement(random));
            sb.Append(nm4.RandomElement(random));
            return sb.ToString();
        }

        #endregion
    }
}
