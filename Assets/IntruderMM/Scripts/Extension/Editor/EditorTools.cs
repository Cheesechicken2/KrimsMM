#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Sabresaurus.SabreCSG;
using System.Reflection;

namespace Assets.IntruderMM.Editor
{
    [InitializeOnLoad]
    public static class EditorTools
    {
        public const float SUB_ACTIVATOR_ALPHA = 0.5f;

        public static GUISkin intruderSkin;
        private static Texture2D boxTex, buttonTex, buttonHoverTex, buttonSelectTex, sideButtonTex, sideButtonHoverTex, intruderLogo, expandedLogo;

        public static GenericMenu proxyMenu;
        private static bool infoBox = false;
        private static string infoBoxTitle, infoBoxText;

        public static GUIStyle box, preferencesHeaderBox, preferencesSmallHeaderBox, bottomButton, sideButton, labelGUI, labelSceneGUI;

        private const int toolbarBottomRightHeight = 64;
        private const int toolbarBottomLeftHeight = 88;
        private const int toolbarActivatorPanelHeight = 150;
        private static bool toolbarStateLoaded = false;

        private static float currentHeight;
        private static bool isToolbarExpanded;

        static EditorTools()
        {
            OnEnable();
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnEnable()
        {
            LoadAssets();
            currentHeight = EditorPrefs.GetFloat("IntruderMM_ToolbarHeight", 88);
        }

        public static void LoadAssets()
        {
            string path = GetEditorToolsPath() + "GUI/";

            intruderSkin = ScriptableObject.CreateInstance<GUISkin>();
            intruderSkin.font = AssetDatabase.LoadAssetAtPath<Font>(path + "Font/ShareTechMono-Regular.ttf");

            boxTex = AssetDatabase.LoadAssetAtPath<Texture2D>(path + "Box.png");
            buttonTex = AssetDatabase.LoadAssetAtPath<Texture2D>(path + "Button.png");
            buttonHoverTex = AssetDatabase.LoadAssetAtPath<Texture2D>(path + "ButtonHover.png");
            buttonSelectTex = AssetDatabase.LoadAssetAtPath<Texture2D>(path + "ButtonSelect.png");
            sideButtonTex = AssetDatabase.LoadAssetAtPath<Texture2D>(path + "ButtonSide.png");
            sideButtonHoverTex = AssetDatabase.LoadAssetAtPath<Texture2D>(path + "ButtonSideHover.png");

            proxyMenu = new GenericMenu();
        }

        public static void SetupStyles()
        {
            box = CreateStyle("Label", 15, boxTex, Preferences.SceneGUITextColor, TextAnchor.MiddleCenter);
            preferencesHeaderBox = new GUIStyle(box) { fontSize = 16, fontStyle = FontStyle.Bold };
            preferencesSmallHeaderBox = CreateStyle("Label", 16, null, Color.black, TextAnchor.MiddleLeft);

            bottomButton = CreateButtonStyle(16, buttonTex, buttonHoverTex, buttonSelectTex);
            sideButton = new GUIStyle(bottomButton) { normal = { background = sideButtonTex }, hover = { background = sideButtonHoverTex } };

            labelGUI = new GUIStyle("Label") { fontSize = 13, margin = new RectOffset(-15, -15, -15, -15), fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft, normal = { textColor = Color.black } };
            labelSceneGUI = new GUIStyle("Label") { fontSize = Preferences.LabelTextSize, alignment = TextAnchor.MiddleCenter, normal = { textColor = Preferences.SceneGUITextColor }, font = intruderSkin.font };

            if (Preferences.ShowLabelBackgrounds)
            {
                labelSceneGUI.normal.background = boxTex;
                labelSceneGUI.border = new RectOffset(4, 4, 4, 4);
                labelSceneGUI.padding = new RectOffset(0, 0, 4, 4);
            }
        }

        private static GUIStyle CreateStyle(string styleName, int fontSize, Texture2D background, Color textColor, TextAnchor alignment)
        {
            var style = new GUIStyle(styleName)
            {
                font = intruderSkin.font,
                fontSize = fontSize,
                normal = { textColor = textColor, background = background },
                alignment = alignment,
                padding = new RectOffset(8, 8, 8, 8),
                border = new RectOffset(4, 4, 4, 4)
            };
            return style;
        }

        private static GUIStyle CreateButtonStyle(int fontSize, Texture2D normalTex, Texture2D hoverTex, Texture2D activeTex)
        {
            return new GUIStyle("Label")
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { background = normalTex, textColor = Preferences.SceneGUITextColor },
                active = { background = activeTex, textColor = Preferences.SceneGUITextColor },
                hover = { background = hoverTex, textColor = Preferences.SceneGUITextColor },
                fontSize = fontSize * Mathf.FloorToInt(Preferences.ToolbarSize),
                padding = new RectOffset(4, 4, 4, 4),
                border = new RectOffset(4, 5, 5, 5),
                font = intruderSkin.font
            };
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (sceneView == null || BuildPipeline.isBuildingPlayer || CSGModel.GetActiveCSGModel() != null) { return; }

            Handles.BeginGUI();
            {
                GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Game);
                GUI.skin.font = intruderSkin.font;
                GUI.backgroundColor = new Color(1, 1, 1, Preferences.ToolbarOpacity);

                SetupStyles();
                DrawToolbar(sceneView);
                DrawBottomRightToolbar(sceneView);
                DrawActivatorInfo(sceneView);
            }
            Handles.EndGUI();
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.update += UpdateToolbar;

            EditorApplication.delayCall += () =>
            {
                if (!toolbarStateLoaded)
                {
                    isToolbarExpanded = EditorPrefs.GetBool("IntruderMM_ToolbarExpanded", true);
                    currentHeight = EditorPrefs.GetFloat("IntruderMM_ToolbarHeight", toolbarBottomLeftHeight);
                    toolbarStateLoaded = true;
                    SceneView.RepaintAll();
                }
            };
        }

        private static void UpdateToolbar()
        {
            if (!Preferences.DisplaySceneToolbar) return;

            bool targetIsExpanded = EditorPrefs.GetBool("IntruderMM_ToolbarExpanded", true);

            float toolbarHeight = toolbarBottomLeftHeight * Preferences.ToolbarSize;
            float logoHeight = 32 * Preferences.ToolbarSize;
            float expandedHeight = toolbarHeight + logoHeight - 20;
            float collapsedHeight = logoHeight + 10;

            float targetHeight = targetIsExpanded ? expandedHeight : collapsedHeight;

            currentHeight = Mathf.Lerp(currentHeight, targetHeight, 0.1f);

            EditorPrefs.SetFloat("IntruderMM_ToolbarHeight", currentHeight);

            SceneView.RepaintAll();
        }

        private static void DrawToolbar(SceneView sceneView)
        {
            if (!Preferences.DisplaySceneToolbar) return;

            GUILayout.BeginArea(new Rect(10, sceneView.position.height - currentHeight - 10, (120 * Preferences.ToolbarSize) * 3, currentHeight));
            {
                if (GUILayout.Button("Krim's IntruderMM", box, GUILayout.MaxHeight(32 * Preferences.ToolbarSize), GUILayout.MaxWidth(360 * Preferences.ToolbarSize)))
                {
                    ToggleToolbarState();
                }

                if (isToolbarExpanded)
                {
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                    {
                        DrawToolbarButtons();
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.Space(10);
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                    {
                        DrawToolbarButtons(true);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndArea();
        }

        private static void ToggleToolbarState()
        {
            isToolbarExpanded = !isToolbarExpanded;
            EditorPrefs.SetBool("IntruderMM_ToolbarExpanded", isToolbarExpanded);
        }

        private static void DrawToolbarButtons(bool isCollapsed = false)
        {
            int buttonHeight = isCollapsed ? 32 : 50;

            if (GUILayout.Button("Test Map", bottomButton, GUILayout.Height(buttonHeight * Preferences.ToolbarSize), GUILayout.ExpandWidth(true)))
            {
                SceneTesting.CheckNameAndPlayInIntruder();
            }
            if (GUILayout.Button("Upload Map", bottomButton, GUILayout.Height(buttonHeight * Preferences.ToolbarSize), GUILayout.ExpandWidth(true)))
            {
                ExportLevel.LoadUploadWindow();
            }
            if (GUILayout.Button("Discord", bottomButton, GUILayout.Height(buttonHeight * Preferences.ToolbarSize), GUILayout.ExpandWidth(true)))
            {
                Application.OpenURL("www.superbossgames.com/chat");
            }
        }

        private static void DrawBottomRightToolbar(SceneView sceneView)
        {
            GUILayout.BeginArea(new Rect(sceneView.position.width - 10 - 160 * Preferences.ToolbarSize, sceneView.position.height - toolbarBottomRightHeight * Preferences.ToolbarSize - 16 - 10, 160 * Preferences.ToolbarSize, toolbarBottomRightHeight * Preferences.ToolbarSize));
            {
                if (GUILayout.Button("Spawn Proxy", bottomButton, GUILayout.MaxWidth(160 * Preferences.ToolbarSize), GUILayout.ExpandHeight(true)))
                {
                    CreateProxyMenu();
                }
            }
            GUILayout.EndArea();
        }

    private static void CreateProxyMenu()
        {
            AddProxyMenuItem(proxyMenu, "Intruder Spawn", "Assets/IntruderMM/Prefabs/SpawnB.prefab");
            AddProxyMenuItem(proxyMenu, "Guard Spawn", "Assets/IntruderMM/Prefabs/SpawnA.prefab");
            proxyMenu.AddItem(new GUIContent("Activator"), false, CreateActivatorObject);

            AddProxyMenuItem(proxyMenu, "Gamemode/Raid/Briefcase", "Assets/IntruderMM/Prefabs/BriefcaseProxy.prefab");
            AddProxyMenuItem(proxyMenu, "Gamemode/Raid/Goal Point", "Assets/IntruderMM/Prefabs/GoalPointProxy.prefab");
            AddProxyMenuItem(proxyMenu, "Gamemode/Hack/Create Gamemode", "Assets/IntruderMM/Prefabs/HackModeProxy.prefab");
            AddProxyMenuItem(proxyMenu, "Gamemode/Hack/Hack Node", "Assets/IntruderMM/Prefabs/HackNodeProxy.prefab");
            AddProxyMenuItem(proxyMenu, "Generic/Remote Mortar", "Assets/IntruderMM/Prefabs/RemoteMortar.prefab");
            AddProxyMenuItem(proxyMenu, "Generic/Sentry Turret", "Assets/IntruderMM/Prefabs/SentryTurret.prefab");
            AddProxyMenuItem(proxyMenu, "Generic/Truck", "Assets/IntruderMM/Prefabs/Proxy/Truck.prefab");
            AddProxyMenuItem(proxyMenu, "Generic/Pickup", "Assets/IntruderMM/Prefabs/Pickup.prefab");
            AddProxyMenuItem(proxyMenu, "Generic/Glass", "Assets/IntruderMM/Prefabs/GlassProxy.prefab");
            AddProxyMenuItem(proxyMenu, "Generic/Observe Camera", "Assets/IntruderMM/Prefabs/ObserveCamProxy.prefab");
            AddProxyMenuItem(proxyMenu, "Generic/Ladder", "Assets/IntruderMM/Prefabs/Ladder.prefab");
            AddProxyMenuItem(proxyMenu, "Generic/Note", "Assets/IntruderMM/Prefabs/NoteProxy.prefab");
            AddProxyMenuItem(proxyMenu, "Generic/Keypad", "Assets/IntruderMM/Prefabs/Keypad.prefab");
            AddProxyMenuItem(proxyMenu, "Generic/Mirror", "Assets/IntruderMM/Prefabs/Proxy/Mirror.prefab");
            proxyMenu.AddItem(new GUIContent("Generic/Compass Direction"), false, CreateCompassGameObject);
            proxyMenu.ShowAsContext();
        }

        private static void DrawActivatorInfo(SceneView sceneView)
        {
            if (infoBox)
            {
                GUIStyle descText = new GUIStyle("Label")
                {
                    alignment = TextAnchor.UpperLeft,
                    fontSize = 14
                };

                GUILayout.BeginArea(new Rect(sceneView.position.width - 10 - 400, sceneView.position.height - toolbarActivatorPanelHeight - toolbarBottomRightHeight - 16 - 10 - 10, 400, toolbarActivatorPanelHeight));
                {
                    EditorGUILayout.BeginVertical(box, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                    GUILayout.Label(infoBoxTitle, labelGUI, GUILayout.ExpandHeight(false), GUILayout.MaxHeight(24));
                    InspectorGUILine(1);
                    GUILayout.Label(infoBoxText, descText, GUILayout.ExpandWidth(true));
                    EditorGUILayout.EndVertical();
                }
                GUILayout.EndArea();
            }
            else
            {
                infoBox = false;
            }
        }

        public static void InspectorGUILine(int height = 1)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            rect.height = height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }

        private static void AddProxyMenuItem(GenericMenu menu, string itemPath, string proxyAssetPath)
        {
            var proxyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(proxyAssetPath);
            menu.AddItem(new GUIContent(itemPath), false, SpawnProxyObject, proxyPrefab);
        }

        private static void CreateActivatorObject()
        {
            var activatorObject = new GameObject("Activator");
            activatorObject.transform.position = SceneView.lastActiveSceneView.camera.transform.TransformPoint(Vector3.forward * 5);
            activatorObject.AddComponent<Activator>();
            Selection.activeGameObject = activatorObject;
        }

        private static void CreateCompassGameObject()
        {
            var compassObject = new GameObject("Compass Direction");
            compassObject.transform.position = SceneView.lastActiveSceneView.camera.transform.TransformPoint(Vector3.forward * 5);
            compassObject.AddComponent<CompassDirection>();
            Selection.activeGameObject = compassObject;
        }

        private static void SpawnProxyObject(object proxyObject)
        {
            var instantiatedObject = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)proxyObject);
            instantiatedObject.transform.position = SceneView.lastActiveSceneView.camera.transform.TransformPoint(Vector3.forward * 5);
            Selection.activeGameObject = instantiatedObject;
        }

        public static void ShowInfoBox(string title, string description)
        {
            infoBox = true;
            infoBoxTitle = title;
            infoBoxText = description;
        }

        public static void HideInfoBox()
        {
            infoBox = false;
        }

        public static string GetEditorToolsPath()
        {
            string[] guids = AssetDatabase.FindAssets("EditorTools t:Script");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                const string suffix = "EditorTools.cs";
                if (path.EndsWith(suffix))
                {
                    return path.Substring(0, path.Length - suffix.Length);
                }
            }
            return string.Empty;
        }

        static public void DrawLinesToObjects(Object[] targetObjects, string labelText, Color lineColor, Component sourceComponent, float alphaMultiplier, float lineThickness)
        {
            if (targetObjects == null || sourceComponent == null) return;

            foreach (var targetObject in targetObjects)
            {
                if (targetObject == null) continue;

                var targetGameObject = targetObject as GameObject;
                var targetComponent = targetObject as Component;

                if (targetGameObject != null)
                {
                    RenderLine(sourceComponent.gameObject, targetGameObject, lineColor, labelText, alphaMultiplier, lineThickness); 
                }
                else if (targetComponent != null)
                {
                    RenderLine(sourceComponent.gameObject, targetComponent.gameObject, lineColor, labelText, alphaMultiplier, lineThickness);
                }
            }
        }
        public static void RenderLine(GameObject start, GameObject end, Color lineColor, string labelPrefix, float alphaMultiplier, float lineThickness)
        {
            var startPos = start.transform.position;
            var endPos = end.transform.position;
            var direction = endPos - startPos;
            var distance = direction.magnitude;

            var lerpedPosition = Vector3.Lerp(startPos, endPos, 0.5f);
            RenderWorldLabel(labelPrefix + end.name, lerpedPosition, labelSceneGUI, alphaMultiplier);

            lineColor.a = alphaMultiplier * Preferences.LineTransparency;
            Handles.color = lineColor;

            float adjustedLineThickness = lineThickness * 2.0f;
            if (Preferences.ShowLines)
            {
                Handles.DrawAAPolyLine(adjustedLineThickness, startPos, endPos);
            }

            lineColor.a = alphaMultiplier * Preferences.BallTransparency;
            Handles.color = lineColor;

            if (Preferences.ShowBalls)
            {
                Handles.SphereHandleCap(0, startPos, start.transform.rotation, Preferences.BallRadius, EventType.Repaint);
            }

            if (distance > 2 && Preferences.ShowArrows)
            {
                Handles.color = new Color(lineColor.r, lineColor.g, lineColor.b, alphaMultiplier * Preferences.ArrowTransparency);

                var normalizedDirection = direction.normalized;

                float arrowThickness = adjustedLineThickness * 0.5f; 

                Handles.ArrowHandleCap(0, startPos + normalizedDirection * (distance * 0.25f), Quaternion.LookRotation(direction), arrowThickness, EventType.Repaint);

                if (distance > 6)
                {
                    Handles.ArrowHandleCap(0, startPos + normalizedDirection * (distance * 0.75f), Quaternion.LookRotation(direction), arrowThickness, EventType.Repaint);
                }
            }
        }

        public static bool IsPointInCameraView(Vector3 point, Transform cameraTransform)
        {
            var directionToPoint = (point - cameraTransform.position).normalized;
            var dotProduct = Vector3.Dot(directionToPoint, cameraTransform.forward);
            var angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;
            return angle < 90;
        }

        [MenuItem("CONTEXT/Transform/Add Activator")]
        public static void AddActivatorComponent(MenuCommand command)
        {
            var transform = (Transform)command.context;
            transform.gameObject.AddComponent<Activator>();
        }

        [MenuItem("CONTEXT/Transform/Add Ignore Sticky")]
        public static void AddIgnoreStickyComponent(MenuCommand command)
        {
            var transform = (Transform)command.context;
            transform.gameObject.AddComponent<IgnoreSticky>();
        }

        public static float RemapValue(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static void RenderWorldLabel(string text, Vector3 position, GUIStyle style, float alphaMultiplier)
        {
            if (!Preferences.ShowLabels) return;

            var camera = SceneView.lastActiveSceneView.camera;
            if (camera == null || !IsPointInCameraView(position, camera.transform)) return;

            var distanceToCamera = Vector3.Distance(position, camera.transform.position);

            if (Preferences.FadeLabelsByDistance)
            {
                if (distanceToCamera >= Preferences.LabelFadeDistances.y) return;

                float alpha = distanceToCamera.RemapValue(
                    Preferences.LabelFadeDistances.x,
                    Preferences.LabelFadeDistances.y,
                    alphaMultiplier,
                    0
                );
                GUI.color = new Color(1, 1, 1, alpha);
            }

            if (Preferences.UseWorldSpaceLabels)
            {
                float labelScaleFactor = Preferences.LabelTextSize / distanceToCamera;
                Vector2 textSize = style.CalcSize(new GUIContent(text)) * labelScaleFactor;

                var scaledStyle = new GUIStyle(style)
                {
                    fontSize = Mathf.Max(1, (int)(textSize.y / 3)),
                    padding = new RectOffset(0, 0, 0, 0),
                    margin = new RectOffset(0, 0, 0, 0),
                    fixedHeight = textSize.y,
                    fixedWidth = textSize.x / 1.25f
                };

                Handles.Label(position, text, scaledStyle);
            }
            else
            {
                Handles.Label(position, text, style);
            }
        }

    }
}
    #endif
