using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogController : MonoBehaviour
{
    public float waterLevel = -1000f;

    public bool fog;

    public FogMode fogMode;

    public Color fogColor;

    public float fogStartDistance;

    public float fogEndDistance;

    public Color waterColor = Color.blue;

    public float waterStartDistance = 4f;

    public float waterEndDistance = 20f;

    public static bool bUnderWater;

    public Color dayColor;

    public Color nightColor;

    public float fogStartDay = 500f;

    public float fogEndDay = 5000f;

    public float fogStartNight = 100f;

    public float fogEndNight = 600f;

    public float fogDensity;

    public float waterDensity = 0.2f;

    public GameObject waterSounds;

    public GameObject aboveSounds;

    public Color currentSkyColor;

    public static FogController main;

}
