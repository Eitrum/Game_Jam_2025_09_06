using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.CodeDom;

namespace Toolkit.CodeAnalysis {
    public class TestVisualClass {

        [SerializeField, Tooltip("Collio")]
        [Header("hello")]
        public TestClass2 testClass2;

        public void MyMethod() {
            testClass2.MyMethodBack();
        }

        public static void Log() {
            Debug.Log("PRint");
        }

        public int Hello232 => 29;

        public int Hello {
            get {
                return 34;
            }
            set {
                MyMethod();
            }
        }
    }

    public struct MyStruct {

    }

    [System.Serializable]
    public class TestClass2 {

        public void MyMethodBack() {
            PlayerPrefs.SetFloat("Temp", 0);
            TestVisualClass.Log();
        }

        public void MyMethodWithParameters(int one, MyStruct myStruct, bool boo, TestVisualClass myclass) {

        }

        public MyStruct MyMethodWithReturn() {
            return default;
        }
    }
}
