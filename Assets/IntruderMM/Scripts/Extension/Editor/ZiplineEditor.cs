#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Assets.IntruderMM.Editor
{
    [CustomEditor(typeof(ZiplineProxy)), CanEditMultipleObjects]
    public class ZiplineEditor : UnityEditor.Editor
    {
        private ZiplineProxy zipline;
        private int currentTab;
        private List<Vector3> pathPoints;
        private GameObject forceDismountPreview;

        private void OnEnable()
        {
            zipline = (ZiplineProxy)target;
            pathPoints = new List<Vector3>();
            UpdateZiplinePath();
            CreateForceDismountPreview();
        }

        private void OnDisable()
        {
            DestroyForceDismountPreview();
        }

        public override void OnInspectorGUI()
        {
            currentTab = GUILayout.Toolbar(currentTab, new string[] { "Zipline Configuration", "Preferences" });

            switch (currentTab)
            {
                case 0:
                    DisplayZiplineConfig();
                    break;

                case 1:
                    Preferences.CustomPreferencesGUI();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DisplayZiplineConfig()
        {
            if (zipline.startPoint == null || zipline.endPoint == null)
            {
                EditorGUILayout.HelpBox("Both start and end points must be assigned.", MessageType.Error);
            }

            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.LabelField("Zipline Endpoints", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("startPoint"), new GUIContent("Start Point"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("endPoint"), new GUIContent("End Point"));
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.LabelField("Zipline Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("zipSpeed"), new GUIContent("Speed"));

                SerializedProperty vertexCountProperty = serializedObject.FindProperty("numberOfVertices");
                SerializedProperty maxSagProperty = serializedObject.FindProperty("maxGravityDangle");
                SerializedProperty forceDismountProperty = serializedObject.FindProperty("forceDismountAtPercent");

                forceDismountProperty.floatValue = Mathf.Clamp(forceDismountProperty.floatValue, 0f, 0.99f);

                float vertexSliderValue = Mathf.Clamp01(vertexCountProperty.intValue / 100f);
                float newVertexSliderValue = EditorGUILayout.Slider("Vertex Count (%)", vertexSliderValue, 0f, 1f) * 100f;
                if (Mathf.Abs(newVertexSliderValue - vertexSliderValue) > 0.1f)
                {
                    vertexCountProperty.intValue = Mathf.RoundToInt(newVertexSliderValue);
                    UpdateZiplinePath();
                    EditorApplication.QueuePlayerLoopUpdate();
                }

                float maxSagSliderValue = Mathf.Clamp01(maxSagProperty.floatValue / 100f);
                float newMaxSagSliderValue = EditorGUILayout.Slider("Max Sag (%)", maxSagSliderValue, 0f, 1f) * 100f;
                if (Mathf.Abs(newMaxSagSliderValue - maxSagSliderValue) > 0.1f)
                {
                    maxSagProperty.floatValue = newMaxSagSliderValue;
                    UpdateZiplinePath();
                    EditorApplication.QueuePlayerLoopUpdate();
                }

                forceDismountProperty.floatValue = EditorGUILayout.Slider("Force Dismount (%)", forceDismountProperty.floatValue, 0f, 1f);
            }
            EditorGUILayout.EndVertical();
        }

        private void UpdateZiplinePath()
        {
            if (zipline.startPoint == null || zipline.endPoint == null)
            {
                Debug.LogWarning("Cannot preview zipline: Start or End point is missing.");
                return;
            }

            pathPoints.Clear();
            Vector3 start = zipline.startPoint.transform.position;
            Vector3 end = zipline.endPoint.transform.position;
            int totalVertices = Mathf.Max(2, zipline.numberOfVertices);
            float sagAmount = zipline.maxGravityDangle;

            float step = 1f / (totalVertices - 1);

            for (int i = 0; i < totalVertices; i++)
            {
                float t = i * step;
                Vector3 point = Vector3.Lerp(start, end, t);
                point.y -= Mathf.Sin(t * Mathf.PI) * sagAmount;
                pathPoints.Add(point);
            }

            UpdateForceDismountPreview();
            SceneView.RepaintAll();
        }

        private void CreateForceDismountPreview()
        {
            if (forceDismountPreview == null)
            {
                var model = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/IntruderMM/Scripts/Extension/Models/ZiplineGuard.fbx");
                if (model != null)
                {
                    forceDismountPreview = PrefabUtility.InstantiatePrefab(model) as GameObject;
                    forceDismountPreview.name = "ForceDismountPreview";
                    forceDismountPreview.hideFlags = HideFlags.HideAndDontSave;
                }
                else
                {
                    Debug.LogWarning("Force dismount model could not be loaded. Check the path: Assets/IntruderMM/Scripts/Extension/Models/ZiplineGuard.fbx");
                }
            }
        }

        private void UpdateForceDismountPreview()
        {
            if (forceDismountPreview != null && zipline.startPoint != null && zipline.endPoint != null)
            {
                float ratio = Mathf.Clamp01(zipline.forceDismountAtPercent);
                Vector3 start = zipline.startPoint.transform.position;
                Vector3 end = zipline.endPoint.transform.position;
                Vector3 position = Vector3.Lerp(start, end, ratio);

                position.y -= Mathf.Sin(ratio * Mathf.PI) * zipline.maxGravityDangle;
                position.y -= 2.0f;

                forceDismountPreview.transform.position = position;

                Vector3 directionToFace;
                if (pathPoints.Count > 1)
                {
                    int segmentIndex = Mathf.FloorToInt(ratio * (pathPoints.Count - 1));
                    segmentIndex = Mathf.Clamp(segmentIndex, 0, pathPoints.Count - 2);

                    Vector3 pointA = pathPoints[segmentIndex];
                    Vector3 pointB = pathPoints[segmentIndex + 1];

                    directionToFace = (pointB - pointA).normalized;
                }
                else
                {
                    directionToFace = (end - start).normalized;
                }

                directionToFace.y = 0;
                directionToFace.Normalize();

                forceDismountPreview.transform.rotation = Quaternion.LookRotation(directionToFace);

                forceDismountPreview.transform.rotation *= Quaternion.Euler(-90, 0, 0);
            }
        }

        private void DestroyForceDismountPreview()
        {
            if (forceDismountPreview != null)
            {
                DestroyImmediate(forceDismountPreview);
            }
        }

        private void OnSceneGUI()
        {
            if (zipline.startPoint == null || zipline.endPoint == null)
            {
                return;
            }

            Tools.current = Tool.None;

            if (pathPoints.Count > 0)
            {
                Handles.color = Color.blue;
                for (int i = 1; i < pathPoints.Count; i++)
                {
                    Handles.DrawLine(pathPoints[i - 1], pathPoints[i]);
                    Handles.SphereHandleCap(0, pathPoints[i - 1], Quaternion.identity, 0.1f, EventType.Repaint);
                }
                Handles.SphereHandleCap(0, pathPoints[pathPoints.Count - 1], Quaternion.identity, 0.1f, EventType.Repaint);
            }

            EditorGUI.BeginChangeCheck();
            Vector3 newStartPosition = Handles.PositionHandle(zipline.startPoint.transform.position, zipline.startPoint.transform.rotation);
            Vector3 newEndPosition = Handles.PositionHandle(zipline.endPoint.transform.position, zipline.endPoint.transform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(zipline, "Modified Zipline Points");
                zipline.startPoint.transform.position = newStartPosition;
                zipline.endPoint.transform.position = newEndPosition;
                UpdateZiplinePath();
            }
        }
    }
}

#endif
