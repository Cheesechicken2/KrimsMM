#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine.Rendering;

namespace Assets.IntruderMM.Editor
{
    [InitializeOnLoad]
    public static class ProjectSettingsInitializer
    {
        static ProjectSettingsInitializer()
        {
            ApplyProjectSettings();
        }

        private static void ApplyProjectSettings()
        {
            if (PlayerSettings.colorSpace == ColorSpace.Linear ||
                EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier3).renderingPath == RenderingPath.DeferredShading)
            {
                return;
            }

            Debug.Log("Applying Project Settings...");
            SetPlayerSettings();
            SetGraphicsSettings();
            EditorPrefs.SetBool("projectSettingsApplied", true);
        }

        private static void SetPlayerSettings()
        {
            PlayerSettings.productName = "IntruderMM";
            PlayerSettings.companyName = "Superboss Games";
            PlayerSettings.colorSpace = ColorSpace.Linear;
        }

        private static void SetGraphicsSettings()
        {
            TierSettings tierSettings = new TierSettings
            {
                standardShaderQuality = ShaderQuality.High,
                reflectionProbeBoxProjection = true,
                reflectionProbeBlending = true,
                detailNormalMap = true,
                semitransparentShadows = true,
                hdr = true,
                hdrMode = CameraHDRMode.FP16,
                cascadedShadowMaps = true,
                enableLPPV = true,
                realtimeGICPUUsage = RealtimeGICPUUsage.Medium,
                renderingPath = RenderingPath.DeferredShading 
            };

            EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier3, tierSettings);

            Camera.main?.SetReplacementShader(Shader.Find("Hidden/DeferredRendering"), "Deferred");
        }
    }
}

#endif
