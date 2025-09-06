///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

// Created by - Toolkit.Unit.CustomSkillsEditor.cs

using System;
using UnityEngine;
using System.Linq;
namespace Toolkit.Unit
{
    [AddComponentMenu("Toolkit/Unit/Skills")]
    public class SkillsBehaviour : MonoBehaviour, ISkills
    {
        [SerializeField] private Skill _Illusion = new Skill(SkillType.Illusion);
        public ISkill Illusion => _Illusion;

        [SerializeField] private Skill _Conjuration = new Skill(SkillType.Conjuration);
        public ISkill Conjuration => _Conjuration;

        [SerializeField] private Skill _Destruction = new Skill(SkillType.Destruction);
        public ISkill Destruction => _Destruction;

        [SerializeField] private Skill _Restoration = new Skill(SkillType.Restoration);
        public ISkill Restoration => _Restoration;

        [SerializeField] private Skill _Alteration = new Skill(SkillType.Alteration);
        public ISkill Alteration => _Alteration;

        [SerializeField] private Skill _Enchanting = new Skill(SkillType.Enchanting);
        public ISkill Enchanting => _Enchanting;

        [SerializeField] private Skill _Smithing = new Skill(SkillType.Smithing);
        public ISkill Smithing => _Smithing;

        [SerializeField] private Skill _Heavy_Armor = new Skill(SkillType.Heavy_Armor);
        public ISkill Heavy_Armor => _Heavy_Armor;

        [SerializeField] private Skill _Block = new Skill(SkillType.Block);
        public ISkill Block => _Block;

        [SerializeField] private Skill _Two_Handed = new Skill(SkillType.Two_Handed);
        public ISkill Two_Handed => _Two_Handed;

        [SerializeField] private Skill _One_Handed = new Skill(SkillType.One_Handed);
        public ISkill One_Handed => _One_Handed;

        [SerializeField] private Skill _Archery = new Skill(SkillType.Archery);
        public ISkill Archery => _Archery;

        [SerializeField] private Skill _Light_Armor = new Skill(SkillType.Light_Armor);
        public ISkill Light_Armor => _Light_Armor;

        [SerializeField] private Skill _Sneak = new Skill(SkillType.Sneak);
        public ISkill Sneak => _Sneak;

        [SerializeField] private Skill _Lockpicking = new Skill(SkillType.Lockpicking);
        public ISkill Lockpicking => _Lockpicking;

        [SerializeField] private Skill _Pickpocket = new Skill(SkillType.Pickpocket);
        public ISkill Pickpocket => _Pickpocket;

        [SerializeField] private Skill _Speech = new Skill(SkillType.Speech);
        public ISkill Speech => _Speech;

        [SerializeField] private Skill _Alchemy = new Skill(SkillType.Alchemy);
        public ISkill Alchemy => _Alchemy;

        public ISkill[] GetAllSkills() => new ISkill[] { _Illusion, _Conjuration, _Destruction, _Restoration, _Alteration, _Enchanting, _Smithing, _Heavy_Armor, _Block, _Two_Handed, _One_Handed, _Archery, _Light_Armor, _Sneak, _Lockpicking, _Pickpocket, _Speech, _Alchemy };

        public ISkill GetSkill(SkillType type) {
            switch(type) {
                case SkillType.Illusion: return _Illusion;
                case SkillType.Conjuration: return _Conjuration;
                case SkillType.Destruction: return _Destruction;
                case SkillType.Restoration: return _Restoration;
                case SkillType.Alteration: return _Alteration;
                case SkillType.Enchanting: return _Enchanting;
                case SkillType.Smithing: return _Smithing;
                case SkillType.Heavy_Armor: return _Heavy_Armor;
                case SkillType.Block: return _Block;
                case SkillType.Two_Handed: return _Two_Handed;
                case SkillType.One_Handed: return _One_Handed;
                case SkillType.Archery: return _Archery;
                case SkillType.Light_Armor: return _Light_Armor;
                case SkillType.Sneak: return _Sneak;
                case SkillType.Lockpicking: return _Lockpicking;
                case SkillType.Pickpocket: return _Pickpocket;
                case SkillType.Speech: return _Speech;
                case SkillType.Alchemy: return _Alchemy;
            }
            return null;

        }

        public ISkill[] GetSkills(SkillCategory category) => GetAllSkills().Where(x => SkillUtility.IsSkillCategory(x.SkillType, category)).ToArray();
    }
}
