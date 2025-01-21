#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Assets.IntruderMM.Editor
{
    [CustomEditor(typeof(ActivateActivator)), CanEditMultipleObjects]
    public class ActivateActivatorEditor : UnityEditor.Editor
    {
        private ActivateActivator activateActivatorTarget;
        private SerializedProperty activatorsProperty;

        private int selectedTab;

        private static readonly string[] TabLabels = { "Activator Settings", "Preferences" };

        private void OnEnable()
        {
            activateActivatorTarget = (ActivateActivator)target;
            activatorsProperty = serializedObject.FindProperty("activators");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawToolbar();

            if (selectedTab == 0)
            {
                RenderActivatorSettings();
            }
            else if (selectedTab == 1)
            {
                Preferences.CustomPreferencesGUI();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawToolbar()
        {
            selectedTab = GUILayout.Toolbar(selectedTab, TabLabels);
        }

        private void RenderActivatorSettings()
        {
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.PropertyField(activatorsProperty, new GUIContent("Activators", "List of objects to activate"), true);

            if (IsActivatorListInvalid())
            {
                EditorGUILayout.HelpBox("Please add valid activators to the list.", MessageType.Warning);
            }

            EditorGUILayout.HelpBox("Use Unity Events or call ActivateAll() / ActivateIndex(int index) to trigger activators.", MessageType.Info);

            EditorGUILayout.EndVertical();
        }

        private bool IsActivatorListInvalid()
        {
            return activateActivatorTarget.activators == null || activateActivatorTarget.activators.Length == 0 || activateActivatorTarget.activators[0] == null;
        }

        private void OnSceneGUI()
        {
            DrawActivatorHandles();

            if (ShouldRenderNestedActivators())
            {
                foreach (var activator in activateActivatorTarget.activators)
                {
                    if (activator != null)
                    {
                        ActivatorEditor.RenderSceneGUI(activator, EditorTools.SUB_ACTIVATOR_ALPHA);
                    }
                }
            }
        }

        private void DrawActivatorHandles()
        {
            Handles.SphereHandleCap(0, activateActivatorTarget.transform.position, activateActivatorTarget.transform.rotation, 0.125f, EventType.Repaint);

            if (activateActivatorTarget.activators != null && activateActivatorTarget.activators.Length > 0)
            {
                EditorTools.DrawLinesToObjects(activateActivatorTarget.activators, "Activator Link", Color.cyan, activateActivatorTarget, 1f, 1f);
            }
        }

        private bool ShouldRenderNestedActivators()
        {
            return Preferences.ShowNestedActivator && activateActivatorTarget.activators != null;
        }
    }
}

#endif
