using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class NudgeZone : MonoBehaviour
{
    [Tooltip("Nudge speed")]
    public float speed = 10f;

    [Tooltip("How quickly to accelerate to max speed")]
    public float lerpSpeed = 10f;

    [Tooltip("(REQUIRED) Assign a transform here with the blue Z-axis (forward) facing the direction you want to nudge")]
    public Transform directionTransform;

    [Tooltip("Speed applied to ragdoll objects")]
    public float ragSpeed = 100f;

    [Tooltip("Torque applied to ragdoll objects")]
    public Vector3 ragTorque;

    [Tooltip("(OPTIONAL) Assign a transform for the ragdoll direction")]
    public Transform ragDirectionTransform;

    [Tooltip("Will nudge the opposite direction if the character is behind the Direction Transform, useful for thin objects like the tops of doors")]
    public bool twoWay = false;

    [Tooltip("(OPTIONAL) Two Way will use this transform forward direction if behind the Direction Transform")]
    public Transform twoWayDirectionTransform;

    [Tooltip("(OPTIONAL) weeeeeee")]
    public bool setYVelocity;

    [Tooltip("Allows player movement during the nudge")]
    public bool allowPlayerMovement;
    private void OnDrawGizmosSelected()
    {
        if (directionTransform == null)
        {
            return;
        }

        float speedVectorSize = speed * 0.25f;

        Gizmos.color = UnityEngine.Color.magenta;
        Gizmos.DrawRay(directionTransform.position, directionTransform.forward * speedVectorSize);

        if (twoWay)
        {
            Gizmos.color = UnityEngine.Color.yellow;

            if (twoWayDirectionTransform)
            {
                Gizmos.DrawRay(twoWayDirectionTransform.position, twoWayDirectionTransform.forward * speedVectorSize);
            }
            else
            {
                Gizmos.DrawRay(directionTransform.position, -directionTransform.forward * speedVectorSize);
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(NudgeZone))]
    public class NudgeZoneEditor : Editor
    {
        private SerializedProperty speedProp;
        private SerializedProperty lerpSpeedProp;
        private SerializedProperty directionTransformProp;
        private SerializedProperty ragSpeedProp;
        private SerializedProperty ragTorqueProp;
        private SerializedProperty ragDirectionTransformProp;
        private SerializedProperty twoWayProp;
        private SerializedProperty twoWayDirectionTransformProp;
        private SerializedProperty setYVelocityProp;
        private SerializedProperty allowPlayerMovementProp;

        private void OnEnable()
        {
            speedProp = serializedObject.FindProperty("speed");
            lerpSpeedProp = serializedObject.FindProperty("lerpSpeed");
            directionTransformProp = serializedObject.FindProperty("directionTransform");
            ragSpeedProp = serializedObject.FindProperty("ragSpeed");
            ragTorqueProp = serializedObject.FindProperty("ragTorque");
            ragDirectionTransformProp = serializedObject.FindProperty("ragDirectionTransform");
            twoWayProp = serializedObject.FindProperty("twoWay");
            twoWayDirectionTransformProp = serializedObject.FindProperty("twoWayDirectionTransform");
            setYVelocityProp = serializedObject.FindProperty("setYVelocity");
            allowPlayerMovementProp = serializedObject.FindProperty("allowPlayerMovement");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Nudge Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(speedProp);
            EditorGUILayout.PropertyField(lerpSpeedProp);
            EditorGUILayout.PropertyField(directionTransformProp);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Ragdoll Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(ragSpeedProp);
            EditorGUILayout.PropertyField(ragTorqueProp);
            EditorGUILayout.PropertyField(ragDirectionTransformProp);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(twoWayProp);

            if (twoWayProp.boolValue)
            {
                EditorGUILayout.PropertyField(twoWayDirectionTransformProp);
            }

            EditorGUILayout.PropertyField(setYVelocityProp);
            EditorGUILayout.PropertyField(allowPlayerMovementProp);

            if (setYVelocityProp.boolValue)
            {
                EditorGUILayout.HelpBox("Set Y Velocity is enabled. Make sure you didnt mess up.", MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
        }
    } 
#endif


