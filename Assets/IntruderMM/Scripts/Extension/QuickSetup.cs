#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif

namespace Assets.IntruderMM.Editor
{
    public class QuickSetup
    {
        private const string ObserveCamPrefabPath = "Assets/IntruderMM/Prefabs/Proxy/ObserveCamProxy.prefab";
        private const string SpawnAPrefabPath = "Assets/IntruderMM/Prefabs/SpawnA.prefab";
        private const string SpawnBPrefabPath = "Assets/IntruderMM/Prefabs/SpawnB.prefab";

#if UNITY_POST_PROCESSING_STACK_V2
        [MenuItem("Intruder/Utilities/Create Post Processing")]
        public static void CreatePostProcessing()
        {
            GameObject postProcessingVolume = CreatePostProcessingVolume();
            ConfigurePostProcessing(postProcessingVolume);

            ConfigureCameraPostProcessingLayer();

            EditorSceneManager.SaveOpenScenes();
            if (EditorUtility.DisplayDialog("Reload Scene?", "Reloads the scene to initialize post-processing", "Yes", "No"))
            {
                ReloadScene();
            }
        }

        private static GameObject CreatePostProcessingVolume()
        {
            GameObject volumeObject = new GameObject("Post Processing Volume");
            volumeObject.layer = LayerMask.NameToLayer("TransparentFX");
            volumeObject.AddComponent<PostProcessVolume>();
            return volumeObject;
        }

        private static void ConfigurePostProcessing(GameObject volumeObject)
        {
            PostProcessVolume volume = volumeObject.GetComponent<PostProcessVolume>();
            PostProcessProfile profile = ScriptableObject.CreateInstance<PostProcessProfile>();
            volume.sharedProfile = profile;
            volume.priority = 100;
            volume.isGlobal = true;

            AddAmbientOcclusion(profile);
            AddColorGrading(profile);
            AddBloom(profile);
        }

        private static void AddAmbientOcclusion(PostProcessProfile profile)
        {
            AmbientOcclusion ao = profile.AddSettings<AmbientOcclusion>();
            ao.mode.value = AmbientOcclusionMode.ScalableAmbientObscurance;
            ao.quality.value = AmbientOcclusionQuality.Medium;
            ao.ambientOnly.value = true;
            ao.intensity.value = 0.15f;
        }


        private static void AddColorGrading(PostProcessProfile profile)
        {
            ColorGrading colorGrading = profile.AddSettings<ColorGrading>();
            colorGrading.tonemapper.value = Tonemapper.ACES;
            colorGrading.postExposure.value = 1.25f;
        }

        private static void AddBloom(PostProcessProfile profile)
        {
            Bloom bloom = profile.AddSettings<Bloom>();
            bloom.intensity.value = 1f;
        }

        private static void ConfigureCameraPostProcessingLayer()
        {
            GameObject cameraObject = FindOrCreateObserveCam();
            PostProcessLayer layer = cameraObject.GetComponent<PostProcessLayer>() ?? cameraObject.AddComponent<PostProcessLayer>();
            layer.volumeLayer = LayerMask.GetMask("TransparentFX");
        }

        private static GameObject FindOrCreateObserveCam()
        {
            GameObject cameraObject = GameObject.FindObjectOfType<ObserveCamProxy>()?.gameObject;
            if (cameraObject == null)
            {
                cameraObject = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(ObserveCamPrefabPath));
            }
            return cameraObject;
        }

        private static void ReloadScene()
        {
            Scene currentScene = EditorSceneManager.GetActiveScene();
            EditorSceneManager.OpenScene(currentScene.path, OpenSceneMode.Single);
        }
#endif

        [MenuItem("Krimbopple's MM/Utilities/Map Setup")]
        public static void SetupMap()
        {
            CreateMapPlane();
            InstantiateSpawnPoints();
        }

        private static void CreateMapPlane()
        {
            GameObject.CreatePrimitive(PrimitiveType.Plane);
        }

        private static void InstantiateSpawnPoints()
        {
            InstantiatePrefabAtPosition(SpawnAPrefabPath, new Vector3(-2, 1, 0));
            InstantiatePrefabAtPosition(SpawnBPrefabPath, new Vector3(2, 1, 0));
        }

        private static void InstantiatePrefabAtPosition(string prefabPath, Vector3 position)
        {
            GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath));
            prefab.transform.position = position;
        }
    }
}

#endif
