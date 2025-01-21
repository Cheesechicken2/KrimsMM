using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class MapMakerTutorialOverlay : MonoBehaviour
{
#if UNITY_EDITOR
    private static bool isActive = false;
    private GUISkin intruderSkin;
    private AudioSource tutorialMusic;
    private AudioClip tutorialClip;
    private GameObject previousObject = null;


    private int currentStep = 0; 
    private string[] tutorialTexts;
    private GameObject hackmode;
    private GameObject sentryturret;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    [MenuItem("Krimbopple's MM/unfinished tutorial")]
    public static void ToggleTutorial()
    {
        isActive = !isActive;

        if (isActive)
        {
            if (!FindObjectOfType<MapMakerTutorialOverlay>())
            {
                GameObject tutorialObject = new GameObject("MapMakerTutorialOverlay");
                tutorialObject.AddComponent<MapMakerTutorialOverlay>();
            }
        }
        else
        {
            var tutorial = FindObjectOfType<MapMakerTutorialOverlay>();
            if (tutorial)
            {
                DestroyImmediate(tutorial.gameObject);
            }
        }
    }

    private void OnEnable()
    {
        LoadAssets();
        SceneView.duringSceneGui += OnSceneGUI;
        tutorialTexts = new string[]
        {
       "placeholder",
       "This is the <color=#3105f5>Briefcase Proxy</color>. It serves as a placeholder for the in-game briefcase. They are used for the <i>Raid Gamemode</i> and <i>Push Gamemode</i> objectives.",
       "The <color=#00ff00>Goal Point Proxy</color> marks where the briefcase needs to be delivered or extracted on the map, crucial for completing the objective.",
        "These are <color=#f50505>Hack Nodes</color>. They are used for the <i>Hack Gamemode</i> objective.",
        "This is a <color=#f50505>Hack Mode Proxy</color>. It is a placeholder for the in-game hack objectives. It is part of a <color=#bd7426>Hub</color> that contains more computers within a <color=#ffee00>Network</color>.",
        "This is a <color=#ffee00>Network</color>. This one consists of 4 <color=#bd7426>Hubs</color>. The number of <color=#f50505>Hack Nodes</color> and <color=#bd7426>Hubs</color> within a <color=#ffee00>Network</color> can be customized. Hacking an entire <color=#ffee00>Network</color> or <color=#bd7426>Hub</color> can trigger in-game events using the \"hack goal is reached\" field. Hacking the entire <color=#ffee00>Network</color> grants the Intruder Team a win.",
        "chump"
        };


        InitializeMusic();
        PlayTutorialMusic();
    }


    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        Cleanup();
    }

    private void LoadAssets()
    {
        intruderSkin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/IntruderMM/Scripts/Extension/Editor/GUI/IntruderSkin.guiskin");
        tutorialClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/IntruderMM/Audio/tutMusic.mp3");
    }

    private void InitializeMusic()
    {
        if (tutorialMusic == null)
        {
            tutorialMusic = gameObject.AddComponent<AudioSource>();
            tutorialMusic.clip = tutorialClip;
            tutorialMusic.loop = true;
            tutorialMusic.playOnAwake = false;
        }
    }

    private void PlayTutorialMusic()
    {
        if (tutorialMusic != null && tutorialClip != null)
        {
            tutorialMusic.Play();
        }
    }

    private void StopTutorialMusic()
    {
        if (tutorialMusic != null && tutorialMusic.isPlaying)
        {
            tutorialMusic.Stop();
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!isActive) return;

        Handles.BeginGUI();
        SetGUISkin();

        RenderTutorialUI();

        Handles.EndGUI();
    }

    private void SetGUISkin()
    {
        if (intruderSkin != null)
        {
            GUI.skin = intruderSkin;
        }
    }

    private void RenderTutorialUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 400), GUI.skin.box);

        RenderTutorialHeader();
        RenderCurrentStepText();
        RenderNavigationButtons();

        GUILayout.EndArea();
    }

    private void RenderTutorialHeader()
    {
        GUIStyle fontStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            alignment = TextAnchor.MiddleCenter,
            richText = true
        };
        GUILayout.Label("<color=#00ff00>Mapmaker Tutorial</color>", fontStyle);
        GUILayout.Space(10);
    }

    private void RenderCurrentStepText()
    {
        GUIStyle textStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            wordWrap = true,
            alignment = TextAnchor.MiddleCenter,
            richText = true        
        };
        GUILayout.Space(10);

        if (currentStep < tutorialTexts.Length)
        {
            GUILayout.Label(tutorialTexts[currentStep], textStyle);
        }
        else
        {
            GUILayout.Label("<color=green>Tutorial Completed!</color>", textStyle);
        }
    }

    private void RenderNavigationButtons()
    {
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();

        if (currentStep > 0 && GUILayout.Button("Previous Step", GUILayout.Height(30)))
        {
            currentStep--;
            HandleStepChange();
        }

        if (currentStep < tutorialTexts.Length - 1 && GUILayout.Button("Next Step", GUILayout.Height(30)))
        {
            currentStep++;
            HandleStepChange();
        }

        GUILayout.EndHorizontal();
    }

    private void HandleStepChange()
    {
        if (previousObject != null)
        {
            DestroyImmediate(previousObject); 
        }

        switch (currentStep)
        {
            case 0:
                ShowBriefcaseTutorial();
                break;
            case 1:
                ShowBriefcaseTutorial();
                break;
            case 2:
                ShowGoalPointTutorial();
                break;
            case 3:
                ShowHackNodeTutorial();
                break;
            case 4:
                ShowHackHub();
                break;
            case 5:
                ShowHackModeProxy();
                break
;            case 6:
                EndTutorial();
                break;
        }
    }


    private void ShowBriefcaseTutorial()
    {
        GameObject briefcasePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/IntruderMM/Prefabs/BriefcaseProxy.prefab");
      briefcasePrefab =  SpawnObject(briefcasePrefab, new Vector3(5, 1, 0), "Briefcase");
        Selection.activeGameObject = briefcasePrefab;
        FrameSceneViewOnObject(briefcasePrefab.gameObject);
    }

    private void ShowGoalPointTutorial()
    {
        GameObject goalPointPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/IntruderMM/Prefabs/GoalPointProxy.prefab");
        goalPointPrefab = SpawnObject(goalPointPrefab, new Vector3(-5, 1, 0), "Goal Point");
        Selection.activeGameObject = goalPointPrefab;
        FrameSceneViewOnObject(goalPointPrefab.gameObject);
    }

    private void ShowHackNodeTutorial()
    {
        GameObject hackNodePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/IntruderMM/Prefabs/HackNodeProxy.prefab");
        hackNodePrefab =  SpawnObject(hackNodePrefab, new Vector3(10, 1, 0), "Hack Node");

        Selection.activeGameObject = hackNodePrefab;
        FrameSceneViewOnObject(hackNodePrefab.gameObject);

    }

    private void ShowHackHub()
    {
        GameObject hackModePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/IntruderMM/Prefabs/HackModeProxy.prefab");
      hackmode = SpawnObject(hackModePrefab, new Vector3(20, 1, 0), "Hack Mode");

        Selection.activeGameObject = hackmode;

        HackHubProxy[] hubs = hackmode.GetComponentsInChildren<HackHubProxy>();
        if (hubs.Length > 0)
        {
            foreach (var hub in hubs)
            {
                if (hub.gameObject.name.Contains("HubA"))
                {
                    Selection.activeGameObject = hub.gameObject;

                    FrameSceneViewOnObject(hub.gameObject);
                    break;
                }
            }
        }
    }

    private void ShowHackModeProxy()
    {
        GameObject hackModePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/IntruderMM/Prefabs/HackModeProxy.prefab");
        hackmode = SpawnObject(hackModePrefab, new Vector3(20, 1, 0), "Hack Mode");

        Selection.activeGameObject = hackmode;
        FrameSceneViewOnObject(hackmode.gameObject);

    }

    private void ShowHeliGun()
    {
        GameObject SentryTurret = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/IntruderMM/Prefabs/SentryTurret.prefab");
        sentryturret = SpawnObject(SentryTurret, new Vector3(20, 1, 0), "Sentry Turret");

        Selection.activeGameObject = hackmode;
        FrameSceneViewOnObject(hackmode.gameObject);

    }

    private void ShowRemoteMortar()
    {
        GameObject SentryTurret = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/IntruderMM/Prefabs/SentryTurret.prefab");
        sentryturret = SpawnObject(SentryTurret, new Vector3(20, 1, 0), "Sentry Turret");

        Selection.activeGameObject = hackmode;
        FrameSceneViewOnObject(hackmode.gameObject);

    }


    private GameObject SpawnObject(GameObject prefab, Vector3 position, string name)
    {
        if (prefab == null)
        {
            Debug.LogError("prefab is null....");
            return null;
        }

        if (previousObject != null)
        {
            DestroyImmediate(previousObject);
        }

        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        obj.transform.position = position;
        obj.name = name;
        Undo.RegisterCreatedObjectUndo(obj, $"Spawn {name}");
        previousObject = obj; 
        Debug.Log($"{name} spawned at position {position}");
        return obj;
    }


    private void FrameSceneViewOnObject(GameObject target)
    {
        if (target == null) return;

        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView != null)
        {
            Bounds bounds = GetObjectBounds(target);
            sceneView.LookAt(bounds.center, sceneView.rotation, bounds.extents.magnitude * 2.0f);
            sceneView.Repaint();
        }
    }

    private Bounds GetObjectBounds(GameObject target)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds;
        }

        return new Bounds(target.transform.position, Vector3.one);
    }

    private void EndTutorial()
    {
        Debug.Log("tutorial complete!");
        StopTutorialMusic();

        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }
        spawnedObjects.Clear();

        ToggleTutorial();
    }


    private void Cleanup()
    {
        if (tutorialMusic != null)
        {
            tutorialMusic.Stop();
            DestroyImmediate(tutorialMusic);
        }

        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }
        spawnedObjects.Clear();
    }
#endif
}