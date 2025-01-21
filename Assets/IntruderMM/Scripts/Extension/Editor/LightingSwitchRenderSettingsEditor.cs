using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

[CustomEditor(typeof(LightingSwitchRenderSettingsData))]
public class LightingSwitchRenderSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LightingSwitchRenderSettingsData settingsData = (LightingSwitchRenderSettingsData)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Set Current Lighting"))
        {
            settingsData.Set();
            EditorUtility.SetDirty(settingsData);
        }

        if (GUILayout.Button("Apply Lighting"))
        {
            settingsData.Apply();
        }

        LightingSwitchGroup group = FindObjectOfType<LightingSwitchGroup>();
        if (group != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Lightning Switch Group", EditorStyles.boldLabel);

            group.groupName = EditorGUILayout.TextField("Group Name", group.groupName);
            group.skyboxReflection = (Cubemap)EditorGUILayout.ObjectField("Skybox Reflection", group.skyboxReflection, typeof(Cubemap), false);
            group.clockTimeOffsetRange = EditorGUILayout.Vector2Field("Clock Time Offset Range", group.clockTimeOffsetRange);
            group.disableBakedLightsOnStart = EditorGUILayout.Toggle("Disable Baked Lights On Start", group.disableBakedLightsOnStart);

            SerializedProperty bakedLightsProp = serializedObject.FindProperty("bakedLights");
            EditorGUILayout.PropertyField(bakedLightsProp, true);

            SerializedProperty lightmapsProp = serializedObject.FindProperty("lightmaps");
            EditorGUILayout.PropertyField(lightmapsProp, true);

            SerializedProperty lightprobesProp = serializedObject.FindProperty("lightprobes");
            EditorGUILayout.PropertyField(lightprobesProp, true);

            SerializedProperty reflectionMapsProp = serializedObject.FindProperty("reflectionMaps");
            EditorGUILayout.PropertyField(reflectionMapsProp, true);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(group);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
