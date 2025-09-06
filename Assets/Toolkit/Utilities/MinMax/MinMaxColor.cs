using System;
using UnityEngine;

namespace Toolkit
{
    [System.Serializable]
    public struct MinMaxColor
    {
        public Color min;
        public Color max;

        public Color Random => Color.LerpUnclamped(min, max, UnityEngine.Random.value);

        public MinMaxColor(Color min, Color max) {
            this.min = min;
            this.max = max;
        }

        public Color Evaluate(float t) => Color.LerpUnclamped(min, max, t);


        public override string ToString() => $"([{(int)(min.r * 255)}r,{(int)(min.g * 255)}g,{(int)(min.b * 255)}b,{(int)(min.a * 255)}a] -> [{(int)(max.r * 255)}r,{(int)(max.g * 255)}g,{(int)(max.b * 255)}b,{(int)(max.a * 255)}a])";
    }
}
