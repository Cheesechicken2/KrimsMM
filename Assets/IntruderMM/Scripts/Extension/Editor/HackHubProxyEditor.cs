#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Assets.IntruderMM.Editor
{
    [CustomEditor(typeof(HackHubProxy))]
    [CanEditMultipleObjects]
    public class HackHubProxyEditor : UnityEditor.Editor
    {
        private HackHubProxy _hackHubTarget;

        private int _currentToolbarIndex;

        private void OnEnable()
        {
            _hackHubTarget = (HackHubProxy)target;
        }

        public override void OnInspectorGUI()
        {
            _currentToolbarIndex = GUILayout.Toolbar(_currentToolbarIndex, new[] { "Hack Hub Proxy", "Preferences" });

            switch (_currentToolbarIndex)
            {
                case 0:
                    DrawHackHubProxySection();
                    break;
                case 1:
                    Preferences.CustomPreferencesGUI();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHackHubProxySection()
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Hack Hub Proxy", EditorStyles.boldLabel);

            DrawProperty(nameof(HackHubProxy.hackGoal), "Hack Goal", "How many times this hub needs to be hacked");
            DrawIndentedProperty(nameof(HackHubProxy.nodes), "Nodes", "The nodes associated with this hub");
            DrawProperty(nameof(HackHubProxy.autoGrabNodesFromChildren), "Auto Grab Nodes from Children", "Automatically grab nodes from child objects");
            DrawProperty(nameof(HackHubProxy.autoNameChildNodes), "Auto Name Child Nodes", "Automatically name child nodes");
            DrawProperty(nameof(HackHubProxy.baseNodeName), "Base Node Name", "The base name for the nodes");

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);

            DrawProperty(nameof(HackHubProxy.hackCompleteActivator), "Hack Complete Activator", "Called when the hack hub is fully hacked");
            DrawProperty(nameof(HackHubProxy.hackCompleteEvent), "Hack Complete Event", "Event triggered when the hack hub is fully hacked");
            DrawProperty(nameof(HackHubProxy.roundResetEvent), "Round Reset Event", "Event triggered on round reset");
            DrawProperty(nameof(HackHubProxy.showHackCompleteTransmitWarning), "Hack Complete Transmit Warning", "Show a warning light during hack completion");

            EditorGUILayout.EndVertical();
        }

        private void DrawProperty(string propertyName, string label, string tooltip)
        {
            var property = serializedObject.FindProperty(propertyName);
            EditorGUILayout.PropertyField(property, new GUIContent(label, tooltip), true);
        }


        private void DrawIndentedProperty(string propertyName, string label, string tooltip)
        {
            EditorGUI.indentLevel++;
            DrawProperty(propertyName, label, tooltip);
            EditorGUI.indentLevel--;
        }

        private void OnSceneGUI()
        {
            var nodes = _hackHubTarget.autoGrabNodesFromChildren
                ? _hackHubTarget.GetComponentsInChildren<HackNodeProxy>()
                : _hackHubTarget.nodes;

            EditorTools.DrawLinesToObjects(nodes, "Terminal ", Color.cyan, _hackHubTarget, 1f, 1f);
        }
    }
}

#endif