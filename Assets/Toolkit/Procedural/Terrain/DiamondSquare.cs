using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    [CreateAssetMenu(fileName = "Diamond Square Generator", menuName = "Toolkit/Procedural/Terrain/Diamond Square")]
    public class DiamondSquare : ScriptableObject, IProceduralTerrainGeneration
    {
        #region Variables

        [SerializeField] private MinMax heightRange = new MinMax(0, 200);
        [SerializeField] private AnimationCurve heightWeight = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Header("Points")]
        [SerializeField, Range(0f, 5f)] private float smoothness = 2f;
        [SerializeField, Min(1)] private int startingDepthPer4096 = 4;

        #endregion

        #region Methods

        public bool Generate(Data data) {
            Generate(data.Heightmap, data.Random);
            return true;
        }

        public float[,] Generate(int size)
            => Generate(size, size, Toolkit.Mathematics.Random.Int);

        public float[,] Generate(int width, int height)
            => Generate(width, height, Toolkit.Mathematics.Random.Int);

        public float[,] Generate(int width, int height, int seed) {
            var random = new System.Random(seed);
            float[,] array = new float[width, height];
            Generate(array, random);
            return array;
        }

        public void Generate(float[,] array, System.Random random) {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            // Generate Plots
            var depths = (float)Mathf.Max(width.GetHighestFlagIndex(), height.GetHighestFlagIndex()) + 1f;
            int startingDepth = Mathf.CeilToInt((Mathf.Max(width, height) / 4096f) * startingDepthPer4096);
            Debug.Log($"Starint Depth ({width}w {height}h {startingDepth}) Depths {depths}");
            for(int i = 0; i < startingDepth; i++) {
                var step = 1 << (int)(depths - i - 1f);
                var halfStep = step / 2;
                for(int x = 0; x < width; x += step) {
                    for(int y = 0; y < height; y += step) {
                        if(array[x, y] <= 0f) {
                            array[x, y] = Mathf.Max(Mathf.Epsilon, heightRange.Evaluate(heightWeight.Evaluate(random.NextFloat())));
                        }
                    }
                }
                for(int x = halfStep; x < width; x += step) {
                    for(int y = halfStep; y < height; y += step) {
                        if(array[x, y] <= 0f) {
                            array[x, y] = Mathf.Max(Mathf.Epsilon, heightRange.Evaluate(heightWeight.Evaluate(random.NextFloat())));
                        }
                    }
                }
            }

            for(int i = startingDepth; i < depths; i++) {
                var depthStrength = (2f / smoothness) / Mathf.Pow(2, i);
                var step = 1 << (int)(depths - i - 1f);
                var halfStep = Mathf.Max(1, step / 2);
                Debug.Log($"Depth ({i}) Normalized Depth ({i}) Step ({step})");
                for(int x = 0; x < width; x += step) {
                    for(int y = 0; y < height; y += step) {
                        if(array[x, y] <= 0f) {
                            array[x, y] = CalculateDiamond(array, x, y, width, height, step, depthStrength, random);
                        }
                    }
                }
                for(int x = halfStep; x < width; x += step) {
                    for(int y = halfStep; y < height; y += step) {
                        if(array[x, y] <= 0f) {
                            array[x, y] = CalculateSquare(array, x, y, width, height, halfStep, depthStrength, random);
                        }
                    }
                }
            }
        }

        #endregion

        #region Calculation

        private float CalculateDiamond(float[,] array, int x, int y, int width, int height, int step, float strength, System.Random random) {
            float res = 0f;
            float availablePoints = 0f;
            if(x - step >= 0) {
                var value = array[x - step, y];
                if(value > 0f) {
                    res += value;
                    availablePoints++;
                }
            }
            if(x + step < width) {
                var value = array[x + step, y];
                if(value > 0f) {
                    res += value;
                    availablePoints++;
                }
            }
            if(y - step >= 0) {
                var value = array[x, y - step];
                if(value > 0f) {
                    res += value;
                    availablePoints++;
                }
            }
            if(y + step < height) {
                var value = array[x, y + step];
                if(value > 0f) {
                    res += value;
                    availablePoints++;
                }
            }
            if(availablePoints > 0f)
                return res / availablePoints + heightRange.Evaluate((random.NextFloat() - .5f) * strength);
            else {
                Debug.LogError($"Diamond to get values around {x}x {y}y with step{step} failed");
                return 0f;
            }
        }

        private float CalculateSquare(float[,] array, int x, int y, int width, int height, int halfStep, float strength, System.Random random) {
            float res = 0f;
            float availablePoints = 0f;
            if(x - halfStep >= 0 && y - halfStep >= 0) {
                var value = array[x - halfStep, y - halfStep];
                if(value > 0f) {
                    res += value;
                    availablePoints++;
                }
            }
            if(x + halfStep < width && y - halfStep >= 0) {
                var value = array[x + halfStep, y - halfStep];
                if(value > 0f) {
                    res += value;
                    availablePoints++;
                }
            }
            if(x - halfStep >= 0 && y + halfStep >= 0) {
                var value = array[x - halfStep, y + halfStep];
                if(value > 0f) {
                    res += value;
                    availablePoints++;
                }
            }
            if(x + halfStep < width && y + halfStep < height) {
                var value = array[x + halfStep, y + halfStep];
                if(value > 0f) {
                    res += value;
                    availablePoints++;
                }
            }
            if(availablePoints > 0f)
                return res / availablePoints + heightRange.Evaluate((random.NextFloat() - .5f) * strength);
            else {
                Debug.LogError($"Square to get values around {x}x {y}y with halfstep{halfStep} failed");
                return 0f;
            }
        }

        #endregion
    }
}
