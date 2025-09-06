using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Debugging {
    public abstract class CommandValue {
        public string RawValue { get; protected set; }
        public bool IsValid { get; protected set; }
    }

    public sealed class CommandValue<T> : CommandValue {
        public T Value { get; private set; }
        public TMLNode Data { get; private set; }

        private CommandValue() { }

        private CommandValue(T value) {
            this.Value = value;
            Data = new TMLNode();
            RawValue = value.ToString();
        }

        public CommandValue(TMLNode parameter) {
            try {
                RawValue = parameter.GetString("value");
                Data = parameter.GetNode("data") ?? new TMLNode("data");
                IsValid = Commands.Converter<T>.TryConvert(RawValue, out var val);
                Value = val;
            }
            catch(Exception e) {
                Debug.LogException(e);
            }
        }

        public static implicit operator T(CommandValue<T> value) => value.Value;
        public static implicit operator CommandValue<T>(T value) => new CommandValue<T>(value);
    }
}
