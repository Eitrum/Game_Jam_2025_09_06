using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Toolkit.Debugging {
    public class PerformanceTester : MonoBehaviour {

        public delegate void MethodCall();

        public enum MethodType {
            Lambda,
            Instanced,
            Static
        }

        private class Instance {
            #region Variables

            public string Name;
            public MethodCall Method;

            public int TestsRan;
            public long Elapsed;
            public long CalculatedElapsed;
            public string Error;
            public MethodType Type;

            #endregion

            #region Properties

            public string Text {
                get {
                    if(!string.IsNullOrEmpty(Error))
                        return $"{Name} - {Error}";
                    else
                        return $"{Name} - {CalculatedElapsed} ticks ({(GetTimeSpan(CalculatedElapsed).Milliseconds):0.000}ms)";
                }
            }

            #endregion

            #region Constructor

            public Instance(string name, MethodCall method) {
                this.Name = name;
                this.Method = method;

                var isstatic = method.Method.IsStatic;
                var methodName = method.Method.Name;
                if(isstatic)
                    Type = MethodType.Static;
                else if(methodName.StartsWith("<"))
                    Type = MethodType.Lambda;
                else
                    Type = MethodType.Instanced;
            }

            #endregion

            #region Util

            public void Reset() {
                Elapsed = 0;
                CalculatedElapsed = 0;
                TestsRan = 0;
                Error = string.Empty;
            }

            private TimeSpan GetTimeSpan(long ticks) => TimeSpan.FromTicks((long)(((TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency)) * ticks));

            #endregion

            #region Run

            public void Run(int tests) {
                for(int i = 0; i < tests; i++) {
                    var time = PerformanceUtility.Timestamp;

                    Method();

                    var diff = PerformanceUtility.GetElapsedTicks(time);
                    Elapsed += diff;
                    TestsRan++;

                    // Safety break for performance reasons
                    if(diff > TICKS_PER_SECOND) {
                        Error = $"Attempt {i} took over a second";
                        return;
                    }

                    if(diff * tests > TICKS_PER_TEN_SECOND) {
                        Error = $"Attempt {i} estimated to take too much time";
                        return;
                    }
                }
            }

            #endregion
        }

        #region Variables

        private const string TAG = "[PerformanceTester] - ";
        private static readonly long TICKS_PER_SECOND = Stopwatch.Frequency;
        private static readonly long TICKS_PER_TWO_SECOND = Stopwatch.Frequency * 2;
        private static readonly long TICKS_PER_TEN_SECOND = Stopwatch.Frequency * 10;

        [SerializeField, Tooltip("How many times it should be tested per frame. This averages out the result over multiple attempts")] private int testsPerFrame = 10;
        [SerializeField, Tooltip("How many iterations should be executed per test, allows for very small methods to be called multiple times for larger time values")] private int iterationsPerTest = 100;
        [SerializeField, Tooltip("Prints Log Results.")] private bool logResults = false;
        [SerializeField, Tooltip("Draws a GUI with the output.")] private bool drawGUI = false;
        [SerializeField, Tooltip("Does GC Collect between each test")] private bool useGCCollect = false;

        private List<Instance> instances = new List<Instance>();
        private int test = 0;

        private MethodCall emptyCall1;
        private MethodCall emptyCall2;
        private MethodCall emptyCall3;

        private Instance lambda;
        private Instance instanced;
        private Instance @static;

        private void EmptyCallWrapper2() { }
        private static void EmptyCallWrapper3() { }

        #endregion

        #region Properties

        public int TestsPerFrame {
            get => testsPerFrame;
            set {
                if(testsPerFrame == value)
                    return;
                testsPerFrame = value;
            }
        }

        public int IterationsPerTest {
            get => iterationsPerTest;
            set {
                if(iterationsPerTest == value)
                    return;
                iterationsPerTest = value;
            }
        }

        #endregion

        #region Init

        private void Start() {
            // Setup a method call calculation
            emptyCall1 = () => { };
            emptyCall2 = EmptyCallWrapper2;
            emptyCall3 = EmptyCallWrapper3;

            lambda = new Instance("lambda", emptyCall1);
            instanced = new Instance("instanced", emptyCall2);
            @static = new Instance("static", emptyCall3);

            lambda.Run(10000);
            instanced.Run(10000);
            @static.Run(10000);

            Debug.Log(this.FormatLog("Method call performance test", lambda.Elapsed, instanced.Elapsed, @static.Elapsed));
        }

        #endregion

        #region Register

        public PerformanceTester Register(string name, MethodCall method) {
            instances.Add(new Instance(name, method));
            return this;
        }

        public PerformanceTester Unregister(string name) {
            for(int i = instances.Count - 1; i >= 0; i--) {
                if(instances[i].Name == name)
                    instances.RemoveAt(i);
            }
            return this;
        }

        public PerformanceTester Unregister(MethodCall method) {
            for(int i = instances.Count - 1; i >= 0; i--) {
                if(instances[i].Method == method)
                    instances.RemoveAt(i);
            }
            return this;
        }

        #endregion

        #region Setup

        public PerformanceTester SetTestsPerFrame(int testsPerFrame) {
            this.TestsPerFrame = testsPerFrame;
            return this;
        }

        public PerformanceTester SetIterationsPerTest(int iterationsPerTest) {
            this.IterationsPerTest = iterationsPerTest;
            return this;
        }

        public PerformanceTester SetLogResults(bool enable) {
            this.logResults = enable;
            return this;
        }

        public PerformanceTester SetDrawGUI(bool enable) {
            this.drawGUI = enable;
            return this;
        }

        public PerformanceTester SetConfig(int testsPerFrame, int iterationsPerTest, bool logResults, bool drawGui) {
            this.TestsPerFrame = testsPerFrame;
            this.IterationsPerTest = iterationsPerTest;
            this.logResults = logResults;
            this.drawGUI = drawGui;
            return this;
        }

        #endregion

        #region Update

        private void Update() {
            if(instances == null)
                return;

            if(instances.Count == 0)
                return;

            foreach(var inst in instances)
                inst.Reset();

            for(int i = 0; i < testsPerFrame; i++) {
                var totalTime = PerformanceUtility.Timestamp;
                foreach(var inst in instances)
                    inst.Run(iterationsPerTest);

                var totalElapsed = PerformanceUtility.GetElapsedTicks(totalTime);
                if(totalElapsed > TICKS_PER_TWO_SECOND) {
                    Debug.LogError(TAG + $"All tests takes over 2 seconds to run, cancelling.");
                    break;
                }

                if(useGCCollect) {
                    var gcstart = PerformanceUtility.Timestamp;
                    GC.Collect();
                    var gcduration = PerformanceUtility.GetElapsedTicks(gcstart);
                    if((gcduration * testsPerFrame) > TICKS_PER_TEN_SECOND) {
                        useGCCollect = false;
                        Debug.LogError(TAG + $"GC Collect takes up too much time, disabling it!");
                    }
                }
            }

            foreach(var i in instances) {
                var type = i.Type;
                var toRemovePerCall = TicksToRemovePerCall(type);
                var calls = i.TestsRan;
                var toRemove = (long)(toRemovePerCall * calls);
                i.CalculatedElapsed = (i.Elapsed - toRemove) / testsPerFrame;
            }

            if(logResults) {
                Debug.Log(TAG + $"--- Test {test++} ---");
                foreach(var i in instances)
                    Debug.Log(TAG + i.Text);
            }
        }

        private double TicksToRemovePerCall(MethodType type) {
            switch(type) {
                case MethodType.Lambda: return (lambda.Elapsed / (double)lambda.TestsRan);
                case MethodType.Instanced: return (instanced.Elapsed / (double)instanced.TestsRan);
                case MethodType.Static: return (@static.Elapsed / (double)@static.TestsRan);
                default: return 0d;
            }
        }

        #endregion

        #region GUI

        private void OnGUI() {
            if(!drawGUI)
                return;
            var box = new Rect(10, 10, 300, 300);
            GUI.Box(box, "Performance Test");

            GUILayout.BeginArea(box);
            GUILayout.Space(20f);
            foreach(var i in instances) {
                GUILayout.Label(i.Text);
            }

            GUILayout.EndArea();
        }

        #endregion
    }
}
