using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomLevelSettings))]
public class CustomLevelSettingsEditor : Editor
{
    private bool settingsChanged = false;

    public override void OnInspectorGUI()
    {
        CustomLevelSettings settings = (CustomLevelSettings)target;

        EditorGUILayout.LabelField("Level Settings", EditorStyles.boldLabel);

        bool fogEnabled = EditorGUILayout.Toggle("Enable Fog", settings.fogEnabled);
        if (fogEnabled != settings.fogEnabled)
        {
            settings.fogEnabled = fogEnabled;
            settingsChanged = true;
        }

        if (settings.fogEnabled)
        {
            Color fogColor = EditorGUILayout.ColorField("Fog Color", settings.fogColor);
            if (fogColor != settings.fogColor)
            {
                settings.fogColor = fogColor;
                settingsChanged = true;
            }

            FogMode fogMode = (FogMode)EditorGUILayout.EnumPopup("Fog Mode", settings.fogMode);
            if (fogMode != settings.fogMode)
            {
                settings.fogMode = fogMode;
                settingsChanged = true;
            }

            float fogDensity = EditorGUILayout.Slider("Fog Density", settings.fogDensity, 0f, 1f);
            if (fogDensity != settings.fogDensity)
            {
                settings.fogDensity = fogDensity;
                settingsChanged = true;
            }

            float fogStartDistance = EditorGUILayout.FloatField("Fog Start Distance", settings.fogStartDistance);
            if (fogStartDistance != settings.fogStartDistance)
            {
                settings.fogStartDistance = fogStartDistance;
                settingsChanged = true;
            }

            float fogEndDistance = EditorGUILayout.FloatField("Fog End Distance", settings.fogEndDistance);
            if (fogEndDistance != settings.fogEndDistance)
            {
                settings.fogEndDistance = fogEndDistance;
                settingsChanged = true;
            }
        }

        Color ambientLight = EditorGUILayout.ColorField("Ambient Light", settings.ambientLight);
        if (ambientLight != settings.ambientLight)
        {
            settings.ambientLight = ambientLight;
            settingsChanged = true;
        }

        Material skybox = (Material)EditorGUILayout.ObjectField("Skybox Material", settings.skybox, typeof(Material), false);
        if (skybox != settings.skybox)
        {
            settings.skybox = skybox;
            settingsChanged = true;
        }

        float haloStrength = EditorGUILayout.Slider("Halo Strength", settings.haloStrength, 0f, 10f);
        if (haloStrength != settings.haloStrength)
        {
            settings.haloStrength = haloStrength;
            settingsChanged = true;
        }

        float flareStrength = EditorGUILayout.Slider("Flare Strength", settings.flareStrength, 0f, 10f);
        if (flareStrength != settings.flareStrength)
        {
            settings.flareStrength = flareStrength;
            settingsChanged = true;
        }

        float flareFadeSpeed = EditorGUILayout.Slider("Flare Fade Speed", settings.flareFadeSpeed, 0f, 1f);
        if (flareFadeSpeed != settings.flareFadeSpeed)
        {
            settings.flareFadeSpeed = flareFadeSpeed;
            settingsChanged = true;
        }

        LightProbes lightProbes = (LightProbes)EditorGUILayout.ObjectField("Light Probes", settings.lightProbes, typeof(LightProbes), true);
        if (lightProbes != settings.lightProbes)
        {
            settings.lightProbes = lightProbes;
            settingsChanged = true;
        }

        string mmVersion = EditorGUILayout.TextField("MM Version", settings.mmVersion);
        if (mmVersion != settings.mmVersion)
        {
            settings.mmVersion = mmVersion;
            settingsChanged = true;
        }

        string unityVersion = EditorGUILayout.TextField("Unity Version", settings.unityVersion);
        if (unityVersion != settings.unityVersion)
        {
            settings.unityVersion = unityVersion;
            settingsChanged = true;
        }

        if (settingsChanged)
        {
            settings.ApplySettings();
            settings.SetSettings();
            settingsChanged = false;
            EditorUtility.SetDirty(settings); 
        }
    }
}
