using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Rendering
{
    public static class RenderUtility
    {
        public static IRendererInstance CreateRendererInstance(GameObject gameObject, bool includeChildren = true) {
            var gori = new GameObjectRendererInstance(gameObject, includeChildren);
            var instances = gori.Instances;
            if(instances.Count == 0) {
                return null;
            }
            if(instances.Count == 1) {
                return instances[0];
            }
            return gori;
        }
    }
}
