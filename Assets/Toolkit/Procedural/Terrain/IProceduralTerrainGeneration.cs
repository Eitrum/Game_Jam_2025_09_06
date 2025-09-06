using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Procedural.Terrain
{
    public interface IProceduralTerrainGeneration
    {
        bool Generate(Data data);
    }
}
