using System;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Toolkit {
    public static class PerformanceUtility {
        #region Stopwatch

        public const bool STOPWATCH_SCOPE_PRINT_BY_DEFAULT = true;

        private static readonly double stopwatchToTimeSpan = (System.TimeSpan.TicksPerSecond / (double)System.Diagnostics.Stopwatch.Frequency);

        public static Stopwatch NewStopwatch => new Stopwatch();
        public static long Timestamp => Stopwatch.GetTimestamp();

        public static long GetElapsedTicks(long startTimestamp) => Timestamp - startTimestamp;
        public static long GetElapsedTicks(long startTimestamp, long endTimestamp) => endTimestamp - startTimestamp;

        public static TimeSpan GetElapsedTimeSpan(long startTimestamp) => TimeSpan.FromTicks((long)(stopwatchToTimeSpan * GetElapsedTicks(startTimestamp)));
        public static TimeSpan GetElapsedTimeSpan(long startTimestamp, long endTimestamp) => TimeSpan.FromTicks((long)(stopwatchToTimeSpan * GetElapsedTicks(startTimestamp, endTimestamp)));

        #endregion

        #region Tester

        public static Debugging.PerformanceTester CreateTest(Debugging.PerformanceTester.MethodCall entry0Method, Debugging.PerformanceTester.MethodCall entry1Method) {
            var pt = CreateTest();
            pt.SetLogResults(true);
            pt.SetDrawGUI(true);
            pt.Register("Test 1", entry0Method);
            pt.Register("Test 2", entry1Method);
            return pt;
        }

        public static Debugging.PerformanceTester CreateTest(Debugging.PerformanceTester.MethodCall entry0Method, Debugging.PerformanceTester.MethodCall entry1Method, Debugging.PerformanceTester.MethodCall entry2Method) {
            var pt = CreateTest();
            pt.SetLogResults(true);
            pt.SetDrawGUI(true);
            pt.Register("Test 1", entry0Method);
            pt.Register("Test 2", entry1Method);
            pt.Register("Test 3", entry2Method);
            return pt;
        }

        public static Debugging.PerformanceTester CreateTest(int testsPerFrame, int iterationsPerTest, Debugging.PerformanceTester.MethodCall entry0Method, Debugging.PerformanceTester.MethodCall entry1Method) {
            var pt = CreateTest();
            pt.SetConfig(testsPerFrame, iterationsPerTest, true, true);
            pt.Register("Test 1", entry0Method);
            pt.Register("Test 2", entry1Method);
            return pt;
        }

        public static Debugging.PerformanceTester CreateTest(int testsPerFrame, int iterationsPerTest, Debugging.PerformanceTester.MethodCall entry0Method, Debugging.PerformanceTester.MethodCall entry1Method, Debugging.PerformanceTester.MethodCall entry2Method) {
            var pt = CreateTest();
            pt.SetConfig(testsPerFrame, iterationsPerTest, true, true);
            pt.Register("Test 1", entry0Method);
            pt.Register("Test 2", entry1Method);
            pt.Register("Test 3", entry2Method);
            return pt;
        }

        public static Debugging.PerformanceTester CreateTest() {
            var go = new GameObject("Performance Tester", typeof(Debugging.PerformanceTester));
            var perftest = go.GetOrAddComponent<Debugging.PerformanceTester>();
            return perftest;
        }

        public static Debugging.PerformanceTester CreateTest(string gameobjectname) {
            if(string.IsNullOrEmpty(gameobjectname))
                return CreateTest();
            var go = new GameObject(gameobjectname, typeof(Debugging.PerformanceTester));
            var perftest = go.GetOrAddComponent<Debugging.PerformanceTester>();
            return perftest;
        }

        #endregion

        #region Disposable

        public static StopwatchScope CreateStopwatchScope(Type type, bool printResults = STOPWATCH_SCOPE_PRINT_BY_DEFAULT) {
            return new StopwatchScope(type, printResults: printResults);
        }

        public static StopwatchScope CreateStopwatchScope(Type type, string name, bool printResults = STOPWATCH_SCOPE_PRINT_BY_DEFAULT) {
            return new StopwatchScope(type, name, printResults: printResults);
        }

        public static StopwatchScope CreateStopwatchScope<T>(T source, bool printResults = STOPWATCH_SCOPE_PRINT_BY_DEFAULT) {
            return new StopwatchScope(source, printResults: printResults);
        }

        public static StopwatchScope CreateStopwatchScope<T>(T source, string name, bool printResults = STOPWATCH_SCOPE_PRINT_BY_DEFAULT) {
            return new StopwatchScope(source, name, printResults: printResults);
        }

        public static StopwatchScope CreateStopwatchScope<T>(bool printResults = STOPWATCH_SCOPE_PRINT_BY_DEFAULT) {
            return new StopwatchScope(typeof(T), printResults: printResults);
        }

        public static StopwatchScope CreateStopwatchScope<T>(string name, bool printResults = STOPWATCH_SCOPE_PRINT_BY_DEFAULT) {
            return new StopwatchScope(typeof(T), name, printResults: printResults);
        }

        public static StopwatchScope CreateStopwatchScope(string name, bool printResults = STOPWATCH_SCOPE_PRINT_BY_DEFAULT) {
            return new StopwatchScope(name, printResults: printResults);
        }

        public class StopwatchScope : IDisposable {

            #region Formatting Enum

            private enum Formatting {
                None = 0,
                Name = 1,
                Source = 2,
                SourceName = 3,
                Type = 4,
                TypeName = 5,
            }

            #endregion

            #region Variables

            private readonly Formatting formatting = Formatting.None;
            private readonly object Source;
            public readonly string Name;
            public readonly long Timestamp;
            public bool PrintResults;
            public long ElapsedTicks => GetElapsedTicks(Timestamp);
            public TimeSpan ElapsedTimeSpan => GetElapsedTimeSpan(Timestamp);
            public float ElapsedMilliseconds => ElapsedTicks * StopwatchExtensions.TICKS_TO_MILLISECONDS;

            #endregion

            #region Constructor

            public StopwatchScope(bool printResults = STOPWATCH_SCOPE_PRINT_BY_DEFAULT) {
                this.formatting = Formatting.None;
                this.PrintResults = printResults;
                this.Timestamp = PerformanceUtility.Timestamp;
            }

            public StopwatchScope(string name, bool printResults = STOPWATCH_SCOPE_PRINT_BY_DEFAULT) {
                this.formatting = Formatting.Name;
                this.Name = name;
                this.PrintResults = printResults;
                this.Timestamp = PerformanceUtility.Timestamp;
            }

            public StopwatchScope(object source, bool printResults = STOPWATCH_SCOPE_PRINT_BY_DEFAULT) {
                this.formatting = Formatting.Source;
                this.Source = source;
                this.PrintResults = printResults;
                this.Timestamp = PerformanceUtility.Timestamp;
            }

            public StopwatchScope(string name, object source, bool printResults = STOPWATCH_SCOPE_PRINT_BY_DEFAULT) {
                this.formatting = Formatting.SourceName;
                this.Name = name;
                this.Source = source;
                this.PrintResults = printResults;
                this.Timestamp = PerformanceUtility.Timestamp;
            }

            public StopwatchScope(object source, string name, bool printResults = STOPWATCH_SCOPE_PRINT_BY_DEFAULT) {
                this.formatting = Formatting.SourceName;
                this.Name = name;
                this.Source = source;
                this.PrintResults = printResults;
                this.Timestamp = PerformanceUtility.Timestamp;
            }

            public StopwatchScope(Type type, bool printResults = STOPWATCH_SCOPE_PRINT_BY_DEFAULT) {
                this.formatting = Formatting.Type;
                this.Source = type;
                this.PrintResults = printResults;
                this.Timestamp = PerformanceUtility.Timestamp;
            }

            public StopwatchScope(string name, Type type, bool printResults = STOPWATCH_SCOPE_PRINT_BY_DEFAULT) {
                this.formatting = Formatting.TypeName;
                this.Name = name;
                this.Source = type;
                this.PrintResults = printResults;
                this.Timestamp = PerformanceUtility.Timestamp;
            }

            public StopwatchScope(Type type, string name, bool printResults = STOPWATCH_SCOPE_PRINT_BY_DEFAULT) {
                this.formatting = Formatting.TypeName;
                this.Name = name;
                this.Source = type;
                this.PrintResults = printResults;
                this.Timestamp = PerformanceUtility.Timestamp;
            }

            #endregion

            #region Dispose

            public void Dispose() {
                if(PrintResults)
                    Print();
            }

            #endregion

            #region Helper

            public void Print() {
                Debug.Log(ToString());
            }

            public override string ToString() {
                var ticks = ElapsedTicks;
                switch(formatting) {
                    case Formatting.None: return $"<color=grey>Elapsed: {ticks} ticks ({ticks * StopwatchExtensions.TICKS_TO_MILLISECONDS:0.000}ms)</color>";
                    case Formatting.Name: return $"<color=grey>{Name}: {ticks} ticks ({ticks * StopwatchExtensions.TICKS_TO_MILLISECONDS:0.000}ms)</color>";
                    case Formatting.Source: return Source.FormatLog($"Elapsed: {ticks} ticks ({ticks * StopwatchExtensions.TICKS_TO_MILLISECONDS:0.000}ms)", Color.gray);
                    case Formatting.SourceName: return Source.FormatLog($"{Name}: {ticks} ticks ({ticks * StopwatchExtensions.TICKS_TO_MILLISECONDS:0.000}ms)", Color.gray);
                    case Formatting.Type: return $"<color=grey>[{(Source as Type).FullName}] - Elapsed: {ticks} ticks ({ticks * StopwatchExtensions.TICKS_TO_MILLISECONDS:0.000}ms)</color>";
                    case Formatting.TypeName: return $"<color=grey>[{(Source as Type).FullName}] - {Name}: {ticks} ticks ({ticks * StopwatchExtensions.TICKS_TO_MILLISECONDS:0.000}ms)</color>";
                }
                return $"{ticks} ticks";
            }

            #endregion
        }

        #endregion
    }
}
