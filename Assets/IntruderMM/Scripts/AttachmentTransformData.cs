using System;
using UnityEngine;

[Serializable]
public class AttachmentTransformData
{

    public AttachmentBoneType attachmentBoneType;

    public Vector3 localPosition;

    public Vector3 localRotation;

    public float scaleMultiplier = 1f;
}