using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.TurnEngine {
    public static class TurnManager {

        #region Variables

        private static TurnSystem.Instance instance;
        private static OnRoundPassedCallback onRoundPassed;
        private static OnNewTurnDelegate onNewTurn;
        private static OnEndTurnDelegate onEndTurn;
        private static void onNewTurnInstanceCallback(ITurn turn) => onNewTurn?.Invoke(turn);
        private static void onEndTurnInstanceCallback(ITurn turn) => onEndTurn?.Invoke(turn);
        private static void onRoundPassedCallback(int round) => onRoundPassed?.Invoke(round);

        #endregion

        #region Properties

        public static int Round => instance?.Round ?? -1;
        public static ITurn CurrentTurn => instance?.Turns.FirstOrDefault() ?? null;
        public static IReadOnlyList<ITurn> Turns => instance?.Turns;
        public static bool HasEncounter => instance != null && !instance.IsDestroyed;
        public static TurnSystem.Instance Instance => instance;
        public static event OnRoundPassedCallback OnRoundPassed { add => onRoundPassed += value; remove => onRoundPassed -= value; }
        public static event OnNewTurnDelegate OnNewTurn { add => onNewTurn += value; remove => onNewTurn -= value; }
        public static event OnEndTurnDelegate OnEndTurn { add => onEndTurn += value; remove => onEndTurn -= value; }

        #endregion

        #region New Encounter

        public static void NewEncounter() => NewEncounter(TurnMode.All);

        public static void NewEncounter(TurnMode mode) {
            if(HasEncounter) {
                instance.OnNewTurn -= onNewTurnInstanceCallback;
                instance.OnEndTurn -= onEndTurnInstanceCallback;
                instance.OnRoundPassed -= onRoundPassedCallback;
                TurnSystem.RemoveInstance(instance);
            }
            instance = TurnSystem.CreateInstance(mode);
            instance.OnNewTurn += onNewTurnInstanceCallback;
            instance.OnEndTurn += onEndTurnInstanceCallback;
            instance.OnRoundPassed += onRoundPassedCallback;
        }

        public static void NewEncounter(TurnMode mode, IEnumerable<ITurn> turns) {
            if(HasEncounter) {
                instance.OnNewTurn -= onNewTurnInstanceCallback;
                instance.OnEndTurn -= onEndTurnInstanceCallback;
                instance.OnRoundPassed -= onRoundPassedCallback;
                TurnSystem.RemoveInstance(instance);
            }
            instance = TurnSystem.CreateInstance(mode, turns);
            instance.OnNewTurn += onNewTurnInstanceCallback;
            instance.OnEndTurn += onEndTurnInstanceCallback;
            instance.OnRoundPassed += onRoundPassedCallback;
        }

        public static void NewEncounter(TurnMode mode, IReadOnlyList<ITurn> turns) {
            if(HasEncounter) {
                instance.OnNewTurn -= onNewTurnInstanceCallback;
                instance.OnEndTurn -= onEndTurnInstanceCallback;
                instance.OnRoundPassed -= onRoundPassedCallback;
                TurnSystem.RemoveInstance(instance);
            }
            instance = TurnSystem.CreateInstance(mode, turns);
            instance.OnNewTurn += onNewTurnInstanceCallback;
            instance.OnEndTurn += onEndTurnInstanceCallback;
            instance.OnRoundPassed += onRoundPassedCallback;
        }

        #endregion

        #region Remove Encounter

        /// <summary>
        /// Just removes the active turns but leaves it active.
        /// </summary>
        public static void ClearEncounter() {
            if(HasEncounter) {
                instance.Clear();
            }
        }

        /// <summary>
        /// Completely destroyes the encounter and removes it.
        /// </summary>
        public static void DestroyEncounter() {
            if(HasEncounter) {
                instance.Clear(true);
                instance.Destroy();
                instance.OnNewTurn -= onNewTurnInstanceCallback;
                instance.OnEndTurn -= onEndTurnInstanceCallback;
                instance.OnRoundPassed -= onRoundPassedCallback;
                instance = null;
            }
        }

        #endregion

        #region Update / Next

        public static void UpdateEncounter() {
            if(!HasEncounter) {
                throw new System.Exception("Can't update encounter as no encounter is active");
            }
            if(instance.HasUpdate)
                Debug.LogWarning("Attempting to update the encounter when automatic updates are enabled");
            else
                instance.Update();
        }

        public static void NextRound() {
            if(!HasEncounter) {
                throw new System.Exception("Can't force next round in encounter as no encounter is active");
            }
            instance.NextRound();
        }

        public static void SetTurn(ITurn turn) {
            if(!HasEncounter) {
                throw new System.Exception("Can't force turn round in encounter as no encounter is active");
            }
            instance.SetTurn(turn);
        }

        public static void SkipTurns(int turns) {
            if(!HasEncounter) {
                throw new System.Exception("Can't force skip round in encounter as no encounter is active");
            }
            instance.Skip(turns);
        }

        #endregion

        #region Add / Remove Turns

        public static void InsertTurn(int index, ITurn turn) {
            if(!HasEncounter) {
                throw new System.Exception("Can't insert a turn to encounter as no encounter is active");
            }
            instance.Insert(index, turn);
        }

        public static void InsertTurnAfter(ITurn newTurn, ITurn afterThisTurn) {
            if(!HasEncounter) {
                throw new System.Exception("Can't insert a turn to encounter as no encounter is active");
            }
            var turns = Turns;
            for(int i = turns.Count - 1; i >= 0; i--) {
                if(turns[i] == afterThisTurn) {
                    instance.Insert(i + 1, newTurn);
                    return;
                }
            }
            Debug.LogWarning("Could not find requested turn to put in after, adding it last");
            instance.Add(newTurn, false);
        }

        public static void InsertTurnBefore(ITurn newTurn, ITurn beforeThisTurn) {
            if(!HasEncounter) {
                throw new System.Exception("Can't insert a turn to encounter as no encounter is active");
            }
            var turns = Turns;
            for(int i = turns.Count - 1; i >= 0; i--) {
                if(turns[i] == beforeThisTurn) {
                    instance.Insert(i, newTurn);
                    return;
                }
            }
            Debug.LogWarning("Could not find requested turn to put in before, adding it last");
            instance.Add(newTurn, false);
        }

        public static void AddTurn(ITurn turn, bool beforeRoundIndicator = true) {
            if(!HasEncounter) {
                throw new System.Exception("Can't add a turn to encounter as no encounter is active");
            }
            instance.Add(turn, beforeRoundIndicator);
        }

        public static void RemoveTurn(ITurn turn) {
            if(!HasEncounter) {
                throw new System.Exception("Can't remove a turn from encounter as no encounter is active");
            }
            instance.Remove(turn);
        }

        #endregion

        #region Utility

        public static T CurrentTurnAs<T>() where T : ITurn {
            var t = CurrentTurn;
            if(t != null && t is T val)
                return val;
            return default;
        }

        public static T FindTurn<T>(System.Func<T, bool> function) where T : ITurn {
            foreach(var t in Turns) {
                if(t is T obj && function(obj)) {
                    return obj;
                }
            }
            return default;
        }

        #endregion
    }
}
