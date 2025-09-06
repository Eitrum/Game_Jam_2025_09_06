using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Toolkit
{
    public static class EditorAudioUtility
    {
        #region Variables

        private const string TAG = "<color=aqua>[EditorAudioUtility]</color> - ";

        private static System.Type audioUtil;
        private static MethodInfo play;
        private static MethodInfo stop;
        private static MethodInfo pause;
        private static MethodInfo resume;
        private static MethodInfo isPlaying;

        private static MethodInfo loopClip;
        private static MethodInfo stopAllClips;
        private static MethodInfo getClipSamplePosition;
        private static MethodInfo setClipSamplePosition;
        private static MethodInfo getDuration;

        private static List<AudioClip> clips = new List<AudioClip>();

        #endregion

        #region Constructor / Deconstructor

        static EditorAudioUtility() {
            audioUtil = typeof(AudioImporter).Assembly.GetType("UnityEditor.AudioUtil");
            play = audioUtil.GetMethod("PlayClip", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(AudioClip), typeof(int), typeof(bool) }, null);
            stop = audioUtil.GetMethod("StopClip", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(AudioClip) }, null);
            pause = audioUtil.GetMethod("PauseClip", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(AudioClip) }, null);
            resume = audioUtil.GetMethod("ResumeClip", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(AudioClip) }, null);
            isPlaying = audioUtil.GetMethod("IsClipPlaying", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(AudioClip) }, null);

            loopClip = audioUtil.GetMethod("LoopClip", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(AudioClip), typeof(bool) }, null);
            stopAllClips = audioUtil.GetMethod("StopAllClips", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { }, null);
            getClipSamplePosition = audioUtil.GetMethod("GetClipSamplePosition", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(AudioClip) }, null);
            setClipSamplePosition = audioUtil.GetMethod("SetClipSamplePosition", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(AudioClip), typeof(int) }, null);
            getDuration = audioUtil.GetMethod("GetDuration", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(AudioClip) }, null);

            UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += BeforeReload;
        }

        private static void BeforeReload() {
            if(clips != null) {
                foreach(var clip in clips) {
                    if(clip != null) {
                        Stop(clip);
                    }
                }
            }
        }

        #endregion

        #region Basic Controls

        public static void Play(AudioClip clip) => Play(clip, 0, false);
        public static void Play(AudioClip clip, int sampleOffset) => Play(clip, sampleOffset, false);
        public static void Play(AudioClip clip, bool loop) => Play(clip, 0, loop);
        public static void Play(AudioClip clip, int sampleOffset, bool loop) {
            if(clip == null)
                Debug.LogError(TAG + "Attempting to play a clip that does not exist");
            else {
                clips.Add(clip);
                play?.Invoke(null, new object[] { clip, sampleOffset, loop });
            }
        }

        public static void Stop(AudioClip clip) {
            if(clip == null)
                Debug.LogError(TAG + "Attempting to stop a clip that does not exist");
            else
                stop?.Invoke(null, new object[] { clip });
        }

        public static void Pause(AudioClip clip) {
            if(clip == null)
                Debug.LogError(TAG + "Attempting to pause a clip that does not exist");
            else
                pause?.Invoke(null, new object[] { clip });
        }

        public static void Resume(AudioClip clip) {
            if(clip == null)
                Debug.LogError(TAG + "Attempting to resume a clip that does not exist");
            else
                resume?.Invoke(null, new object[] { clip });
        }

        #endregion

        #region Advanced Controls

        public static bool IsPlaying(AudioClip clip) {
            if(clip == null) {
                Debug.LogError(TAG + "Attempting to check if a clip is playing that does not exist");
                return false;
            }
            else
                return (bool)(isPlaying?.Invoke(null, new object[] { clip }) ?? false);
        }

        public static void LoopClip(AudioClip clip, bool on) {
            if(clip == null)
                Debug.LogError(TAG + "Attempting to loop a clip that does not exist");
            else
                loopClip?.Invoke(null, new object[] { clip, on });
        }

        public static void StopAllClips() {
            stopAllClips?.Invoke(null, null);
        }

        public static int GetClipSamplePosition(AudioClip clip) {
            if(clip == null) {
                Debug.LogError(TAG + "Attempting to get sample position from a clip that does not exist");
                return -1;
            }
            else
                return (int)(getClipSamplePosition?.Invoke(null, new object[] { clip }) ?? -1);
        }

        public static void SetClipSamplePosition(AudioClip clip, int samplePosition) {
            if(clip == null)
                Debug.LogError(TAG + "Attempting to set sample position on a clip that does not exist");
            else
                setClipSamplePosition?.Invoke(null, new object[] { clip, samplePosition });
        }

        public static double GetDuration(AudioClip clip) {
            if(clip == null) {
                Debug.LogError(TAG + "Attempting to get duration from a clip that does not exist");
                return -1d;
            }
            else
                return (double)(getDuration?.Invoke(null, new object[] { clip }) ?? -1d);
        }

        #endregion
    }
}
