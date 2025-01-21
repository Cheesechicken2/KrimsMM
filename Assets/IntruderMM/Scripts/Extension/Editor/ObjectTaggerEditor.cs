#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Assets.IntruderMM.Editor
{
    [CustomEditor(typeof(ObjectTagger)), CanEditMultipleObjects]
    public class ObjectTaggerEditor : UnityEditor.Editor
    {
        private ObjectTagger taggerTarget;
        private int tagsChoiceIndex = 0;
        private int layersChoiceIndex = 0;

        private static readonly string[] tags = new[]
        {
            "Untagged", "PrefabPools", "Enemy", "Door", "Elevator", "Glass", "Slope", "Stairs", "Metal",
            "AIPath", "Slippery", "SuperSlippery", "Water", "Dirt", "Carpet", "Movable", "Tire", "ThickMetal",
            "Deathzone", "MainLevel", "Destructible", "Ladder", "Mover", "SteepSlope"
        };

        private static readonly string[] layers = new[]
        {
            "Default", "TransparentFX", "Ignore Raycast", "Water", "UI", "Doors", "MyCharGraphics", "MyFPGraphics",
            "CharControllers", "OPFPGraphics", "DoorTrigger", "Glass", "Screen", "MyStandHitbox", "IgnorePlayer",
            "IgnoreBullet", "Hitbox", "Projectile", "Special", "Rooms", "Lights", "Plants", "Terrain", "Terrain2",
            "OnlyHitLevel", "IgnoreViewCast", "IgnoreMeshMerge"
        };

        private string oldTag;
        private string oldLayer;

        private void OnEnable()
        {
            taggerTarget = (ObjectTagger)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("This isn't required anymore, you can set tags and layers the regular way by changing them at the top.", MessageType.Warning);

            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Tags and Layers List", EditorStyles.boldLabel);

            tagsChoiceIndex = EditorGUILayout.Popup("Object Tag List", tagsChoiceIndex, tags);
            layersChoiceIndex = EditorGUILayout.Popup("Object Layer List", layersChoiceIndex, layers);

            EditorGUILayout.EndVertical();
            if (oldTag != taggerTarget.objectTag)
            {
                tagsChoiceIndex = System.Array.IndexOf(tags, taggerTarget.objectTag);
                oldTag = taggerTarget.objectTag;
            }

            if (oldLayer != taggerTarget.objectLayer)
            {
                layersChoiceIndex = System.Array.IndexOf(layers, taggerTarget.objectLayer);
                oldLayer = taggerTarget.objectLayer;
            }

            serializedObject.FindProperty("objectTag").stringValue = tags[tagsChoiceIndex];
            serializedObject.FindProperty("objectLayer").stringValue = layers[layersChoiceIndex];

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif
