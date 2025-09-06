using System.Collections.Generic;
using System.Linq;
using Toolkit.IO;

namespace Toolkit.Serializable
{
    public class Block
    {
        #region Variables

        private Dictionary<int, Variable> variables = new Dictionary<int, Variable>();
        private Dictionary<int, Block> blocks = new Dictionary<int, Block>();
        private Dictionary<int, ArrayBlock> arrays = new Dictionary<int, ArrayBlock>();

        private string name = "";
        private int id = 0;

        #endregion

        #region Properties

        public string Name => name;
        public int Id => id;

        #endregion

        #region Constructor

        internal Block() { }
        public Block(string name) {
            this.name = name;
            this.id = name.GetHash32();
        }
        public Block(string name, ISerializableObject serializableObject) {
            this.name = name;
            this.id = name.GetHash32();
            serializableObject.Save(this);
        }

        public Block(int id, ISerializableObject serializableObject) {
            this.name = id.ToString();
            this.id = id;
            serializableObject.Save(this);
        }

        #endregion

        #region Variable

        public void Set<T>(string name, T value) {
            var key = name.GetHash32();
            if(variables.TryGetValue(key, out Variable variable)) {
                variable.Value = value;
            }
            else {
                var newVar = new Variable(name, key, BlockSerializer.GetTypeId<T>(), value);
                variables.Add(key, newVar);
            }
        }

        public T Get<T>(string name, T defaultValue = default) {
            if(variables.TryGetValue(name.GetHash32(), out Variable var) && var.Type == BlockSerializer.GetTypeId<T>()) {
                return (T)var.Value;
            }
            return defaultValue;
        }

        #endregion

        #region Block

        public Block SetBlock(string name) {
            var key = name.GetHash32();
            if(blocks.TryGetValue(key, out Block block)) {
                return block;
            }
            var b = new Block(name);
            blocks.Add(key, b);
            return b;
        }

        public void SetBlock<T>(string name, T serializableObject) where T : ISerializableObject {
            var key = name.GetHash32();
            if(blocks.TryGetValue(key, out Block block)) {
                serializableObject.Save(block);
            }
            else {
                var b = new Block(name);
                serializableObject.Save(b);
                blocks.Add(key, b);
            }
        }

        public void GetBlock<T>(string name, T serializableObject) where T : ISerializableObject {
            if(blocks.TryGetValue(name.GetHash32(), out Block block)) {
                serializableObject.Load(block);
            }
            else {
                var b = new Block(name);
                serializableObject.Load(b);
            }
        }

        public Block GetBlock(string name) {
            var key = name.GetHash32();
            if(blocks.TryGetValue(key, out Block block))
                return block;

            var b = new Block(name);
            blocks.Add(key, b);
            return b;
        }

        #endregion

        #region Array

        public void SetArray<T>(string name, IReadOnlyList<T> array) {
            var key = name.GetHash32();
            if(arrays.TryGetValue(key, out ArrayBlock arrayBlock)) {
                arrayBlock.Set(array);
            }
            else {
                var newArrayBlock = new ArrayBlock(name);
                newArrayBlock.Set(array);
                arrays.Add(key, newArrayBlock);
            }
        }

        public void GetArray<T>(string name, IList<T> array, bool clear = true) {
            if(arrays.TryGetValue(name.GetHash32(), out ArrayBlock arrayBlock))
                arrayBlock.Get(array, clear);
            else if(clear && !array.IsReadOnly) {
                array.Clear();
            }
        }

        public Block[] GetArrayBlocks(string name) {
            if(arrays.TryGetValue(name.GetHash32(), out ArrayBlock arrayBlock))
                return arrayBlock.GetBlocks();
            return new Block[0];
        }

        #endregion

        #region Binary

        public void Write(IBuffer buffer) {
            buffer.Write(id);
            var index = buffer.Index;
            // Skip to implement length after
            buffer.Index = (index + 4);

            // Write the block
            buffer.Write((byte)variables.Count);
            foreach(var variable in variables) {
                buffer.Write(variable.Key);
                var type = variable.Value.Type;
                buffer.Write(type);
                BlockSerializer.Write(type, variable.Value.Value, buffer);
            }

            // Write blocks
            buffer.Write((byte)blocks.Count);
            foreach(var block in blocks) {
                block.Value.Write(buffer);
            }

            // Write Arrays
            buffer.Write((byte)arrays.Count);
            foreach(var array in arrays) {
                array.Value.Write(buffer);
            }

            // Add length to the block
            var length = buffer.Index - (index + 4);
            buffer.Insert(index, length);
        }

        public void Read(IBuffer buffer) {
            // Safety clear
            variables.Clear();
            blocks.Clear();
            arrays.Clear();

            // Read metadata
            id = buffer.ReadInt();
            var length = buffer.ReadInt();
            var index = buffer.Index;

            // Read variables
            var varLength = buffer.ReadByte();
            for(int i = 0; i < varLength; i++) {
                var key = buffer.ReadInt();
                var typeId = buffer.ReadByte();
                var value = BlockSerializer.Read(typeId, buffer);
                variables.Add(key, new Variable(key, typeId, value));
            }

            // Read blocks
            var bloLength = buffer.ReadByte();
            for(int i = 0; i < bloLength; i++) {
                var block = new Block();
                block.Read(buffer);
                blocks.Add(block.id, block);
            }

            // Read arrays
            var arrLength = buffer.ReadByte();
            for(int i = 0; i < arrLength; i++) {
                var array = new ArrayBlock();
                array.Read(buffer);
                arrays.Add(array.Id, array);
            }

            buffer.Index = (index + length);
        }

        #endregion

        #region XML



        #endregion
    }
}
