using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Inventory
{
    [AddComponentMenu("Toolkit/Inventory/Attachment Point Drawer")]
    public class AttachmentPointDrawer : MonoBehaviour
    {
        [SerializeField] private DrawerMode mode = DrawerMode.None;

        private void OnDrawGizmos() {
            var pos = transform.position;
            var rot = transform.rotation;
            var size = transform.lossyScale;

            switch(mode) {
                case DrawerMode.Sword:
                    GizmosUtility.DrawWireCube(pos, rot, new Vector3(0.05f, 0.3f, 0.05f).Multiply(size));
                    GizmosUtility.DrawWireCube(pos + rot * new Vector3(0, 0.15f, 0), rot, new Vector3(0.05f, 0.05f, 0.15f).Multiply(size));
                    GizmosUtility.DrawWireCube(pos + rot * new Vector3(0, 0.55f, 0), rot, new Vector3(0.03f, 0.8f, 0.1f).Multiply(size));
                    Gizmos.DrawLine(pos - rot * Vector3.up, pos + rot * Vector3.up); 
                    break;
                case DrawerMode.Helmet:
                    Gizmos.DrawWireSphere(pos, 0.15f);
                    GizmosUtility.DrawWireCube(pos + rot * new Vector3(0, 0.12f, -0.05f), rot * Quaternion.Euler(-15f, 0, 0), new Vector3(0.25f, 0.05f, 0.25f).Multiply(size));
                    GizmosUtility.DrawWireCube(pos + rot * new Vector3(0, 0.17f, -0.05f), rot * Quaternion.Euler(-15f, 0, 0), new Vector3(0.15f, 0.05f, 0.15f).Multiply(size));
                    break;
                case DrawerMode.Backpack:
                    GizmosUtility.DrawWireCube(pos + rot * new Vector3(0, -0.1f, -0.05f), rot, new Vector3(0.2f, 0.4f, 0.1f).Multiply(size));
                    GizmosUtility.DrawWireCube(pos + rot * new Vector3(0, -0.1f, -0.125f), rot, new Vector3(0.1f, 0.3f, 0.05f).Multiply(size));
                    GizmosUtility.DrawWireCube(pos + rot * new Vector3(0.1115f, -0.03f, -0.065f), rot, new Vector3(0.025f, 0.15f, 0.05f).Multiply(size));
                    GizmosUtility.DrawWireCube(pos + rot * new Vector3(-0.1115f, -0.03f, -0.065f), rot, new Vector3(0.025f, 0.15f, 0.05f).Multiply(size));
                    break;
                case DrawerMode.Cape:
                    GizmosUtility.DrawWireCube(pos, rot * Quaternion.Euler(15f, 0, 0), new Vector3(0.1f, 0.02f, 0.02f));
                    GizmosUtility.DrawWireCube(pos + rot * new Vector3(0f, -0.06f, -0.02f), rot * Quaternion.Euler(10f, 0, 0), new Vector3(0.2f, 0.1f, 0.02f).Multiply(size));
                    GizmosUtility.DrawWireCube(pos + rot * new Vector3(0f, -0.15f, -0.04f), rot * Quaternion.Euler(5f, 0, 0), new Vector3(0.25f, 0.1f, 0.02f).Multiply(size));
                    GizmosUtility.DrawWireCube(pos + rot * new Vector3(0f, -0.3f, -0.045f), rot * Quaternion.Euler(-4f, 0, 0), new Vector3(0.25f, 0.2f, 0.02f).Multiply(size));
                    GizmosUtility.DrawWireCube(pos + rot * new Vector3(0f, -0.5f, -0.015f), rot * Quaternion.Euler(-8f, 0, 0), new Vector3(0.3f, 0.2f, 0.02f).Multiply(size));
                    GizmosUtility.DrawWireCube(pos + rot * new Vector3(0f, -0.7f, 0.03f), rot * Quaternion.Euler(-15f, 0, 0), new Vector3(0.35f, 0.2f, 0.02f).Multiply(size));
                    break;
            }
        }

        public enum DrawerMode
        {
            None,
            Sword,
            Helmet,
            Backpack,
            Cape,

        }
    }
}
