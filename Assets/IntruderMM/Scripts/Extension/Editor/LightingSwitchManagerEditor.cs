using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LightingSwitchManager))]
public class LightingSwitchManagerEditor : Editor
{
    private GUIStyle buttonStyle;
    private GUIStyle headerStyle;

    private GUISkin customSkin;
    private Font customFont;

    public override void OnInspectorGUI()
    {
        LightingSwitchManager manager = (LightingSwitchManager)target;

        InitializeStyles();

        EditorGUILayout.Space();

        manager.currentState = EditorGUILayout.IntField("Current State", manager.currentState);

        SerializedProperty lightingSwitchGroupsProp = serializedObject.FindProperty("lightingSwitchGroups");
        EditorGUILayout.PropertyField(lightingSwitchGroupsProp, true);

        SerializedProperty objectsToSetAsStaticProp = serializedObject.FindProperty("objectsToSetAsStatic");
        EditorGUILayout.PropertyField(objectsToSetAsStaticProp, true);

        SerializedProperty reflectionProbesProp = serializedObject.FindProperty("reflectionProbes");
        EditorGUILayout.PropertyField(reflectionProbesProp, true);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Actions", headerStyle);

        if (GUILayout.Button("Update Lighting Switch Groups", buttonStyle))
        {
            manager.UpdateLightingSwitchGroups();
            EditorUtility.SetDirty(manager);
        }

        if (GUILayout.Button("Reset States", buttonStyle))
        {
            manager.ResetStates();
            EditorUtility.SetDirty(manager);
        }

        serializedObject.ApplyModifiedProperties();
        Repaint();
    }

    private void InitializeStyles()
    {
        customSkin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/IntruderMM/Scripts/Extension/Editor/GUI/IntruderSkin.guiskin");
        customFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/IntruderMM/Scripts/Extension/Editor/GUI/Font/ShareTechMono-Regular.ttf");

        if (customSkin != null)
        {
            buttonStyle = new GUIStyle(customSkin.button)
            {
                fontSize = 15,
                font = customFont
            };
        }
        else
        {
            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 15,
                font = customFont
            };
        }

        headerStyle = new GUIStyle(EditorStyles.label)
        {
            font = customFont,
            fontSize = 15,
            alignment = TextAnchor.MiddleCenter
        };
    }
}
