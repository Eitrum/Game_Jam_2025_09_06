
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Debugging {
    public class CommandHelper {
        #region Variables

        private string command = string.Empty;

        private bool isValidParsing = false;
        private TMLNode parsed = new TMLNode();
        private Privilege privilege = Privilege.None;

        private List<Command> previousPossible = new List<Command>();
        private List<Command> possibleCommands = new List<Command>();
        private List<Command> validCommands = new List<Command>();

        #endregion

        #region Properties

        public bool IsValid => isValidParsing;
        public IReadOnlyList<Command> PossibleCommands {
            get {
                if(possibleCommands.Count == 0 && previousPossible.Count > 0)
                    return previousPossible;
                return possibleCommands;
            }
        }
        public IReadOnlyList<Command> ValidCommands => validCommands;
        public string Current {
            get => command;
            set {
                if(value != command) {
                    command = value;
                    Update();
                }
            }
        }

        public Privilege Privilege {
            get => privilege;
            set {
                if(privilege != value) {
                    privilege = value;
                    Update();
                }
            }
        }

        #endregion

        #region Constructor

        public CommandHelper() {
            privilege = Commands.DefaultPrivilage;
        }

        public CommandHelper(Privilege privilege) {
            this.privilege = privilege;
        }

        #endregion

        #region Methods

        public void Clear() {
            command = string.Empty;
            isValidParsing = false;
            possibleCommands.Clear();
        }

        public void Run() {
            Commands.Run(command, privilege);
        }

        public void Update() {
            if(string.IsNullOrEmpty(command)) {
                isValidParsing = false;
                return;
            }

            isValidParsing = CommandParser.TryParse(command, out parsed);
            if(!isValidParsing)
                return;
            Commands.FindPossibleCommands(parsed, possibleCommands, privilege);
            validCommands.Clear();
            if(possibleCommands.Count > 0) {
                previousPossible.Clear();
                previousPossible.AddRange(possibleCommands);
            }

            foreach(var p in possibleCommands)
                if(p.IsValid(parsed, privilege))
                    validCommands.Add(p);

        }

        #endregion
    }
}
