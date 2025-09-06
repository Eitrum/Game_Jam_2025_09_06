using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    public interface IPortrait
    {
        Texture2D Portrait { get; }
        int Width { get; }
        int Height { get; }
    }
}
