using UnityEngine;
using System.Threading;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Toolkit.Threading
{
    public class ThreadedUpdate
    {
        #region Variables

        // Consts
        private const int MAX_FPS = 16384;
        private const int DEFAULT_FPS = 8192;
        private long TICKS => Stopwatch.Frequency;
        private float TICKS_FLOAT => Stopwatch.Frequency;
        private const int FPS_STORAGE = 20;

        // Global Registry
        private static List<ThreadedUpdate> threadedUpdates = new List<ThreadedUpdate>();

        // Configuration
        private string name = "";
        private int maxFps = 0;
        private long maxFrameLengthInTicks = 0;
        private Thread thread;

        // Update
        private TLinkedList<IThreadedUpdate> nodes = new TLinkedList<IThreadedUpdate>();
        private long ticks = 0;
        private bool isRunning = true;
        private Stopwatch sw = new Stopwatch();

        // FPS Calculations
        private int fps_index = 0;
        private long[] fps_ticksContainer = new long[FPS_STORAGE];

        #endregion

        #region Properties

        public string Name => name;
        public int Count => nodes.Count;

        /// <summary>
        /// A range between 1 and 16384 (inclusive)
        /// </summary>
        public int MaxFps {
            get => maxFps;
            set {
                if(maxFps != value) {
                    maxFps = Mathf.Clamp(value, 1, 16384);
                    maxFrameLengthInTicks = TICKS / maxFps;
                }
            }
        }

        public float Fps {
            get {
                long totalTicks = 0;
                for(int i = 0; i < FPS_STORAGE; i++) {
                    totalTicks += fps_ticksContainer[i];
                }
                return TICKS_FLOAT / (float)(totalTicks / (double)FPS_STORAGE);
            }
        }

        public long TicksLastFrame => ticks;
        public float DeltaTime => ticks / TICKS_FLOAT;

        public bool IsThreadAlive => thread.IsAlive;

        public bool IsRunning {
            get => isRunning || thread.IsAlive;
            set {
                if(isRunning == false && value) {
                    if(thread.IsAlive) {
                        isRunning = true;
                    }
                    else {
                        Initialize();
                    }
                }
                else {
                    isRunning = false;
                }
            }
        }

        #endregion

        #region Constructor

        public ThreadedUpdate(string name) : this(name, DEFAULT_FPS) { }

        public ThreadedUpdate(string name, int fps) : this(name, fps, true) { }

        public ThreadedUpdate(string name, int fps, bool initialize) {
            this.name = name;
            MaxFps = fps;
            if(initialize)
                Initialize();
            threadedUpdates.Add(this);
        }

        private void Initialize() {
            if(thread != null && thread.IsAlive ) {
                if(!isRunning) 
                    UnityEngine.Debug.LogError("Attempting to initialize a thread that already exists but not running: " + name);
                else
                    UnityEngine.Debug.LogError("Could not initialize thread as it is already created: " + name);
                return;
            }
            thread = new Thread(new ThreadStart(Update));
            thread.IsBackground = true; // Ensure it closes if main thread is closed.
            thread.Start();
            // Handle Thread closing
            UnityThreading.OnClose -= Kill;
            UnityThreading.OnClose += Kill;
        }

        #endregion

        #region Deconstructor

        public void Kill() {
            UnityThreading.OnClose -= Kill;
            isRunning = false;
            thread.Abort();
            nodes.Clear();
        }

        ~ThreadedUpdate() {
            Kill();
        }

        #endregion

        #region Update

        void Update() {
            while(isRunning) {
                sw.Start();

                var dt = ticks / TICKS_FLOAT;

                try {
                    IThreadedUpdate update;
                    var iterator = nodes.GetEnumerator();
                    while(iterator.MoveNext(out update)) {
                        try {
                            if(update == null || update.IsNull)
                                iterator.DestroyCurrent();
                            else
                                update.Update(dt);
                        }
                        catch(Exception e) {
                            UnityEngine.Debug.LogException(e);
                            iterator.DestroyCurrent();
                        }
                    }
                }
                catch(Exception e) {
                    UnityEngine.Debug.LogException(e);
                }

                // Sleep first to include it into frame time and calculations.
                //Thread.Sleep(new System.TimeSpan(Math.Max(1000, maxFrameLengthInTicks)));
                while(sw.ElapsedTicks < maxFrameLengthInTicks) {
                    Thread.Yield();
                }

                ticks = sw.ElapsedTicks;
                // FPS
                fps_ticksContainer[fps_index] = ticks;
                fps_index = (fps_index + 1) % FPS_STORAGE;

                sw.Reset();
            }
        }

        #endregion

        #region Subscribe / Unsubscribe

        public TLinkedListNode<IThreadedUpdate> Add(IThreadedUpdate update) => Subscribe(update);
        public TLinkedListNode<IThreadedUpdate> Subscribe(IThreadedUpdate update) {
            return nodes.Add(update);
        }

        public bool Remove(IThreadedUpdate update) => Unsubscribe(update);
        public bool Unsubscribe(IThreadedUpdate update) {
            return nodes.Remove(update);
        }

        public bool Unsubscribe(TLinkedListNode<IThreadedUpdate> node) {
            return nodes.Remove(node);
        }

        #endregion

        #region Global Registry

        internal static int RegisteredCount => threadedUpdates.Count;
        internal static ThreadedUpdate GetThreadedUpdate(int index) => threadedUpdates[index];
        internal static IEnumerable<ThreadedUpdate> GetThreadedUpdates(Func<ThreadedUpdate, bool> search) => threadedUpdates.Where(search);

        #endregion
    }
}
