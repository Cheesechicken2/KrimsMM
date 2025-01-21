#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Assets.IntruderMM.Editor
{
    [CustomEditor(typeof(KeypadProxy)), CanEditMultipleObjects]
    public class KeypadProxyEditor : UnityEditor.Editor
    {
        private KeypadProxy keypadTarget;
        private int selectedTab;

        private void OnEnable()
        {
            keypadTarget = (KeypadProxy)target;
        }

        public override void OnInspectorGUI()
        {
            selectedTab = GUILayout.Toolbar(selectedTab, new string[] { "Keypad Proxy", "Preferences" });

            switch (selectedTab)
            {
                case 0:
                    DrawKeypadProxySection();
                    break;

                case 1:
                    Preferences.CustomPreferencesGUI();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawKeypadProxySection()
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Keypad Passcode Options", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("myNote"), new GUIContent("Passcode Note", "Note where the passcode is displayed. Use {0} to specify where the code appears"));
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("otherNotes"), new GUIContent("Additional Passcode Notes", "Similar to the main passcode note but for additional notes"), true);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Activator Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("myActivator"), new GUIContent("Activator Object", "Object that gets activated upon keypad unlock"));
            EditorGUILayout.HelpBox("You'd be better off using an activator for unlocking actions.", MessageType.Info);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Unlocking Options", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("myDoorProxy"), new GUIContent("Unlock Door", "Unlocks the linked door when the keypad is unlocked"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("myCustomDoor"), new GUIContent("Unlock Custom Door", "Unlocks a custom door on unlock"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lockedObject"), new GUIContent("Unlock GameObject", "Unlocks a GameObject when the keypad is unlocked"));
            EditorGUILayout.EndVertical();
        }

        private void OnSceneGUI()
        {
            Handles.SphereHandleCap(0, keypadTarget.transform.position, keypadTarget.transform.rotation, 0.125f, EventType.Repaint);

            DrawConnectionLines();

            EditorTools.DrawLinesToObjects(keypadTarget.otherNotes, "Passcode Notes on ", Color.cyan, keypadTarget, 1, 3f);
        }

        private void DrawConnectionLines()
        {
            if (keypadTarget.myCustomDoor != null)
                EditorTools.RenderLine(keypadTarget.gameObject, keypadTarget.myCustomDoor.gameObject, Color.red, "Unlocks ", 1, 2f);
            if (keypadTarget.myDoorProxy != null)
                EditorTools.RenderLine(keypadTarget.gameObject, keypadTarget.myDoorProxy.gameObject, Color.red, "Unlocks ", 1, 2f);

            if (keypadTarget.lockedObject != null)
                EditorTools.RenderLine(keypadTarget.gameObject, keypadTarget.lockedObject.gameObject, Color.red, "Unlocks ", 1, 2f);

            if (keypadTarget.myNote != null)
                EditorTools.RenderLine(keypadTarget.gameObject, keypadTarget.myNote.gameObject, Color.red, "Passcode Text on ", 1, 2f);

            if (keypadTarget.myActivator != null)
            {
                EditorTools.RenderLine(keypadTarget.gameObject, keypadTarget.myActivator.gameObject, Color.red, "Activates ", 1, 2f);
                if (Preferences.ShowNestedActivator)
                {
                    ActivatorEditor.RenderSceneGUI(keypadTarget.myActivator, EditorTools.SUB_ACTIVATOR_ALPHA);
                }
            }
        }
    }
}

#endif
