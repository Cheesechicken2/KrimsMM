using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[InitializeOnLoad]
public class TagAndLayerSetup
{
    static TagAndLayerSetup()
    {
        SetupTagsAndLayers();
    }

    static void SetupTagsAndLayers()
    {
        // Tags 
        string[] tags = new[] {
            "Untagged", "PrefabPools", "Enemy", "Door", "Elevator", "Glass", "Slope", "Stairs", "Metal",
            "AIPath", "Slippery", "SuperSlippery", "Water", "Dirt", "Carpet", "Movable", "Tire", "ThickMetal",
            "Deathzone", "MainLevel", "Destructible", "Ladder", "Mover", "SteepSlope"
        };

        // Layers 
        string[] layers = new[] {
            "Default", "TransparentFX", "Ignore Raycast", "Water", "UI", "Doors", "MyCharGraphics", "MyFPGraphics",
            "CharControllers", "OPFPGraphics", "DoorTrigger", "Glass", "Screen", "MyStandHitbox", "IgnorePlayer",
            "IgnoreBullet", "Hitbox", "Projectile", "Special", "Rooms", "Lights", "Plants", "Terrain", "Terrain2",
            "OnlyHitLevel", "IgnoreViewCast", "IgnoreMeshMerge"
        };

        if (AreTagsAndLayersCorrect(tags, layers))
        {
            Debug.Log("All good. Tags and layers are already set up.");
            return;
        }

        foreach (string tag in tags)
        {
            if (!IsTagExist(tag))
            {
                AddTag(tag);
            }
        }

        // layers 8 to 16 (excluding 3, 6, 7, 17)
        SetLayer(8, "Doors");
        SetLayer(9, "MyCharGraphics");
        SetLayer(10, "MyFPGraphics");
        SetLayer(11, "CharControllers");
        SetLayer(12, "OPFPGraphics");
        SetLayer(13, "DoorTrigger");
        SetLayer(14, "Glass");
        SetLayer(15, "Screen");
        SetLayer(16, "MyStandHitbox");

        // layers 18 to 29
        SetLayer(18, "IgnorePlayer");
        SetLayer(19, "IgnoreBullet");
        SetLayer(20, "Hitbox");
        SetLayer(21, "Projectile");
        SetLayer(22, "Special");
        SetLayer(23, "Rooms");
        SetLayer(24, "Lights");
        SetLayer(25, "Plants");
        SetLayer(26, "Terrain");
        SetLayer(27, "Terrain2");
        SetLayer(28, "OnlyHitLevel");
        SetLayer(29, "IgnoreViewCast");

        Debug.Log("Tags and Layers have been set up.");
    }

    static bool AreTagsAndLayersCorrect(string[] tags, string[] layers)
    {
        foreach (string tag in tags)
        {
            if (!IsTagExist(tag))
            {
                return false;
            }
        }

        for (int i = 8; i <= 16; i++)
        {
            if (string.IsNullOrEmpty(LayerMask.LayerToName(i)) || LayerMask.LayerToName(i) == "Nothing")
            {
                return false;
            }
        }

        for (int i = 18; i <= 29; i++)
        {
            if (string.IsNullOrEmpty(LayerMask.LayerToName(i)) || LayerMask.LayerToName(i) == "Nothing")
            {
                return false;
            }
        }

        return true;
    }

    static bool IsTagExist(string tag)
    {
        foreach (string existingTag in UnityEditorInternal.InternalEditorUtility.tags)
        {
            if (existingTag == tag)
            {
                return true;
            }
        }
        return false;
    }

    static void AddTag(string tag)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProperty = tagManager.FindProperty("tags");

        tagsProperty.InsertArrayElementAtIndex(tagsProperty.arraySize);
        tagsProperty.GetArrayElementAtIndex(tagsProperty.arraySize - 1).stringValue = tag;

        tagManager.ApplyModifiedProperties();
    }

    static void SetLayer(int index, string layer)
    {
        if (index < 8 || index >= 32)
        {
            Debug.LogError("Layer index out of range (must be between 8 and 31).");
            return;
        }

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProperty = tagManager.FindProperty("layers");

        layersProperty.GetArrayElementAtIndex(index).stringValue = layer;

        tagManager.ApplyModifiedProperties();
    }

    // Editor Window 
    [MenuItem("Krimbopple's MM/Utilities/Tag & Layer Setup")]
    public static void ShowWindow()
    {
        TagLayerEditorWindow window = EditorWindow.GetWindow<TagLayerEditorWindow>("Tag & Layer Setup");
        window.Show();
    }

    public class TagLayerEditorWindow : EditorWindow
    {
        private bool resetTagsAndLayers;

        private void OnGUI()
        {
            GUILayout.Label("Tag and Layer Setup", EditorStyles.boldLabel);

            resetTagsAndLayers = GUILayout.Toggle(resetTagsAndLayers, "Reset Tags and Layers");

            if (resetTagsAndLayers)
            {
                if (GUILayout.Button("Reset Tags and Layers"))
                {
                    SetupTagsAndLayers();
                    EditorUtility.DisplayDialog("Tags and Layers", "Tags and layers have been reset and added.", "OK");
                }
            }
            else
            {
                GUILayout.Label("Tags and layers are not reset. Enable the checkbox to reset.");
            }
        }
    }
}
#endif
