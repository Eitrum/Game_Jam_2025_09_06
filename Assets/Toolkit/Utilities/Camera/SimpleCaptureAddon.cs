using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Utility
{
    [AddComponentMenu("Toolkit/Canera/Capture Addon (Simple)")]
    [RequireComponent(typeof(SimpleCapture))]
    public class SimpleCaptureAddon : MonoBehaviour
    {
        #region Variables

        [SerializeField] private bool openFileLocation = false;
        [SerializeField] private ScreenshotDirectoryMode directoryMode = ScreenshotDirectoryMode.Default;
        [SerializeField] private string customDirectory = "";

        [SerializeField] private AudioClip captureAudioClip = null;
        [SerializeField] private Audio.AudioSourceSettings audioSettings = null;

        private SimpleCapture capture;
        private AudioSource source;

        #endregion

        #region Properties

        public SimpleCapture CaptureReference => capture;

        // File Location

        public bool OpenFileLocation {
            get => openFileLocation;
            set => openFileLocation = value;
        }

        public ScreenshotDirectoryMode FileLocation {
            get => directoryMode;
            set {
                if(directoryMode != value) {
                    directoryMode = value;
                    UpdateDirectory();
                }
            }
        }

        public string CaptureDirectory {
            get {
                return capture.Directory;
            }
            set {
                directoryMode = ScreenshotDirectoryMode.Custom;
                customDirectory = value;
                UpdateDirectory();
            }
        }

        // Audio 

        public AudioClip CaptureAudioClip {
            get => captureAudioClip;
            set => captureAudioClip = value;
        }

        public Audio.IAudioSourceSettings CaptureAudioSettings {
            get => audioSettings;
            set {
                audioSettings.Copy(value);
                if(source)
                    value.ApplyTo(source);
            }
        }

        public AudioSource Source {
            get => source;
            set => source = value;
        }

        #endregion

        #region Enable Disable

        private void Awake() {
            capture = GetComponent<SimpleCapture>();
        }

        private void Start() {
            UpdateDirectory();
        }

        private void OnEnable() {
            if(capture) {
                capture.OnCaptureToFile += OnCaptureToFile;
                capture.OnCaptureToTexture += OnCaptureToTexture;
            }
        }

        private void OnDisable() {
            if(capture) {
                capture.OnCaptureToFile -= OnCaptureToFile;
                capture.OnCaptureToTexture -= OnCaptureToTexture;
            }
        }

        #endregion

        #region Utility

        public void UpdateDirectory() {
            if(capture == null)
                return;
            switch(directoryMode) {
                case ScreenshotDirectoryMode.Default: capture.Directory = null; break;
                case ScreenshotDirectoryMode.Custom:
                    capture.Directory = customDirectory;
                    customDirectory = capture.Directory;
                    break;
                case ScreenshotDirectoryMode.DataPath:
                    capture.Directory = Application.dataPath;
                    break;
                case ScreenshotDirectoryMode.PersistentDataPath:
                    capture.Directory = Application.persistentDataPath;
                    break;
                case ScreenshotDirectoryMode.Pictures:
                    try {
                        var picturesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                        if(string.IsNullOrEmpty(picturesFolder)) {
                            directoryMode = ScreenshotDirectoryMode.Default;
                            capture.Directory = null;
                        }
                        else {
                            capture.Directory = picturesFolder + "/Yaengard";
                        }
                    }
                    catch(System.Exception e) {
                        Debug.LogException(e);
                        capture.Directory = null;
                        directoryMode = ScreenshotDirectoryMode.Default;
                    }
                    break;
            }
        }

        public void PlayCaptureAudio() {
            if(captureAudioClip == null)
                return;
            if(source == null) {
                source = this.AddComponent<AudioSource>();
                audioSettings.ApplyTo(source);
            }

            source.PlayOneShot(captureAudioClip);
        }

        #endregion

        #region Callbacks

        private void OnCaptureToTexture(Texture2D output) {
            PlayCaptureAudio();
        }

        private void OnCaptureToFile(string output) {
            PlayCaptureAudio();
            if(openFileLocation) {
                Application.OpenURL(System.IO.Path.GetDirectoryName(output));
            }
        }

        #endregion

        public enum ScreenshotDirectoryMode
        {
            Default = 0,
            Pictures = 1,
            DataPath = 2,
            PersistentDataPath = 3,
            Custom = 4,

        }
    }
}
