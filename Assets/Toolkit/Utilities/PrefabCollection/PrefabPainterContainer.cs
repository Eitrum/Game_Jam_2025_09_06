using System;
using UnityEngine;

namespace Toolkit.Utility
{
    public class PrefabPainterContainer : MonoBehaviour
    {
        [SerializeField, Readonly] private string containerName = "";

        public string ContainerName {
            get => containerName;
            internal set => containerName = value;
        }

        public int ChildCount => transform.childCount;
    }
}
