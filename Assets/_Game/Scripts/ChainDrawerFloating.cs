using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toolkit;
using UnityEngine;

namespace Spaghetti
{
    public class ChainDrawerFloating : MonoBehaviour
    {
        #region Variables

        [SerializeField] private float scale = 8f;
        [SerializeField] private float speed = 2f;
        [SerializeField] private AnimationCurve distance = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        [SerializeField] private AnimationCurve rotation = AnimationCurve.Linear(0f, 15f, 1f, 15f);
        [Space]
        [SerializeField] private bool includeEnd;
        [SerializeField] private Transform endContainer;
        [Space]
        [SerializeField] private bool includeStart;
        [SerializeField] private Transform startContainer;

        private ChainDrawer chain;
        private Pose[] startingPoses;
        private float time;

        private Pose endContainerOffset;
        private Pose startContainerOffset;

        #endregion

        #region Init

        void Awake() {
            chain = GetComponent<ChainDrawer>();
        }

        void OnEnable() {
            if(chain == null)
                return;
            startingPoses = chain.poses.ToArray();
        }

        void OnDisable() {
            if(chain == null)
                return;
            for(int i = 0, length = startingPoses.Length; i < length; i++) {
                chain.poses[i] = startingPoses[i];
            }
        }

        #endregion

        #region Update

        void LateUpdate() {
            time += Time.deltaTime * speed;
            if(time > 100000f)
                time = 0f;
            MinMaxInt range = new MinMaxInt(includeStart ? 0 : 1, chain.poses.Count - (includeEnd ? 0 : 1));
            for(int i = range.min, length = range.max; i < length; i++) {
                var t = range.InverseEvaluate(i);
                var d = distance.Evaluate(t);
                var rStr = rotation.Evaluate(t);

                var pose = startingPoses[i];
                var offset = new Vector3(Mathf.PerlinNoise(pose.position.x / scale + Mathf.Sin(13.38f + time), Mathf.Cos(time)),
                            Mathf.PerlinNoise(pose.position.y / scale + Mathf.Sin(7.12f + time), Mathf.Cos(time)),
                            Mathf.PerlinNoise(pose.position.z / scale + Mathf.Sin(1.87f + time), Mathf.Cos(time)));
                chain.poses[i] = new Pose(pose.position + offset * d, pose.rotation);

                if(includeEnd && i == (length - 1)) {
                    endContainer.SetPositionAndRotation(chain.poses[i], Space.Self);
                }
            }

            // Very Expensive
            chain.UpdateCurve();
        }

        #endregion

        private void OnValidate() {
            var tchain = GetComponent<ChainDrawer>();
            if(!tchain)
                return;

            if(endContainer)
                endContainer.SetPositionAndRotation(tchain.poses.Last(), Space.Self);
            if(startContainer)
                startContainer.SetPositionAndRotation(tchain.poses.First(), Space.Self);
        }
    }
}
