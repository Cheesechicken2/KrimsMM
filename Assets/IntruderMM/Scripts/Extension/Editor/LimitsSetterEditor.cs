using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LimitsSetter))]
public class LimitsSetterEditor : Editor
{
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUIForChildren;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUIForChildren;
    }

    private void OnSceneGUIForChildren(SceneView sceneView)
    {
        LimitsSetter limitsSetter = (LimitsSetter)target;

        if (Selection.activeGameObject == null) return;

        Transform selectedTransform = Selection.activeGameObject.transform;

        if (selectedTransform == limitsSetter.transform ||
            selectedTransform == limitsSetter.Xn ||
            selectedTransform == limitsSetter.Xp ||
            selectedTransform == limitsSetter.Yn ||
            selectedTransform == limitsSetter.Yp ||
            selectedTransform == limitsSetter.Zn ||
            selectedTransform == limitsSetter.Zp)
        {
            DrawHandles(limitsSetter);
        }
    }

    private void DrawHandles(LimitsSetter limitsSetter)
    {
        if (limitsSetter.Xn == null || limitsSetter.Xp == null ||
            limitsSetter.Yn == null || limitsSetter.Yp == null ||
            limitsSetter.Zn == null || limitsSetter.Zp == null)
        {
            Handles.color = Color.red;
            Handles.Label(limitsSetter.transform.position, "Assign all boundary GameObjects.");
            return;
        }
        Vector3 Xn = Handles.PositionHandle(limitsSetter.Xn.position, Quaternion.identity);
        Vector3 Xp = Handles.PositionHandle(limitsSetter.Xp.position, Quaternion.identity);
        Vector3 Yn = Handles.PositionHandle(limitsSetter.Yn.position, Quaternion.identity);
        Vector3 Yp = Handles.PositionHandle(limitsSetter.Yp.position, Quaternion.identity);
        Vector3 Zn = Handles.PositionHandle(limitsSetter.Zn.position, Quaternion.identity);
        Vector3 Zp = Handles.PositionHandle(limitsSetter.Zp.position, Quaternion.identity);
        //update
        if (Xn != limitsSetter.Xn.position) limitsSetter.Xn.position = Xn;
        if (Xp != limitsSetter.Xp.position) limitsSetter.Xp.position = Xp;
        if (Yn != limitsSetter.Yn.position) limitsSetter.Yn.position = Yn;
        if (Yp != limitsSetter.Yp.position) limitsSetter.Yp.position = Yp;
        if (Zn != limitsSetter.Zn.position) limitsSetter.Zn.position = Zn;
        if (Zp != limitsSetter.Zp.position) limitsSetter.Zp.position = Zp;

        float XnPos = limitsSetter.Xn.position.x;
        float XpPos = limitsSetter.Xp.position.x;
        float YnPos = limitsSetter.Yn.position.y;
        float YpPos = limitsSetter.Yp.position.y;
        float ZnPos = limitsSetter.Zn.position.z;
        float ZpPos = limitsSetter.Zp.position.z;

        // define corners
        Vector3[] corners = new Vector3[]
        {
            new Vector3(XnPos, YnPos, ZnPos),
            new Vector3(XnPos, YnPos, ZpPos),
            new Vector3(XnPos, YpPos, ZnPos),
            new Vector3(XnPos, YpPos, ZpPos),
            new Vector3(XpPos, YnPos, ZnPos),
            new Vector3(XpPos, YnPos, ZpPos),
            new Vector3(XpPos, YpPos, ZnPos),
            new Vector3(XpPos, YpPos, ZpPos)
        };

        // draw edge
        Handles.color = new Color(0.5f, 0f, 0.5f);

        // draw line for edge
        Handles.DrawLine(corners[0], corners[1]);
        Handles.DrawLine(corners[1], corners[5]);
        Handles.DrawLine(corners[5], corners[4]);
        Handles.DrawLine(corners[4], corners[0]);

        Handles.DrawLine(corners[2], corners[3]);
        Handles.DrawLine(corners[3], corners[7]);
        Handles.DrawLine(corners[7], corners[6]);
        Handles.DrawLine(corners[6], corners[2]);

        Handles.DrawLine(corners[0], corners[2]);
        Handles.DrawLine(corners[1], corners[3]);
        Handles.DrawLine(corners[4], corners[6]);
        Handles.DrawLine(corners[5], corners[7]);

        Handles.color = new Color(0.5f, 0f, 0.5f, 0.2f);

        Handles.DrawAAConvexPolygon(corners[0], corners[1], corners[3], corners[2]); // front face
        Handles.DrawAAConvexPolygon(corners[4], corners[5], corners[7], corners[6]); // back face
        Handles.DrawAAConvexPolygon(corners[0], corners[2], corners[6], corners[4]); // left face
        Handles.DrawAAConvexPolygon(corners[1], corners[3], corners[7], corners[5]); // right face
        Handles.DrawAAConvexPolygon(corners[2], corners[3], corners[7], corners[6]); // top face
        Handles.DrawAAConvexPolygon(corners[0], corners[1], corners[5], corners[4]); // bottom face
    }
}
