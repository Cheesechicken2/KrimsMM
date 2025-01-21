using System.Collections;
using UnityEngine;

public class Joystick : MonoBehaviour
{
	public GameObject myObject;

	public Vector2 sensitivity = new Vector2(1f, 1f);

	public float updateRate = 0.25f;

	private Quaternion lastRot;

	private Quaternion desiredRot;

	public Pivot myPivot;

	private const float _sensitivityMult = 4f;

	public Vector3 startLocalRotation;

	public bool resetRotationOnNewRound = true;

	public void OnEnable()
	{
	}

	public void ResetRotation()
	{
	}

	public void Updater()
	{
	}

}