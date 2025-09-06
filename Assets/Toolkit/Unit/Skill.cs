using System;
using System.Linq;
using UnityEngine;

namespace Toolkit.Unit
{
    public interface ISkill
    {
        int TotalLevel { get; }
        IExperience Experience { get; }
        int TemporaryLevels { get; set; }
        SkillType SkillType { get; }
    }

    [System.Serializable]
    public class Skill : ISkill
    {
        private SkillType type = SkillType.None;
        [SerializeField] private ExperienceClass experience = new ExperienceClass(0, 0);
        private int temporaryLevels = 0;

        public int TotalLevel => experience.Level + temporaryLevels;
        public IExperience Experience => experience;
        public int TemporaryLevels { get => temporaryLevels; set => temporaryLevels = value; }
        public SkillType SkillType => type;

        public Skill(SkillType type) => this.type = type;
    }
}
