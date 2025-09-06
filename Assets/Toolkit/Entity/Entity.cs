using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit
{
    [AddComponentMenu("Toolkit/Entity")]
    public sealed class Entity : BaseComponent
    {
        #region Variables

        [SerializeField] private string entityName = "";
        [SerializeField] private EntityType type = EntityType.Uncategorized;

        private Rigidbody bodyReference;
        private Collider colliderReference;
        private AudioSource audioSourceReference;

        private static Dictionary<EntityType, List<Entity>> entities = new Dictionary<EntityType, List<Entity>>();
        public static event OnEntityCallback OnEntityAdded;
        public static event OnEntityCallback OnEntityRemoved;

        #endregion

        #region Properties

        public EntityType Type => type;
        public string EntityName {
            get => string.IsNullOrEmpty(entityName) ? name : entityName;
            set => entityName = value;
        }

        public Rigidbody Body {
            get {
                if(bodyReference == null) {
                    bodyReference = GetComponent<Rigidbody>();
                }
                return bodyReference;
            }
        }

        public Collider Collider {
            get {
                if(colliderReference == null) {
                    colliderReference = GetComponent<Collider>();
                }
                return colliderReference;
            }
        }

        public AudioSource AudioSource {
            get {
                if(audioSourceReference == null) {
                    audioSourceReference = GetComponent<AudioSource>();
                }
                return audioSourceReference;
            }
        }

        #endregion

        #region Initialization

        static Entity() {
            var entityType = typeof(EntityType);
            var values = System.Enum.GetValues(entityType);
            foreach(var val in values) {
                var type = (EntityType)val;
                entities.Add(type, new List<Entity>());
            }
        }

        #endregion

        #region Unity Methods

        private void OnEnable() {
            entities[type]?.Add(this);
            OnEntityAdded?.Invoke(type, this);
        }

        private void OnDisable() {
            entities[type]?.Remove(this);
            OnEntityRemoved?.Invoke(type, this);
        }

        #endregion

        #region Finding

        public static IReadOnlyList<Entity> GetEntities(EntityType type) => entities[type];
        public static IEnumerable<Entity> GetEntities(string name) => entities.SelectMany(x => x.Value).Where(x => x.EntityName == name);
        public static IEnumerable<Entity> GetEntities(EntityType type, string name) => entities[type].Where(x => x.EntityName == name);

        #endregion
    }

    public delegate void OnEntityCallback(EntityType type, Entity entity);
}
