using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Toolkit {
    public static class LabelsDatabase {

        [System.Serializable]
        private class Storage { public List<int> recentlyUsed = new List<int>(); public List<LabelData> data = new List<LabelData>(); }

        #region Const Defaults

        public const int DEFAULT_ORDER_TYPE = 1000;
        public const int DEFAULT_ORDER_CATEGORY = 800;
        public const int DEFAULT_ORDER_OBJECT = 700;
        public const int DEFAULT_ORDER_ENVIRONMENT = 600;
        public const int DEFAULT_ORDER_UNIT = 500;
        public const int DEFAULT_ORDER_MATERIAL = 400;
        public const int DEFAULT_ORDER_DESCRIPTOR = 300;
        public const int DEFAULT_ORDER_ACTION = 200;
        public const int DEFAULT_ORDER_SUFFIX = -1000;
        public const int DEFAULT_ORDER_SUFFIX_AUDIOTYPE = -1001;

        #endregion

        #region Variables

        private const string FOLDER = "ProjectSettings/Labels";
        private const string FILE = "ProjectSettings/Labels/labeldatas";
        private const string TAG = "[Toolkit.LabelsDatabase] - ";
        private static List<LabelData> allLabels = new List<LabelData>();
        private static Dictionary<int, LabelData> idToLabel = new Dictionary<int, LabelData>();
        private static bool isDirty = false;
        private static Action<LabelData> onAdd;
        private static List<LabelData> lastUsedLabels = new List<LabelData>(30);

        #endregion

        #region Properties

        public static IReadOnlyList<LabelData> AllLabels => allLabels;
        public static IReadOnlyDictionary<int, LabelData> IdToLabel => idToLabel;

        public static IEnumerable<LabelData> LastUsedLabels => lastUsedLabels;

        public static event Action<LabelData> OnAdd {
            add => onAdd += value;
            remove => onAdd -= value;
        }

        #endregion

        #region Init

        static LabelsDatabase() {
            if(System.IO.File.Exists(FILE)) {
                var json = System.IO.File.ReadAllText(FILE);
                var storage = new Storage();
                EditorJsonUtility.FromJsonOverwrite(json, storage);
                allLabels = storage.data;

                foreach(var label in allLabels)
                    idToLabel.Add(label.Id, label);

                lastUsedLabels.Clear();
                foreach(var recentid in storage.recentlyUsed) {
                    if(idToLabel.TryGetValue(recentid, out var labelData))
                        lastUsedLabels.Add(labelData);
                }
            }
            else {
                InitializeDefaultLabels();
            }
        }

        public static void InitializeDefaultLabels() {
            using(new OnAddProcedure("Type")) {
                using(new OnAddProcedure(typeof(UnityEngine.AudioClip))) {
                    Add("Ambience", DEFAULT_ORDER_TYPE, "AMB").AddType<UnityEngine.AudioClip>();
                    Add("SFX", DEFAULT_ORDER_TYPE);
                    Add("Music", DEFAULT_ORDER_TYPE);
                    Add("Voice", DEFAULT_ORDER_TYPE, "Vo");
                    Add("UI", DEFAULT_ORDER_TYPE);
                    Add("System", DEFAULT_ORDER_TYPE - 1, "Sys");
                }
                using(new OnAddProcedure(typeof(Material), typeof(GameObject))) {
                    Add("VFX", DEFAULT_ORDER_TYPE);
                    Add("UI", DEFAULT_ORDER_TYPE).AddType<AnimationClip>();
                    Add("HUD", DEFAULT_ORDER_TYPE - 1).AddType<AnimationClip>();
                }
                Add("Animation", 1000, "ANIM").AddType<AnimationClip>();
            }
            using(new OnAddProcedure("Category")) {
                using(new OnAddProcedure(typeof(GameObject), typeof(AudioClip))) {
                    Add("Weapon", DEFAULT_ORDER_CATEGORY);
                    Add("Vehicle", DEFAULT_ORDER_CATEGORY);
                    Add("Explosion", DEFAULT_ORDER_CATEGORY);
                }
            }
            using(new OnAddProcedure("Category")) {
                Add("Arrow", DEFAULT_ORDER_OBJECT);
                Add("Barrel", DEFAULT_ORDER_OBJECT);
                Add("Box", DEFAULT_ORDER_OBJECT);
                Add("Door", DEFAULT_ORDER_OBJECT);
                Add("Chest", DEFAULT_ORDER_OBJECT);
                Add("Sword", DEFAULT_ORDER_OBJECT);
                Add("Bow", DEFAULT_ORDER_OBJECT);
                Add("Gun", DEFAULT_ORDER_OBJECT);
                Add("Lever", DEFAULT_ORDER_OBJECT);
                Add("Button", DEFAULT_ORDER_OBJECT);
                Add("Switch", DEFAULT_ORDER_OBJECT);

                using(new OnAddProcedure((x) => x.SetHidden(true))) {
                    Add("Crate", DEFAULT_ORDER_OBJECT);
                    Add("Bag", DEFAULT_ORDER_OBJECT);
                    Add("Key", DEFAULT_ORDER_OBJECT);
                    Add("Gate", DEFAULT_ORDER_OBJECT);
                    Add("Bridge", DEFAULT_ORDER_OBJECT);
                    Add("Ladder", DEFAULT_ORDER_OBJECT);
                    Add("Rope", DEFAULT_ORDER_OBJECT);
                    Add("Chain", DEFAULT_ORDER_OBJECT);
                    Add("Coin", DEFAULT_ORDER_OBJECT);
                    Add("Gem", DEFAULT_ORDER_OBJECT);
                    Add("Potion", DEFAULT_ORDER_OBJECT);
                    Add("Scroll", DEFAULT_ORDER_OBJECT);
                    Add("Book", DEFAULT_ORDER_OBJECT);
                    Add("Torch", DEFAULT_ORDER_OBJECT);
                    Add("Lamp", DEFAULT_ORDER_OBJECT);
                    Add("Lantern", DEFAULT_ORDER_OBJECT);
                    Add("Shield", DEFAULT_ORDER_OBJECT);
                    Add("Spear", DEFAULT_ORDER_OBJECT);
                    Add("Bomb", DEFAULT_ORDER_OBJECT);
                    Add("Mine", DEFAULT_ORDER_OBJECT);
                    Add("Anvil", DEFAULT_ORDER_OBJECT);
                    Add("Statue", DEFAULT_ORDER_OBJECT);
                    Add("Sign", DEFAULT_ORDER_OBJECT);
                    Add("Banner", DEFAULT_ORDER_OBJECT);
                    Add("Map", DEFAULT_ORDER_OBJECT);
                    Add("Tent", DEFAULT_ORDER_OBJECT);

                    Add("Bed", DEFAULT_ORDER_OBJECT);
                    Add("Chair", DEFAULT_ORDER_OBJECT);
                    Add("Table", DEFAULT_ORDER_OBJECT);
                    Add("Bench", DEFAULT_ORDER_OBJECT);
                    Add("Cabinet", DEFAULT_ORDER_OBJECT);
                    Add("Shelf", DEFAULT_ORDER_OBJECT);
                    Add("Candle", DEFAULT_ORDER_OBJECT);
                    Add("Cup", DEFAULT_ORDER_OBJECT);
                    Add("Bottle", DEFAULT_ORDER_OBJECT);
                    Add("Plate", DEFAULT_ORDER_OBJECT);
                    Add("Bowl", DEFAULT_ORDER_OBJECT);
                    Add("Fork", DEFAULT_ORDER_OBJECT);
                    Add("Spoon", DEFAULT_ORDER_OBJECT);
                    Add("Bucket", DEFAULT_ORDER_OBJECT);
                    Add("Bar", DEFAULT_ORDER_OBJECT);
                    Add("Cart", DEFAULT_ORDER_OBJECT);
                    Add("Wagon", DEFAULT_ORDER_OBJECT);
                    Add("Wheel", DEFAULT_ORDER_OBJECT);
                    Add("Axle", DEFAULT_ORDER_OBJECT);
                    Add("Anchor", DEFAULT_ORDER_OBJECT);
                    Add("Oar", DEFAULT_ORDER_OBJECT);
                    Add("Sail", DEFAULT_ORDER_OBJECT);
                    Add("Crank", DEFAULT_ORDER_OBJECT);
                    Add("Pulley", DEFAULT_ORDER_OBJECT);
                    Add("Trap", DEFAULT_ORDER_OBJECT);
                    Add("Cage", DEFAULT_ORDER_OBJECT);
                    Add("Fence", DEFAULT_ORDER_OBJECT);
                    Add("Gatehouse", DEFAULT_ORDER_OBJECT);
                    Add("Tower", DEFAULT_ORDER_OBJECT);
                    Add("Wall", DEFAULT_ORDER_OBJECT);
                    Add("Rock", DEFAULT_ORDER_OBJECT);
                    Add("Tree", DEFAULT_ORDER_OBJECT);
                    Add("Log", DEFAULT_ORDER_OBJECT);
                    Add("Bush", DEFAULT_ORDER_OBJECT);
                    Add("Flower", DEFAULT_ORDER_OBJECT);
                    Add("Mushroom", DEFAULT_ORDER_OBJECT);
                    Add("Skull", DEFAULT_ORDER_OBJECT);
                    Add("Bone", DEFAULT_ORDER_OBJECT);
                    Add("Shell", DEFAULT_ORDER_OBJECT);
                    Add("Horn", DEFAULT_ORDER_OBJECT);
                    Add("Cloth", DEFAULT_ORDER_OBJECT);
                    Add("Tapestry", DEFAULT_ORDER_OBJECT);
                    Add("Drum", DEFAULT_ORDER_OBJECT);
                    Add("Bell", DEFAULT_ORDER_OBJECT);
                    Add("Pipe", DEFAULT_ORDER_OBJECT);
                    Add("Lever Wheel", DEFAULT_ORDER_OBJECT);
                    Add("Crank Handle", DEFAULT_ORDER_OBJECT);
                    Add("Workbench", DEFAULT_ORDER_OBJECT);
                    Add("Forge", DEFAULT_ORDER_OBJECT);
                    Add("Cauldron", DEFAULT_ORDER_OBJECT);
                    Add("Barrel Rack", DEFAULT_ORDER_OBJECT);
                    Add("Hay Bale", DEFAULT_ORDER_OBJECT);
                    Add("Water Trough", DEFAULT_ORDER_OBJECT);
                    Add("Feed Sack", DEFAULT_ORDER_OBJECT);
                }
            }

            using(new OnAddProcedure("Descriptor")) {
                Add("Small", DEFAULT_ORDER_DESCRIPTOR);
                Add("Big", DEFAULT_ORDER_DESCRIPTOR);
                Add("Fast", DEFAULT_ORDER_DESCRIPTOR);
                Add("Slow", DEFAULT_ORDER_DESCRIPTOR);
                Add("Heavy", DEFAULT_ORDER_DESCRIPTOR);
                Add("Light", DEFAULT_ORDER_DESCRIPTOR);
                Add("Soft", DEFAULT_ORDER_DESCRIPTOR);
                Add("Hard", DEFAULT_ORDER_DESCRIPTOR);
                Add("Impact", DEFAULT_ORDER_DESCRIPTOR);
                using(new OnAddProcedure((x => x.SetHidden(true)))) {
                    Add("Sharp", DEFAULT_ORDER_DESCRIPTOR);
                    Add("Blunt", DEFAULT_ORDER_DESCRIPTOR);
                    Add("Wet", DEFAULT_ORDER_DESCRIPTOR);
                    Add("Dry", DEFAULT_ORDER_DESCRIPTOR);
                    Add("Hot", DEFAULT_ORDER_DESCRIPTOR);
                    Add("Cold", DEFAULT_ORDER_DESCRIPTOR);
                    Add("Magic", DEFAULT_ORDER_DESCRIPTOR);
                    Add("Electric", DEFAULT_ORDER_DESCRIPTOR);
                    Add("Fiery", DEFAULT_ORDER_DESCRIPTOR);
                    Add("Poisoned", DEFAULT_ORDER_DESCRIPTOR);
                    Add("Explosive", DEFAULT_ORDER_DESCRIPTOR);
                }
            }

            using(new OnAddProcedure("Action")) {
                Add("Open", DEFAULT_ORDER_ACTION);
                Add("Close", DEFAULT_ORDER_ACTION);
                Add("Break", DEFAULT_ORDER_ACTION);
                Add("Jump", DEFAULT_ORDER_ACTION);
                Add("Run", DEFAULT_ORDER_ACTION);
                Add("Walk", DEFAULT_ORDER_ACTION);
                Add("Attack", DEFAULT_ORDER_ACTION);
                Add("Hit", DEFAULT_ORDER_ACTION);
                Add("Activate", DEFAULT_ORDER_ACTION);
                Add("Deactivate", DEFAULT_ORDER_ACTION);
                using(new OnAddProcedure((x => x.SetHidden(true)))) {
                    Add("Slash", DEFAULT_ORDER_ACTION);
                    Add("Stab", DEFAULT_ORDER_ACTION);
                    Add("Shoot", DEFAULT_ORDER_ACTION);
                    Add("Reload", DEFAULT_ORDER_ACTION);
                    Add("Cast", DEFAULT_ORDER_ACTION);
                    Add("Charge", DEFAULT_ORDER_ACTION);
                    Add("Land", DEFAULT_ORDER_ACTION);
                    Add("Fall", DEFAULT_ORDER_ACTION);
                    Add("Climb", DEFAULT_ORDER_ACTION);
                    Add("Grab", DEFAULT_ORDER_ACTION);
                    Add("Throw", DEFAULT_ORDER_ACTION);
                    Add("Block", DEFAULT_ORDER_ACTION);
                    Add("Dodge", DEFAULT_ORDER_ACTION);
                    Add("Die", DEFAULT_ORDER_ACTION);
                    Add("Respawn", DEFAULT_ORDER_ACTION);
                }
            }

            using(new OnAddProcedure((d) => d.AddGroup("Environment").SetColor(LabelsDrawer.LabelColor.Green))) {
                Add("Forest", DEFAULT_ORDER_ENVIRONMENT);
                Add("City", DEFAULT_ORDER_ENVIRONMENT);
                Add("Town", DEFAULT_ORDER_ENVIRONMENT);
                Add("Village", DEFAULT_ORDER_ENVIRONMENT);
                Add("Desert", DEFAULT_ORDER_ENVIRONMENT);
                Add("Cave", DEFAULT_ORDER_ENVIRONMENT);
                using(new OnAddProcedure((d) => d.HideInFullList = true)) {
                    Add("Space", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Underwater", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Tavern", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Grassland", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Grassland", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Jungle", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Swamp", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Marsh", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Grassland", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Meadow", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Savanna", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Tundra", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Arctic", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Mountain", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Volcano", DEFAULT_ORDER_ENVIRONMENT);
                    Add("River", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Lake", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Ocean", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Beach", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Cliff", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Waterfall", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Glacier", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Wetlands", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Rainforest", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Steppe", DEFAULT_ORDER_ENVIRONMENT);

                    Add("Suburb", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Ruins", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Factory", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Warehouse", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Construction Site", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Road", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Highway", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Airport", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Station", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Harbor", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Dock", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Market", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Mall", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Office", DEFAULT_ORDER_ENVIRONMENT);
                    Add("School", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Hospital", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Prison", DEFAULT_ORDER_ENVIRONMENT);
                }
            }

            using(new OnAddProcedure((d) => d.AddGroup("Weather").SetColor(LabelsDrawer.LabelColor.Green))) {
                Add("Rain", DEFAULT_ORDER_ENVIRONMENT);
                Add("Thunder", DEFAULT_ORDER_ENVIRONMENT);
                Add("Storm", DEFAULT_ORDER_ENVIRONMENT);
                Add("Windy", DEFAULT_ORDER_ENVIRONMENT);

                using(new OnAddProcedure((d) => d.HideInFullList = true)) {
                    Add("Sunny", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Cloudy", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Overcast", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Fog", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Mist", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Drizzle", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Blizzard", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Hail", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Sleet", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Dust Storm", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Sandstorm", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Tornado", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Hurricane", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Extreme Heat", DEFAULT_ORDER_ENVIRONMENT);
                    Add("Extreme Cold", DEFAULT_ORDER_ENVIRONMENT);
                }
            }

            using(new OnAddProcedure((x) => x.AddGroup("Material").SetColor(LabelsDrawer.LabelColor.Blue))) {

                Add("Wood", DEFAULT_ORDER_MATERIAL, "Wood");
                Add("Dirt", DEFAULT_ORDER_MATERIAL, "Dirt");
                Add("Stone", DEFAULT_ORDER_MATERIAL, "Stone");
                Add("Metal", DEFAULT_ORDER_MATERIAL, "Metal");
                Add("Glass", DEFAULT_ORDER_MATERIAL, "Glass");
                Add("Water", DEFAULT_ORDER_MATERIAL, "Water");
                Add("Ceramic", DEFAULT_ORDER_MATERIAL, "Ceramic");
                Add("Fabric", DEFAULT_ORDER_MATERIAL, "Fabric");

                using(new OnAddProcedure((d) => d.HideInFullList = true)) {
                    Add("Wood (Solid)", DEFAULT_ORDER_MATERIAL, "Wood-Solid");
                    Add("Wood (Hollow)", DEFAULT_ORDER_MATERIAL, "Wood-Hollow");
                    Add("Stone (Rough)", DEFAULT_ORDER_MATERIAL, "Stone-Rough");
                    Add("Stone (Smooth)", DEFAULT_ORDER_MATERIAL, "Stone-Smooth");
                    Add("Brick", DEFAULT_ORDER_MATERIAL, "Brick");
                    Add("Concrete", DEFAULT_ORDER_MATERIAL, "Concrete");
                    Add("Soil", DEFAULT_ORDER_MATERIAL, "Soil");
                    Add("Mud", DEFAULT_ORDER_MATERIAL, "Mud");
                    Add("Gravel", DEFAULT_ORDER_MATERIAL, "Gravel");
                    Add("Sand", DEFAULT_ORDER_MATERIAL, "Sand");
                    Add("Mud", DEFAULT_ORDER_MATERIAL, "Mud");
                    Add("Grass", DEFAULT_ORDER_MATERIAL, "Grass");
                    Add("Leaves", DEFAULT_ORDER_MATERIAL, "Leaves");
                    Add("Ice", DEFAULT_ORDER_MATERIAL, "Ice");
                    Add("Snow", DEFAULT_ORDER_MATERIAL, "Snow");
                    Add("Snow (Fresh)", DEFAULT_ORDER_MATERIAL, "Snow-Fresh");
                    Add("Snow (Packed)", DEFAULT_ORDER_MATERIAL, "Snow-Packed");
                    Add("Water (Still)", DEFAULT_ORDER_MATERIAL, "Water-Still");
                    Add("Water (Moving)", DEFAULT_ORDER_MATERIAL, "Water-Moving");

                    Add("Metal (Solid)", DEFAULT_ORDER_MATERIAL, "Metal-Solid");
                    Add("Metal (Hollow)", DEFAULT_ORDER_MATERIAL, "Metal-Hollow");
                    Add("Metal (Sheet)", DEFAULT_ORDER_MATERIAL, "Metal-Sheet");
                    Add("Metal (Chain)", DEFAULT_ORDER_MATERIAL, "Metal-Chain");
                    Add("Metal (Mesh)", DEFAULT_ORDER_MATERIAL, "Metal-Mesh");
                    Add("Metal (Liquid)", DEFAULT_ORDER_MATERIAL, "Metal-Liquid");
                    Add("Glass (Clear)", DEFAULT_ORDER_MATERIAL, "Glass-Clear");
                    Add("Glass (Frosted)", DEFAULT_ORDER_MATERIAL, "Glass-Frosted");
                    Add("Porcelain", DEFAULT_ORDER_MATERIAL, "Porcelain");
                    Add("Stoneware", DEFAULT_ORDER_MATERIAL, "Stoneware");
                    Add("Terracotta", DEFAULT_ORDER_MATERIAL, "Terracotta");

                    Add("Fabric (Cloth)", DEFAULT_ORDER_MATERIAL, "Fabric-Cloth");
                    Add("Fabric (Heavy)", DEFAULT_ORDER_MATERIAL, "Fabric-Heavy");
                    Add("Leather", DEFAULT_ORDER_MATERIAL, "Leather");
                    Add("Rubber (Solid)", DEFAULT_ORDER_MATERIAL, "Rubber-Solid");
                    Add("Rubber (Soft/Foam)", DEFAULT_ORDER_MATERIAL, "Rubber-Soft");
                    Add("Paper (Thin)", DEFAULT_ORDER_MATERIAL, "Paper-Thin");
                    Add("Paper (Thick/)", DEFAULT_ORDER_MATERIAL, "Paper-Thick");
                    Add("Twine", DEFAULT_ORDER_MATERIAL, "Twine");
                    Add("Flesh", DEFAULT_ORDER_MATERIAL, "Flesh");
                    Add("Bone M", DEFAULT_ORDER_MATERIAL, "Bone");
                    Add("Shell M", DEFAULT_ORDER_MATERIAL, "Shell");

                    Add("Energy (Electric)", DEFAULT_ORDER_MATERIAL, "Energy-Electric");
                    Add("Energy (Plasma)", DEFAULT_ORDER_MATERIAL, "Energy-Plasma");
                    Add("Magic (Arcane)", DEFAULT_ORDER_MATERIAL, "Magic-Arcane");
                    Add("Magic (Dark/Corrupt)", DEFAULT_ORDER_MATERIAL, "Magic-Dark");
                    Add("Crystal", DEFAULT_ORDER_MATERIAL, "Crystal");
                    Add("Plastic (Hard)", DEFAULT_ORDER_MATERIAL, "Plastic-Hard");
                    Add("Plastic (Soft)", DEFAULT_ORDER_MATERIAL, "Plastic-Soft");
                    Add("Synthetic Composite", DEFAULT_ORDER_MATERIAL, "Synthetic");
                }
            }

            using(new OnAddProcedure((x) => x.AddGroup("Unit").SetColor(LabelsDrawer.LabelColor.Orange))) {
                Add("Human", DEFAULT_ORDER_UNIT);
                Add("Goblin", DEFAULT_ORDER_UNIT);
                Add("Beast", DEFAULT_ORDER_UNIT);
                Add("Undead", DEFAULT_ORDER_UNIT);
                Add("Construct", DEFAULT_ORDER_UNIT);
                using(new OnAddProcedure((x) => x.HideInFullList = true)) {
                    Add("Orc", DEFAULT_ORDER_UNIT);
                    Add("Elf", DEFAULT_ORDER_UNIT);
                    Add("Dwarf", DEFAULT_ORDER_UNIT);
                    Add("Troll", DEFAULT_ORDER_UNIT);
                    Add("Ogre", DEFAULT_ORDER_UNIT);
                    Add("Giant", DEFAULT_ORDER_UNIT);
                    Add("Halfling", DEFAULT_ORDER_UNIT);
                    Add("Skeleton", DEFAULT_ORDER_UNIT);
                    Add("Zombie", DEFAULT_ORDER_UNIT);
                    Add("Ghost", DEFAULT_ORDER_UNIT);
                    Add("Vampire", DEFAULT_ORDER_UNIT);
                    Add("Werewolf", DEFAULT_ORDER_UNIT);
                    Add("Demon", DEFAULT_ORDER_UNIT);
                    Add("Angel", DEFAULT_ORDER_UNIT);
                    Add("Dragon", DEFAULT_ORDER_UNIT);
                }
            }

            using(new OnAddProcedure((x) => x.AddGroup("File Descriptor").SetColor(LabelsDrawer.LabelColor.Purple))) {
                Add("Prefab", DEFAULT_ORDER_SUFFIX, "Pref");
                Add("Material", DEFAULT_ORDER_SUFFIX, "Mat");
                Add("Texture", DEFAULT_ORDER_SUFFIX, "Tex");
                Add("Audio Clip", DEFAULT_ORDER_SUFFIX, "Sound").SetIncludeInName(false);
                Add("Atlas", DEFAULT_ORDER_SUFFIX, "Atlas");

                Add("Loop", DEFAULT_ORDER_SUFFIX_AUDIOTYPE).AddType<AudioClip>();
                Add("OneShot", DEFAULT_ORDER_SUFFIX_AUDIOTYPE).AddType<AudioClip>();
                Add("Stinger", DEFAULT_ORDER_SUFFIX_AUDIOTYPE).AddType<AudioClip>();
            }
        }

        #endregion

        #region Save

        public static void Save() {
            if(isDirty)
                return;
            isDirty = true;
            EditorApplication.delayCall += Internal_Save;
        }

        private static void Internal_Save() {
            isDirty = false;
            try {
                var storage = new Storage(){ data = allLabels };
                storage.recentlyUsed.AddRange(lastUsedLabels.Select(x => x.Id));
                var json = EditorJsonUtility.ToJson(storage, true);
                IO.IOUtility.File.TryWrite(FILE, json);
            }
            catch(Exception ex) {
                Debug.LogException(ex);
            }
        }

        #endregion

        #region Add

        public static LabelData Add(string label) {
            return Add(label, 0);
        }

        public static LabelData Add(string label, int order, string nameOverride = null, bool includeInNames = true) {
            var id = label.GetHash32();
            if(idToLabel.TryGetValue(id, out var existing)) {
                onAdd?.Invoke(existing);
                return existing;
            }

            var labelData = new LabelData(label, order, includeInNames);
            idToLabel.Add(id, labelData);
            allLabels.Add(labelData);
            onAdd?.Invoke(labelData);
            Save();
            return labelData;
        }

        #endregion

        #region Get

        public static IEnumerable<LabelData> GetLabels(UnityEngine.Object obj) {
            try {
                var labels = AssetDatabase.GetLabels(obj);
                foreach(var label in labels) {
                    if(TryGet(label, out var labelData))
                        yield return labelData;
                }
            }
            finally {

            }
        }

        public static bool TryGetLabelsSorted(UnityEngine.Object obj, out LabelData[] labels) {
            labels = GetLabels(obj).ToArray();
            Sort.Merge(labels, (a, b) => Sort.ByLargest(a.Order, b.Order));
            return labels != null;
        }

        public static bool TryGet(string label, out LabelData labelData) {
            return TryGet(label.GetHash32(), out labelData);
        }

        public static bool TryGet(int id, out LabelData labelData) {
            return idToLabel.TryGetValue(id, out labelData);
        }

        #endregion

        #region Mark Use

        public static void MarkUsed(LabelData lastUsed) {
            if(lastUsedLabels.Count >= lastUsedLabels.Capacity)
                lastUsedLabels.RemoveAt(lastUsedLabels.Capacity - 1);
            lastUsedLabels.Remove(lastUsed);
            lastUsedLabels.Insert(0, lastUsed);
        }

        #endregion

        #region Scope

        public class OnAddProcedure : IDisposable {
            private Action<LabelData> onAdd;
            private int cachedGroupId;
            private LabelTypeBinding cachedTypeBinding;
            private List<LabelTypeBinding> cachedTypeBindings;
            public OnAddProcedure(Action<LabelData> onAdd) {
                this.onAdd = onAdd;
                LabelsDatabase.onAdd += onAdd;
            }
            public OnAddProcedure(string group) {
                cachedGroupId = LabelGroupBindings.GetId(group);
                this.onAdd = (d) => d.AddGroup(cachedGroupId);
                LabelsDatabase.onAdd += onAdd;
            }
            public OnAddProcedure(System.Type type) {
                if(!LabelTypeBindings.TryGet(type, out cachedTypeBinding))
                    return;
                this.onAdd = (d) => d.AddType(cachedTypeBinding.TypeId);
                LabelsDatabase.onAdd += onAdd;
            }
            public OnAddProcedure(params System.Type[] types) {
                cachedTypeBindings = new List<LabelTypeBinding>();
                foreach(var t in types)
                    if(LabelTypeBindings.TryGet(t, out cachedTypeBinding))
                        cachedTypeBindings.Add(cachedTypeBinding);

                this.onAdd = (d) => {
                    foreach(var t in cachedTypeBindings)
                        d.AddType(t.TypeId);
                };
                LabelsDatabase.onAdd += onAdd;
            }
            public void Dispose() {
                if(onAdd != null)
                    LabelsDatabase.onAdd -= onAdd;
            }
        }

        #endregion
    }
}
