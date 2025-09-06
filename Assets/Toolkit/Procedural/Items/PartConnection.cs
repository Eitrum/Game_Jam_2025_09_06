using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Procedural.Items
{
    [System.Serializable]
    public class PartConnection
    {
        [SerializeField] private Part otherPart = Part.None;
        [SerializeField] private Pose anchor = Pose.identity;

        public Part OtherPart => otherPart;

        public Pose Anchor {
            get => anchor;
            set => anchor = value;
        }

        public PartConnection() { }
        public PartConnection(Part otherPart, Pose anchor) {
            this.otherPart = otherPart;
            this.anchor = anchor;
        }
    }
}
