using System;
using UnityEditor;
using UnityEngine;

public enum PickupType
{
    Custom = -2,
    Null = -1,
    SniperRifle = 0,
    RedDot = 1,
    Shield = 2,
    Binoculars = 3,
    Banana = 4,
    SMG = 5,
    Pistol = 6,
    Grenade = 7,
    SmokeGrenade = 8,
    CSGrenade = 9,
    BushCamo = 10,
    LaserSensor = 11,
    RemoteCharge = 12,
    CardboardDecoy = 13,
    SMGAmmox30 = 14,
    PistolAmmox15 = 15,
    SniperAmmox5 = 16,
    Shotgun = 17,
    ShotgunAmmox6 = 18,
    SnowballLauncher = 19,
    SnowballLauncherAmmo = 20,
    Pistol2 = 21,
    BoxingGloves = 22,
    BloonGun = 23,
    BloonCam = 24,
    BananaRifleAmmo = 25,
    BananaRifle = 26,
    Medkit = 27,
    AR1 = 28,
    CaselessAmmo = 29
}

[ExecuteInEditMode]
public class PickupProxy : MonoBehaviour
{
    public ItemProxy pickupItem;
    [Header("Deprecated Settings (to be removed in 2.3)")]
    [Obsolete("This field will be deprecated in version 2.3.")]
    public PickupType pickupType = PickupType.Custom;

    public string pickupMessage = "";
    public int addedAmmo = -1;
    public int loadedAmmo = -1;
    public float respawnTime = -1;
    public Activator activatorToActivate;
    public ActivatorTeam teamsAllowed;

    private MeshFilter meshFilter;

    private void SetMeshFilter()
    {
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();
        }
    }

    private void AssignMesh(Mesh mesh, Vector3 scale)
    {
        SetMeshFilter();
        meshFilter.mesh = mesh;
        transform.localScale = scale;
    }

    protected void OnValidate()
    {
        if (pickupItem != null && pickupItem.pickupMesh != null)
        {
            AssignMesh(pickupItem.pickupMesh, Vector3.one);
        }
        else
        {
            Mesh defaultMesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            Vector3 defaultScale = new Vector3(0.1f, 0.4f, 0.4f);
            AssignMesh(defaultMesh, defaultScale);
        }
    }
}