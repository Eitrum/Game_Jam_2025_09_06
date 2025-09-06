using System;
using UnityEngine;

namespace Toolkit
{
    public enum MetricSuffix
    {
        Yocto = -24,
        Zepto = -21,
        Atto = -18,
        Femto = -15,
        Pico = -12,
        Nano = -9,
        Micro = -6,
        Milli = -3,
        Centi = -2,
        Deci = -1,

        None = 0,

        Deca = 1,
        Hecto = 2,
        Kilo = 3,
        Mega = 6,
        Giga = 9,
        Tera = 12,
        Peta = 15,
        Exa = 18,
        Zetta = 21,
        Yotta = 24
    }

    public static class MetricUtility
    {
        public static double GetDoubleMultiplier(this MetricSuffix suffix) {
            return Math.Pow(10d, (double)suffix);
        }

        public static float GetFloatMultiplier(this MetricSuffix suffix) {
            return Mathf.Pow(10f, (float)suffix);
        }

        public static string GetSuffixSymbol(this MetricSuffix suffix) {
            switch(suffix) {
                case MetricSuffix.Deca: return "da";
                case MetricSuffix.Hecto: return "h";
                case MetricSuffix.Kilo: return "k";
                case MetricSuffix.Mega: return "M";
                case MetricSuffix.Giga: return "G";
                case MetricSuffix.Tera: return "T";
                case MetricSuffix.Peta: return "P";
                case MetricSuffix.Exa: return "E";
                case MetricSuffix.Zetta: return "Z";
                case MetricSuffix.Yotta: return "Y";

                case MetricSuffix.Deci: return "d";
                case MetricSuffix.Centi: return "c";
                case MetricSuffix.Milli: return "m";
                case MetricSuffix.Micro: return "μ";
                case MetricSuffix.Nano: return "n";
                case MetricSuffix.Pico: return "p";
                case MetricSuffix.Femto: return "f";
                case MetricSuffix.Atto: return "a";
                case MetricSuffix.Zepto: return "z";
                case MetricSuffix.Yocto: return "y";
            }
            return "";
        }
    }
}
