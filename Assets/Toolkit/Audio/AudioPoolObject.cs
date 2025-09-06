using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Audio {
    public class AudioPoolObject : NullableBehaviour, IPoolable, IPostUpdate {
        #region Variables

        private AudioPool parent;
        private AudioSource source;
        private TLinkedListNode<IPostUpdate> updateNode;
        private bool follow = false;
        private IAudioSourceSettings settings;
        private Transform target = null;
        private IAudioClipPlayer clipPlayer = null;
        private float totalTime = 0f;

        #endregion

        #region Properties

        public AudioPool Parent => parent;
        public bool IsPlaying => source.isPlaying;
        public AudioSource Source => source;
        public Transform Target => target;
        public IAudioSourceSettings Settings => settings;

        public bool IsFree => !source.isPlaying;

        #endregion

        #region Initialize

        public void ApplySettings(AudioPool parent, IAudioSourceSettings settings) {
            this.parent = parent;
            if(source == null)
                source = this.GetOrAddComponent<AudioSource>();
            this.settings = settings;
            settings.ApplyTo(source);
        }

        public void Destroy() {
            if(this != null)
                Destroy(this.gameObject);
        }

        #endregion

        #region Restore

        [Button(EditorGUIMode.RuntimeOnly)]
        public void Restore() {
            settings.ApplyTo(source);
        }

        #endregion

        #region Update

        void IPostUpdate.PostUpdate(float dt) {
            if(!source.isPlaying) {
                source.Stop();
                UpdateSystem.Unsubscribe(updateNode);
                return;
            }
            if(follow && target)
                this.transform.localPosition = target.position;
            totalTime += dt;
            clipPlayer?.DynamicUpdate(source, settings, dt, totalTime);
        }

        #endregion

        #region Play Methods

        public void Play(AudioClip clip) {
            source.PlayOneShot(clip);
        }

        public void Play(AudioClip clip, float volume) {
            source.PlayOneShot(clip, volume);
        }

        public void PlayAt(AudioClip clip, Vector3 position) {
            transform.localPosition = position;
            source.PlayOneShot(clip);
        }

        public void PlayAt(AudioClip clip, Vector3 position, float volume) {
            transform.localPosition = position;
            source.PlayOneShot(clip, volume);
        }

        public void PlayAt(AudioClip clip, Transform target) {
            transform.localPosition = target.position;
            source.PlayOneShot(clip);
        }

        public void PlayAt(AudioClip clip, Transform target, float volume) {
            transform.localPosition = target.position;
            source.PlayOneShot(clip, volume);
        }

        public void PlayAndFollow(AudioClip clip, Transform target) {
            if(clip == null || target == null)
                return;
            source.PlayOneShot(clip);
            this.transform.position = transform.position;
            this.target = transform;
            updateNode = UpdateSystem.Subscribe(this as IPostUpdate);
        }

        public void PlayAndFollow(AudioClip clip, Transform target, float volume) {
            if(clip == null || target == null)
                return;
            source.PlayOneShot(clip, volume);
            this.transform.position = transform.position;
            this.target = transform;
            updateNode = UpdateSystem.Subscribe(this as IPostUpdate);
        }

        #endregion

        #region Play ACPlayer Methods

        public AudioClip Play(IAudioClipPlayer audioClipPlayer) {
            if(audioClipPlayer == null)
                return null;
            var clip = audioClipPlayer.Clip;
            audioClipPlayer.Prepare(source, settings);
            source.PlayOneShot(clip);
            this.clipPlayer = audioClipPlayer;
            if(audioClipPlayer.UseDynamicUpdate)
                updateNode = UpdateSystem.Subscribe(this as IPostUpdate);
            return clip;
        }

        public AudioClip Play(IAudioClipPlayer audioClipPlayer, float volume) {
            if(audioClipPlayer == null)
                return null;
            var clip = audioClipPlayer.Clip;
            audioClipPlayer.Prepare(source, settings);
            source.PlayOneShot(clip, volume);
            this.clipPlayer = audioClipPlayer;
            if(audioClipPlayer.UseDynamicUpdate)
                updateNode = UpdateSystem.Subscribe(this as IPostUpdate);
            return clip;
        }

        public AudioClip PlayAt(IAudioClipPlayer audioClipPlayer, Vector3 position) {
            if(audioClipPlayer == null)
                return null;
            transform.localPosition = position;
            var clip = audioClipPlayer.Clip;
            audioClipPlayer.Prepare(source, settings);
            source.PlayOneShot(clip);
            this.clipPlayer = audioClipPlayer;
            if(audioClipPlayer.UseDynamicUpdate)
                updateNode = UpdateSystem.Subscribe(this as IPostUpdate);
            return clip;
        }

        public AudioClip PlayAt(IAudioClipPlayer audioClipPlayer, Vector3 position, float volume) {
            if(audioClipPlayer == null)
                return null;
            transform.localPosition = position;
            var clip = audioClipPlayer.Clip;
            audioClipPlayer.Prepare(source, settings);
            source.PlayOneShot(clip, volume);
            this.clipPlayer = audioClipPlayer;
            if(audioClipPlayer.UseDynamicUpdate)
                updateNode = UpdateSystem.Subscribe(this as IPostUpdate);
            return clip;
        }

        public AudioClip PlayAt(IAudioClipPlayer audioClipPlayer, Transform target) {
            if(audioClipPlayer == null)
                return null;
            transform.localPosition = target.position;
            var clip = audioClipPlayer.Clip;
            audioClipPlayer.Prepare(source, settings);
            source.PlayOneShot(clip);
            this.clipPlayer = audioClipPlayer;
            if(audioClipPlayer.UseDynamicUpdate)
                updateNode = UpdateSystem.Subscribe(this as IPostUpdate);
            return clip;
        }

        public AudioClip PlayAt(IAudioClipPlayer audioClipPlayer, Transform target, float volume) {
            if(audioClipPlayer == null)
                return null;
            transform.localPosition = target.position;
            var clip = audioClipPlayer.Clip;
            audioClipPlayer.Prepare(source, settings);
            source.PlayOneShot(clip, volume);
            this.clipPlayer = audioClipPlayer;
            if(audioClipPlayer.UseDynamicUpdate)
                updateNode = UpdateSystem.Subscribe(this as IPostUpdate);
            return clip;
        }

        public AudioClip PlayAndFollow(IAudioClipPlayer audioClipPlayer, Transform target) {
            if(audioClipPlayer == null)
                return null;
            if(target == null)
                return null;
            var clip = audioClipPlayer.Clip;
            audioClipPlayer.Prepare(source, settings);
            source.PlayOneShot(clip);
            this.transform.position = transform.position;
            this.target = transform;
            this.follow = true;
            this.clipPlayer = audioClipPlayer;
            updateNode = UpdateSystem.Subscribe(this as IPostUpdate);
            return clip;
        }

        public AudioClip PlayAndFollow(IAudioClipPlayer audioClipPlayer, Transform target, float volume) {
            if(audioClipPlayer == null)
                return null;
            if(target == null)
                return null;
            var clip = audioClipPlayer.Clip;
            audioClipPlayer.Prepare(source, settings);
            source.PlayOneShot(clip, volume);
            this.transform.position = transform.position;
            this.target = transform;
            this.follow = true;
            this.clipPlayer = audioClipPlayer;
            updateNode = UpdateSystem.Subscribe(this as IPostUpdate);
            return clip;
        }

        #endregion

        #region IPoolable Impl

        void IPoolable.OnPoolInitialize() {
            if(source.isPlaying)
                source.Stop();
            totalTime = 0;
        }

        void IPoolable.OnPoolDestroy() {
            if(source)
                source.Stop();
            follow = false;
            clipPlayer?.Restore(source, settings);
            clipPlayer = null;
            UpdateSystem.Unsubscribe(updateNode);
        }

        #endregion
    }
}
