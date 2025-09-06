#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Threading.Tasks;

namespace Toolkit.IO.TML.Test {
    public static class TMLJsonParserTest {
        // https://developer.mozilla.org/en-US/docs/Learn/JavaScript/Objects/JSON
        public const string TEST_TEXT = @"{
  ""squadName"": ""Super hero squad"",
  ""homeTown"": ""Metro City"",
  ""formed"": 2016,
  ""secretBase"": ""Super tower"",
  ""active"": true,
  ""members"": [
    {
      ""name"": ""Molecule Man"",
      ""age"": 29,
      ""secretIdentity"": ""Dan Jukes"",
      ""powers"": [""Radiation resistance"", ""Turning tiny"", ""Radiation blast""]
    },
    {
      ""name"": ""Madame Uppercut"",
      ""age"": 39,
      ""secretIdentity"": ""Jane Wilson"",
      ""powers"": [
        ""Million tonne punch"",
        ""Damage resistance"",
        ""Superhuman reflexes""
      ],
      ""hello, world"":[[""jesus"", ""water""],""lowest"", [""empty""], [[""pikaboo""],[""No""]]]
    },
    {
      ""name"": ""Eternal Flame"",
      ""age"": 1000000,
      ""secretIdentity"": ""Unknown"",
      ""powers"": [
        ""Immortality"",
        ""Heat Immunity"",
        ""Inferno"",
        ""Teleportation"",
        ""Interdimensional travel""
      ]
    }
  ]
}
";

        [MenuItem("Toolkit/Tests/IO/JsonParser")]
        public static void RunTest() {
            Debug.Log(TEST_TEXT);
            TMLJsonParser.TryParse(TEST_TEXT, out Toolkit.IO.TMLNode root);
            var output = TMLParser.ToString(root, true);
            Debug.Log(output);
        }
    }
}
#endif
