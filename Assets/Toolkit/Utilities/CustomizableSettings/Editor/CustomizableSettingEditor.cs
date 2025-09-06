using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Toolkit
{
    public static class CustomizableSettingEditor
    {
        private static bool isLoaded = false;
        private static Type[] supportedTypes;
        private static string[] supportedTypesNames;

        public static string[] SupportedTypePaths {
            get {
                if(!isLoaded)
                    Load();
                return supportedTypesNames;
            }
        }

        public static Type[] SupportedTypes {
            get {
                if(!isLoaded)
                    Load();
                return supportedTypes;
            }
        }

        private static void Load() {
            isLoaded = true;
            var unityObjectType = typeof(UnityEngine.Object);
            supportedTypes = MonoImporter.GetAllRuntimeMonoScripts()
                .Select(x => x.GetClass())
                .Where(x => x != null)
                .Select(x => x.Assembly)
                .Insert(0, typeof(int).Assembly)
                .Insert(1, typeof(MonoBehaviour).Assembly)
                .Insert(2, typeof(Vector3).Assembly)
                .Unique()
                .SelectMany(x => x.GetTypes())
                .Where(x => (!x.IsGenericType && x.IsPublic) &&
                (x.IsSerializable || x.IsValueType ||
                x.IsDefined(typeof(System.Runtime.Serialization.DataContractAttribute), false) ||
                x.IsDefined(typeof(System.SerializableAttribute), false) || x.IsSubclassOf(unityObjectType)))
                .Insert(0, null)
                .ToArray();
            supportedTypesNames = supportedTypes.Select(x => x?.FullName.Replace(".", "/") ?? "None").ToArray();
        }
    }
}
