using UnityEngine;
using System;
using TMPro;
/// <summary>
/// This script allows you to display a note with a message that can be split into pages. The pages can be navigated through the Unity Editor. 
/// Unity's StyledText feature (https://docs.unity3d.com/Packages/com.unity3d.com/ugui@1.0/manual/StyledText.html) allows you to apply rich text formatting like bold, italics, color, etc. 
/// To use it, you can format your text using the appropriate tags such as <b> for bold, <i> for italics, <color=#ff0000> for color, etc.
/// </summary>

public class NoteProxy : MonoBehaviour
{
    [TextAreaAttribute(10, 10)]
    public string message;

    public Activator activatorToActivate;

    [HideInInspector]
    public string[] pages;
    [HideInInspector]
    public int currentPage = 0;

    private void Awake()
    {
        UpdatePages();
    }

    private void OnValidate()
    {
        UpdatePages();
        UpdatePreviewText();
    }

    public void UpdatePages()
    {
        if (string.IsNullOrEmpty(message))
        {
            pages = new string[0];
        }
        else
        {
            pages = message.Split(new string[] { "[p]" }, StringSplitOptions.None);
        }

        if (pages.Length == 0)
        {
            currentPage = 0;
        }
        else if (currentPage >= pages.Length || currentPage < 0)
        {
            currentPage = 0;
        }
    }

    public void UpdatePreviewText()
    {
        Transform textTransform = transform.Find("Text");
        if (textTransform == null)
            return;

        TextMeshPro textMeshPro = textTransform.GetComponent<TextMeshPro>();
        if (textMeshPro == null)
            return;

        if (pages.Length > 0 && currentPage < pages.Length)
        {
            textMeshPro.text = pages[currentPage];
        }

        textMeshPro.gameObject.SetActive(true);
    }
}
