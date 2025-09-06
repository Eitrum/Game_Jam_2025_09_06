using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Toolkit
{
    public static class TextFileEditor
    {
        #region Variables

        private const string DEFAULT_PATH = "Assets/Create/Toolkit/Utility/Text File/";

        #endregion

        #region Create Text Files

        [MenuItem(DEFAULT_PATH + "Txt")] private static void CreateTxt() => CreateFile(GetPath("new text.txt"), "");
        [MenuItem(DEFAULT_PATH + "Json")] private static void CreateJson() => CreateFile(GetPath("new text.json"), "");
        [MenuItem(DEFAULT_PATH + "Xml")] private static void CreateXml() => CreateFile(GetPath("new text.xml"), "");

        private static string GetPath(string name) {
            MethodInfo getActiveFolderPath = typeof(ProjectWindowUtil).GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            string folderPath = (string)getActiveFolderPath.Invoke(null, null);
            return AssetDatabase.GenerateUniqueAssetPath(folderPath + "/" + name);
        }

        private static void CreateFile(string path, string content) {
            ProjectWindowUtil.CreateAssetWithContent(path, content);
        }

        #endregion
    }
}
