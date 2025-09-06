using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Toolkit.Audio
{
    class AudioImportSettingsUtility
    {
        #region Variables

        private const string CONFIGURE_ALL_PATH = "Toolkit/Audio/Import/Configure All";

        private const string NORMALIZE_ALL_PATH = "Toolkit/Audio/Import/Normalize All";

        #endregion

        #region Configure

        /// <summary>
        /// Configuration settings used in Yaengard to optimize load time.
        /// </summary>
        [MenuItem(CONFIGURE_ALL_PATH)]
        public static void ConfigureAll() {
            var importers = AllAudioImporters();
            bool[] hasChangeArr = new bool[importers.Length];

            UnityEditor.EditorUtility.DisplayCancelableProgressBar("Converting Audio", "", 0f);

            for(int i = 0, length = importers.Length; i < length; i++) {
                var imp = importers[i];
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(imp.assetPath);
                var clipLength = clip.length;
                bool hasChange = false;
                if(UnityEditor.EditorUtility.DisplayCancelableProgressBar("Converting Audio", imp.assetPath, i / (float)length))
                    break;

                if(!imp.loadInBackground) {
                    hasChange = true;
                    imp.loadInBackground = true;
                }

                var settings = imp.defaultSampleSettings;
                if(clipLength < 1f) { // Small clips ( hits and effects )
                    if(settings.loadType != AudioClipLoadType.DecompressOnLoad || settings.compressionFormat != AudioCompressionFormat.ADPCM) {
                        settings.loadType = AudioClipLoadType.DecompressOnLoad;
                        settings.compressionFormat = AudioCompressionFormat.ADPCM;
                        hasChange = true;
                    }
                    if(!imp.forceToMono) {
                        imp.forceToMono = true;
                        hasChange = true;
                    }
                }
                else if(clipLength < 15f) { // Slightly longer clips ( voice lines )
                    if(settings.loadType != AudioClipLoadType.CompressedInMemory || settings.compressionFormat != AudioCompressionFormat.ADPCM) {
                        settings.loadType = AudioClipLoadType.CompressedInMemory;
                        settings.compressionFormat = AudioCompressionFormat.ADPCM;
                        hasChange = true;
                    }
                    if(!imp.forceToMono) {
                        imp.forceToMono = true;
                        hasChange = true;
                    }
                }
                else { // LONG CLIPS AND AMBIENCE/MUSIC
                    if(settings.loadType != AudioClipLoadType.Streaming || settings.compressionFormat != AudioCompressionFormat.Vorbis) {
                        settings.loadType = AudioClipLoadType.Streaming;
                        settings.compressionFormat = AudioCompressionFormat.Vorbis;
                        settings.quality = 0.7f;
                        hasChange = true;
                    }
                    if(imp.forceToMono) {
                        imp.forceToMono = false;
                        hasChange = true;
                    }
                }
                imp.defaultSampleSettings = settings;
                imp.ClearSampleSettingOverride("Standalone");
                hasChangeArr[i] = hasChange;
            }

            EditorUtility.ClearProgressBar();


            for(int i = 0, length = importers.Length; i < length; i++) {
                if(hasChangeArr[i])
                    importers[i].SaveAndReimport();
            }
        }

        #endregion

        #region Normalization

        [MenuItem(NORMALIZE_ALL_PATH)]
        public static void NormalizeAll() {
            var sos = AllAudioImportersAsSerializedObjects();

            UnityEditor.EditorUtility.DisplayCancelableProgressBar("Fixing Normalized Audio", "", 0f);

            for(int i = 0, length = sos.Length; i < length; i++) {
                var importer = sos[i].targetObject as AudioImporter;
                if(UnityEditor.EditorUtility.DisplayCancelableProgressBar("Fixing Normalized Audio", importer.name, i / (float)length))
                    break;
                if(importer.forceToMono) {
                    var prop = sos[i].FindProperty("m_Normalize");
                    if(prop.boolValue) {
                        prop.boolValue = false;
                        sos[i].ApplyModifiedProperties();
                    }
                    importer.SaveAndReimport();
                }
            }
            EditorUtility.ClearProgressBar();
        }

        #endregion

        #region Utility

        public static AudioImporter[] AllAudioImporters() {
            UnityEditor.EditorUtility.DisplayCancelableProgressBar("Finding AudioClips", "AssetDatabase.FindAssets", 0f);
            var guids = AssetDatabase.FindAssets("t:AudioClip");
            AudioImporter[] importers = new AudioImporter[guids.Length];

            for(int i = 0, length = guids.Length; i < length; i++) {
                var p = AssetDatabase.GUIDToAssetPath(guids[i]);
                if(UnityEditor.EditorUtility.DisplayCancelableProgressBar("Finding AudioClips", p, i / (float)length))
                    return new AudioImporter[0];
                importers[i] = AudioImporter.GetAtPath(p) as AudioImporter;
            }
            EditorUtility.ClearProgressBar();

            return importers;
        }

        public static SerializedObject[] AllAudioImportersAsSerializedObjects() {
            UnityEditor.EditorUtility.DisplayCancelableProgressBar("Finding AudioClips", "AssetDatabase.FindAssets", 0f);
            var guids = AssetDatabase.FindAssets("t:AudioClip");
            SerializedObject[] importers = new SerializedObject[guids.Length];

            for(int i = 0, length = guids.Length; i < length; i++) {
                var p = AssetDatabase.GUIDToAssetPath(guids[i]);
                if(UnityEditor.EditorUtility.DisplayCancelableProgressBar("Finding AudioClips", p, i / (float)length))
                    return new SerializedObject[0];
                importers[i] = new SerializedObject(AudioImporter.GetAtPath(p));
            }
            EditorUtility.ClearProgressBar();

            return importers;
        }

        #endregion
    }
}
