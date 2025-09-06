using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Toolkit.Audio
{
    public interface IAudioVariation
    {
        AudioClip Clip { get; }
        int Count { get; }
        AudioClip this[int index] { get; }

        AudioClip GetRandom();
        AudioClip GetRandom(System.Random random);
        AudioClip GetClip(int index);
    }
}
