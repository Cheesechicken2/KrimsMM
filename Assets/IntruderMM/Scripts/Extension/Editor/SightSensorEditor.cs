#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SightSensor))]
public class SightSensorEditor : Editor
{
    private bool showAdvancedSettings = false;

    public override void OnInspectorGUI()
    {
        SightSensor sensor = (SightSensor)target;

        sensor.verticalFOV = EditorGUILayout.FloatField("Vertical FOV", sensor.verticalFOV);
        sensor.horizontalFOV = EditorGUILayout.FloatField("Horizontal FOV", sensor.horizontalFOV);
        sensor.belowEyesFOV = EditorGUILayout.FloatField("Below Eyes FOV", sensor.belowEyesFOV);
        sensor.finalBelowEyesFOV = EditorGUILayout.FloatField("Final Below Eyes FOV", sensor.finalBelowEyesFOV);
        sensor.autoSetHorizontalFOV = EditorGUILayout.Toggle("Auto Set Horizontal FOV", sensor.autoSetHorizontalFOV);
        sensor.eyes = (Transform)EditorGUILayout.ObjectField("Eyes", sensor.eyes, typeof(Transform), true);
        sensor.eyeCam = (Camera)EditorGUILayout.ObjectField("Eye Camera", sensor.eyeCam, typeof(Camera), true);

        EditorGUILayout.Space();
        showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "Advanced Settings", true);

        if (showAdvancedSettings)
        {
            EditorGUI.indentLevel++;

            sensor.debugTransform = (Transform)EditorGUILayout.ObjectField("Debug Transform", sensor.debugTransform, typeof(Transform), true);
            sensor.arcColor = EditorGUILayout.ColorField("Arc Color", sensor.arcColor);
            sensor.arcDist = EditorGUILayout.FloatField("Arc Distance", sensor.arcDist);
            sensor.textY = EditorGUILayout.FloatField("Text Y", sensor.textY);
            sensor.verticalFOVBasedOnDistance = EditorGUILayout.Toggle("Vertical FOV Based On Distance", sensor.verticalFOVBasedOnDistance);
            sensor.horizontalFOVBasedOnDistance = EditorGUILayout.Toggle("Horizontal FOV Based On Distance", sensor.horizontalFOVBasedOnDistance);
            sensor.belowEyesFOVBasedOnDistance = EditorGUILayout.Toggle("Below Eyes FOV Based On Distance", sensor.belowEyesFOVBasedOnDistance);

            EditorGUILayout.CurveField("Vertical FOV Distance Curve", sensor.verticalFOVDistanceCurve);
            EditorGUILayout.CurveField("Horizontal FOV Distance Curve", sensor.horizontalFOVDistanceCurve);
            EditorGUILayout.CurveField("Below Eyes FOV Distance Curve", sensor.belowEyesFOVDistanceCurve);

            sensor.targetDistance = EditorGUILayout.FloatField("Target Distance", sensor.targetDistance);

            EditorGUI.indentLevel--;
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(sensor);
        }
    }
}
#endif
