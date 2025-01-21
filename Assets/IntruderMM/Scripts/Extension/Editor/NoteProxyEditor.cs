#if UNITY_EDITOR

using static UnityEngine.GraphicsBuffer;
using UnityEditor;
using UnityEngine;

namespace Assets.IntruderMM.Editor
{
    [CustomEditor(typeof(NoteProxy))]
    public class NoteProxyEditor : UnityEditor.Editor
    {
        private NoteProxy noteProxyTarget;
        private SerializedProperty messageProperty;
        private SerializedProperty activatorToActivateProperty;
        private GUISkin customSkin;

        private void OnEnable()
        {
            noteProxyTarget = (NoteProxy)target;
            messageProperty = serializedObject.FindProperty("message");
            activatorToActivateProperty = serializedObject.FindProperty("activatorToActivate");

            noteProxyTarget.UpdatePages();

            TrackSelection(true);

            customSkin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/IntruderMM/Scripts/Extension/Editor/GUI/IntruderSkin.guiskin");
        }

        private void OnDisable()
        {
            TrackSelection(false);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (customSkin != null)
            {
                GUI.skin = customSkin;
            }

            EditorGUILayout.HelpBox("Unity's StyledText allows you to format text with tags like <b> for bold, <i> for italics, <color=#ff0000> for color, etc. For more information, visit: https://docs.unity3d.com/Packages/com.unity3d.com/ugui@1.0/manual/StyledText.html", MessageType.Info);

            EditorGUILayout.PropertyField(messageProperty, new GUIContent("Note Message", "The string displayed when the note is read by the player."));
            EditorGUILayout.PropertyField(activatorToActivateProperty, new GUIContent("Activate Activator", "The activator that gets triggered when the player starts reading the note."));

            DrawPageNavigation();

            serializedObject.ApplyModifiedProperties();
        }

        private void TrackSelection(bool isSelected)
        {
            if (isSelected)
            {
                EnableText();
            }
            else
            {
                DisableText();
            }
        }

        private void DisableText()
        {
            Transform textTransform = noteProxyTarget.transform.Find("Text");
            if (textTransform != null)
            {
                textTransform.gameObject.SetActive(false);
            }
        }

        private void EnableText()
        {
            Transform textTransform = noteProxyTarget.transform.Find("Text");
            if (textTransform != null)
            {
                textTransform.gameObject.SetActive(true);
            }
        }

        private void DrawPageNavigation()
        {
            if (noteProxyTarget.pages.Length == 0)
                return;

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Previous Page"))
            {
                if (noteProxyTarget.currentPage > 0)
                {
                    noteProxyTarget.currentPage--;
                    noteProxyTarget.UpdatePreviewText();
                    ForceReSelectObject();
                }
            }

            if (GUILayout.Button("Next Page"))
            {
                if (noteProxyTarget.currentPage < noteProxyTarget.pages.Length - 1)
                {
                    noteProxyTarget.currentPage++;
                    noteProxyTarget.UpdatePreviewText();
                    ForceReSelectObject();
                }
            }

            GUILayout.EndHorizontal();

            if (customSkin != null && customSkin.label != null)
            {
                GUIStyle labelStyle = new GUIStyle(customSkin.label)
                {
                    alignment = TextAnchor.MiddleCenter
                };
                GUILayout.Label($"Page {noteProxyTarget.currentPage + 1} of {noteProxyTarget.pages.Length}", labelStyle);
            }
            else
            {
                GUILayout.Label($"Page {noteProxyTarget.currentPage + 1} of {noteProxyTarget.pages.Length}", EditorStyles.centeredGreyMiniLabel);
            }
        }

        private void ForceReSelectObject()
        {
            Selection.activeObject = noteProxyTarget;
            EditorUtility.SetDirty(noteProxyTarget);
        }

        private void OnSceneGUI()
        {
            KeypadProxy[] keypads = FindObjectsOfType<KeypadProxy>();
            foreach (var keypad in keypads)
            {
                if (keypad.myNote == noteProxyTarget)
                {
                    EditorTools.DrawLinesToObjects(
                        new Object[] { keypad.gameObject },
                        "Passcode Text on " + keypad.name, 
                        Color.green,
                        noteProxyTarget,
                        1f, 
                        2f 
                    );
                }
            }
        }

    }
}
#endif