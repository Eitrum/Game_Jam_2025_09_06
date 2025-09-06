using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit
{
    public static class PrimitiveUtility
    {
        private static Dictionary<PrimitiveType, Mesh> typeToMesh = new Dictionary<PrimitiveType, Mesh>();

        public static Mesh GetMesh(PrimitiveType type) {
            if(typeToMesh.TryGetValue(type, out Mesh mesh))
                return mesh;
            return CreateMesh(type);
        }

        private static Mesh CreateMesh(PrimitiveType type) {
            if(typeToMesh.ContainsKey(type))
                return typeToMesh[type];
            var go = GameObject.CreatePrimitive(type);
            var mesh = go.GetComponent<MeshFilter>().sharedMesh;
#if UNITY_EDITOR
            if(Application.isPlaying)
                GameObject.Destroy(go);
            else
                GameObject.DestroyImmediate(go);
            typeToMesh.Add(type, mesh);
#else
            GameObject.Destroy(go);
#endif
            return mesh;
        }
    }
}
