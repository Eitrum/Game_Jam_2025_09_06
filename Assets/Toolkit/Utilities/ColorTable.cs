using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit {
    // Based on https://www.rapidtables.com/web/color/RGB_Color.html
    // Partial classes:
    //      - ColorTableRichTextTags.cs
    public static partial class ColorTable {
        #region Variables

        // Basic
        public static Color32 Clear => new Color32(0, 0, 0, 0);
        public static Color32 Black => new Color32(0, 0, 0, 255);
        public static Color32 White => new Color32(255, 255, 255, 255);
        public static Color32 Red => new Color32(255, 0, 0, 255);
        public static Color32 Lime => new Color32(0, 255, 0, 255);
        public static Color32 Blue => new Color32(0, 0, 255, 255);
        public static Color32 Yellow => new Color32(255, 255, 0, 255);
        public static Color32 Cyan => new Color32(0, 255, 255, 255);
        public static Color32 Aqua => new Color32(0, 255, 255, 255);
        public static Color32 Magenta => new Color32(255, 0, 255, 255);
        public static Color32 Fuchsia => new Color32(255, 0, 255, 255);
        public static Color32 Silver => new Color32(192, 192, 192, 255);
        public static Color32 Gray => new Color32(128, 128, 128, 255);
        public static Color32 Grey => new Color32(128, 128, 128, 255);
        public static Color32 Maroon => new Color32(128, 0, 0, 255);
        public static Color32 Olive => new Color32(128, 128, 0, 255);
        public static Color32 Green => new Color32(0, 128, 0, 255);
        public static Color32 Purple => new Color32(128, 0, 128, 255);
        public static Color32 Teal => new Color32(0, 128, 128, 255);
        public static Color32 Navy => new Color32(0, 0, 128, 255);

        // All
        //public static Color32 Maroon => new Color32(128, 0, 0, 255);
        public static Color32 DarkRed => new Color32(139, 0, 0, 255);
        public static Color32 Brown => new Color32(165, 42, 42, 255);
        public static Color32 Firebrick => new Color32(178, 34, 34, 255);
        public static Color32 Crimson => new Color32(220, 20, 60, 255);
        //public static Color32 Red => new Color32(255, 0, 0, 255);
        public static Color32 Tomato => new Color32(255, 99, 71, 255);
        public static Color32 Coral => new Color32(255, 127, 80, 255);
        public static Color32 IndianRed => new Color32(205, 92, 92, 255);
        public static Color32 LightCoral => new Color32(240, 128, 128, 255);
        public static Color32 DarkSalmon => new Color32(233, 150, 122, 255);
        public static Color32 Salmon => new Color32(250, 128, 114, 255);
        public static Color32 LightSalmon => new Color32(255, 160, 122, 255);
        public static Color32 OrangeRed => new Color32(255, 69, 0, 255);
        public static Color32 DarkOrange => new Color32(255, 140, 0, 255);
        public static Color32 Orange => new Color32(255, 165, 0, 255);
        public static Color32 Gold => new Color32(255, 215, 0, 255);
        public static Color32 DarkGoldenRod => new Color32(184, 134, 11, 255);
        public static Color32 GoldenRod => new Color32(218, 165, 32, 255);
        public static Color32 PaleGoldenRod => new Color32(238, 232, 170, 255);
        public static Color32 DarkKhaki => new Color32(189, 183, 107, 255);
        public static Color32 Khaki => new Color32(240, 230, 140, 255);
        //public static Color32 Olive => new Color32(128, 128, 0, 255);
        //public static Color32 Yellow => new Color32(255, 255, 0, 255);
        public static Color32 YellowGreen => new Color32(154, 205, 50, 255);
        public static Color32 DarkOliveGreen => new Color32(85, 107, 47, 255);
        public static Color32 OliveDrab => new Color32(107, 142, 35, 255);
        public static Color32 LawnGreen => new Color32(124, 252, 0, 255);
        public static Color32 ChartReuse => new Color32(127, 255, 0, 255);
        public static Color32 GreenYellow => new Color32(173, 255, 47, 255);
        public static Color32 DarkGreen => new Color32(0, 100, 0, 255);
        //public static Color32 Green => new Color32(0, 128, 0, 255);
        public static Color32 ForestGreen => new Color32(34, 139, 34, 255);
        //public static Color32 Lime => new Color32(0, 255, 0, 255);
        public static Color32 LimeGreen => new Color32(50, 205, 50, 255);
        public static Color32 LightGreen => new Color32(144, 238, 144, 255);
        public static Color32 PaleGreen => new Color32(152, 251, 152, 255);
        public static Color32 DarkSeaGreen => new Color32(143, 188, 143, 255);
        public static Color32 MediumSpringGreen => new Color32(0, 250, 154, 255);
        public static Color32 SpringGreen => new Color32(0, 255, 127, 255);
        public static Color32 SeaGreen => new Color32(46, 139, 87, 255);
        public static Color32 MediumAquaMarine => new Color32(102, 205, 170, 255);
        public static Color32 MediumSeaGreen => new Color32(60, 179, 113, 255);
        public static Color32 LightSeaGreen => new Color32(32, 178, 170, 255);
        public static Color32 DarkSlateGray => new Color32(47, 79, 79, 255);
        //public static Color32 Teal => new Color32(0, 128, 128, 255);
        public static Color32 DarkCyan => new Color32(0, 139, 139, 255);
        //public static Color32 Aqua => new Color32(0, 255, 255, 255);
        //public static Color32 Cyan => new Color32(0, 255, 255, 255);
        public static Color32 LightCyan => new Color32(224, 255, 255, 255);
        public static Color32 DarkTurquoise => new Color32(0, 206, 209, 255);
        public static Color32 Turquoise => new Color32(64, 224, 208, 255);
        public static Color32 MediumTurquoise => new Color32(72, 209, 204, 255);
        public static Color32 PaleTurquoise => new Color32(175, 238, 238, 255);
        public static Color32 AquaMarine => new Color32(127, 255, 212, 255);
        public static Color32 PowderBlue => new Color32(176, 224, 230, 255);
        public static Color32 CadetBlue => new Color32(95, 158, 160, 255);
        public static Color32 SteelBlue => new Color32(70, 130, 180, 255);
        public static Color32 CornFlowerBlue => new Color32(100, 149, 237, 255);
        public static Color32 DeepSkyBlue => new Color32(0, 191, 255, 255);
        public static Color32 DodgerBlue => new Color32(30, 144, 255, 255);
        public static Color32 LightBlue => new Color32(173, 216, 230, 255);
        public static Color32 SkyBlue => new Color32(135, 206, 235, 255);
        public static Color32 LightSkyBlue => new Color32(135, 206, 250, 255);
        public static Color32 MidnightBlue => new Color32(25, 25, 112, 255);
        //public static Color32 Navy => new Color32(0, 0, 128, 255);
        public static Color32 DarkBlue => new Color32(0, 0, 139, 255);
        public static Color32 MediumBlue => new Color32(0, 0, 205, 255);
        //public static Color32 Blue => new Color32(0, 0, 255, 255);
        public static Color32 RoyalBlue => new Color32(65, 105, 225, 255);
        public static Color32 BlueViolet => new Color32(138, 43, 226, 255);
        public static Color32 Indigo => new Color32(75, 0, 130, 255);
        public static Color32 DarkSlateBlue => new Color32(72, 61, 139, 255);
        public static Color32 SlateBlue => new Color32(106, 90, 205, 255);
        public static Color32 MediumSlateBlue => new Color32(123, 104, 238, 255);
        public static Color32 MediumPurple => new Color32(147, 112, 219, 255);
        public static Color32 DarkMagenta => new Color32(139, 0, 139, 255);
        public static Color32 DarkViolet => new Color32(148, 0, 211, 255);
        public static Color32 DarkOrchid => new Color32(153, 50, 204, 255);
        public static Color32 MediumOrchid => new Color32(186, 85, 211, 255);
        //public static Color32 Purple => new Color32(128, 0, 128, 255);
        public static Color32 Thistle => new Color32(216, 191, 216, 255);
        public static Color32 Plum => new Color32(221, 160, 221, 255);
        public static Color32 Violet => new Color32(238, 130, 238, 255);
        //public static Color32 Magenta/Fuchsia => new Color32(255,0,255,255);
        public static Color32 Orchid => new Color32(218, 112, 214, 255);
        public static Color32 MediumVioletRed => new Color32(199, 21, 133, 255);
        public static Color32 PaleVioletRed => new Color32(219, 112, 147, 255);
        public static Color32 DeepPink => new Color32(255, 20, 147, 255);
        public static Color32 HotPink => new Color32(255, 105, 180, 255);
        public static Color32 LightPink => new Color32(255, 182, 193, 255);
        public static Color32 Pink => new Color32(255, 192, 203, 255);
        public static Color32 AntiqueWhite => new Color32(250, 235, 215, 255);
        public static Color32 Beige => new Color32(245, 245, 220, 255);
        public static Color32 Bisque => new Color32(255, 228, 196, 255);
        public static Color32 BlanchedAlmond => new Color32(255, 235, 205, 255);
        public static Color32 Wheat => new Color32(245, 222, 179, 255);
        public static Color32 CornSilk => new Color32(255, 248, 220, 255);
        public static Color32 LemonChiffon => new Color32(255, 250, 205, 255);
        public static Color32 LightGoldenRodYellow => new Color32(250, 250, 210, 255);
        public static Color32 LightYellow => new Color32(255, 255, 224, 255);
        public static Color32 SaddleBrown => new Color32(139, 69, 19, 255);
        public static Color32 Sienna => new Color32(160, 82, 45, 255);
        public static Color32 Chocolate => new Color32(210, 105, 30, 255);
        public static Color32 Peru => new Color32(205, 133, 63, 255);
        public static Color32 SandyBrown => new Color32(244, 164, 96, 255);
        public static Color32 BurlyWood => new Color32(222, 184, 135, 255);
        public static Color32 Tan => new Color32(210, 180, 140, 255);
        public static Color32 RosyBrown => new Color32(188, 143, 143, 255);
        public static Color32 Moccasin => new Color32(255, 228, 181, 255);
        public static Color32 NavajoWhite => new Color32(255, 222, 173, 255);
        public static Color32 PeachPuff => new Color32(255, 218, 185, 255);
        public static Color32 MistyRose => new Color32(255, 228, 225, 255);
        public static Color32 LavenderBlush => new Color32(255, 240, 245, 255);
        public static Color32 Linen => new Color32(250, 240, 230, 255);
        public static Color32 OldLace => new Color32(253, 245, 230, 255);
        public static Color32 PapayaWhip => new Color32(255, 239, 213, 255);
        public static Color32 SeaShell => new Color32(255, 245, 238, 255);
        public static Color32 MintCream => new Color32(245, 255, 250, 255);
        public static Color32 SlateGray => new Color32(112, 128, 144, 255);
        public static Color32 LightSlateGray => new Color32(119, 136, 153, 255);
        public static Color32 LightSteelBlue => new Color32(176, 196, 222, 255);
        public static Color32 Lavender => new Color32(230, 230, 250, 255);
        public static Color32 FloralWhite => new Color32(255, 250, 240, 255);
        public static Color32 AliceBlue => new Color32(240, 248, 255, 255);
        public static Color32 GhostWhite => new Color32(248, 248, 255, 255);
        public static Color32 Honeydew => new Color32(240, 255, 240, 255);
        public static Color32 Ivory => new Color32(255, 255, 240, 255);
        public static Color32 Azure => new Color32(240, 255, 255, 255);
        public static Color32 Snow => new Color32(255, 250, 250, 255);
        //public static Color32 Black => new Color32(0, 0, 0, 255);
        public static Color32 DimGray => new Color32(105, 105, 105, 255);
        public static Color32 DimGrey => new Color32(105, 105, 105, 255);
        //public static Color32 Gray => new Color32(128,128,128,255);
        //public static Color32 Grey => new Color32(128,128,128,255);
        public static Color32 DarkGray => new Color32(169, 169, 169, 255);
        public static Color32 DarkGrey => new Color32(169, 169, 169, 255);
        //public static Color32 Silver => new Color32(192, 192, 192, 255);
        public static Color32 LightGrey => new Color32(211, 211, 211, 255);
        public static Color32 LightGray => new Color32(211, 211, 211, 255);
        public static Color32 Gainsboro => new Color32(220, 220, 220, 255);
        public static Color32 WhiteSmoke => new Color32(245, 245, 245, 255);
        //public static Color32 White => new Color32(255, 255, 255, 255);

        #endregion

        #region Get Color Extension

        public static Color32 ToColor32(this ColorTableType type) => GetColor(type);
        public static Color ToColor(this ColorTableType type) => GetColor(type);

        public static Color32 GetColor(ColorTableType color) {
            switch(color) {
                case ColorTableType.Transparent: return new Color32(0, 0, 0, 0);
                case ColorTableType.Maroon: return Maroon;
                case ColorTableType.DarkRed: return DarkRed;
                case ColorTableType.Brown: return Brown;
                case ColorTableType.Firebrick: return Firebrick;
                case ColorTableType.Crimson: return Crimson;
                case ColorTableType.Red: return Red;
                case ColorTableType.Tomato: return Tomato;
                case ColorTableType.Coral: return Coral;
                case ColorTableType.IndianRed: return IndianRed;
                case ColorTableType.LightCoral: return LightCoral;
                case ColorTableType.DarkSalmon: return DarkSalmon;
                case ColorTableType.Salmon: return Salmon;
                case ColorTableType.LightSalmon: return LightSalmon;
                case ColorTableType.OrangeRed: return OrangeRed;
                case ColorTableType.DarkOrange: return DarkOrange;
                case ColorTableType.Orange: return Orange;
                case ColorTableType.Gold: return Gold;
                case ColorTableType.DarkGoldenRod: return DarkGoldenRod;
                case ColorTableType.GoldenRod: return GoldenRod;
                case ColorTableType.PaleGoldenRod: return PaleGoldenRod;
                case ColorTableType.DarkKhaki: return DarkKhaki;
                case ColorTableType.Khaki: return Khaki;
                case ColorTableType.Olive: return Olive;
                case ColorTableType.Yellow: return Yellow;
                case ColorTableType.YellowGreen: return YellowGreen;
                case ColorTableType.DarkOliveGreen: return DarkOliveGreen;
                case ColorTableType.OliveDrab: return OliveDrab;
                case ColorTableType.LawnGreen: return LawnGreen;
                case ColorTableType.ChartReuse: return ChartReuse;
                case ColorTableType.GreenYellow: return GreenYellow;
                case ColorTableType.DarkGreen: return DarkGreen;
                case ColorTableType.Green: return Green;
                case ColorTableType.ForestGreen: return ForestGreen;
                case ColorTableType.Lime: return Lime;
                case ColorTableType.LimeGreen: return LimeGreen;
                case ColorTableType.LightGreen: return LightGreen;
                case ColorTableType.PaleGreen: return PaleGreen;
                case ColorTableType.DarkSeaGreen: return DarkSeaGreen;
                case ColorTableType.MediumSpringGreen: return MediumSpringGreen;
                case ColorTableType.SpringGreen: return SpringGreen;
                case ColorTableType.SeaGreen: return SeaGreen;
                case ColorTableType.MediumAquaMarine: return MediumAquaMarine;
                case ColorTableType.MediumSeaGreen: return MediumSeaGreen;
                case ColorTableType.LightSeaGreen: return LightSeaGreen;
                case ColorTableType.DarkSlateGray: return DarkSlateGray;
                case ColorTableType.Teal: return Teal;
                case ColorTableType.DarkCyan: return DarkCyan;
                case ColorTableType.Aqua: return Aqua;
                case ColorTableType.Cyan: return Cyan;
                case ColorTableType.LightCyan: return LightCyan;
                case ColorTableType.DarkTurquoise: return DarkTurquoise;
                case ColorTableType.Turquoise: return Turquoise;
                case ColorTableType.MediumTurquoise: return MediumTurquoise;
                case ColorTableType.PaleTurquoise: return PaleTurquoise;
                case ColorTableType.AquaMarine: return AquaMarine;
                case ColorTableType.PowderBlue: return PowderBlue;
                case ColorTableType.CadetBlue: return CadetBlue;
                case ColorTableType.SteelBlue: return SteelBlue;
                case ColorTableType.CornFlowerBlue: return CornFlowerBlue;
                case ColorTableType.DeepSkyBlue: return DeepSkyBlue;
                case ColorTableType.DodgerBlue: return DodgerBlue;
                case ColorTableType.LightBlue: return LightBlue;
                case ColorTableType.SkyBlue: return SkyBlue;
                case ColorTableType.LightSkyBlue: return LightSkyBlue;
                case ColorTableType.MidnightBlue: return MidnightBlue;
                case ColorTableType.Navy: return Navy;
                case ColorTableType.DarkBlue: return DarkBlue;
                case ColorTableType.MediumBlue: return MediumBlue;
                case ColorTableType.Blue: return Blue;
                case ColorTableType.RoyalBlue: return RoyalBlue;
                case ColorTableType.BlueViolet: return BlueViolet;
                case ColorTableType.Indigo: return Indigo;
                case ColorTableType.DarkSlateBlue: return DarkSlateBlue;
                case ColorTableType.SlateBlue: return SlateBlue;
                case ColorTableType.MediumSlateBlue: return MediumSlateBlue;
                case ColorTableType.MediumPurple: return MediumPurple;
                case ColorTableType.DarkMagenta: return DarkMagenta;
                case ColorTableType.DarkViolet: return DarkViolet;
                case ColorTableType.DarkOrchid: return DarkOrchid;
                case ColorTableType.MediumOrchid: return MediumOrchid;
                case ColorTableType.Purple: return Purple;
                case ColorTableType.Thistle: return Thistle;
                case ColorTableType.Plum: return Plum;
                case ColorTableType.Violet: return Violet;
                case ColorTableType.Magenta: return Magenta;
                case ColorTableType.Orchid: return Orchid;
                case ColorTableType.MediumVioletRed: return MediumVioletRed;
                case ColorTableType.PaleVioletRed: return PaleVioletRed;
                case ColorTableType.DeepPink: return DeepPink;
                case ColorTableType.HotPink: return HotPink;
                case ColorTableType.LightPink: return LightPink;
                case ColorTableType.Pink: return Pink;
                case ColorTableType.AntiqueWhite: return AntiqueWhite;
                case ColorTableType.Beige: return Beige;
                case ColorTableType.Bisque: return Bisque;
                case ColorTableType.BlanchedAlmond: return BlanchedAlmond;
                case ColorTableType.Wheat: return Wheat;
                case ColorTableType.CornSilk: return CornSilk;
                case ColorTableType.LemonChiffon: return LemonChiffon;
                case ColorTableType.LightGoldenRodYellow: return LightGoldenRodYellow;
                case ColorTableType.LightYellow: return LightYellow;
                case ColorTableType.SaddleBrown: return SaddleBrown;
                case ColorTableType.Sienna: return Sienna;
                case ColorTableType.Chocolate: return Chocolate;
                case ColorTableType.Peru: return Peru;
                case ColorTableType.SandyBrown: return SandyBrown;
                case ColorTableType.BurlyWood: return BurlyWood;
                case ColorTableType.Tan: return Tan;
                case ColorTableType.RosyBrown: return RosyBrown;
                case ColorTableType.Moccasin: return Moccasin;
                case ColorTableType.NavajoWhite: return NavajoWhite;
                case ColorTableType.PeachPuff: return PeachPuff;
                case ColorTableType.MistyRose: return MistyRose;
                case ColorTableType.LavenderBlush: return LavenderBlush;
                case ColorTableType.Linen: return Linen;
                case ColorTableType.OldLace: return OldLace;
                case ColorTableType.PapayaWhip: return PapayaWhip;
                case ColorTableType.SeaShell: return SeaShell;
                case ColorTableType.MintCream: return MintCream;
                case ColorTableType.SlateGray: return SlateGray;
                case ColorTableType.LightSlateGray: return LightSlateGray;
                case ColorTableType.LightSteelBlue: return LightSteelBlue;
                case ColorTableType.Lavender: return Lavender;
                case ColorTableType.FloralWhite: return FloralWhite;
                case ColorTableType.AliceBlue: return AliceBlue;
                case ColorTableType.GhostWhite: return GhostWhite;
                case ColorTableType.Honeydew: return Honeydew;
                case ColorTableType.Ivory: return Ivory;
                case ColorTableType.Azure: return Azure;
                case ColorTableType.Snow: return Snow;
                case ColorTableType.Black: return Black;
                case ColorTableType.DimGray: return DimGray;
                case ColorTableType.Gray: return Gray;
                case ColorTableType.DarkGray: return DarkGray;
                case ColorTableType.Silver: return Silver;
                case ColorTableType.LightGray: return LightGray;
                case ColorTableType.Gainsboro: return Gainsboro;
                case ColorTableType.WhiteSmoke: return WhiteSmoke;
                case ColorTableType.White: return White;
            }
            return new Color32(0, 0, 0, 0);
        }

        #endregion
    }

    #region Enum

    public enum ColorTableType {
        None,
        [InspectorName("Transparent")] Transparent,
        [InspectorName("Red / Maroon")] Maroon,
        [InspectorName("Red / Dark Red")] DarkRed,
        [InspectorName("Brown / Brown")] Brown,
        [InspectorName("Red / Firebrick")] Firebrick,
        [InspectorName("Red / Crimson")] Crimson,
        [InspectorName("Red / Red")] Red,
        [InspectorName("Red / Tomato")] Tomato,
        [InspectorName("Red / Coral")] Coral,
        [InspectorName("Red / Indian")] IndianRed,
        [InspectorName("Red / Light Coral")] LightCoral,
        [InspectorName("Red / Dark Salmon")] DarkSalmon,
        [InspectorName("Red / Salmon")] Salmon,
        [InspectorName("Red / Light Salmon")] LightSalmon,
        [InspectorName("Red / Orange Red")] OrangeRed,
        [InspectorName("Red / Dark Orange")] DarkOrange,
        [InspectorName("Red / Orange")] Orange,
        [InspectorName("Yellow / Gold")] Gold,
        [InspectorName("Yellow / Dark Golden Rod")] DarkGoldenRod,
        [InspectorName("Yellow / Golden Rod")] GoldenRod,
        [InspectorName("Yellow / Pale Golden Rod")] PaleGoldenRod,
        [InspectorName("Yellow / Dark Khaki")] DarkKhaki,
        [InspectorName("Yellow / Khaki")] Khaki,
        [InspectorName("Green / Olive")] Olive,
        [InspectorName("Yellow / Yellow")] Yellow,
        [InspectorName("Green / Yellow Green")] YellowGreen,
        [InspectorName("Green / Dark Olive Green")] DarkOliveGreen,
        [InspectorName("Green / Olive Drab")] OliveDrab,
        [InspectorName("Green / Lawn Green")] LawnGreen,
        [InspectorName("Green / Chart Reuse")] ChartReuse,
        [InspectorName("Green / Green Yellow")] GreenYellow,
        [InspectorName("Green / Dark Green")] DarkGreen,
        [InspectorName("Green / Green")] Green,
        [InspectorName("Green / Forest Green")] ForestGreen,
        [InspectorName("Green / Lime")] Lime,
        [InspectorName("Green / Lime Green")] LimeGreen,
        [InspectorName("Green / Light Green")] LightGreen,
        [InspectorName("Green / Pale Green")] PaleGreen,
        [InspectorName("Green / Dark Sea Green")] DarkSeaGreen,
        [InspectorName("Green / Medium Spring Green")] MediumSpringGreen,
        [InspectorName("Green / Spring Green")] SpringGreen,
        [InspectorName("Green / Sea Green")] SeaGreen,
        [InspectorName("Blue / Medium Aqua Marine")] MediumAquaMarine,
        [InspectorName("Green / Medium Sea Green")] MediumSeaGreen,
        [InspectorName("Green / Light Sea Green")] LightSeaGreen,
        [InspectorName("Gray / Dark Slate Gray")] DarkSlateGray,
        [InspectorName("Blue / Teal")] Teal,
        [InspectorName("Blue / Dark Cyan")] DarkCyan,
        [InspectorName("Blue / Aqua")] Aqua,
        [InspectorName("Blue / Cyan")] Cyan,
        [InspectorName("Blue / Light Cyan")] LightCyan,
        [InspectorName("Blue / Dark Turquoise")] DarkTurquoise,
        [InspectorName("Blue / Turquoise")] Turquoise,
        [InspectorName("Blue / Medium Turquoise")] MediumTurquoise,
        [InspectorName("Blue / Pale Turquoise")] PaleTurquoise,
        [InspectorName("Blue / Aqua Marine")] AquaMarine,
        [InspectorName("Blue / Powder Blue")] PowderBlue,
        [InspectorName("Blue / Cadet Blue")] CadetBlue,
        [InspectorName("Blue / Steel Blue")] SteelBlue,
        [InspectorName("Blue / Corn Flower Blue")] CornFlowerBlue,
        [InspectorName("Blue / Deep Sky Blue")] DeepSkyBlue,
        [InspectorName("Blue / Dodger Blue")] DodgerBlue,
        [InspectorName("Blue / Light Blue")] LightBlue,
        [InspectorName("Blue / Sky Blue")] SkyBlue,
        [InspectorName("Blue / Light Sky Blue")] LightSkyBlue,
        [InspectorName("Purple / Midnight Blue")] MidnightBlue,
        [InspectorName("Blue / Navy")] Navy,
        [InspectorName("Blue / Dark Blue")] DarkBlue,
        [InspectorName("Blue / Medium Blue")] MediumBlue,
        [InspectorName("Blue / Blue")] Blue,
        [InspectorName("Blue / Royal Blue")] RoyalBlue,
        [InspectorName("Purple / Blue Violet")] BlueViolet,
        [InspectorName("Purple / Indigo")] Indigo,
        [InspectorName("Purple / Dark Slate Blue")] DarkSlateBlue,
        [InspectorName("Purple / Slate Blue")] SlateBlue,
        [InspectorName("Purple / Medium Slate Blue")] MediumSlateBlue,
        [InspectorName("Purple / Medium Purple")] MediumPurple,
        [InspectorName("Purple / Dark Magenta")] DarkMagenta,
        [InspectorName("Purple / Dark Violet")] DarkViolet,
        [InspectorName("Purple / Dark Orchid")] DarkOrchid,
        [InspectorName("Purple / Medium Orchid")] MediumOrchid,
        [InspectorName("Purple / Purple")] Purple,
        [InspectorName("Pink / Thistle")] Thistle,
        [InspectorName("Pink / Plum")] Plum,
        [InspectorName("Pink / Violet")] Violet,
        [InspectorName("Pink / Magenta")] Magenta,
        [InspectorName("Pink / Orchid")] Orchid,
        [InspectorName("Pink / Medium Violet Red")] MediumVioletRed,
        [InspectorName("Pink / Pale Violet Red")] PaleVioletRed,
        [InspectorName("Pink / Deep Pink")] DeepPink,
        [InspectorName("Pink / Hot Pink")] HotPink,
        [InspectorName("Pink / Light Pink")] LightPink,
        [InspectorName("Pink / Pink")] Pink,
        [InspectorName("White / Antique White")] AntiqueWhite,
        [InspectorName("White / Beige")] Beige,
        [InspectorName("White / Bisque")] Bisque,
        [InspectorName("White / Blanched Almond")] BlanchedAlmond,
        [InspectorName("White / Wheat")] Wheat,
        [InspectorName("White / Corn Silk")] CornSilk,
        [InspectorName("White / Lemon Chiffon")] LemonChiffon,
        [InspectorName("White / Light Golden Rod Yellow")] LightGoldenRodYellow,
        [InspectorName("White / Light Yellow")] LightYellow,
        [InspectorName("Brown / Saddle Brown")] SaddleBrown,
        [InspectorName("Brown / Sienna")] Sienna,
        [InspectorName("Brown / Chocolate")] Chocolate,
        [InspectorName("Brown / Peru")] Peru,
        [InspectorName("Brown / Sandy Brown")] SandyBrown,
        [InspectorName("Brown / Burly Wood")] BurlyWood,
        [InspectorName("Brown / Tan")] Tan,
        [InspectorName("Brown / Rosy Brown")] RosyBrown,
        [InspectorName("Brown / Moccasin")] Moccasin,
        [InspectorName("White / Navajo White")] NavajoWhite,
        [InspectorName("White / Peach Puff")] PeachPuff,
        [InspectorName("White / Misty Rose")] MistyRose,
        [InspectorName("White / Lavender Blush")] LavenderBlush,
        [InspectorName("White / Linen")] Linen,
        [InspectorName("White / Old Lace")] OldLace,
        [InspectorName("White / Papaya Whip")] PapayaWhip,
        [InspectorName("White / Sea Shell")] SeaShell,
        [InspectorName("White / Mint Cream")] MintCream,
        [InspectorName("Gray / Slate Gray")] SlateGray,
        [InspectorName("Gray / Light Slate Gray")] LightSlateGray,
        [InspectorName("Whiteish / Light Steel Blue")] LightSteelBlue,
        [InspectorName("Whiteish / Lavender")] Lavender,
        [InspectorName("Whiteish / Floral White")] FloralWhite,
        [InspectorName("Whiteish / Alice Blue")] AliceBlue,
        [InspectorName("Whiteish / Ghost White")] GhostWhite,
        [InspectorName("Whiteish / Honeydew")] Honeydew,
        [InspectorName("Whiteish / Ivory")] Ivory,
        [InspectorName("Whiteish / Azure")] Azure,
        [InspectorName("White / Snow")] Snow,
        [InspectorName("Black")] Black,
        [InspectorName("Gray / Dim Gray")] DimGray,
        [InspectorName("Gray / Gray")] Gray,
        [InspectorName("Gray / Dark Gray")] DarkGray,
        [InspectorName("Gray / Silver")] Silver,
        [InspectorName("Gray / Light Gray")] LightGray,
        [InspectorName("Gray / Gainsboro")] Gainsboro,
        [InspectorName("White / White Smoke")] WhiteSmoke,
        [InspectorName("White / White")] White,
    }

    #endregion
}
