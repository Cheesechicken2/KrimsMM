using UnityEngine;
using UnityEditor;

namespace Assets.IntruderMM.Editor
{
    [CustomEditor(typeof(CompassDirection))]
    public class CompassDirectionEditor : UnityEditor.Editor
    {
        private CompassDirection _compassDirectionTarget;

        private void OnEnable()
        {
            _compassDirectionTarget = (CompassDirection)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("The Z Axis = North", MessageType.Info);
            DrawDefaultInspector();
        }

        private void OnSceneGUI()
        {
            if (_compassDirectionTarget == null) return;

            Handles.color = Color.magenta;

            Handles.ArrowHandleCap(
                controlID: 0,
                position: _compassDirectionTarget.transform.position,
                rotation: Quaternion.Euler(_compassDirectionTarget.transform.localRotation.eulerAngles),
                size: 1f,
                eventType: EventType.Repaint
            );

            Handles.color = Color.white;
        }
    }
}
