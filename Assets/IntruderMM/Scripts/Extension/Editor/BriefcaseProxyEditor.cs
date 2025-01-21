#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Assets.IntruderMM.Editor
{
    [CustomEditor(typeof(BriefcaseProxy)), CanEditMultipleObjects]
    public class BriefcaseProxyEditor : UnityEditor.Editor
    {
        private BriefcaseProxy briefcaseTarget;
        private List<GoalPointProxy> goalPoints = new List<GoalPointProxy>();

        private void OnEnable()
        {
            briefcaseTarget = (BriefcaseProxy)target;

            // hunt every goalpoint down
            goalPoints.Clear();
            foreach (GoalPointProxy item in GameObject.FindObjectsOfType<GoalPointProxy>())
            {
                goalPoints.Add(item);
            }
        }

        private void OnSceneGUI()
        {
            if (briefcaseTarget == null || goalPoints == null) return;

            foreach (GoalPointProxy item in goalPoints)
            {
                if (item != null)
                {
                    EditorTools.RenderLine(briefcaseTarget.gameObject, item.gameObject, Color.cyan, "", 1f, 1f);
                }
            }
        }
    }
}

#endif
