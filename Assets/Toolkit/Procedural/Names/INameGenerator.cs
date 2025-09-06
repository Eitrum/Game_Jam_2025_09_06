using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Procedural.Names
{
    public interface INameGenerator
    {
        string Generate();
    }

    public interface INameGenerator<T> : INameGenerator
    {
        string Generate(T input);
    }

    public interface INameGenerator<T0, T1> : INameGenerator
    {
        string Generate(T0 input0, T1 input1);
    }

    public interface INameGenerator<T0, T1, T2> : INameGenerator
    {
        string Generate(T0 input0, T1 input1, T2 input2);
    }

    public interface INameGenerator<T0, T1, T2, T3> : INameGenerator
    {
        string Generate(T0 input0, T1 input1, T2 input2, T3 input3);
    }
}
