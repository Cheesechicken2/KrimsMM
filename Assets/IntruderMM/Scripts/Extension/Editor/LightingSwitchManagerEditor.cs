using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LightingSwitchManager))]
public class LightingSwitchManagerEditor : Editor
{
    private GUIStyle buttonStyle;
    private GUIStyle headerStyle;

    private GUISkin customSkin;
    private Font customFont;

    private SerializedProperty lightingSwitchGroupsProp;
    private SerializedProperty objectsToSetAsStaticProp;
    private SerializedProperty reflectionProbesProp;

    private void OnEnable()
    {
        lightingSwitchGroupsProp = serializedObject.FindProperty("lightingSwitchGroups");
        objectsToSetAsStaticProp = serializedObject.FindProperty("objectsToSetAsStatic");
        reflectionProbesProp = serializedObject.FindProperty("reflectionProbes");

        InitializeStyles();
    }

    public override void OnInspectorGUI()
    {
        LightingSwitchManager manager = (LightingSwitchManager)target;

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentState"), new GUIContent("Current State"));

        EditorGUILayout.PropertyField(lightingSwitchGroupsProp, true);
        EditorGUILayout.PropertyField(objectsToSetAsStaticProp, true);
        EditorGUILayout.PropertyField(reflectionProbesProp, true);

        EditorGUILayout.Space();

        CreateActionButton("Update Lighting Switch Groups", () => {
            manager.UpdateLightingSwitchGroups();
            EditorUtility.SetDirty(manager);
        });

        CreateActionButton("Reset States", () => {
            manager.ResetStates();
            EditorUtility.SetDirty(manager);
        });

        serializedObject.ApplyModifiedProperties();
        Repaint();
    }

    private void InitializeStyles()
    {
        customSkin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/IntruderMM/Scripts/Extension/Editor/GUI/IntruderSkin.guiskin");
        customFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/IntruderMM/Scripts/Extension/Editor/GUI/Font/ShareTechMono-Regular.ttf");

        buttonStyle = new GUIStyle(customSkin?.button ?? GUI.skin.button)
        {
            fontSize = 15,
            font = customFont
        };

        headerStyle = new GUIStyle(EditorStyles.label)
        {
            font = customFont,
            fontSize = 15,
            alignment = TextAnchor.MiddleCenter
        };
    }

    private void CreateActionButton(string label, System.Action onClickAction)
    {
        if (GUILayout.Button(label, buttonStyle))
        {
            onClickAction?.Invoke();
        }
    }
}
