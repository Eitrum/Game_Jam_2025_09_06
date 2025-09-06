using System;
using System.Collections.Generic;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Serializable
{
    public static class BlockSerializer
    {
        private static Dictionary<byte, Converter> converters = new Dictionary<byte, Converter>();
        private static Dictionary<byte, Type> idToType = new Dictionary<byte, Type>();
        private static Dictionary<Type, byte> typeToId = new Dictionary<Type, byte>();

        static BlockSerializer() {
            Register<byte>((o, b) => b.Write((byte)o), (b) => b.ReadByte());
            Register<sbyte>((o, b) => b.Write((sbyte)o), (b) => b.ReadSByte());
            Register<bool>((o, b) => b.Write((bool)o), (b) => b.ReadBool());
            Register<short>((o, b) => b.Write((short)o), (b) => b.ReadShort());
            Register<ushort>((o, b) => b.Write((ushort)o), (b) => b.ReadUShort());
            Register<uint>((o, b) => b.Write((uint)o), (b) => b.ReadUInt());
            Register<int>((o, b) => b.Write((int)o), (b) => b.ReadInt());
            Register<ulong>((o, b) => b.Write((ulong)o), (b) => b.ReadULong());
            Register<long>((o, b) => b.Write((long)o), (b) => b.ReadLong());
            Register<float>((o, b) => b.Write((float)o), (b) => b.ReadFloat());
            Register<double>((o, b) => b.Write((double)o), (b) => b.ReadDouble());
            Register<decimal>((o, b) => b.Write((decimal)o), (b) => b.ReadDouble());
            Register<char>((o, b) => b.Write((char)o), (b) => b.ReadChar());
            Register<Color>((o, b) => b.Write((Color)o), (b) => b.ReadColor());
            Register<Color32>((o, b) => b.Write((Color32)o), (b) => b.ReadColor32());
            Register<Vector2>((o, b) => b.Write((Vector2)o), (b) => b.ReadVector2());
            Register<Vector3>((o, b) => b.Write((Vector3)o), (b) => b.ReadVector3());
            Register<Vector4>((o, b) => b.Write((Vector4)o), (b) => b.ReadVector4());
            Register<Quaternion>((o, b) => b.Write((Quaternion)o), (b) => b.ReadQuaternion());
            Register<string>((o, b) => b.Write((string)o), (b) => b.ReadString());
        }

        public static void SetStringEncodingType(IO.EncodingType encoding) {
            var id = typeToId[typeof(string)];
            converters[id] = new Converter((o, b) => b.Write((string)o, encoding), (b) => b.ReadString(encoding));
        }

        public static object Read(byte typeId, IBuffer buffer) {
            return converters[typeId].read(buffer);
        }

        public static void Write(byte typeId, object obj, IBuffer buffer) {
            converters[typeId].write(obj, buffer);
        }

        public static void Register<T>(WriteCallback write, ReadCallback read) => Register<T>((byte)(idToType.Count + 1), write, read);

        public static void Register<T>(byte typeId, WriteCallback write, ReadCallback read)
            => Register<T>(typeId, new Converter(write, read));

        public static void Register<T>(byte typeId, Converter converter) {
            var t = typeof(T);
            if(idToType.ContainsKey(typeId)) {
                throw new Exception($"Can't register type {typeof(T)} with id {typeId} as it is already in use");
            }
            idToType.Add(typeId, t);
            typeToId.Add(t, typeId);
            converters.Add(typeId, converter);
        }

        public static byte GetTypeId<T>() {
            return typeToId[typeof(T)];
        }

        public static Type GetTypeFromId(byte id) {
            return idToType[id];
        }


        public class Converter
        {
            public WriteCallback write;
            public ReadCallback read;

            public Converter(WriteCallback write, ReadCallback read) {
                this.write = write;
                this.read = read;
            }
        }

        public delegate void WriteCallback(object obj, IBuffer buffer);
        public delegate object ReadCallback(IBuffer buffer);
    }
}
