#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.IntruderMM.Editor
{
    [CustomEditor(typeof(HeliGun)), CanEditMultipleObjects]
    public class HeliGunEditor : UnityEditor.Editor
    {
        private HeliGun heliGunTarget;
        private int currentToolbarButton;

        private GUIStyle buttonStyle;
        private GUIStyle boxStyle;
        private GUIStyle labelStyle;
        private GUIStyle textAreaStyle;
        private GUIStyle headerStyle;
        private Font customFont;

        private void SetStyles()
        {
            if (buttonStyle == null)
            {
                // Load textures
                Texture2D buttonTexture = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/IntruderMM/Scripts/Extension/Editor/GUI/button.png", typeof(Texture2D));
                Texture2D buttonHoverTexture = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/IntruderMM/Scripts/Extension/Editor/GUI/buttonHover.png", typeof(Texture2D));
                Texture2D buttonSelectTexture = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/IntruderMM/Scripts/Extension/Editor/GUI/buttonSelect.png", typeof(Texture2D));
                Texture2D boxTexture = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/IntruderMM/Scripts/Extension/Editor/GUI/box.png", typeof(Texture2D));

                // Load custom font
                customFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/IntruderMM/Scripts/Extension/Editor/GUI/Font/ShareTechMono-Regular.ttf");

                // Button style
                buttonStyle = new GUIStyle(GUI.skin.button)
                {
                    normal = { background = buttonTexture },
                    hover = { background = buttonHoverTexture },
                    active = { background = buttonSelectTexture },
                    fontSize = 14,
                    font = customFont,
                    alignment = TextAnchor.MiddleCenter,
                    fixedHeight = 40,
                    fixedWidth = 200
                };

                // Box style
                boxStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    normal = { background = boxTexture },
                    padding = new RectOffset(10, 10, 10, 10),
                    margin = new RectOffset(0, 0, 10, 10)
                };

                // Label style
                labelStyle = new GUIStyle(EditorStyles.label)
                {
                    fontSize = 14,
                    font = customFont,
                    alignment = TextAnchor.MiddleLeft
                };

                // TextArea style
                textAreaStyle = new GUIStyle(EditorStyles.textArea)
                {
                    fontSize = 14,
                    font = customFont,
                    wordWrap = true
                };

                // Header style
                headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 16,
                    font = customFont,
                    alignment = TextAnchor.MiddleCenter
                };
            }
        }

        private void OnEnable()
        {
            heliGunTarget = (HeliGun)target;
        }

        public override void OnInspectorGUI()
        {
            SetStyles();
            serializedObject.Update();

            if (heliGunTarget == null) { return; }

            // Toolbar GUI
            currentToolbarButton = GUILayout.Toolbar(currentToolbarButton, new string[] { "Settings", "Shooting", "Targeting" });
            EditorGUILayout.Space();

            switch (currentToolbarButton)
            {
                case 0:
                    DrawSettingsTab();
                    break;
                case 1:
                    DrawShootingTab();
                    break;
                case 2:
                    DrawTargetingTab();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSettingsTab()
        {
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.LabelField("Settings", headerStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(HeliGun.on)), new GUIContent("On", "Is the HeliGun active?"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(HeliGun.checkInterval)), new GUIContent("Check Interval", "Time interval (in seconds) between target checks"));
            EditorGUILayout.EndVertical();
        }

        private void DrawShootingTab()
        {
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.LabelField("Shooting", headerStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(HeliGun.shootTransform)), new GUIContent("Shoot Transform", "Transform from where the gun will shoot"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(HeliGun.shouldShootMask)), new GUIContent("Target Layer Mask", "Layers that the HeliGun should shoot at"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(HeliGun.spawnProjectile)), new GUIContent("Spawn Projectile", "Controls projectile spawning"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(HeliGun.sightSensor)), new GUIContent("Sight Sensor", "How the gun sees"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(HeliGun.shootIntruders)), new GUIContent("Shoot Intruders", "Should the HeliGun shoot intruders?"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(HeliGun.shootGuards)), new GUIContent("Shoot Guards", "Should the HeliGun shoot guards?"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(HeliGun.shotsPerSalvo)), new GUIContent("Shots Per Salvo", "Number of shots fired per salvo"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(HeliGun.fireRate)), new GUIContent("Fire Rate", "Time between shots within a salvo (in seconds)"));

            EditorGUILayout.HelpBox("Fire rate should always be less than 2! The lower the better!", MessageType.Info);
            EditorGUILayout.EndVertical();
        }

        private void DrawTargetingTab()
        {
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.LabelField("Targeting", headerStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(HeliGun.targetingRotationSpeed)), new GUIContent("Targeting Rotation Speed", "Speed at which the HeliGun rotates to target"));
            EditorGUILayout.EndVertical();
        }

        private void OnSceneGUI()
        {
            RenderSceneGUI(heliGunTarget, 1);
        }

        public static void RenderSceneGUI(HeliGun target, float alpha)
        {
            if (target == null) { return; }
            // Add custom scene GUI rendering logic here
        }
    }
}

#endif
