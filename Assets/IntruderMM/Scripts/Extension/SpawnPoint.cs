#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Assets.IntruderMM.Editor
{
    public class SpawnPoint : MonoBehaviour
    {
        private bool needsUpdate;
        private Mesh spawnMesh;
        private Quaternion meshRotation;
        private Color solidColor;
        private Color outlineColor;
        private Transform lastParent;

        private void OnDrawGizmos()
        {
            CheckParentChange();
            ConfigureMeshAndColors();
            AdjustMeshRotation();
            RenderGizmo();
        }

        private void CheckParentChange()
        {
            if (lastParent != transform.parent)
            {
                needsUpdate = true;
                lastParent = transform.parent;
            }
        }

        private void ConfigureMeshAndColors()
        {
            if (spawnMesh == null || needsUpdate)
            {
                needsUpdate = false;

                if (transform.parent != null)
                {
                    switch (transform.parent.name)
                    {
                        case "SpawnA": // gurd spawn point
                            spawnMesh = LoadMeshAsset("Assets/IntruderMM/Scripts/Extension/Models/mdl_Guard.fbx");
                            solidColor = new Color(0.1f, 0.2f, 0.6f, 0.05f); 
                            outlineColor = new Color(0.1f, 0.2f, 0.6f, 0.04f); 
                            break;
                        case "SpawnB": // truder spawn point
                            spawnMesh = LoadMeshAsset("Assets/IntruderMM/Scripts/Extension/Models/mdl_Intruder.fbx");
                            solidColor = new Color(0.6f, 0.1f, 0.1f, 0.05f); 
                            outlineColor = new Color(0.6f, 0.1f, 0.1f, 0.04f);
                            break;
                    }
                }
            }
        }

        private Mesh LoadMeshAsset(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Mesh>(path);
        }

        private void AdjustMeshRotation()
        {
            Quaternion desiredRotation = transform.rotation * Quaternion.Euler(90, 0, 0);
            if (meshRotation != desiredRotation)
            {
                meshRotation = desiredRotation;
            }
        }

        private void RenderGizmo()
        {
            if (spawnMesh == null) return;

            // outline
            Gizmos.color = outlineColor;
            Gizmos.DrawWireMesh(spawnMesh, 0, transform.position, meshRotation, transform.localScale);

            // mesh
            Gizmos.color = solidColor;
            Gizmos.DrawMesh(spawnMesh, 0, transform.position, meshRotation, transform.localScale);
        }
    }
}
#endif
