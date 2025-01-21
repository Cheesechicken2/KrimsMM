#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Assets.IntruderMM.Editor
{
    [CustomEditor(typeof(Activator)), CanEditMultipleObjects]
    public class ActivatorEditor : UnityEditor.Editor
    {
        private Activator activatorTarget;
        private int currentToolbarButton;

        private void OnEnable()
        {
            activatorTarget = (Activator)target;
        }

        public override void OnInspectorGUI()
        {
            if (activatorTarget == null) return;

            serializedObject.Update();

            currentToolbarButton = GUILayout.Toolbar(currentToolbarButton, new[] { "Trigger", "Actions", "Options", "Preferences" });

            switch (currentToolbarButton)
            {
                case 0:
                    DrawTriggerSection();
                    break;
                case 1:
                    DrawActionsSection();
                    break;
                case 2:
                    DrawOptionsSection();
                    break;
                case 3:
                    Preferences.CustomPreferencesGUI();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTriggerSection()
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Triggers", EditorStyles.boldLabel);

            DrawTriggerProperties();

            EditorGUILayout.HelpBox("Trigger Types: You can use multiple! Some require an associated collider to function correctly.", MessageType.Info);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Trigger Options", EditorStyles.boldLabel);

            DrawTriggerOptions();

            EditorGUILayout.HelpBox("Trigger Options: Some require specific trigger types to be enabled.", MessageType.Info);
            GUILayout.EndVertical();
        }

        private void DrawActionsSection()
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Activate Events", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Activator.activateEvent)), new GUIContent("Activate Event", "Event triggered upon activation."), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Activator.resetEvent)), new GUIContent("Reset Event", "Event triggered upon reset."), true);
            GUILayout.EndVertical();

            DrawSubActionSections();

            EditorGUILayout.HelpBox("Actions: Define what happens when the activator is triggered.", MessageType.Info);
        }

        private void DrawOptionsSection()
        {
            DrawCollapsibleSection("Basic Options", () =>
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Activator.activatorTeam)), new GUIContent("Activator Team", "The team that can interact with this activator."));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Activator.delayTime)), new GUIContent("Delay", "Time delay before activation."));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Activator.resetDelay)), new GUIContent("Reset Delay", "Time delay before reset."));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Activator.localOnly)), new GUIContent("Clientsided Activator", "Whether this activator is local to the client."));
            });

            DrawCollapsibleSection("Redo Options", () =>
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Activator.canRedo)), new GUIContent("Can Redo", "Allows reactivation after being triggered."));
                if (activatorTarget.canRedo)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Activator.redoTime)), new GUIContent("Redo Wait Time", "Time to wait before reactivation."));
                }
            });

            DrawCollapsibleSection("Interactable Options", () =>
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Activator.enabledByDefault)), new GUIContent("Visible By Default", "Whether the activator is visible at start."));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Activator.enabledAfterTime)), new GUIContent("Visible After Time", "Time after which the activator becomes visible."));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Activator.allowActivateDelay)), new GUIContent("Interactable After Time", "Time after which the activator becomes interactable."));
            });

            DrawCollapsibleSection("Item Options", () =>
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Activator.neededItems)), new GUIContent("Needed Items", "Items required to activate."), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Activator.dontHaveAllItemsMessage)), new GUIContent("Don't Have Items Message", "Message displayed when items are missing."));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Activator.useUpItems)), new GUIContent("Use Up Items", "Whether the items are consumed upon activation."));
            });
        }

        private void DrawTriggerProperties()
        {
            DrawHorizontalProperties(
                nameof(Activator.triggerByUse), "On Use",
                nameof(Activator.triggerByShoot), "On Shot"
            );

            DrawHorizontalProperties(
                nameof(Activator.triggerByExplosion), "On Explosion",
                nameof(Activator.triggerByEnter), "On Enter"
            );

            DrawHorizontalProperties(
                nameof(Activator.triggerByRagdollEnter), "On Ragdoll Enter",
                nameof(Activator.activateAfterTime), "After Time"
            );
        }

        private void DrawTriggerOptions()
        {
            EditorGUI.BeginDisabledGroup(!activatorTarget.triggerByExplosion);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Activator.explosionMustBeDirectHit)), new GUIContent("Explosion Must Be Direct Hit", "Requires a direct hit for activation."));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Activator.explosionTriggerDistance)), new GUIContent("Explosion Trigger Distance", "Maximum distance for explosion activation."));
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!activatorTarget.triggerByShoot);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Activator.hp)), new GUIContent("Health", "Health of the activator."));
            EditorGUI.EndDisabledGroup();
        }

        private void DrawSubActionSections()
        {
            DrawSection("Animation Actions", new[]
            {
                nameof(Activator.objectsToAnimate),
                nameof(Activator.objectsToStop),
                nameof(Activator.animationNames)
            });

            DrawSection("Object Actions", new[]
            {
                nameof(Activator.objectsToEnable),
                nameof(Activator.objectsToDisable),
                nameof(Activator.randomObjectsToEnable)
            });

            DrawSection("Door Actions", new[]
            {
                nameof(Activator.doorsToUnlock),
                nameof(Activator.doorsToLock),
                nameof(Activator.customDoorsToUnlock),
                nameof(Activator.customDoorsToLock)
            });

            DrawSection("Goal Action", new[]
            {
                nameof(Activator.goal),
                nameof(Activator.goalMessage),
                nameof(Activator.goalWinner)
            });

            DrawSection("Teleport Action", new[]
            {
                nameof(Activator.possibleTeleportDestinations)
            });
        }

        private void DrawHorizontalProperties(string prop1, string label1, string prop2, string label2)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(prop1), new GUIContent(label1));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(prop2), new GUIContent(label2));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSection(string title, string[] properties)
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

            foreach (var property in properties)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(property), new GUIContent(ObjectNames.NicifyVariableName(property), "Tooltip for " + property), true);
            }

            GUILayout.EndVertical();
        }

        private void DrawCollapsibleSection(string title, System.Action drawContent)
        {
            bool foldout = EditorPrefs.GetBool(title, true);
            foldout = EditorGUILayout.Foldout(foldout, title, true);
            EditorPrefs.SetBool(title, foldout);

            if (foldout)
            {
                GUILayout.BeginVertical("Box");
                drawContent();
                GUILayout.EndVertical();
            }
        }

        private void OnSceneGUI()
        {
            RenderSceneGUI(activatorTarget, 1);
        }

        public static void RenderSceneGUI(Activator target, float alpha)
        {
            if (target == null) return;

            GUI.color = new Color(1, 1, 1, alpha);

            RenderLines(target.objectsToEnable, "Enable", Color.green, target, alpha);
            RenderLines(target.objectsToDisable, "Disable", Color.red, target, alpha);
            RenderLines(target.randomObjectsToEnable, "Randomly Enable", Color.cyan, target, alpha);
            RenderLines(target.possibleTeleportDestinations, "Teleport Destination", Color.magenta, target, alpha);
            RenderLines(target.customDoorsToLock, "Lock", Color.yellow, target, alpha);
            RenderLines(target.customDoorsToUnlock, "Unlock", Color.blue, target, alpha);
        }

        private static void RenderLines(Object[] objects, string label, Color color, Activator target, float alpha)
        {
            if (Preferences.EnableObjects)
            {
                EditorTools.DrawLinesToObjects(objects, label, color, target, alpha, 1f);
            }
        }
    }
}

#endif
