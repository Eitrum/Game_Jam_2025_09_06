using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Debugging {
    public class Command {
        #region Variables

        private const string TAG = "[Toolkit.Debugging.Command] - ";

        public string BaseCommand { get; private set; }
        public MethodInfo Method { get; private set; }
        public List<Parameter> parameters = new List<Parameter>();

        public Privilege Privilege { get; private set; }

        public string FullCommandVisual { get; private set; }

        private Action<object[]> genericWrapper;

        #endregion

        #region Constructor

        public Command(string baseCommand, Action<object[]> genericWrapper, MethodInfo method, Privilege privilege = Privilege.Debug) {
            this.genericWrapper = genericWrapper;
            Method = method;
            try {
                // Begin Base Command Parse
                CommandParser.TryParse(baseCommand, out TMLNode node);
                this.BaseCommand = node.Children[0].GetString("value");

                // Add parameters from parsed
                for(int i = 1; i < node.Children.Count; i++)
                    this.parameters.Add(new Parameter(node.Children[i].GetString("value")));

                // Bind method parameters
                var mparam = Method.GetParameters();
                int paramIndex = 0;

                for(int i = 0; i < mparam.Length; i++) {
                    if(paramIndex >= this.parameters.Count) {
                        this.parameters.Add(new Parameter(mparam[i].Name, mparam[i].ParameterType));
                        paramIndex++;
                        continue;
                    }

                    if(this.parameters[paramIndex].PType == Parameter.ParamType.Fixed) { // Try next parameter
                        paramIndex++;
                        i--;
                        continue;
                    }

                    if(this.parameters[paramIndex].Name.Equals("?"))
                        this.parameters[paramIndex] = new Parameter(mparam[i].Name, mparam[i].ParameterType);
                    else
                        this.parameters[paramIndex].AddType(mparam[i].ParameterType);
                    paramIndex++;
                }

                FullCommandVisual = $"{BaseCommand} {string.Join(" ", this.parameters.Select(x => x.Printable))}";
                Privilege = privilege;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
            }
        }

        #endregion

        #region Modify

        public Command Modify(int parameterIndex, Action<Parameter> method) {
            method.Invoke(parameters[parameterIndex]);
            return this;
        }

        #endregion

        #region IsValid Check

        public bool IsValid(TMLNode parsed)
            => IsValid(parsed, Commands.DefaultPrivilage);

        public bool IsValid(TMLNode parsed, Privilege privilege) {
            if(privilege < this.Privilege)
                return false;
            var count = parsed.Children.Count - 1;
            if(count != parameters.Count)
                return false;

            if(!parsed.TryGetNode("command", out TMLNode commandNode) || !commandNode.TryGetString("value", out string baseCommand))
                return false;

            if(!this.BaseCommand.Equals(baseCommand))
                return false;

            for(int i = 0; i < count; i++) {
                if(!parameters[i].IsValid(parsed.Children[i + 1])) {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region IsPossible

        public bool IsParametersPossible(TMLNode parsed)
            => IsParametersPossible(parsed, Commands.DefaultPrivilage);

        public bool IsParametersPossible(TMLNode parsed, Privilege privilege) {
            if(privilege < this.Privilege)
                return false;
            var count = parsed.Children.Count - 1;
            // Unable to be this command as there are more paramters than this command has.
            if(count > parameters.Count) return false;

            for(int i = 0; i < count; i++) {
                if(!parameters[i].IsValid(parsed.Children[i + 1]))
                    return false;
            }

            return true;
        }

        #endregion

        #region Run

        public void Run(TMLNode parsed)
            => Run(parsed, Commands.DefaultPrivilage);

        public void Run(TMLNode parsed, Privilege privilege) {
            if(privilege < this.Privilege) {
                Debug.LogError(TAG + $"Unable to run command due to '{Privilege}' privilage level required. (Current: '{privilege}')");
                return;
            }

            if(!parsed.TryGetNode("command", out var cnode)) {
                Debug.LogError(TAG + "Unable to run command as it's missing command node");
                return;
            }

            var length = parameters.Count;
            if(parsed.Children.Count != length + 1) { // children has 1 extra for the base command
                Debug.LogError(TAG + $"Unable to run command due to miss-matching parameters");
                return;
            }

            int nonFixed = parameters.Count(x=>x.PType != Parameter.ParamType.Fixed);

            object[] ptemp = new object[nonFixed];
            int index = 0;
            for(int i = 0; i < length; i++) {
                if(parameters[i].PType == Parameter.ParamType.Fixed)
                    continue;

                var t = parameters[i].Type;
                ptemp[index++] = parameters[i].Parse(parsed.Children[i + 1]);
                //if(t.IsSubclassOf(typeof(CommandValue))) {
                //    ptemp[index++] = Commands.ConverterRegistry.toCommandValueConverter.TryGetValue(t, out var func) ? func(parsed.Children[i + 1]) : null;
                //}
                //else if(t.IsEnum) {
                //    ptemp[index++] = Enum.TryParse(t, parsed.Children[i + 1].GetString("value"), true, out object eval) ? eval : 0;
                //}
                //else
                //    ptemp[index++] = Commands.ConverterRegistry.toObjectConverter.TryGetValue(t, out var func) ? func(parsed.Children[i + 1].GetString("value")) : null;
            }

            try {
                genericWrapper(ptemp);
            }
            catch(Exception e) {
                Debug.LogException(e);
            }
        }

        #endregion

        #region Debugging

        public string GetRemainingFullName(int paramtersUsed) {
            return string.Join(' ', parameters.Skip(paramtersUsed).Select(x => x.Printable));
        }

        public Command Print() {
            Debug.Log(TAG + FullCommandVisual);
            return this;
        }

        #endregion
    }
}
