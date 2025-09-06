using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Network {
    public static class ServerTime {

        #region Variables

        private const string TAG = "<color=#FFA500>[Toolkit.Network.ServerTime]</color> - ";

        private const float MILLISECONDS_TO_SECONDS = 0.001f;
        private const float SECONDS_TO_MILLISECONDS = 1000f;
        private const int MILLISECONDS_IN_DAY = 86_400_000;

        private static float accumulateTime;
        private static int milliseconds = 0;
        private static int days = 0;

        private static int lastUpdate;
        private static CircularBuffer<int> diffStorage = new CircularBuffer<int>(4);
        private static CircularBuffer<int> roundTripTimeStorage = new CircularBuffer<int>(4);
        private static int roundTripTimeCalculated;

        #endregion

        #region Properties

        public static int Milliseconds => milliseconds;
        public static int RoundTripTime => roundTripTimeCalculated;
        public static int LastUpdate => lastUpdate;
        public static int AverageFault => diffStorage.Average();
        public static MinMaxInt FaultRange => new MinMaxInt(diffStorage.Min(), diffStorage.Max());

        #endregion

        #region Init

        static ServerTime() {
            PlayerLoopUtilty.AddLast("TimeUpdate", typeof(ServerTime), InternalUpdate);
            Debugging.Commands.Add<int>("servertime set", UpdateFromServerInMilliseconds, Debugging.Privilege.Admin);
            Debugging.Commands.Add<int, int>("servertime set", UpdateFromServerInMilliseconds, Debugging.Privilege.Admin);
            Debugging.Commands.Add("servertime get", PrintCurrentTime, Debugging.Privilege.Normal);
            Debugging.Commands.Add<int>("servertime gettimesince", PrintTimeSince, Debugging.Privilege.Normal);
        }

        #endregion

        #region Update

        private static void InternalUpdate() {
            var dt = Time.unscaledDeltaTime;
            Update(dt);
        }

        private static void Update(float deltaTime) {
            accumulateTime += deltaTime;
            var accumelateMilliseconds = Mathf.FloorToInt(accumulateTime * SECONDS_TO_MILLISECONDS);
            accumulateTime -= accumelateMilliseconds * MILLISECONDS_TO_SECONDS;
            milliseconds += accumelateMilliseconds;
            if(milliseconds > MILLISECONDS_IN_DAY) {
                milliseconds -= MILLISECONDS_IN_DAY;
                days++;
                // All tech reliant on time might need to temporarily shift everything by a day to handle the reset
            }
        }

        /// <summary>
        /// Set server time without calculating ping
        /// </summary>
        public static void UpdateFromServerInMilliseconds(int milliseconds)
            => UpdateFromServerInMilliseconds(milliseconds, 0);

        /// <summary>
        /// Set server time with roundtrip time included
        /// </summary>
        public static void UpdateFromServerInMilliseconds(int milliseconds, int roundTripTimeInMilliseconds) {
            var old = ServerTime.milliseconds;
            ServerTime.milliseconds = milliseconds + roundTripTimeInMilliseconds / 2;
            lastUpdate = ServerTime.milliseconds;

            var diff = ServerTime.milliseconds - old;
            if(Mathf.Abs(diff) < 100)
                diffStorage.Write(diff);

            roundTripTimeStorage.Write(roundTripTimeInMilliseconds);
            roundTripTimeCalculated = roundTripTimeStorage.Average();
        }

        #endregion

        #region Commands

        private static void PrintCurrentTime() {
            Debug.Log(TAG + $"ServerTime: {Milliseconds}ms");
        }

        private static void PrintTimeSince(int oldTime) {
            Debug.Log(TAG + $"ServerTimeSince '{oldTime}': {GetTimeSince(oldTime):0.00}s");
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Returns the time difference in seconds.
        /// </summary>
        public static float GetTimeSince(int oldTime) {
            var deltaMs = Milliseconds - oldTime;
            return deltaMs * MILLISECONDS_TO_SECONDS;
        }

        /// <summary>
        /// Returns a DateTime (local time) based on the server time provided.
        /// </summary>
        public static DateTime GetDateTimeAt(int serverTime) {
            var dt = DateTime.UtcNow;
            var delta = GetTimeSince(serverTime);
            return dt.AddSeconds(delta);
        }

        #endregion
    }
}
