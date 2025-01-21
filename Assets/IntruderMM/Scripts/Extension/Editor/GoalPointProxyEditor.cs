#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Assets.IntruderMM.Editor
{
    [CustomEditor(typeof(GoalPointProxy)), CanEditMultipleObjects]
    public class GoalPointProxyEditor : UnityEditor.Editor
    {
        private GoalPointProxy _goalPointProxy;
        private List<BriefcaseProxy> _briefcases = new List<BriefcaseProxy>();

        private void OnEnable()
        {
            _goalPointProxy = (GoalPointProxy)target;
            _briefcases.AddRange(GameObject.FindObjectsOfType<BriefcaseProxy>());
        }

        private void OnDisable()
        {
            _briefcases.Clear();
        }

        private void OnSceneGUI()
        {
            if (Preferences.ShowLines)
            {
                DrawWireDiscs();
            }

            DrawLinesToBriefcases();
        }

        private void DrawWireDiscs()
        {
            Color discColor = Color.blue;
            Vector3 basePosition = _goalPointProxy.transform.position - new Vector3(0, 2.33252f, 0);

            for (int i = 0; i < 20; i++)
            {
                discColor.a = Preferences.LineTransparency;
                Handles.color = discColor;
                Handles.DrawWireDisc(basePosition + (Vector3.up * 0.25f * i), _goalPointProxy.transform.up, 1);
            }
        }

        private void DrawLinesToBriefcases()
        {
            foreach (var briefcase in _briefcases)
            {
                EditorTools.RenderLine(
                    _goalPointProxy.gameObject,
                    briefcase.gameObject,
                    Color.magenta,
                    "",
                    1f,
                    1f
                );
            }
        }
    }
}

#endif
