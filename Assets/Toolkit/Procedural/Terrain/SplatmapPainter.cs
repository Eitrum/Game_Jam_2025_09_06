using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    [CreateAssetMenu(fileName = "Painter", menuName = "Toolkit/Procedural/Terrain/Splatmap/Painter")]
    public class SplatmapPainter : ScriptableObject, IProceduralTerrainGeneration
    {
        public Utility.TerrainLayerCollection layerCollection;

        public Layer[] paintLayers;

        public bool Generate(Data data) {
            var width = data.SplatWidth;
            var height = data.SplatHeight;
            var modX = (float)data.Width / (float)width;
            var modY = (float)data.Height / (float)height;
            var splatmap = data.Splatmap;

            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    var hx = x * modX;
                    var hy = y * modY;
                    var h = data.GetBilinearHeight(hx, hy);
                    var s = data.GetSlopeDegrees(hx, hy);
                    SplatData splatData = new SplatData();

                    foreach(var l in paintLayers) {
                        var paintAmount = GetPaintPercentage(l.rules, h, s);
                        if(paintAmount > 0.01f) {
                            if(l.paintMode == PaintMode.Replace) {
                                splatData.ClearAll();
                            }
                            splatData.Add(l.layerIndex, paintAmount);
                        }
                    }

                    if(!splatData.HasPaint) {
                        Debug.LogError($"Failed to apply paint! {x}x {y}y");
                        foreach(var l in paintLayers) {
                            Debug.Log($"{l.layerIndex} {l.paintMode} {GetPaintPercentage(l.rules, h, s)}");
                        }
                        splatData.Add(0, 1);
                    }
                    splatmap[x, y] = splatData;
                }
            }
            return true;
        }

        #region Calculations

        public static float GetPaintPercentage(Rule[] rules, float height, float slope) {
            float value = 1f;
            foreach(var r in rules) {
                switch(r.mode) {
                    case Mode.Height:
                        if(!r.range.Contains(height))
                            value *= 0f;
                        break;
                    case Mode.Tilt:
                        if(!r.range.Contains(slope))
                            value *= 0f;
                        break;
                }
            }
            return value;
        }

        #endregion

        #region Classes

        [System.Serializable]
        public class Layer
        {
            public int layerIndex = 0;
            public PaintMode paintMode = PaintMode.Mix;
            public Rule[] rules;
        }

        [System.Serializable]
        public class Rule
        {
            public Mode mode = Mode.Height;
            public MinMax range = new MinMax(0, 90);
        }

        public enum PaintMode
        {
            Mix,
            Replace
        }

        public enum Mode
        {
            Height,
            Tilt,
        }

        #endregion
    }
}
