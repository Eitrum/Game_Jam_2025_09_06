using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Debugging {

    public class Parameter {

        public enum ParamType {
            None,
            Fixed,
            CommandValue,
            CommandValueEnum,
            Value,
            Enum,
        }

        #region Variables

        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public Type Type { get; protected set; }
        public ParamType PType { get; protected set; }

        protected Action<string, List<string>> filter;
        protected Func<string, bool> isValid;

        #endregion

        #region Properties

        public string Printable {
            get {
                switch(PType) {
                    case ParamType.CommandValueEnum:
                    case ParamType.Enum:
                        if(FastEnum.GetValueCount(Type) < 8) {
                            return $"({string.Join("|", FastEnum.GetNames(Type))})";
                        }
                        break;
                    case ParamType.Fixed:
                        return Name;
                }
                return $"<{Name}>";
            }
        }

        public string TypeName {
            get {
                switch(PType) {
                    case ParamType.CommandValue:
                    case ParamType.CommandValueEnum:
                        return $"CommandValue<{Type.Name}>";
                    case ParamType.Fixed:
                        return Name;
                }
                return Type.Name;
            }
        }

        public static Parameter FloatValue => new Parameter("value").AddIsValid((s) => float.TryParse(s, out float val));
        public static Parameter StringName => new Parameter("name");

        public static Parameter XValue => new Parameter("x").AddIsValid((s) => float.TryParse(s, out float val));
        public static Parameter YValue => new Parameter("y").AddIsValid((s) => float.TryParse(s, out float val));
        public static Parameter ZValue => new Parameter("z").AddIsValid((s) => float.TryParse(s, out float val));

        #endregion

        #region Constructor

        public Parameter(string name) {
            this.Name = name;
            if(name.StartsWith("<") && name.EndsWith(">")) {
                PType = ParamType.None;
                this.Name = this.Name.Trim('<', '>');
            }
            else
                PType = ParamType.Fixed;
        }

        public Parameter(string name, Type type) {
            this.Name = name;
            AddType(type);
        }

        #endregion

        #region Add Methods

        public Parameter AddDescription(string description) {
            this.Description = description;
            return this;
        }

        public Parameter AddType(Type type) {
            this.Type = type;
            var IsCommandValue = this.Type.IsSubclassOf(typeof(CommandValue));
            if(IsCommandValue)
                Type = Type.GenericTypeArguments[0];
            var IsEnum = Type.IsEnum;

            if(IsCommandValue && IsEnum) {
                PType = ParamType.CommandValueEnum;
            }
            else if(IsCommandValue) {
                PType = ParamType.CommandValue;
            }
            else if(IsEnum) {
                PType = ParamType.Enum;
            }
            else {
                PType = ParamType.Value;
            }

            return this;
        }

        public Parameter AddFilter(Action<string, List<string>> filter) {
            this.filter += filter;
            return this;
        }

        public Parameter AddIsValid(Func<string, bool> validCheck) {
            this.isValid += validCheck;
            return this;
        }

        #endregion

        #region Methods

        public void Filter(string current, List<string> list) => filter?.Invoke(current, list);
        public bool IsValid(string input) => isValid?.Invoke(input) ?? true;
        public bool IsValid(TMLNode parameterNode) {
            try {
                switch(PType) {
                    case ParamType.CommandValue:
                    case ParamType.CommandValueEnum:
                        Commands.ConverterRegistry.toCommandValueConverter.TryGetValue(Type, out var converter);
                        var commandValue = converter.Invoke(parameterNode);
                        return commandValue.IsValid;
                    case ParamType.Enum: {
                            var rawValue = parameterNode.GetString("value");
                            if(int.TryParse(rawValue, out var value)) {
                                return false;
                            }
                            return Enum.TryParse(Type, rawValue, true, out object res);
                        }
                    case ParamType.Value:
                        return Commands.ConverterRegistry.TryConvert(Type, parameterNode.GetString("value"), out object outputValue);
                    case ParamType.Fixed:
                        return parameterNode.GetString("value") == Name;
                    default:
                        return false;
                }
            }
            catch {
                return false;
            }
        }

        public object Parse(TMLNode parsed) {
            try {
                switch(PType) {
                    case ParamType.CommandValue:
                    case ParamType.CommandValueEnum:
                        Commands.ConverterRegistry.toCommandValueConverter.TryGetValue(Type, out var converter);
                        return converter.Invoke(parsed);
                    case ParamType.Enum:
                        return Enum.TryParse(Type, parsed.GetString("value"), true, out object res) ? res : null;
                    case ParamType.Value:
                        return Commands.ConverterRegistry.TryConvert(Type, parsed.GetString("value"), out object outputValue) ? outputValue : null;
                    case ParamType.Fixed:
                        return parsed.GetString("value");
                    default:
                        return null;
                }
            }
            catch {
                return null;
            }
        }

        #endregion
    }
}
