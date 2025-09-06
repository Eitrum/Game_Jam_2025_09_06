using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Rendering
{
    public interface IRendererInstance
    {
        void Draw(Matrix4x4 matrix);
        void Draw(Matrix4x4 matrix, int layer);
        void Draw(Matrix4x4 matrix, int layer, Camera camera);

        void Draw(Material customMaterial, Matrix4x4 matrix);
        void Draw(Material customMaterial, Matrix4x4 matrix, int layer);
        void Draw(Material customMaterial, Matrix4x4 matrix, int layer, Camera camera);
    }
}
