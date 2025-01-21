using System;
using UnityEngine;

[Serializable]
public class Pivot : MonoBehaviour
{
	public float multiplier = 1f;

	public int speed = 1;

	private Quaternion desiredRot;

	public Transform myTransform;

	public bool autoAddPhotonView;

}
