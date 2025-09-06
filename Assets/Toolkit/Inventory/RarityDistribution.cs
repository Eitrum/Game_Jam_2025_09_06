///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

// Created by - Toolkit.Inventory.RarityDistributionCodeGeneration.cs

using System;
using UnityEngine;
namespace Toolkit.Inventory
{
    [UnityEngine.CreateAssetMenu(fileName = "Rarity Distribution", menuName = "Toolkit/Inventory/Rarity Distribution")]
    public class RarityDistribution : ScriptableObject
    {
        [System.Serializable]
        public class Weight
        {
            [UnityEngine.SerializeField] private float common = 0;
            [UnityEngine.SerializeField] private float uncommon = 0;
            [UnityEngine.SerializeField] private float rare = 0;
            [UnityEngine.SerializeField] private float epic = 0;
            [UnityEngine.SerializeField] private float legendary = 0;
            [UnityEngine.SerializeField] private float unique = 0;
            [UnityEngine.SerializeField] private float npc = 0;
            [UnityEngine.SerializeField] private float total = 0;
            public float CommonWeight => common;

            public float CommonPercentage => common / total;

            public float UncommonWeight => uncommon;

            public float UncommonPercentage => uncommon / total;

            public float RareWeight => rare;

            public float RarePercentage => rare / total;

            public float EpicWeight => epic;

            public float EpicPercentage => epic / total;

            public float LegendaryWeight => legendary;

            public float LegendaryPercentage => legendary / total;

            public float UniqueWeight => unique;

            public float UniquePercentage => unique / total;

            public float NPCWeight => npc;

            public float NPCPercentage => npc / total;

            public Rarity GetRarity() {
                var value = UnityEngine.Random.value * total;
                if(value <= common)
                    return Rarity.Common;
                if(value <= uncommon)
                    return Rarity.Uncommon;
                if(value <= rare)
                    return Rarity.Rare;
                if(value <= epic)
                    return Rarity.Epic;
                if(value <= legendary)
                    return Rarity.Legendary;
                if(value <= unique)
                    return Rarity.Unique;
                if(value <= npc)
                    return Rarity.NPC;
                return Rarity.None;

            }

            public Rarity GetRarity(System.Random random) {
                var value = (float)(random.NextDouble()) * total;
                if(value <= common)
                    return Rarity.Common;
                if(value <= uncommon)
                    return Rarity.Uncommon;
                if(value <= rare)
                    return Rarity.Rare;
                if(value <= epic)
                    return Rarity.Epic;
                if(value <= legendary)
                    return Rarity.Legendary;
                if(value <= unique)
                    return Rarity.Unique;
                if(value <= npc)
                    return Rarity.NPC;
                return Rarity.None;

            }
        }

        [UnityEngine.SerializeField] private Weight[] weight;
        [UnityEngine.SerializeField] private bool levelBased = true;
        public Weight GetWeightByIndex(int index) => weight[index];

        public Weight GetWeightByLevel(int level) => weight[Mathf.Clamp(level, 0, Toolkit.Unit.ExperienceUtility.Levels)];

        public int WeightCount => weight.Length;

        public bool IsLevelBased => levelBased;
    }
}
