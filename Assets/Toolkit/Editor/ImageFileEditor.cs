using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Toolkit.Utility
{
    internal static class ImageFileEditor
    {
        #region Variables

        private const string DEFAULT_PATH = "Assets/Create/Toolkit/Utility/Basic Texture/";

        #endregion

        #region Methods

        [MenuItem(DEFAULT_PATH + "White")] private static void CreateWhiteTexture() => CreateTexture(GetPath("white.png"), ColorTable.White);
        [MenuItem(DEFAULT_PATH + "Gray")] private static void CreateGrayTexture() => CreateTexture(GetPath("gray.png"), ColorTable.Gray);
        [MenuItem(DEFAULT_PATH + "Black")] private static void CreateBlackTexture() => CreateTexture(GetPath("black.png"), ColorTable.Black);
        [MenuItem(DEFAULT_PATH + "Transparent")] private static void CreateTransparentTexture() => CreateTexture(GetPath("transparent.png"), Color.clear);
        [MenuItem(DEFAULT_PATH + "Blue")] private static void CreateBlueTexture() => CreateTexture(GetPath("blue.png"), ColorTable.Blue);
        [MenuItem(DEFAULT_PATH + "Red")] private static void CreateRedTexture() => CreateTexture(GetPath("red.png"), ColorTable.Red);
        [MenuItem(DEFAULT_PATH + "Green")] private static void CreateGreenTexture() => CreateTexture(GetPath("green.png"), ColorTable.Green);
        [MenuItem(DEFAULT_PATH + "Prototype 2x2", priority = 10000)] private static void CreatePrototype2x2() => Internal_CreatePrototypeTexture(GetPath("prototype_2x2.png"), 512, 2, 2, 2, Color.black);
        [MenuItem(DEFAULT_PATH + "Prototype 3x3", priority = 10000)] private static void CreatePrototype3x3() => Internal_CreatePrototypeTexture(GetPath("prototype_3x3.png"), 342, 3, 3, 2, Color.black);
        [MenuItem(DEFAULT_PATH + "Prototype 4x4", priority = 10000)] private static void CreatePrototype4x4() => Internal_CreatePrototypeTexture(GetPath("prototype_4x4.png"), 256, 4, 4, 2, Color.black);
        [MenuItem(DEFAULT_PATH + "Prototype 5x5", priority = 10001)] private static void CreatePrototype5x5() => Internal_CreatePrototypeTexture(GetPath("prototype_5x5.png"), 204, 5, 5, 2, Color.black);
        [MenuItem(DEFAULT_PATH + "Prototype 6x6", priority = 10002)] private static void CreatePrototype6x6() => Internal_CreatePrototypeTexture(GetPath("prototype_6x6.png"), 170, 6, 6, 2, Color.black);
        [MenuItem(DEFAULT_PATH + "Prototype 8x8", priority = 10003)] private static void CreatePrototype8x8() => Internal_CreatePrototypeTexture(GetPath("prototype_8x8.png"), 64, 8, 8, 2, Color.black);

        #endregion

        #region Utility

        public static string GetPath(string name) {
            MethodInfo getActiveFolderPath = typeof(ProjectWindowUtil).GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            string folderPath = (string)getActiveFolderPath.Invoke(null, null);
            return AssetDatabase.GenerateUniqueAssetPath(folderPath + "/" + name);
        }

        private static void CreateTexture(string path, Color color) {
            Texture2D texture = new Texture2D(4, 4);
            for(int x = 0; x < 4; x++) {
                for(int y = 0; y < 4; y++) {
                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply();
            System.IO.File.WriteAllBytes(path, texture.EncodeToPNG());
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        private static void Internal_CreatePrototypeTexture(string path, int textureScale, int xScale, int yScale, int lineWidth, Color lineColor) {
            Texture2D texture = new Texture2D(textureScale * xScale, textureScale * yScale);
            int index = 0;
            float lineColorStrenght = lineColor.a;
            lineColor.a = 1f;
            for(int xChunk = 0; xChunk < xScale; xChunk++) {
                for(int yChunk = 0; yChunk < yScale; yChunk++) {
                    Color c = ColorUtilityEx.GetAreaColor(index);

                    for(int x = 0; x < textureScale; x++) {
                        for(int y = 0; y < textureScale; y++) {

                            if(x < lineWidth || y < lineWidth || x >= (textureScale - lineWidth) || y >= (textureScale - lineWidth))
                                texture.SetPixel(xChunk * textureScale + x, yChunk * textureScale + y, Color.Lerp(c, lineColor, lineColorStrenght));
                            else
                                texture.SetPixel(xChunk * textureScale + x, yChunk * textureScale + y, c);
                        }
                    }

                    index++;
                }
            }

            texture.Apply();
            System.IO.File.WriteAllBytes(path, texture.EncodeToPNG());
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        #endregion
    }
}
