
namespace Toolkit.Unit {
    public interface IStats {
        event OnStatsChangedDelegate OnStatsChanged;
        Stat GetStat(StatType type);
        void SetStat(StatType type, Stat stat);
    }

    public delegate void OnStatsChangedDelegate(StatType type);
}
