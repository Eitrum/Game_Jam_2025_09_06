using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    [CreateAssetMenu(fileName = "Generation Preset", menuName = "Toolkit/Procedural/Terrain/Generation Preset")]
    public class GenerationPreset : ScriptableObject
    {
        #region Variables

        [SerializeField] private int seed = 0;
        [SerializeField, TypeFilter(typeof(IProceduralTerrainGeneration))] private UnityEngine.Object[] steps;

        #endregion

        #region Properties

        public int StepCount => steps.Length;
        public IProceduralTerrainGeneration[] Steps => steps.Select(x => x as IProceduralTerrainGeneration).ToArray();
        public IProceduralTerrainGeneration this[int index] => steps[index] as IProceduralTerrainGeneration;

        #endregion

        #region Generate

        public Data Generate(UnityEngine.Terrain terrain) {
            var data = new Data(terrain, seed);
            foreach(var g in steps) {
                if(g is IProceduralTerrainGeneration ptg)
                    ptg.Generate(data);
            }
            data.Apply(terrain);
            return data;
        }

        public Data Generate(UnityEngine.Terrain terrain, int seed) {
            var data = new Data(terrain, seed);
            foreach(var g in steps) {
                if(g is IProceduralTerrainGeneration ptg)
                    ptg.Generate(data);
            }
            data.Apply(terrain);
            return data;
        }

        public Data Generate(UnityEngine.Terrain terrain, bool applyToTerrain) {
            if(seed == 0) {
                seed = Toolkit.Mathematics.Random.Int;
            }
            var data = new Data(terrain, seed);
            foreach(var g in steps) {
                if(g is IProceduralTerrainGeneration ptg)
                    ptg.Generate(data);
            }
            if(applyToTerrain)
                data.Apply(terrain);
            return data;
        }

        public Data Generate(UnityEngine.Terrain terrain, int seed, bool applyToTerrain) {
            var data = new Data(terrain, seed);
            foreach(var g in steps) {
                if(g is IProceduralTerrainGeneration ptg)
                    ptg.Generate(data);
            }
            if(applyToTerrain)
                data.Apply(terrain);
            return data;
        }

        #endregion
    }
}
