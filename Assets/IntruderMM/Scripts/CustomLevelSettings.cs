using UnityEngine;

public class CustomLevelSettings : MonoBehaviour
{
    public bool fogEnabled = false;
    public Color fogColor = Color.gray;
    public FogMode fogMode = FogMode.Exponential;
    public float fogDensity = 0.1f;
    public float fogStartDistance = 0f;
    public float fogEndDistance = 300f;
    public Color ambientLight = Color.white;
    public Material skybox;
    public float haloStrength = 1f;
    public float flareStrength = 1f;
    public float flareFadeSpeed = 0.1f;

    public LightProbes lightProbes;
    public bool forceFog = false;
    public string mmVersion = "";
    public string unityVersion = "";

    public void SetSettings()
    {
        this.hideFlags = HideFlags.HideInHierarchy;

        fogEnabled = RenderSettings.fog;
        fogColor = RenderSettings.fogColor;
        fogMode = RenderSettings.fogMode;
        fogDensity = RenderSettings.fogDensity;
        fogStartDistance = RenderSettings.fogStartDistance;
        fogEndDistance = RenderSettings.fogEndDistance;
        ambientLight = RenderSettings.ambientLight;
        skybox = RenderSettings.skybox;
        haloStrength = RenderSettings.haloStrength;
        flareStrength = RenderSettings.flareStrength;
        flareFadeSpeed = RenderSettings.flareFadeSpeed;

        lightProbes = LightmapSettings.lightProbes;
    }

    public void SetVersion(string mm)
    {
        mmVersion = mm;
        unityVersion = Application.unityVersion;
    }

    public void ApplySettings()
    {
        RenderSettings.fog = fogEnabled;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogMode = fogMode;
        RenderSettings.fogDensity = fogDensity;
        RenderSettings.fogStartDistance = fogStartDistance;
        RenderSettings.fogEndDistance = fogEndDistance;
        RenderSettings.ambientLight = ambientLight;
        RenderSettings.skybox = skybox;
        RenderSettings.haloStrength = haloStrength;
        RenderSettings.flareStrength = flareStrength;
        RenderSettings.flareFadeSpeed = flareFadeSpeed;

        LightmapSettings.lightProbes = lightProbes;
    }
}
