using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

[CustomEditor(typeof(LightingSwitchGroup))]
public class LightingSwitchGroupEditor : Editor
{
    private GUISkin customSkin;
    private Font customFont;

    private void OnEnable()
    {
        customSkin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/IntruderMM/Scripts/Extension/Editor/GUI/IntruderSkin.guiskin");
        customFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/IntruderMM/Scripts/Extension/Editor/GUI/Font/ShareTechMono-Regular.ttf");
    }

    private void AutomateSetup()
    {
        LightingSwitchGroup data = (LightingSwitchGroup)target;

        if (string.IsNullOrEmpty(data.name) || data.renderSettings == null)
        {
            Debug.LogWarning("GroupName or RenderSettings is not set. Denying the automation..");
            return;
        }

        FetchLightmaps(data);
        FetchLightProbes(data);
        EnableLightMode(data);
    }

    private void FetchLightmaps(LightingSwitchGroup data)
    {
        data.lightmaps = new Texture2D[LightmapSettings.lightmaps.Length * 3];

        string parentFolderPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(data.renderSettings));
        string folderName = data.name + "-Lightmaps";
        string folderPath = parentFolderPath + "/" + folderName;

        if (AssetDatabase.IsValidFolder(folderPath)) AssetDatabase.DeleteAsset(folderPath);
        AssetDatabase.CreateFolder(parentFolderPath, folderName);

        for (int i = 0; i < LightmapSettings.lightmaps.Length; i++)
        {
            string dirPath = folderPath + $"/Lightmap-{i}_comp_dir.png";
            string lightPath = folderPath + $"/Lightmap-{i}_comp_light.exr";

            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(LightmapSettings.lightmaps[i].lightmapDir), dirPath);
            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(LightmapSettings.lightmaps[i].lightmapColor), lightPath);

            data.lightmaps[i * 3] = AssetDatabase.LoadAssetAtPath<Texture2D>(dirPath);
            data.lightmaps[i * 3 + 1] = AssetDatabase.LoadAssetAtPath<Texture2D>(lightPath);
            data.lightmaps[i * 3 + 2] = null;
        }
    }

    private void FetchLightProbes(LightingSwitchGroup data)
    {
        if (LightmapSettings.lightProbes != null && LightmapSettings.lightProbes.bakedProbes != null)
        {
            data.lightprobes = new SphericalHarmonicsL2[LightmapSettings.lightProbes.bakedProbes.Length];
            for (int i = 0; i < LightmapSettings.lightProbes.bakedProbes.Length; i++)
            {
                data.lightprobes[i] = LightmapSettings.lightProbes.bakedProbes[i];
            }
        }
        else
        {
            Debug.LogWarning("No Lightprobes found! either that or the script is fucked.");
            data.lightprobes = new SphericalHarmonicsL2[0]; 
        }
    }


    private void EnableLightMode(LightingSwitchGroup data)
    {
        data.gameObject.SetActive(true);
        for (int i = 0; i < data.transform.parent.childCount; i++)
        {
            Transform child = data.transform.parent.GetChild(i);
            if (child != data.transform) child.gameObject.SetActive(false);
        }

        LightmapData[] lightmapArray = new LightmapData[data.lightmaps.Length / 3];
        for (int i = 0; i < data.lightmaps.Length / 3; i++)
        {
            LightmapData mapData = new LightmapData
            {
                lightmapDir = data.lightmaps[i * 3],
                lightmapColor = data.lightmaps[i * 3 + 1]
            };
            lightmapArray[i] = mapData;
        }
        LightmapSettings.lightmaps = lightmapArray;

        for (int i = 0; i < data.lightprobes.Length; i++)
        {
            LightmapSettings.lightProbes.bakedProbes[i] = data.lightprobes[i];
        }

        ApplyRenderSettings(data.renderSettings);
    }

    private void ApplyRenderSettings(LightingSwitchRenderSettingsData settings)
    {
        RenderSettings.fog = settings.fog;
        RenderSettings.fogStartDistance = settings.fogStartDistance;
        RenderSettings.fogEndDistance = settings.fogEndDistance;
        RenderSettings.fogMode = settings.fogMode;
        RenderSettings.fogColor = settings.fogColor;
        RenderSettings.fogDensity = settings.fogDensity;
        RenderSettings.ambientMode = settings.ambientMode;
        RenderSettings.ambientSkyColor = settings.ambientSkyColor;
        RenderSettings.ambientEquatorColor = settings.ambientEquatorColor;
        RenderSettings.ambientGroundColor = settings.ambientGroundColor;
        RenderSettings.ambientIntensity = settings.ambientIntensity;
        RenderSettings.ambientLight = settings.ambientLight;
        RenderSettings.subtractiveShadowColor = settings.subtractiveShadowColor;
        RenderSettings.skybox = settings.skybox;
        RenderSettings.sun = settings.sun;
        RenderSettings.ambientProbe = settings.ambientProbe;
        RenderSettings.customReflection = settings.customReflection;
        RenderSettings.reflectionIntensity = settings.reflectionIntensity;
        RenderSettings.reflectionBounces = settings.reflectionBounces;
        RenderSettings.defaultReflectionMode = settings.defaultReflectionMode;
        RenderSettings.defaultReflectionResolution = settings.defaultReflectionResolution;
        RenderSettings.haloStrength = settings.haloStrength;
        RenderSettings.flareStrength = settings.flareStrength;
        RenderSettings.flareFadeSpeed = settings.flareFadeSpeed;
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Automate Setup")) AutomateSetup();
    }
}
