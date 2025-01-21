#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FogController)), CanEditMultipleObjects]
public class FogControllerEditor : UnityEditor.Editor
{
    private FogController fogControllerTarget;
    private int currentToolbarButton;

    private void OnEnable()
    {
        fogControllerTarget = (FogController)target;
    }

    public override void OnInspectorGUI()
    {
        if (fogControllerTarget == null) { return; }
        currentToolbarButton = GUILayout.Toolbar(currentToolbarButton, new string[] { "General", "Day/Night" });
        switch (currentToolbarButton)
        {
            case 0:
                // Water Settings
                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Water Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(FogController.waterLevel)), new GUIContent("Water Level", "Sets the vertical position of the water surface."));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(FogController.waterColor)), new GUIContent("Water Color", "Determines the color of the water."));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(FogController.waterStartDistance)), new GUIContent("Water Start Distance", "Defines the distance from the camera where the water fog starts."));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(FogController.waterEndDistance)), new GUIContent("Water End Distance", "Defines the distance from the camera where the water fog ends."));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(FogController.waterDensity)), new GUIContent("Water Density", "Controls the density of the fog underwater."));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(FogController.currentSkyColor)), new GUIContent("Current Sky Color", "Specifies the color of the sky when underwater, can see this for yourself with the camstick."));
              //  EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(FogController.waterSounds)), new GUIContent("Water Sounds", "You probably shouldn't mess with these..."));
              //  EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(FogController.aboveSounds)), new GUIContent("Above Water Sounds", "You probably shouldn't mess with these..."));
                GUILayout.EndVertical();
                break;

            case 1:
                // Day/Night Settings
                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Day/Night Settings", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Only change these if you know what you're doing. These are for official maps, via unreleased scripts.", EditorStyles.wordWrappedLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(FogController.dayColor)), new GUIContent("Day Fog Color", "Defines the color of the fog during the daytime."));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(FogController.nightColor)), new GUIContent("Night Fog Color", "Defines the color of the fog during the nighttime."));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(FogController.fogStartDay)), new GUIContent("Day Fog Start Distance", "Sets the starting distance of the fog during the day."));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(FogController.fogEndDay)), new GUIContent("Day Fog End Distance", "Sets the ending distance of the fog during the day."));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(FogController.fogStartNight)), new GUIContent("Night Fog Start Distance", "Sets the starting distance of the fog during the night."));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(FogController.fogEndNight)), new GUIContent("Night Fog End Distance", "Sets the ending distance of the fog during the night."));
                GUILayout.EndVertical();
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif
