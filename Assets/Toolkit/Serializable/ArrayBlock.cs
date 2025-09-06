using System;
using System.Collections.Generic;
using System.Linq;
using Toolkit.IO;

namespace Toolkit.Serializable
{
    internal class ArrayBlock
    {
        #region Variables

        private List<Variable> variables = new List<Variable>();
        private List<Block> blocks = new List<Block>();

        private int length = 0;
        private byte type = 0;
        private string name = "";
        private int id = 0;
        private bool isBlocks = false;

        public int Id => id;
        public int Length => length;

        #endregion

        #region Constructor

        public ArrayBlock() { }

        public ArrayBlock(string name) {
            this.name = name;
            this.id = name.GetHash32();
        }

        #endregion

        #region Set/get

        public void Set<T>(IReadOnlyList<T> array) {
            isBlocks = typeof(ISerializableObject).IsAssignableFrom(typeof(T));
            length = array.Count;
            type = isBlocks ? (byte)0 : BlockSerializer.GetTypeId<T>();
            blocks.Clear();
            variables.Clear();
            if(isBlocks) {
                for(int i = 0, length = array.Count; i < length; i++) {
                    blocks.Add(new Block(i, array[i] as ISerializableObject));
                }
            }
            else {
                var typeId = BlockSerializer.GetTypeId<T>();
                for(int i = 0, length = array.Count; i < length; i++) {
                    variables.Add(new Variable(i, typeId, array[i]));
                }
            }
        }

        public void Get<T>(IList<T> array, bool clearArray = true) {
            var isArrayBlocks = typeof(ISerializableObject).IsAssignableFrom(typeof(T));
            if(isArrayBlocks != isBlocks)
                return;
            if(isBlocks)
                GetBlocks(array, clearArray);
            else
                GetVariables(array, clearArray);
        }

        public Block[] GetBlocks() {
            return blocks.ToArray();
        }

        private int GetVariables<T>(IList<T> array, bool clearArray) {
            if(clearArray && !array.IsReadOnly)
                array.Clear();
            if(length == 0 || typeof(T) != variables[0].Value.GetType())
                return 0;

            if(array.IsReadOnly) {
                var tLength = Math.Min(length, array.Count);
                for(int i = 0; i < tLength; i++) {
                    array[i] = (T)variables[i].Value;
                }
                return tLength;
            }
            else {
                for(int i = 0; i < length; i++) {
                    array.Add((T)variables[i].Value);
                }
                return length;
            }
        }

        private int GetBlocks<T>(IList<T> array, bool clearArray) {
            if(clearArray && !array.IsReadOnly)
                array.Clear();
            if(length == 0)
                return 0;

            if(array.IsReadOnly) {
                var tLength = Math.Min(length, array.Count);
                for(int i = 0; i < tLength; i++) {
                    if(array[i] == null) {
                        array[i] = Activator.CreateInstance<T>();
                    }
                    (array[i] as ISerializableObject).Load(blocks[i]);
                }
                return tLength;
            }
            else {
                array.Clear();
                for(int i = 0; i < length; i++) {
                    var t = Activator.CreateInstance<T>();
                    (t as ISerializableObject).Load(blocks[i]);
                    array.Add(t);
                }
                return length;
            }
        }

        #endregion

        #region Binary

        public void Write(IBuffer buffer) {
            // Write metadata
            buffer.Write(id);
            buffer.Write(type);
            var index = buffer.Index;
            buffer.Index = (index + 4);

            // Write content
            buffer.Write(length);
            if(isBlocks) {
                foreach(var block in blocks) {
                    block.Write(buffer);
                }
            }
            else {
                for(int i = 0; i < length; i++) {
                    BlockSerializer.Write(type, variables[i].Value, buffer);
                }
            }

            // Insert length to metadata
            var tLength = buffer.Index - (index + 4);
            buffer.Insert(index, tLength);
        }

        public void Read(IBuffer buffer) {
            // Safety clear
            blocks.Clear();
            variables.Clear();

            id = buffer.ReadInt();
            type = buffer.ReadByte();
            isBlocks = type == 0;
            var tlength = buffer.ReadInt();
            var index = buffer.Index;

            length = buffer.ReadInt();
            if(isBlocks) {
                for(int i = 0; i < length; i++) {
                    var block = new Block();
                    block.Read(buffer);
                    blocks.Add(block);
                }
            }
            else {
                for(int i = 0; i < length; i++) {
                    variables.Add(new Variable(i, type, BlockSerializer.Read(type, buffer)));
                }
            }
            // Safety set index
            buffer.Index = (index + length);
        }

        #endregion
    }
}
