using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit
{
    /// NOT YET BETTTER THAN UNITYS BUILT IN WAY OF DOIN IT!!
    public class ProjectSettings<T> : ScriptableObject where T : ProjectSettings<T>
    {
        private static T instance = default;

        public static string DefaultPath => $"ProjectSettings/{typeof(T).FullName.Replace(".", "/")}.asset";

        protected static T Instance {
            get {
                if(instance == null) {
                    var assets = UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(DefaultPath);
                    if(assets != null && assets.Length > 0)
                        instance = assets[0] as T;
                    if(instance == null) {
                        instance = CreateInstance<T>();
                        var filePath = DefaultPath;
                        if(!string.IsNullOrEmpty(filePath)) {
                            string folderPath = System.IO.Path.GetDirectoryName(filePath);
                            if(!System.IO.Directory.Exists(folderPath))
                                System.IO.Directory.CreateDirectory(folderPath);

                            UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new[] { instance }, filePath, true);
                        }
                    }
                }
                return instance;
            }
        }

        protected void Save() {
            UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new[] { instance }, DefaultPath, true);
        }
    }
}
