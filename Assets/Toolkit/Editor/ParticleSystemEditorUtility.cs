using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit
{
    public static class ParticleSystemEditorUtility
    {
        public const int BASE_PRIORITY = 10000;

        #region Scale Mode 

        [MenuItem("CONTEXT/ParticleSystem/Scale Mode/Hierarchy/Self Only", priority = BASE_PRIORITY + 1)]
        private static void SetSelectedParticleSystemScaleModeToHierarchy(MenuCommand command) {
            if(command.context is Component comp)
                SetScaleMode(comp, ParticleSystemScalingMode.Hierarchy, false);
        }

        [MenuItem("CONTEXT/ParticleSystem/Scale Mode/Hierarchy/Children", priority = BASE_PRIORITY + 1)]
        private static void SetSelectedParticleSystemScaleModeToHierarchyIncludingChildren(MenuCommand command) {
            if(command.context is Component comp)
                SetScaleMode(comp, ParticleSystemScalingMode.Hierarchy, true);
        }


        [MenuItem("CONTEXT/ParticleSystem/Scale Mode/Local/Self Only", priority = BASE_PRIORITY + 1)]
        private static void SetSelectedParticleSystemScaleModeToLocal(MenuCommand command) {
            if(command.context is Component comp)
                SetScaleMode(comp, ParticleSystemScalingMode.Local, false);
        }

        [MenuItem("CONTEXT/ParticleSystem/Scale Mode/Local/Children", priority = BASE_PRIORITY + 1)]
        private static void SetSelectedParticleSystemScaleModeToLocalIncludingChildren(MenuCommand command) {
            if(command.context is Component comp)
                SetScaleMode(comp, ParticleSystemScalingMode.Local, true);
        }


        [MenuItem("CONTEXT/ParticleSystem/Scale Mode/Shape/Self Only", priority = BASE_PRIORITY + 1)]
        private static void SetSelectedParticleSystemScaleModeToShape(MenuCommand command) {
            if(command.context is Component comp)
                SetScaleMode(comp, ParticleSystemScalingMode.Shape, false);
        }

        [MenuItem("CONTEXT/ParticleSystem/Scale Mode/Shape/Children", priority = BASE_PRIORITY + 1)]
        private static void SetSelectedParticleSystemScaleModeToShapeIncludingChildren(MenuCommand command) {
            if(command.context is Component comp)
                SetScaleMode(comp, ParticleSystemScalingMode.Shape, true);
        }


        public static void SetScaleMode<T>(T component, ParticleSystemScalingMode scalingMode, bool includeChildren = false) where T : Component {
            if(component == null) {
                Debug.LogError($"Unable to assign scaling mode to '{scalingMode}' as component provided is null");
                return;
            }
            var particles = includeChildren ? component.GetComponentsInChildren<ParticleSystem>() : component.GetComponents<ParticleSystem>();
            var scaleValue = scalingMode.ToInt();
            foreach(var ps in particles) {
                SerializedObject so = new SerializedObject(ps);
                var prop = so.FindProperty("scalingMode");
                prop.intValue = scaleValue;
                so.ApplyModifiedProperties();
                so.Dispose();
            }
        }

        #endregion

        #region Simulation Space

        [MenuItem("CONTEXT/ParticleSystem/Simulation Space/World/Self Only", priority = BASE_PRIORITY + 2)]
        private static void SetSelectedParticleSystemSimulationSpaceToHierarchy(MenuCommand command) {
            if(command.context is Component comp)
                SetSimulationSpace(comp, ParticleSystemSimulationSpace.World, false);
        }

        [MenuItem("CONTEXT/ParticleSystem/Simulation Space/World/Children", priority = BASE_PRIORITY + 2)]
        private static void SetSelectedParticleSystemSimulationSpaceToHierarchyIncludingChildren(MenuCommand command) {
            if(command.context is Component comp)
                SetSimulationSpace(comp, ParticleSystemSimulationSpace.World, true);
        }


        [MenuItem("CONTEXT/ParticleSystem/Simulation Space/Local/Self Only", priority = BASE_PRIORITY + 2)]
        private static void SetSelectedParticleSystemSimulationSpaceToLocal(MenuCommand command) {
            if(command.context is Component comp)
                SetSimulationSpace(comp, ParticleSystemSimulationSpace.Local, false);
        }

        [MenuItem("CONTEXT/ParticleSystem/Simulation Space/Local/Children", priority = BASE_PRIORITY + 2)]
        private static void SetSelectedParticleSystemSimulationSpaceToLocalIncludingChildren(MenuCommand command) {
            if(command.context is Component comp)
                SetSimulationSpace(comp, ParticleSystemSimulationSpace.Local, true);
        }


        public static void SetSimulationSpace<T>(T component, ParticleSystemSimulationSpace simulationSpace, bool includeChildren = false) where T : Component {
            if(component == null) {
                Debug.LogError($"Unable to assign simulation space to '{simulationSpace}' as component provided is null");
                return;
            }
            var particles = includeChildren ? component.GetComponentsInChildren<ParticleSystem>() : component.GetComponents<ParticleSystem>();
            var simValue = simulationSpace.ToInt();
            foreach(var ps in particles) {
                SerializedObject so = new SerializedObject(ps);
                var prop = so.FindProperty("simulationSpace");
                prop.intValue = simValue;
                so.ApplyModifiedProperties();
                so.Dispose();
            }
        }

        #endregion

        #region Looping

        [MenuItem("CONTEXT/ParticleSystem/Looping/Enable (Include Children)", priority = BASE_PRIORITY + 3)]
        private static void SetSelectedParticleSystemLoopingEnabled(MenuCommand command) {
            if(command.context is Component comp)
                SetLoopingEnabled(comp, true, true);
        }

        [MenuItem("CONTEXT/ParticleSystem/Looping/Disable (Include Children)", priority = BASE_PRIORITY + 3)]
        private static void SetSelectedParticleSystemLoopingDisabled(MenuCommand command) {
            if(command.context is Component comp)
                SetLoopingEnabled(comp, false, true);
        }

        public static void SetLoopingEnabled<T>(T component, bool loopEnabled, bool includeChildren = false) where T : Component {
            if(component == null) {
                Debug.LogError($"Unable to assign looping to '{loopEnabled}' as component provided is null");
                return;
            }
            var particles = includeChildren ? component.GetComponentsInChildren<ParticleSystem>() : component.GetComponents<ParticleSystem>();
            foreach(var ps in particles) {
                SerializedObject so = new SerializedObject(ps);
                var prop = so.FindProperty("looping");
                prop.boolValue = loopEnabled;
                so.ApplyModifiedProperties();
                so.Dispose();
            }
        }

        #endregion
    }
}
