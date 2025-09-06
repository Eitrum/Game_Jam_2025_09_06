using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit {
    public static class StatsExtensions {
        #region Add

        public static void Add(this IStats stats, StatsModifier modifier) {
            var stat = stats.GetStat(modifier.StatType);
            stat.Add(modifier.ValueType, modifier.Value);
            stats.SetStat(modifier.StatType, stat);
        }

        public static void Add(this IStats stats, StatType statType, Stat.FieldValue fieldValue) {
            var stat = stats.GetStat(statType);
            stat.Add(fieldValue.Type, fieldValue.Value);
            stats.SetStat(statType, stat);
        }

        public static void Add(this IStats stats, StatType statType, float value) {
            var stat = stats.GetStat(statType);
            stat.AddBase(value);
            stats.SetStat(statType, stat);
        }

        public static void Add(this IStats stats, StatType statType, Stat stat) {
            var tstat = stats.GetStat(statType);
            tstat += stat;
            stats.SetStat(statType, tstat);
        }

        public static void Add(this IStats stats, StatType statType, Stat.ValueType valueType, float value) {
            var stat = stats.GetStat(statType);
            stat.Add(valueType, value);
            stats.SetStat(statType, stat);
        }

        public static void AddAll(this IStats stats, IReadOnlyList<StatsModifier> modifiers) {
            foreach(var m in modifiers)
                stats.Add(m);
        }

        #endregion

        #region Remove

        public static void Remove(this IStats stats, StatsModifier modifier) {
            var stat = stats.GetStat(modifier.StatType);
            stat.Remove(modifier.ValueType, modifier.Value);
            stats.SetStat(modifier.StatType, stat);
        }

        public static void Remove(this IStats stats, StatType statType, Stat.FieldValue fieldValue) {
            var stat = stats.GetStat(statType);
            stat.Remove(fieldValue.Type, fieldValue.Value);
            stats.SetStat(statType, stat);
        }

        public static void Remove(this IStats stats, StatType statType, float value) {
            var stat = stats.GetStat(statType);
            stat.RemoveBase(value);
            stats.SetStat(statType, stat);
        }

        public static void Remove(this IStats stats, StatType statType, Stat stat) {
            var tstat = stats.GetStat(statType);
            tstat -= stat;
            stats.SetStat(statType, tstat);
        }

        public static void Remove(this IStats stats, StatType statType, Stat.ValueType valueType, float value) {
            var stat = stats.GetStat(statType);
            stat.Remove(valueType, value);
            stats.SetStat(statType, stat);
        }

        #endregion

        #region Reset

        public static void Reset(this IStats stats) {
            foreach(var t in FastEnum.GetValues<StatType>())
                stats.SetStat(t, StatsDefaults.Get(t));
        }

        #endregion

        #region Payload

        public static void Add(this IStats stats, StatsModifierPayload payload) {
            foreach(var m in payload.Modifiers)
                Add(stats, m);
        }

        public static void Add(this IStats stats, StatsEntryPayload payload) {
            foreach(var e in payload.Entries)
                Add(stats, e.Type, e.Stat);
        }

        public static void Remove(this IStats stats, StatsModifierPayload payload) {
            foreach(var m in payload.Modifiers)
                Remove(stats, m);
        }

        public static void Remove(this IStats stats, StatsEntryPayload payload) {
            foreach(var e in payload.Entries)
                Remove(stats, e.Type, e.Stat);
        }

        #endregion

        #region Get

        public static float GetStat(this IStats stats, StatType type, Stat.ValueType valueType) {
            var stat = stats.GetStat(type);
            return stat[valueType];
        }

        #endregion
    }
}
