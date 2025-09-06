using System;
using UnityEngine;

namespace Toolkit.Unit
{
    [Serializable]
    public class ExperienceClass : IExperience
    {
        #region Variables

        [SerializeField] private double experience = 0;
        [SerializeField] private int level = 1;
        [SerializeField] private IObjRef<IExperienceTable> table = new IObjRef<IExperienceTable>();

        private event OnExperienceUpdateCallback onExperienceUpdate;
        private event OnLevelUpdateCallback onLevelUpdate;

        #endregion

        #region Properties

        public IExperienceTable Table {
            get => table.Reference ?? ExperienceUtility.Table;
            set => table.Reference = value;
        }

        public event OnExperienceUpdateCallback OnExperienceUpdate {
            add => onExperienceUpdate += value;
            remove => onExperienceUpdate -= value;
        }
        public event OnLevelUpdateCallback OnLevelUpdate {
            add => onLevelUpdate += value;
            remove => onLevelUpdate -= value;
        }

        public bool IsMaxLevel => level >= Table.Levels;

        public double Experience {
            get => experience;
            set {
                var diff = value - experience;
                experience = value;
                while(level > 0 && experience < 0) {
                    experience += ExperienceUtility.ExperienceNeededForLevel(Table, level);
                    level--;
                    onLevelUpdate?.Invoke(level);
                }
                double expNextLevel = 0;
                while(!IsMaxLevel && experience >= (expNextLevel = ExperienceUtility.ExperienceNeededForLevel(Table, level + 1))) {
                    experience -= expNextLevel;
                    level++;
                    onLevelUpdate?.Invoke(level);
                }
                experience = System.Math.Max(0, experience);
                onExperienceUpdate?.Invoke(experience, diff);

            }
        }
        public double TotalExperience {
            get => ExperienceUtility.TotalExperienceNeededForLevel(Table, level) + experience;
            set => Experience += value - TotalExperience;
        }
        public int Level {
            get => level;
            set {
                value = ExperienceUtility.ClampLevel(Table, value);
                if(level != value) {
                    var diff = (ExperienceUtility.TotalExperienceNeededForLevel(Table, value) - ExperienceUtility.TotalExperienceNeededForLevel(Table, level)) - experience;
                    level = value;
                    experience = 0;
                    onLevelUpdate?.Invoke(level);
                    onExperienceUpdate?.Invoke(experience, diff);
                }
                else {
                    var lost = -experience;
                    experience = 0;
                    onExperienceUpdate?.Invoke(experience, lost);
                }

            }
        }
        public float LevelingPercentage {
            get => level >= Table.Levels ? 1f : (float)(experience / ExperienceUtility.ExperienceNeededForLevel(Table, level + 1));
            set => Experience = level < Table.Levels ? (int)(ExperienceUtility.ExperienceNeededForLevel(Table, level + 1) * value) : 0;
        }

        #endregion

        #region Constructor

        public ExperienceClass() { }
        public ExperienceClass(int level) => this.level = level;

        public ExperienceClass(int level, int experience) {
            this.level = level;
            this.experience = experience;
        }

        public ExperienceClass(int level, int experience, IExperienceTable table) {
            this.level = level;
            this.experience = experience;
            this.table.Reference = table;
        }

        #endregion
    }
}
