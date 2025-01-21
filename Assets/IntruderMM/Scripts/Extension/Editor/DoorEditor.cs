#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.IntruderMM.Editor
{
    [CustomEditor(typeof(CustomDoorProxy)), CanEditMultipleObjects]
    public class DoorEditor : UnityEditor.Editor
    {
        private CustomDoorProxy doorTarget;
        private Color lineColor;
        private Vector3 hingeLocalPos;
        private bool transformCached;

        private enum DoorMode { Hinged, Sliding }
        private enum LockState { Locked, Unlocked, Random }

        private int currentToolbarButton;
        private DoorMode doorMode;
        private LockState lockState;

        private bool showDoorSettings = true;
        private bool showLockSettings = true;

        private GameObject previousHinge;

        private void OnEnable()
        {
            doorTarget = (CustomDoorProxy)target;
            if (doorTarget.doorHinge != null)
            {
                hingeLocalPos = doorTarget.doorHinge.transform.localPosition;
                transformCached = true;
            }

            doorMode = doorTarget.slidingDoor ? DoorMode.Sliding : DoorMode.Hinged;
            EditorSceneManager.sceneSaving += OnSceneSaving;

            SetLockState();
        }

        private void OnDisable()
        {
            if (doorTarget == null) return;

            ResetTransforms();
            EditorSceneManager.sceneSaving -= OnSceneSaving;
        }

        private void OnSceneSaving(Scene scene, string path)
        {
            ResetTransforms();
        }

        private void SetLockState()
        {
            if (doorTarget == null) return;

            if (doorTarget.alwaysLock && !doorTarget.neverLock)
                lockState = LockState.Locked;
            else if (doorTarget.neverLock && !doorTarget.alwaysLock)
                lockState = LockState.Unlocked;
            else
                lockState = LockState.Random;
        }

        private void ResetTransforms()
        {
            if (doorTarget?.doorHinge == null) return;

            doorTarget.doorHinge.transform.localRotation = Quaternion.Euler(Vector3.zero);
            if (doorTarget.slidingDoor) doorTarget.doorHinge.transform.localPosition = Vector3.zero;
        }

        public override void OnInspectorGUI()
        {
            // Toolbar for toggling sections
            currentToolbarButton = GUILayout.Toolbar(currentToolbarButton, new string[] { "Door Settings", "Preferences" });

            switch (currentToolbarButton)
            {
                case 0:
                    DrawDoorSettings();
                    DrawLockSettings();
                    break;
                case 1:
                    Preferences.CustomPreferencesGUI();
                    break;
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDoorSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Door Settings", EditorStyles.boldLabel);

            showDoorSettings = EditorGUILayout.Foldout(showDoorSettings, "Configure Door", true);
            if (showDoorSettings)
            {
                doorMode = (DoorMode)EditorGUILayout.EnumPopup("Mode", doorMode);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("doorHinge"), new GUIContent("Hinge"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("partnerDoor"), new GUIContent("Partner Door"));

                if (doorMode == DoorMode.Hinged)
                {
                    DrawHingedSettings();
                }
                else
                {
                    DrawSlidingSettings();
                }
            }
            GUILayout.EndVertical();
        }

        private void DrawHingedSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Hinged Settings", EditorStyles.boldLabel);
            doorTarget.reverse = EditorGUILayout.Toggle("Invert Rotation", doorTarget.reverse);
            doorTarget.maxDoorAngle = EditorGUILayout.IntSlider("Max Angle", doorTarget.maxDoorAngle, 0, 270);
            doorTarget.startOpenPercent = EditorGUILayout.Slider("Start Open (%)", doorTarget.startOpenPercent, 0.0f, 1.0f);
            GUILayout.EndVertical();

        }

        private void DrawSlidingSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Sliding Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("slideDistance"), new GUIContent("Max Slide Distance"));
            EditorGUILayout.HelpBox("Ensure the hinge pivot is at (0, 0, 0) for proper sliding functionality.", MessageType.Info);
            GUILayout.EndVertical();
        }

        private void DrawLockSettings()
        {
            GUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Lock Settings", EditorStyles.boldLabel);

            showLockSettings = EditorGUILayout.Foldout(showLockSettings, "Configure Lock", true);
            if (showLockSettings)
            {
                lockState = (LockState)EditorGUILayout.EnumPopup("Lock State", lockState);
                SetLockStateProperties();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("canLockPick"), new GUIContent("Can Be Lock Picked"));
            }
            GUILayout.EndVertical();
        }

        private void SetLockStateProperties()
        {
            var door = (CustomDoorProxy)target;

            switch (lockState)
            {
                case LockState.Locked:
                    door.alwaysLock = true;
                    door.neverLock = false;
                    break;
                case LockState.Unlocked:
                    door.alwaysLock = false;
                    door.neverLock = true;
                    break;
                default:
                    door.alwaysLock = false;
                    door.neverLock = false;
                    break;
            }

            EditorUtility.SetDirty(door);
        }


        private void OnSceneGUI()
        {
            lineColor = Color.white;

            if (doorTarget.doorHinge == null && previousHinge != null)
            {
                previousHinge.transform.localPosition = hingeLocalPos;
                previousHinge.transform.localRotation = Quaternion.Euler(0, 0, 0);
                return;
            }

            if (doorTarget.doorHinge != null && !transformCached)
            {
                hingeLocalPos = doorTarget.doorHinge.transform.localPosition;
            }

            if (doorTarget.doorHinge == null) return;

            if (previousHinge != doorTarget.doorHinge) previousHinge = doorTarget.doorHinge;

            DrawHandles();
        }

        private void DrawHandles()
        {
            if (Preferences.ShowBalls)
            {
                lineColor.a = Preferences.BallTransparency;
                Handles.color = lineColor;
                Handles.SphereHandleCap(0, doorTarget.doorHinge.transform.position, doorTarget.doorHinge.transform.rotation, 0.0625f, EventType.Repaint);
            }

            Vector3 target = doorTarget.transform.position;
            EditorTools.RenderWorldLabel(doorTarget.slidingDoor ? $"Slide Distance: {doorTarget.slideDistance}" : $"Max Rotation Angle: {doorTarget.maxDoorAngle}", target + new Vector3(0, 1, 0), EditorTools.labelSceneGUI, 1);
            EditorTools.RenderWorldLabel("Door Hinge Point", target, EditorTools.labelSceneGUI, 1);

            if (Preferences.ShowDoorPath)
            {
                DrawDoorPath();
            }
            else
            {
                ResetTransforms();
            }
        }

        private void DrawDoorPath()
        {
            if (Preferences.ShowLines)
            {
                SetLineColor();

                if (doorTarget.slidingDoor)
                {
                    DrawSlidingDoorPath();
                }
                else
                {
                    DrawHingedPath();
                }
            }

            UpdateHingeTransform();
        }

        private void SetLineColor()
        {
            lineColor.a = Preferences.LineTransparency;
            Handles.color = lineColor;
        }

        private void DrawSlidingDoorPath()
        {
            Handles.DrawLine(doorTarget.transform.position, doorTarget.doorHinge.transform.position);
        }

        private void DrawHingedPath()
        {
            Vector3 hingePosition = new Vector3(doorTarget.doorHinge.transform.position.x, doorTarget.transform.position.y, doorTarget.doorHinge.transform.position.z);
            float angle = doorTarget.maxDoorAngle * (doorTarget.reverse ? -1 : 1);

            for (int i = 0; i < 8; i++)
            {
                Handles.DrawWireArc(hingePosition, doorTarget.transform.up, doorTarget.transform.right, angle, 0.625f / 5 * i);
            }
            Handles.color = new Color(1f, 0.647f, 0f, 0.25f);
            Handles.DrawSolidArc(hingePosition, doorTarget.transform.up, doorTarget.transform.right, angle, 1f);
        }

        private void UpdateHingeTransform()
        {
            if (doorTarget.slidingDoor)
            {
                doorTarget.doorHinge.transform.localRotation = Quaternion.Euler(0, 0, 0);
                doorTarget.doorHinge.transform.localPosition = new Vector3(doorTarget.slideDistance.x, doorTarget.doorHinge.transform.localPosition.y, doorTarget.doorHinge.transform.localPosition.z);
            }
            else
            {
                doorTarget.doorHinge.transform.localPosition = hingeLocalPos;
                doorTarget.doorHinge.transform.localRotation = Quaternion.Euler(0, doorTarget.maxDoorAngle * (doorTarget.reverse ? -1 : 1), 0);
            }
        }
    }
}
#endif
