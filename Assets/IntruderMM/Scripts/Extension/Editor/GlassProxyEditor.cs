#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Assets.IntruderMM.Editor
{
    [CustomEditor(typeof(GlassProxy)), CanEditMultipleObjects]
    public class GlassProxyEditor : UnityEditor.Editor
    {
        private int currentToolbarButton;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            currentToolbarButton = GUILayout.Toolbar(currentToolbarButton, new string[] { "Glass Proxy", "Preferences" });

            EditorGUILayout.Space();

            switch (currentToolbarButton)
            {
                case 0:
                    DrawGlassProxySettings();
                    break;

                case 1:
                    Preferences.CustomPreferencesGUI();
                    break;

                default:
                    Debug.LogWarning("Unknown toolbar index");
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGlassProxySettings()
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Glass Proxy Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("hp"),
                new GUIContent("Health", "How many times the glass needs to get shot to break")
            );

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("showBrokenEdgeShards"),
                new GUIContent("Show Broken Edge Shards", "Show the edge shards after breaking?")
            );

            EditorGUILayout.EndVertical();
        }
    }
}

#endif
