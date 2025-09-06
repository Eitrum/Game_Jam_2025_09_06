using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Debugging {
    public static class QualitySettingsCommands {

        #region Variables

        private const string BASE_COMMAND = "qualitysettings ";
        private static bool initialized = false;

        #endregion

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        private static void Initialize() {
            if(initialized) return;
            initialized = true;

            Commands.Add(BASE_COMMAND + "activeColorSpace", () => Commands.PrintToConsole($"activeColorSpace={QualitySettings.activeColorSpace}"));
            Commands.Add(BASE_COMMAND + "desiredColorSpace", () => Commands.PrintToConsole($"desiredColorSpace={QualitySettings.desiredColorSpace}"));

            Commands.Add(BASE_COMMAND + "anisotropicFiltering", () => Commands.PrintToConsole($"anisotropicFiltering={QualitySettings.anisotropicFiltering}"));
            Commands.Add(BASE_COMMAND + "anisotropicFiltering", (AnisotropicFiltering filtering) => QualitySettings.anisotropicFiltering = filtering);

            Commands.Add(BASE_COMMAND + "antiAliasing", () => Commands.PrintToConsole($"antiAliasing={QualitySettings.antiAliasing}"));
            Commands.Add(BASE_COMMAND + "antiAliasing", (int value) => QualitySettings.antiAliasing = value);

            Commands.Add(BASE_COMMAND + "asyncUploadBufferSize", () => Commands.PrintToConsole($"asyncUploadBufferSize={QualitySettings.asyncUploadBufferSize}"));
            Commands.Add(BASE_COMMAND + "asyncUploadBufferSize", (int value) => QualitySettings.asyncUploadBufferSize = value);

            Commands.Add(BASE_COMMAND + "asyncUploadPersistentBuffer", () => Commands.PrintToConsole($"asyncUploadPersistentBuffer={QualitySettings.asyncUploadPersistentBuffer}"));
            Commands.Add(BASE_COMMAND + "asyncUploadPersistentBuffer", (bool value) => QualitySettings.asyncUploadPersistentBuffer = value);

            Commands.Add(BASE_COMMAND + "asyncUploadTimeSlice", () => Commands.PrintToConsole($"asyncUploadTimeSlice={QualitySettings.asyncUploadTimeSlice}"));
            Commands.Add(BASE_COMMAND + "asyncUploadTimeSlice", (int value) => QualitySettings.asyncUploadTimeSlice = value);

            Commands.Add(BASE_COMMAND + "billboardsFaceCameraPosition", () => Commands.PrintToConsole($"billboardsFaceCameraPosition={QualitySettings.billboardsFaceCameraPosition}"));
            Commands.Add(BASE_COMMAND + "billboardsFaceCameraPosition", (bool value) => QualitySettings.billboardsFaceCameraPosition = value);

            Commands.Add(BASE_COMMAND + "enableLODCrossFade", () => Commands.PrintToConsole($"enableLODCrossFade={QualitySettings.enableLODCrossFade}"));
            Commands.Add(BASE_COMMAND + "enableLODCrossFade", (bool value) => QualitySettings.enableLODCrossFade = value);

            Commands.Add(BASE_COMMAND + "globalTextureMipmapLimit", () => Commands.PrintToConsole($"globalTextureMipmapLimit={QualitySettings.globalTextureMipmapLimit}"));
            Commands.Add(BASE_COMMAND + "globalTextureMipmapLimit", (int value) => QualitySettings.globalTextureMipmapLimit = value);

            Commands.Add(BASE_COMMAND + "lodBias", () => Commands.PrintToConsole($"lodBias={QualitySettings.lodBias}"));
            Commands.Add(BASE_COMMAND + "lodBias", (float value) => QualitySettings.lodBias = value);

            Commands.Add(BASE_COMMAND + "maximumLODLevel", () => Commands.PrintToConsole($"maximumLODLevel={QualitySettings.maximumLODLevel}"));
            Commands.Add(BASE_COMMAND + "maximumLODLevel", (int value) => QualitySettings.maximumLODLevel = value);

            Commands.Add(BASE_COMMAND + "maxQueuedFrames", () => Commands.PrintToConsole($"maxQueuedFrames={QualitySettings.maxQueuedFrames}"));
            Commands.Add(BASE_COMMAND + "maxQueuedFrames", (int value) => QualitySettings.maxQueuedFrames = value);

            Commands.Add(BASE_COMMAND + "particleRaycastBudget", () => Commands.PrintToConsole($"particleRaycastBudget={QualitySettings.particleRaycastBudget}"));
            Commands.Add(BASE_COMMAND + "particleRaycastBudget", (int value) => QualitySettings.particleRaycastBudget = value);

            Commands.Add(BASE_COMMAND + "pixelLightCount", () => Commands.PrintToConsole($"pixelLightCount={QualitySettings.pixelLightCount}"));
            Commands.Add(BASE_COMMAND + "pixelLightCount", (int value) => QualitySettings.pixelLightCount = value);

            Commands.Add(BASE_COMMAND + "realtimeGICPUUsage", () => Commands.PrintToConsole($"realtimeGICPUUsage={QualitySettings.realtimeGICPUUsage}"));
            Commands.Add(BASE_COMMAND + "realtimeGICPUUsage", (int value) => QualitySettings.realtimeGICPUUsage = value);

            Commands.Add(BASE_COMMAND + "realtimeReflectionProbes", () => Commands.PrintToConsole($"realtimeReflectionProbes={QualitySettings.realtimeReflectionProbes}"));
            Commands.Add(BASE_COMMAND + "realtimeReflectionProbes", (bool value) => QualitySettings.realtimeReflectionProbes = value);

            Commands.Add(BASE_COMMAND + "renderPipeline", () => Commands.PrintToConsole($"renderPipeline={QualitySettings.renderPipeline}"));

            Commands.Add(BASE_COMMAND + "resolutionScalingFixedDPIFactor", () => Commands.PrintToConsole($"resolutionScalingFixedDPIFactor={QualitySettings.resolutionScalingFixedDPIFactor}"));
            Commands.Add(BASE_COMMAND + "resolutionScalingFixedDPIFactor", (float value) => QualitySettings.resolutionScalingFixedDPIFactor = value);

            Commands.Add(BASE_COMMAND + "shadowCascade2Split", () => Commands.PrintToConsole($"shadowCascade2Split={QualitySettings.shadowCascade2Split}"));
            Commands.Add(BASE_COMMAND + "shadowCascade2Split", (float value) => QualitySettings.shadowCascade2Split = value);

            Commands.Add(BASE_COMMAND + "shadowCascade4Split", () => Commands.PrintToConsole($"shadowCascade4Split={QualitySettings.shadowCascade4Split}"));
            Commands.Add(BASE_COMMAND + "shadowCascade4Split", (Vector3 value) => QualitySettings.shadowCascade4Split = value);

            Commands.Add(BASE_COMMAND + "shadowCascades", () => Commands.PrintToConsole($"shadowCascades={QualitySettings.shadowCascades}"));
            Commands.Add(BASE_COMMAND + "shadowCascades", (int value) => QualitySettings.shadowCascades = value);

            Commands.Add(BASE_COMMAND + "shadowDistance", () => Commands.PrintToConsole($"shadowDistance={QualitySettings.shadowDistance}"));
            Commands.Add(BASE_COMMAND + "shadowDistance", (float value) => QualitySettings.shadowDistance = value);

            Commands.Add(BASE_COMMAND + "shadowmaskMode", () => Commands.PrintToConsole($"shadowmaskMode={QualitySettings.shadowmaskMode}"));
            Commands.Add(BASE_COMMAND + "shadowmaskMode", (ShadowmaskMode value) => QualitySettings.shadowmaskMode = value);

            Commands.Add(BASE_COMMAND + "shadowNearPlaneOffset", () => Commands.PrintToConsole($"shadowNearPlaneOffset={QualitySettings.shadowNearPlaneOffset}"));
            Commands.Add(BASE_COMMAND + "shadowNearPlaneOffset", (float value) => QualitySettings.shadowNearPlaneOffset = value);

            Commands.Add(BASE_COMMAND + "shadowProjection", () => Commands.PrintToConsole($"shadowProjection={QualitySettings.shadowProjection}"));
            Commands.Add(BASE_COMMAND + "shadowProjection", (ShadowProjection value) => QualitySettings.shadowProjection = value);

            Commands.Add(BASE_COMMAND + "shadowResolution", () => Commands.PrintToConsole($"shadowResolution={QualitySettings.shadowResolution}"));
            Commands.Add(BASE_COMMAND + "shadowResolution", (ShadowResolution value) => QualitySettings.shadowResolution = value);

            Commands.Add(BASE_COMMAND + "shadows", () => Commands.PrintToConsole($"shadows={QualitySettings.shadows}"));
            Commands.Add(BASE_COMMAND + "shadows", (ShadowQuality value) => QualitySettings.shadows = value);

            Commands.Add(BASE_COMMAND + "skinWeights", () => Commands.PrintToConsole($"skinWeights={QualitySettings.skinWeights}"));
            Commands.Add(BASE_COMMAND + "skinWeights", (SkinWeights value) => QualitySettings.skinWeights = value);

            Commands.Add(BASE_COMMAND + "softParticles", () => Commands.PrintToConsole($"softParticles={QualitySettings.softParticles}"));
            Commands.Add(BASE_COMMAND + "softParticles", (bool value) => QualitySettings.softParticles = value);

            Commands.Add(BASE_COMMAND + "softVegetation", () => Commands.PrintToConsole($"softVegetation={QualitySettings.softVegetation}"));
            Commands.Add(BASE_COMMAND + "softVegetation", (bool value) => QualitySettings.softVegetation = value);

            Commands.Add(BASE_COMMAND + "streamingMipmapsActive", () => Commands.PrintToConsole($"streamingMipmapsActive={QualitySettings.streamingMipmapsActive}"));
            Commands.Add(BASE_COMMAND + "streamingMipmapsActive", (bool value) => QualitySettings.streamingMipmapsActive = value);

            Commands.Add(BASE_COMMAND + "streamingMipmapsAddAllCameras", () => Commands.PrintToConsole($"streamingMipmapsAddAllCameras={QualitySettings.streamingMipmapsAddAllCameras}"));
            Commands.Add(BASE_COMMAND + "streamingMipmapsAddAllCameras", (bool value) => QualitySettings.streamingMipmapsAddAllCameras = value);

            Commands.Add(BASE_COMMAND + "streamingMipmapsMaxFileIORequests", () => Commands.PrintToConsole($"streamingMipmapsMaxFileIORequests={QualitySettings.streamingMipmapsMaxFileIORequests}"));
            Commands.Add(BASE_COMMAND + "streamingMipmapsMaxFileIORequests", (int value) => QualitySettings.streamingMipmapsMaxFileIORequests = value);

            Commands.Add(BASE_COMMAND + "streamingMipmapsMaxLevelReduction", () => Commands.PrintToConsole($"streamingMipmapsMaxLevelReduction={QualitySettings.streamingMipmapsMaxLevelReduction}"));
            Commands.Add(BASE_COMMAND + "streamingMipmapsMaxLevelReduction", (int value) => QualitySettings.streamingMipmapsMaxLevelReduction = value);

            Commands.Add(BASE_COMMAND + "streamingMipmapsMemoryBudget", () => Commands.PrintToConsole($"streamingMipmapsMemoryBudget={QualitySettings.streamingMipmapsMemoryBudget}"));
            Commands.Add(BASE_COMMAND + "streamingMipmapsMemoryBudget", (float value) => QualitySettings.streamingMipmapsMemoryBudget = value);

            Commands.Add(BASE_COMMAND + "streamingMipmapsRenderersPerFrame", () => Commands.PrintToConsole($"streamingMipmapsRenderersPerFrame={QualitySettings.streamingMipmapsRenderersPerFrame}"));
            Commands.Add(BASE_COMMAND + "streamingMipmapsRenderersPerFrame", (int value) => QualitySettings.streamingMipmapsRenderersPerFrame = value);

            Commands.Add(BASE_COMMAND + "terrainBasemapDistance", () => Commands.PrintToConsole($"terrainBasemapDistance={QualitySettings.terrainBasemapDistance}"));
            Commands.Add(BASE_COMMAND + "terrainBasemapDistance", (float value) => QualitySettings.terrainBasemapDistance = value);

            Commands.Add(BASE_COMMAND + "terrainBillboardStart", () => Commands.PrintToConsole($"terrainBillboardStart={QualitySettings.terrainBillboardStart}"));
            Commands.Add(BASE_COMMAND + "terrainBillboardStart", (float value) => QualitySettings.terrainBillboardStart = value);

            Commands.Add(BASE_COMMAND + "terrainDetailDensityScale", () => Commands.PrintToConsole($"terrainDetailDensityScale={QualitySettings.terrainDetailDensityScale}"));
            Commands.Add(BASE_COMMAND + "terrainDetailDensityScale", (float value) => QualitySettings.terrainDetailDensityScale = value);

            Commands.Add(BASE_COMMAND + "terrainDetailDistance", () => Commands.PrintToConsole($"terrainDetailDistance={QualitySettings.terrainDetailDistance}"));
            Commands.Add(BASE_COMMAND + "terrainDetailDistance", (float value) => QualitySettings.terrainDetailDistance = value);

            Commands.Add(BASE_COMMAND + "terrainFadeLength", () => Commands.PrintToConsole($"terrainFadeLength={QualitySettings.terrainFadeLength}"));
            Commands.Add(BASE_COMMAND + "terrainFadeLength", (float value) => QualitySettings.terrainFadeLength = value);

            Commands.Add(BASE_COMMAND + "terrainMaxTrees", () => Commands.PrintToConsole($"terrainMaxTrees={QualitySettings.terrainMaxTrees}"));
            Commands.Add(BASE_COMMAND + "terrainMaxTrees", (float value) => QualitySettings.terrainMaxTrees = value);

            Commands.Add(BASE_COMMAND + "terrainPixelError", () => Commands.PrintToConsole($"terrainPixelError={QualitySettings.terrainPixelError}"));
            Commands.Add(BASE_COMMAND + "terrainPixelError", (float value) => QualitySettings.terrainPixelError = value);

            Commands.Add(BASE_COMMAND + "terrainQualityOverrides", () => Commands.PrintToConsole($"terrainQualityOverrides={QualitySettings.terrainQualityOverrides}"));
            Commands.Add(BASE_COMMAND + "terrainQualityOverrides", (TerrainQualityOverrides value) => QualitySettings.terrainQualityOverrides = value);

            Commands.Add(BASE_COMMAND + "terrainTreeDistance", () => Commands.PrintToConsole($"terrainTreeDistance={QualitySettings.terrainTreeDistance}"));
            Commands.Add(BASE_COMMAND + "terrainTreeDistance", (float value) => QualitySettings.terrainTreeDistance = value);

            Commands.Add(BASE_COMMAND + "useLegacyDetailDistribution", () => Commands.PrintToConsole($"useLegacyDetailDistribution={QualitySettings.useLegacyDetailDistribution}"));
            Commands.Add(BASE_COMMAND + "useLegacyDetailDistribution", (bool value) => QualitySettings.useLegacyDetailDistribution = value);

            Commands.Add(BASE_COMMAND + "vSyncCount", () => Commands.PrintToConsole($"vSyncCount={QualitySettings.vSyncCount}"));
            Commands.Add(BASE_COMMAND + "vSyncCount", (int value) => QualitySettings.vSyncCount = value);

            // Quality Levels
            Commands.Add(BASE_COMMAND + "qualityLevelCount", () => Commands.PrintToConsole($"count={QualitySettings.count}"));
            Commands.Add(BASE_COMMAND + "qualityNames", () => Commands.PrintToConsole($"qualityNames={string.Join('|', QualitySettings.names)}"));
            Commands.Add(BASE_COMMAND + "quality increase", () => QualitySettings.IncreaseLevel(true));
            Commands.Add(BASE_COMMAND + "quality decrease", () => QualitySettings.DecreaseLevel(true));
            Commands.Add(BASE_COMMAND + "quality get", () => Commands.PrintToConsole($"qualityLevel={QualitySettings.GetQualityLevel()}"));
            Commands.Add(BASE_COMMAND + "quality set", (int level) => QualitySettings.SetQualityLevel(level));

        }
    }
}
