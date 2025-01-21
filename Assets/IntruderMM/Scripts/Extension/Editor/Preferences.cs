#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Assets.IntruderMM.Editor
{
    [InitializeOnLoad]
    public static class Preferences
    {
        private static bool isPreferencesLoaded;

        // scenegui stuff 
        public static Color SceneGUITextColor { get; private set; }
        public static Color GUITextColor { get; private set; }
        public static bool ShowLabelBackgrounds { get; private set; }

        public static bool ShowLines { get; private set; }
        public static bool ShowArrows { get; private set; }
        public static bool ShowLabels { get; private set; }
        public static bool ShowBalls { get; private set; }
        public static bool ShowNestedActivator { get; private set; }

        public static float LineTransparency { get; private set; }
        public static float ArrowTransparency { get; private set; }
        public static float BallTransparency { get; private set; }

        public static int LabelTextSize { get; private set; }
        public static float LineThickness { get; private set; }
        public static float BallRadius { get; private set; }
        public static float ArrowSize { get; private set; }

        public static bool UseWorldSpaceLabels { get; private set; }
        public static bool FadeLabelsByDistance { get; private set; }
        public static Vector2 LabelFadeDistances { get; private set; }

        // toolbar
        public static bool DisplaySceneToolbar { get; private set; }
        public static float ToolbarOpacity { get; private set; }
        public static float ToolbarSize { get; private set; }

        // activatorr... 
        public static bool EnableObjects { get; private set; }
        public static bool DisableObjects { get; private set; }
        public static bool RandomizeEnabledObjects { get; private set; }
        public static bool AnimateObjects { get; private set; }
        public static bool StopAnimatingObjects { get; private set; }
        public static bool UnlockCustomDoors { get; private set; }
        public static bool LockCustomDoors { get; private set; }
        public static bool ShowTeleportLocations { get; private set; }

        // noor 
        public static bool ShowDoorPath { get; set; }
        

        static Preferences()
        {
            if (!isPreferencesLoaded)
            {
                LoadPreferences();
            }
        }
        [SettingsProvider]
        public static SettingsProvider CreatePreferencesProvider()
        {
            var provider = new SettingsProvider("Preferences/IntruderMM", SettingsScope.User)
            {
                label = "IntruderMM",
                guiHandler = (searchContext) => CustomPreferencesGUI()
            };

            return provider;
        }

        public static void CustomPreferencesGUI()
        {
            EditorGUILayout.LabelField("IntruderMM Created by Superboss Games", EditorStyles.centeredGreyMiniLabel);
            EditorTools.InspectorGUILine(2);

            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Scene Configuration", EditorTools.preferencesHeaderBox);
                DrawSceneOptions();
                EditorTools.InspectorGUILine(2);

                EditorGUILayout.LabelField("Visual Settings", EditorTools.preferencesHeaderBox);
                DrawTransparencySliders();
                DrawSizeSliders();
                EditorTools.InspectorGUILine(2);

                EditorGUILayout.LabelField("Object Control Settings", EditorTools.preferencesHeaderBox);
                DrawActivatorOptions();
                EditorTools.InspectorGUILine(2);

                EditorGUILayout.LabelField("Interface Settings", EditorTools.preferencesSmallHeaderBox);
                DisplaySceneToolbar = EditorGUILayout.Toggle("Show Scene Toolbar? ", DisplaySceneToolbar);
                ToolbarSize = EditorGUILayout.Slider("Toolbar Scale", ToolbarSize, 0.75f, 1.5f);
                ToolbarOpacity = EditorGUILayout.Slider("Toolbar Alpha", ToolbarOpacity, 0.05f, 1);
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            if (GUILayout.Button("Reset Preferences"))
            {
                ResetPreferences();
                LoadPreferences();
            }

            GUILayout.FlexibleSpace();
            SavePreferences();
        }

        private static void DrawSceneOptions()
        {
            EditorGUILayout.BeginVertical("box");
            {
                DrawLabelOptions();
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawLabelOptions()
        {
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Text Label Customization", EditorTools.preferencesSmallHeaderBox);
                ShowLabels = EditorGUILayout.Toggle("Show SceneGUI Labels? ", ShowLabels);
                EditorGUI.BeginDisabledGroup(!ShowLabels);
                {
                    EditorGUI.indentLevel++;
                    LabelTextSize = EditorGUILayout.IntSlider("SceneGUI Text Size", LabelTextSize, 1, 32);
                    SceneGUITextColor = EditorGUILayout.ColorField("SceneGUI Text Color", SceneGUITextColor);
                    ShowLabelBackgrounds = EditorGUILayout.Toggle("Show Label Background? ", ShowLabelBackgrounds);
                    UseWorldSpaceLabels = EditorGUILayout.Toggle("Use Worldspace Label? ", UseWorldSpaceLabels);
                    FadeLabelsByDistance = EditorGUILayout.Toggle("Fade Label At Distance? ", FadeLabelsByDistance);
                    if (FadeLabelsByDistance)
                    {
                        EditorGUI.indentLevel++;
                        LabelFadeDistances = EditorGUILayout.Vector2Field("Fade Distances", LabelFadeDistances);
                        EditorGUILayout.LabelField("X = Close, Y = Far", EditorStyles.centeredGreyMiniLabel);
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUI.EndDisabledGroup();
                GUITextColor = EditorGUILayout.ColorField("GUI Text Color", GUITextColor);
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawTransparencySliders()
        {
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Opacity Controls", EditorTools.preferencesSmallHeaderBox);
                LineTransparency = EditorGUILayout.Slider("Lines Alpha", LineTransparency, 0.05f, 1);
                ArrowTransparency = EditorGUILayout.Slider("Arrows Alpha", ArrowTransparency, 0.05f, 1);
                BallTransparency = EditorGUILayout.Slider("Ball Alpha", BallTransparency, 0.05f, 1);
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawSizeSliders()
        {
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Scale Settings", EditorTools.preferencesSmallHeaderBox);
                LineThickness = EditorGUILayout.Slider("Line Width", LineThickness, 0.1f, 32);
                BallRadius = EditorGUILayout.Slider("Ball Size", BallRadius, 0.01f, 1);
                ArrowSize = EditorGUILayout.Slider("Arrow Size", ArrowSize, 0.01f, 2);
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawActivatorOptions()
        {
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("Activator Configuration", EditorTools.preferencesSmallHeaderBox);
                DrawActivatorToggleOptions();
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawActivatorToggleOptions()
        {
            EditorGUILayout.BeginVertical("box");
            {
                EnableObjects = EditorGUILayout.Toggle("Objects to Enable", EnableObjects);
                DisableObjects = EditorGUILayout.Toggle("Objects to Disable", DisableObjects);
                RandomizeEnabledObjects = EditorGUILayout.Toggle("Objects to Randomly Enable", RandomizeEnabledObjects);
                AnimateObjects = EditorGUILayout.Toggle("Objects to Animate", AnimateObjects);
                StopAnimatingObjects = EditorGUILayout.Toggle("Objects to Stop Animating", StopAnimatingObjects);
                UnlockCustomDoors = EditorGUILayout.Toggle("Doors to Unlock", UnlockCustomDoors);
                LockCustomDoors = EditorGUILayout.Toggle("Doors to Lock", LockCustomDoors);
                ShowTeleportLocations = EditorGUILayout.Toggle("Teleport Locations", ShowTeleportLocations);
            }
            EditorGUILayout.EndVertical();
        }


        private static void ResetPreferences()
        {
            EditorPrefs.DeleteKey("DisplaySceneToolbar" + "Key");
            EditorPrefs.DeleteKey("ShowLabelBackgrounds" + "Key");
            EditorPrefs.DeleteKey("SceneGUITextColor-R");
            EditorPrefs.DeleteKey("SceneGUITextColor-G");
            EditorPrefs.DeleteKey("SceneGUITextColor-B");
            EditorPrefs.DeleteKey("SceneGUITextColor-A");
            EditorPrefs.DeleteKey("GUITextColor-R");
            EditorPrefs.DeleteKey("GUITextColor-G");
            EditorPrefs.DeleteKey("GUITextColor-B");
            EditorPrefs.DeleteKey("GUITextColor-A");
            EditorPrefs.DeleteKey("ShowLines" + "Key");
            EditorPrefs.DeleteKey("ShowArrows" + "Key");
            EditorPrefs.DeleteKey("ShowLabels" + "Key");
            EditorPrefs.DeleteKey("ShowBalls" + "Key");
            EditorPrefs.DeleteKey("ShowNestedActivator" + "Key");
            EditorPrefs.DeleteKey("LineTransparenc " + "Key");
            EditorPrefs.DeleteKey("ArrowTransparency" + "Key");
            EditorPrefs.DeleteKey("BallTransparency" + "Key");
            EditorPrefs.DeleteKey("ToolbarOpacity" + "Key");
            EditorPrefs.DeleteKey("LabelTextSize" + "Key");
            EditorPrefs.DeleteKey("LineThickness" + "Key");
            EditorPrefs.DeleteKey("BallRadius" + "Key");
            EditorPrefs.DeleteKey("ArrowSize" + "Key");
            EditorPrefs.DeleteKey("UseWorldSpaceLabels" + "Key");
            EditorPrefs.DeleteKey("FadeLabelsByDistance" + "Key");
            EditorPrefs.DeleteKey("EnableObjects" + "Key");
            EditorPrefs.DeleteKey("DisableObjects" + "Key");
            EditorPrefs.DeleteKey("StartupNoise" + "Key");
            EditorPrefs.DeleteKey("RandomizeEnabledObjects" + "Key");
            EditorPrefs.DeleteKey("AnimateObjects" + "Key");
            EditorPrefs.DeleteKey("StopAnimatingObjects" + "Key");
            EditorPrefs.DeleteKey("UnlockCustomDoors" + "Key");
            EditorPrefs.DeleteKey("LockCustomDoors" + "Key");
            EditorPrefs.DeleteKey("ShowTeleportLocations" + "Key");
            EditorPrefs.DeleteKey("ShowDoorPath" + "Key");
            EditorPrefs.DeleteKey("ToolbarSize" + "Key");
        }

        private static void SavePreferences()
        {
            EditorPrefs.SetBool("ShowBalls" + "Key", ShowBalls);
            EditorPrefs.SetBool("AnimateObjects" + "Key", AnimateObjects);
            EditorPrefs.SetFloat("ArrowSize" + "Key", ArrowSize);
            EditorPrefs.SetBool("ShowTeleportLocations" + "Key", ShowTeleportLocations);
            EditorPrefs.SetFloat("ToolbarOpacity" + "Key", ToolbarOpacity);
            EditorPrefs.SetBool("DisableObjects" + "Key", DisableObjects);
            EditorPrefs.SetFloat("LineTransparency" + "Key", LineTransparency);
            EditorPrefs.SetFloat("SceneGUITextColor-B", SceneGUITextColor.b);
            EditorPrefs.SetFloat("ArrowTransparency" + "Key", ArrowTransparency);
            EditorPrefs.SetBool("ShowLines" + "Key", ShowLines);
            EditorPrefs.SetBool("StopAnimatingObjects" + "Key", StopAnimatingObjects);
            EditorPrefs.SetBool("ShowLabels" + "Key", ShowLabels);
            EditorPrefs.SetFloat("BallTransparency" + "Key", BallTransparency);
            EditorPrefs.SetBool("UnlockCustomDoors" + "Key", UnlockCustomDoors);
            EditorPrefs.SetBool("ShowArrows" + "Key", ShowArrows);
            EditorPrefs.SetFloat("LabelFadeDistances" + "KeyY", LabelFadeDistances.y);
            EditorPrefs.SetBool("LockCustomDoors" + "Key", LockCustomDoors);
            EditorPrefs.SetFloat("LineThickness" + "Key", LineThickness);
            EditorPrefs.SetBool("RandomizeEnabledObjects" + "Key", RandomizeEnabledObjects);
            EditorPrefs.SetFloat("BallRadius" + "Key", BallRadius);
            EditorPrefs.SetFloat("SceneGUITextColor-A", SceneGUITextColor.a);
            EditorPrefs.SetFloat("LabelFadeDistances" + "KeyX", LabelFadeDistances.x);
            EditorPrefs.SetFloat("GUITextColor-B", GUITextColor.b);
            EditorPrefs.SetInt("LabelTextSize" + "Key", LabelTextSize);
            EditorPrefs.SetBool("UseWorldSpaceLabels" + "Key", UseWorldSpaceLabels);
            EditorPrefs.SetBool("ShowLabelBackgrounds" + "Key", ShowLabelBackgrounds);
            EditorPrefs.SetBool("DisplaySceneToolbar" + "Key", DisplaySceneToolbar);
            EditorPrefs.SetFloat("GUITextColor-A", GUITextColor.a);
            EditorPrefs.SetBool("EnableObjects" + "Key", EnableObjects);
            EditorPrefs.SetFloat("SceneGUITextColor-R", SceneGUITextColor.r);
            EditorPrefs.SetFloat("ArrowSize" + "Key", ArrowSize);
            EditorPrefs.SetFloat("LineTransparency" + "Key", LineTransparency);
            EditorPrefs.SetFloat("ToolbarSize" + "Key", ToolbarSize);
            EditorPrefs.SetFloat("GUITextColor-R", GUITextColor.r);
            EditorPrefs.SetFloat("GUITextColor-G", GUITextColor.g);
            EditorPrefs.SetFloat("SceneGUITextColor-G", SceneGUITextColor.g);
            EditorPrefs.SetFloat("LabelFadeDistances" + "KeyY", LabelFadeDistances.y);
        }

        private static void LoadPreferences()
        {
            LineTransparency = EditorPrefs.GetFloat("LineTransparency" + "Key", 1);
            LabelTextSize = EditorPrefs.GetInt("LabelTextSize" + "Key", 10);
            BallRadius = EditorPrefs.GetFloat("BallRadius" + "Key", 0.1f);
            ToolbarOpacity = EditorPrefs.GetFloat("ToolbarOpacity" + "Key", 1);
            ShowArrows = EditorPrefs.GetBool("ShowArrows" + "Key", true);
            ShowBalls = EditorPrefs.GetBool("ShowBalls" + "Key", true);
            BallTransparency = EditorPrefs.GetFloat("BallTransparency" + "Key", 1);
            ToolbarSize = EditorPrefs.GetFloat("ToolbarSize" + "Key", 1);
            ArrowSize = EditorPrefs.GetFloat("ArrowSize" + "Key", 1);
            ShowLines = EditorPrefs.GetBool("ShowLines" + "Key", true);
            SceneGUITextColor = new Color(EditorPrefs.GetFloat("SceneGUITextColor-R", 1), EditorPrefs.GetFloat("SceneGUITextColor-G", 1), EditorPrefs.GetFloat("SceneGUITextColor-B", 1), EditorPrefs.GetFloat("SceneGUITextColor-A", 1));
            ShowNestedActivator = EditorPrefs.GetBool("ShowNestedActivator" + "Key", true);
            DisableObjects = EditorPrefs.GetBool("DisableObjects" + "Key", true);
            ArrowTransparency = EditorPrefs.GetFloat("ArrowTransparency" + "Key", 1);
            UnlockCustomDoors = EditorPrefs.GetBool("UnlockCustomDoors" + "Key", true);
            UseWorldSpaceLabels = EditorPrefs.GetBool("UseWorldSpaceLabels" + "Key", false);
            LabelFadeDistances = new Vector2(EditorPrefs.GetFloat("FadeLabelsByDistance " + "KeyX", 5), EditorPrefs.GetFloat("FadeLabelsByDistance" + "KeyY", 10));
            ShowLabelBackgrounds = EditorPrefs.GetBool("ShowLabelBackgrounds" + "Key", true);
            ShowLabels = EditorPrefs.GetBool("ShowLabels" + "Key", true);
            EnableObjects = EditorPrefs.GetBool("EnableObjects" + "Key", true);
            ShowTeleportLocations = EditorPrefs.GetBool("ShowTeleportLocations" + "Key", true);
            LineThickness = EditorPrefs.GetFloat("LineThickness" + "Key", 1);
            RandomizeEnabledObjects = EditorPrefs.GetBool("RandomizeEnabledObjects" + "Key", true);
            LockCustomDoors = EditorPrefs.GetBool("LockCustomDoors" + "Key", true);
            ShowDoorPath = EditorPrefs.GetBool("ShowDoorPath" + "Key", true);
            GUITextColor = new Color(EditorPrefs.GetFloat("GUITextColor-R", 1), EditorPrefs.GetFloat("GUITextColor-G", 1), EditorPrefs.GetFloat("GUITextColor-B", 1), EditorPrefs.GetFloat("GUITextColor-A", 1));
            AnimateObjects = EditorPrefs.GetBool("AnimateObjects" + "Key", true);
            StopAnimatingObjects = EditorPrefs.GetBool("StopAnimatingObjects" + "Key", true);
            DisplaySceneToolbar = EditorPrefs.GetBool("DisplaySceneToolbar" + "Key", true);
        }
    }
}

#endif
