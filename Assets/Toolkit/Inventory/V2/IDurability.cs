using System;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Inventory.V2 {
    public interface IDurability {
        float Current { get; }
        float Max { get; }

        bool Use(float amount);
        bool Restore(float amount);
    }

    public class Durability : BaseItemData, IDurability {
        #region Variables

        public float Current { get; set; } = 1f;
        public float Max { get; set; } = 1f;

        #endregion

        #region Constructor

        public Durability() {

        }

        public Durability(int current, int max) {
            Current = current;
            Max = max;
        }

        #endregion

        #region Durability Impl

        public bool Use(float amount) {
            if(amount <= Mathf.Epsilon)
                return true;
            if(Current <= Mathf.Epsilon)
                return false;
            Current -= amount;
            return true;
        }

        public bool Restore(float amount) {
            if(amount <= Mathf.Epsilon)
                return true;
            Current = Mathf.Min(Current + amount, Max);
            return true;
        }

        #endregion

        #region Serialization

        public override void Serialize(TMLNode node) {
            node.AddProperty("current", Current);
            node.AddProperty("max", Max);
        }

        public override void Deserialize(TMLNode node) {
            Current = node.GetFloat("current", Current);
            Max = node.GetFloat("max", Max);
        }

        #endregion
    }

    public static class DurabilityExtensions {
        #region Broken

        public static bool IsBroken(this IDurability durability) {
            return durability.Current <= UnityEngine.Mathf.Epsilon;
        }

        public static bool Break(this IDurability durability) {
            return durability.Use(durability.Current);
        }

        #endregion

        #region Restore

        public static bool RestorePercentage(this IDurability durability, float percentage) {
            return durability.Restore(percentage * durability.Max);
        }

        public static bool RestoreFull(this IDurability durability) {
            return durability.Restore(durability.Max);
        }

        #endregion

        #region Calculations

        public static float GetPercentage(this IDurability durability) {
            var max = durability.Max;
            if(max <= Mathf.Epsilon)
                return durability.Current;
            return durability.Current / max;
        }

        #endregion
    }
}
