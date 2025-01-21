using UnityEngine;

public class ItemProxy : ScriptableObject
{
    [Header("Basic Information")]
    [Tooltip("Unique identifier for the item.")]
    public string idName;

    [Tooltip("Icon displayed in the inventory.")]
    public Sprite inventoryIcon;

    [Header("Pickup Configuration")]
    [Tooltip("Mesh used for the item's pickup representation.")]
    public Mesh pickupMesh;

    [Tooltip("Custom prefab for the item's pickup.")]
    public PickupProxy customPickupPrefab;

    [Tooltip("Custom name for the item.")]
    public string customItemName;

    [Tooltip("Description of the item.")]
    [TextArea]
    public string customItemDescription;

    [Header("Character Settings")]
    [Tooltip("Whether the item should be visible on the character.")]
    public bool showOnCharacter = false;

    [Tooltip("Transform data for attaching the item to the character.")]
    public AttachmentTransformData attachmentTransformData;

    [Header("Team Restrictions")]
    [Tooltip("Teams allowed to use this item.")]
    public ActivatorTeam teamsAllowed;
}
//krimbopple was here
// V
//. .
// ᗜ