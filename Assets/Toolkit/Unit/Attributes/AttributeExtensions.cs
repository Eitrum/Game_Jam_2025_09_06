using System.Collections.Generic;

namespace Toolkit.Unit {

    public static class AttributeExtensions {
        #region Add

        public static void Add(this IAttributes attributes, AttributeModifier modifier) {
            var stat = attributes.GetAttribute(modifier.AttributeType);
            stat.Add(modifier.ValueType, modifier.Value);
            attributes.SetAttribute(modifier.AttributeType, stat);
        }

        public static void Add(this IAttributes attributes, AttributeType attributeType, Stat.FieldValue fieldValue) {
            var stat = attributes.GetAttribute(attributeType);
            stat.Add(fieldValue.Type, fieldValue.Value);
            attributes.SetAttribute(attributeType, stat);
        }

        public static void Add(this IAttributes attributes, AttributeType attributeType, float value) {
            var stat = attributes.GetAttribute(attributeType);
            stat.AddBase(value);
            attributes.SetAttribute(attributeType, stat);
        }

        public static void Add(this IAttributes attributes, AttributeType attributeType, Stat stat) {
            var tstat = attributes.GetAttribute(attributeType);
            tstat += stat;
            attributes.SetAttribute(attributeType, tstat);
        }

        public static void Add(this IAttributes attributes, AttributeType attributeType, Stat.ValueType valueType, float value) {
            var stat = attributes.GetAttribute(attributeType);
            stat.Add(valueType, value);
            attributes.SetAttribute(attributeType, stat);
        }

        public static void AddAll(this IAttributes attributes, IReadOnlyList<AttributeModifier> modifiers) {
            foreach(var m in modifiers)
                attributes.Add(m);
        }

        #endregion

        #region Remove

        public static void Remove(this IAttributes attributes, AttributeModifier modifier) {
            var stat = attributes.GetAttribute(modifier.AttributeType);
            stat.Remove(modifier.ValueType, modifier.Value);
            attributes.SetAttribute(modifier.AttributeType, stat);
        }

        public static void Remove(this IAttributes attributes, AttributeType attributeType, Stat.FieldValue fieldValue) {
            var stat = attributes.GetAttribute(attributeType);
            stat.Remove(fieldValue.Type, fieldValue.Value);
            attributes.SetAttribute(attributeType, stat);
        }

        public static void Remove(this IAttributes attributes, AttributeType attributeType, float value) {
            var stat = attributes.GetAttribute(attributeType);
            stat.RemoveBase(value);
            attributes.SetAttribute(attributeType, stat);
        }

        public static void Remove(this IAttributes attributes, AttributeType attributeType, Stat stat) {
            var tstat = attributes.GetAttribute(attributeType);
            tstat -= stat;
            attributes.SetAttribute(attributeType, tstat);
        }

        public static void Remove(this IAttributes attributes, AttributeType attributeType, Stat.ValueType valueType, float value) {
            var stat = attributes.GetAttribute(attributeType);
            stat.Remove(valueType, value);
            attributes.SetAttribute(attributeType, stat);
        }

        public static void RemoveAll(this IAttributes attributes, IReadOnlyList<AttributeModifier> modifiers) {
            foreach(var m in modifiers)
                attributes.Remove(m);
        }

        #endregion

        #region Reset

        public static void Reset(this IAttributes attributes) {
            foreach(var t in FastEnum.GetValues<AttributeType>())
                attributes.SetAttribute(t, new Stat());
        }

        #endregion

        #region Get

        public static float GetAttribute(this IAttributes attributes, AttributeType type, Stat.ValueType valueType) {
            var stat = attributes.GetAttribute(type);
            return stat[valueType];
        }

        #endregion
    }
}
