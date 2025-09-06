using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Unit
{
    public interface IExperience
    {
        double Experience { get; set; }
        int Level { get; set; }
        float LevelingPercentage { get; set; }
        double TotalExperience { get; set; }
        IExperienceTable Table { get; set; }
        event OnExperienceUpdateCallback OnExperienceUpdate;
        event OnLevelUpdateCallback OnLevelUpdate;
    }

    public delegate void OnExperienceUpdateCallback(double experience, double deltaGained);
    public delegate void OnLevelUpdateCallback(int level);
}
