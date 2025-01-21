using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WindProxy))]
public class WindProxyEditor : Editor
{
    private SerializedProperty windMinProp;
    private SerializedProperty windMaxProp;
    private SerializedProperty timerResetMinProp;
    private SerializedProperty timerResetMaxProp;
    private SerializedProperty flagsProp;

    private void OnEnable()
    {
        InitializeSerializedProperties();
    }

    private void InitializeSerializedProperties()
    {
        windMinProp = serializedObject.FindProperty("windMin");
        windMaxProp = serializedObject.FindProperty("windMax");
        timerResetMinProp = serializedObject.FindProperty("timerResetMin");
        timerResetMaxProp = serializedObject.FindProperty("timerResetMax");
        flagsProp = serializedObject.FindProperty("flags");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawWindSettings();
        DrawTimerSettings();
        AutoFindClothButton();
        DrawFlagsField();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawWindSettings()
    {
        EditorGUILayout.LabelField("Wind Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(windMinProp, new GUIContent("Wind Min"));
        EditorGUILayout.PropertyField(windMaxProp, new GUIContent("Wind Max"));
    }

    private void DrawTimerSettings()
    {
        EditorGUILayout.LabelField("Timer Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(timerResetMinProp, new GUIContent("Timer Reset Min"));
        EditorGUILayout.PropertyField(timerResetMaxProp, new GUIContent("Timer Reset Max"));
    }

    private void AutoFindClothButton()
    {
        EditorGUILayout.Space();
        if (GUILayout.Button("Automatically Find Cloth"))
        {
            FindAndAddCloth();
        }
    }

    private void DrawFlagsField()
    {
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(flagsProp, new GUIContent("Cloths"), true);
    }

    private void FindAndAddCloth()
    {
        WindProxy windProxy = (WindProxy)target;

        Cloth[] cloths = FindObjectsOfType<Cloth>();

        if (cloths.Length > 0)
        {
            windProxy.flags = cloths;
            EditorUtility.SetDirty(windProxy);
            Debug.Log($"Auto-populated {cloths.Length} Cloth components for {windProxy.name}.");
        }
        else
        {
            Debug.LogWarning($"No Cloth components found in the scene for {windProxy.name}.");
        }
    }
}
