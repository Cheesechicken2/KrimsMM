#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class BriefcaseProxy : MonoBehaviour
{
    private bool NeedsRecalculation;

    private Mesh gizmoMesh;
    private Quaternion gizmoMeshRotation;

    private Color meshcoolr;
    private Color wireeColon;

    private Transform cachedParent;

    private void OnDrawGizmos()
    {
        if (cachedParent != transform.parent)
        {
            NeedsRecalculation = true;
            if (transform.parent != null) cachedParent = transform.parent;
            else cachedParent = null;
        }

        // Set up da model
        if (gizmoMesh == null || NeedsRecalculation)
        {
            gizmoMesh = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/IntruderMM/Models/BriefcaseDecimated.obj", typeof(Mesh));
            meshcoolr = new Color(0.0f, 0.0f, 1.0f, 0.75f); 
            wireeColon = new Color(0.0f, 0.0f, 1.0f, 0.5f);
        }

        // rotations... thats tuff.
        if (gizmoMeshRotation != transform.rotation * Quaternion.Euler(0, 0, 0))
            gizmoMeshRotation = transform.rotation * Quaternion.Euler(0, 0, 0);

        // cool wireframe..
        Gizmos.color = wireeColon;
        Gizmos.DrawWireMesh(gizmoMesh, 0, transform.position, gizmoMeshRotation, transform.localScale);

        // meshy
        Gizmos.color = meshcoolr;
        Gizmos.DrawMesh(gizmoMesh, 0, transform.position, gizmoMeshRotation, transform.localScale);
    }
}
#endif
