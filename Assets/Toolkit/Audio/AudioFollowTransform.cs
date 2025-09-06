using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Audio
{
    public class AudioFollowTransform : MonoBehaviour, INullable, IPostUpdate
    {
        #region Variables

        private AudioSource source = null;
        private Transform followTarget = null;
        private TLinkedListNode<IPostUpdate> postUpdateNode = null;

        #endregion

        #region Properties

        bool INullable.IsNull => this == null;
        public Transform Target => followTarget;
        public AudioSource Source => source;
        public bool IsFollowing => source.isPlaying && followTarget && (postUpdateNode != null);

        #endregion

        #region Init

        private void Awake() {
            source = GetComponent<AudioSource>();
        }

        #endregion

        #region Update

        void IPostUpdate.PostUpdate(float dt) {
            if(!source.isPlaying || !followTarget) {
                UpdateSystem.Unsubscribe(postUpdateNode);
                postUpdateNode = null;
                source.Stop();
            }
            else
                transform.position = followTarget.position;
        }

        #endregion

        #region Methods

        public void PlayAndFollow(AudioClip clip, Transform target) {
            if(clip == null || target == null)
                return;
            source.PlayOneShot(clip);
            this.transform.position = transform.position;
            this.followTarget = transform;
            postUpdateNode = UpdateSystem.Subscribe(this as IPostUpdate);
        }

        public void Follow(Transform transform) {
            if(transform == null)
                return;
            this.transform.position = transform.position;
            this.followTarget = transform;
            postUpdateNode = UpdateSystem.Subscribe(this as IPostUpdate);
        }

        public void Cancel() {
            followTarget = null;
            UpdateSystem.Unsubscribe(postUpdateNode);
            postUpdateNode = null;
        }

        #endregion
    }
}
