using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TuningProxy : MonoBehaviour
{
    [TextArea(10, 50)]
    public string tuningParameters;

    public void CheckJSON()
    {
        string escName = UnityWebRequest.EscapeURL(tuningParameters);
        escName = escName.Replace("+", "%20");

        string url = "https://jsonlint.com/?json=" + escName;

        try
        {
            Application.OpenURL(url);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to open URL: " + e.Message);
        }
    }
}
