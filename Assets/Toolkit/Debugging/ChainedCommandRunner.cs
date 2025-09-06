using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit.IO;
using UnityEngine;

namespace Toolkit.Debugging {
    /// <summary>
    /// Utility class to handle multiple commands within one string.
    /// </summary>
    public class ChainedCommandRunner {
        #region Runnable Instance

        private class CCRCoroutine : MonoSingleton<CCRCoroutine> {
            protected override bool KeepAlive { get => true; set => base.KeepAlive = value; }
        }

        #endregion

        #region Variables

        private string rawCommands;
        public bool IsValid { get; private set; }
        private string[] commands;

        #endregion

        #region Properties

        public string RawCommands => rawCommands;
        public int Count => commands.Length;
        public string this[int index] => commands[index];

        #endregion

        #region Constructor

        public ChainedCommandRunner(string commands) {
            rawCommands = commands;
            IsValid = CommandParser.TryExtractChainedCommands(commands, out this.commands);
        }

        static ChainedCommandRunner() {
            Commands.Add("ccr stop", () => CCRCoroutine.Instance.StopAllCoroutines(), Privilege.Normal);
        }

        #endregion

        #region Run

        public void Run() {
            if(!IsValid)
                return;
            if(commands.Length == 0)
                return;
            if(commands.Length == 1)
                Commands.Run(commands[0]);
            else {
                if(!Application.isPlaying) {
                    Debug.LogWarning(this.FormatLog("Attempting to run multi-commands in-editor. Not supported"));
                    return;
                }
                CCRCoroutine.Instance.StartCoroutine(Runner());
            }
        }

        IEnumerator Runner() {
            foreach(var cmd in commands) {
                if(cmd.StartsWith("wait")) {
                    var timeSpan = cmd.AsSpan().Slice(4).Trim();
                    if(!float.TryParse(timeSpan, out float time)) {
                        Commands.PrintToConsole($"Failed to read wait time '{cmd}'");
                        continue;
                    }
                    yield return new WaitForSecondsRealtime(time);
                    continue;
                }
                if(cmd.Equals("next")) {
                    yield return null;
                    continue;
                }
                Commands.Run(cmd);
            }
            yield return null;
        }

        #endregion
    }
}
