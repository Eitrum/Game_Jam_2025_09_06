using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Spells.V2
{
    public interface IAbilityData
    {
        string Name { get; }
        string Description { get; }
        int Hash { get; }
        object this[DataKey key] { get; set; }

        void Add<T>(DataKey key, T value);
        void Override<T>(DataKey key, T value);

        IAbilityData Copy();
    }

    public enum DataKey // Custom editor, Groups. 
    {
        // Empty, should never be used! Using "none" is concidered an error in configuration!
        None = 0,

        // Hidden
        [InspectorName("")] HiddenDataEnabled = 1,
        [InspectorName("")] CurrentFunctionId = 2,
        [InspectorName("")] CurrentTime = 3,
        [InspectorName("")] CurrentTotalTime = 4,
        [InspectorName("")] Seed = 5,

        // Metadata ( 10 -> 100 )
        [InspectorName("Metadata")] _Group_Metadata = 10,
        [InspectorName("Metadata/Name")] Name = _Group_Metadata + 1,
        [InspectorName("Metadata/Description")] Description = _Group_Metadata + 2,
        [InspectorName("Metadata/Tags")] Tags = _Group_Metadata + 3,

        [InspectorName("Metadata/Level Requirement"), Tooltip("The level requirement to unlock or cast the ability.")] LevelReqirement = _Group_Metadata + 11,
        [InspectorName("Metadata/Skill Requirement"), Tooltip("The skill level requirement. (Not to be confused with level requirement)")] SkillRequirement = _Group_Metadata + 12,
        [InspectorName("Metadata/Attribute Requirement"), Tooltip("The required attributes to use or cast the ability.")] AttributeRequirement = _Group_Metadata + 13,

        [InspectorName("Metadata/School"), Tooltip("The type of school the spell is grouped with.")] School = _Group_Metadata + 21,
        [InspectorName("Metadata/Element")] Element = _Group_Metadata + 22,

        // Projectile ( 100 -> 199 )
        [InspectorName("Projectile")] _Group_Projectile = 100,
        [InspectorName("Projectile/Projectile Mode")] ProjectileMode = _Group_Projectile + 1,
        [InspectorName("Projectile/Projectile Count")] ProjectileCount = _Group_Projectile + 2,
        [InspectorName("Projectile/Chain"), Tooltip("Amount of times the projectile will chain to other targets")] Chain = _Group_Projectile + 3,
        [InspectorName("Projectile/Fork"), Tooltip("Amount of times the projectile will fork in other directions")] Fork = _Group_Projectile + 4,

        // Area ( 200 -> 299 )
        [InspectorName("Area")] _Group_Area = 200,
        [InspectorName("Area/Shape")] AreaShape = _Group_Area + 1,
        [InspectorName("Area/Size")] AreaSize = _Group_Area + 2,
        [InspectorName("Area/Falloff Curve")] AreaFalloff = _Group_Area + 3,
    }

    [System.Flags]
    public enum School // Custom editor
    {
        None = 0,

        Alteration = 1 << 0,
        [Tooltip("Manupulation of time")] Chronurgy = 1 << 1,
        Conjuration = 1 << 2,
        Divination = 1 << 3,
        Destruction = 1 << 4,
        Enchantment = 1 << 5,
        Evocation = 1 << 6,
        Illusion = 1 << 7,
        Mysticism = 1 << 8,
        Necromancy = 1 << 9,
        Restoration = 1 << 10,
        Transmutation = 1 << 11,
    }

    [System.Flags]
    public enum Element // Should be custom editor
    {
        None = 0,

        Air = 1 << 0,
        Earth = 1 << 1,
        Fire = 1 << 2,
        Water = 1 << 3,

        Light = 1 << 4,
        Shadow = 1 << 5,
    }

    [System.Flags]
    public enum Tags
    {
        None = 0,
        Projectile = 1 << 0,
        AoE = 1 << 1,

    }

    public class AbilityData : IAbilityData
    {
        #region Variables

        private Dictionary<DataKey, object> storage;

        #endregion

        #region Properties

        public string Name => storage.TryGetValue(DataKey.Name, out string descr) ? descr : "missing";
        public string Description => storage.TryGetValue(DataKey.Description, out string descr) ? descr : "missing";
        public int Hash => throw new System.NotImplementedException();
        public object this[DataKey key] {
            get => storage[key];
            set {
                if(storage.ContainsKey(key))
                    storage[key] = value;
                else
                    storage.Add(key, value);
            }
        }

        #endregion

        #region Constructor

        public AbilityData() {
            storage = new Dictionary<DataKey, object>();
        }

        public AbilityData(Dictionary<DataKey, object> values, bool createNewCopy = true) {
            storage = createNewCopy ? values.CreateCopy() : values;
        }

        #endregion

        #region Storage Methods

        public void Add<T>(DataKey key, T value) {
            storage.Add(key, value);
        }

        public void Override<T>(DataKey key, T value) {
            storage[key] = value;
        }

        public T Get<T>(DataKey key) {
            return storage.TryGetValue(key, out T value) ? value : default;
        }

        #endregion

        #region Copy

        public IAbilityData Copy() {
            return new AbilityData(storage);
        }

        #endregion
    }
}
