using UnityEditor;
using UnityEngine;

public class KrimbopplesMMCredits : EditorWindow
{
    private GUISkin intruderSkin;
    private Font customFont;
    private Texture2D polyIcon;

    [MenuItem("Krimbopple's MM/Credits")]
    public static void ShowWindow()
    {
        KrimbopplesMMCredits window = GetWindow<KrimbopplesMMCredits>("Credits");
        window.minSize = new Vector2(500, 500); 
        window.maxSize = new Vector2(500, 500); 
        window.Show();
    }

    private void OnEnable()
    {
        intruderSkin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/IntruderMM/Scripts/Extension/Editor/GUI/IntruderSkin.guiskin");

        customFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/IntruderMM/Scripts/Extension/Editor/GUI/Font/ShareTechMono-Regular.ttf");

        polyIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IntruderMM/Scripts/Extension/Editor/GUI/Credits/poly.png");

        if (intruderSkin != null && customFont != null)
        {
            intruderSkin.font = customFont;

            intruderSkin.label.font = customFont;
            intruderSkin.box.font = customFont;
            intruderSkin.button.font = customFont;
        }
    }

    private void OnGUI()
    {
        if (intruderSkin != null)
        {
            GUI.skin = intruderSkin;
        }

        GUILayout.Space(10);
        GUILayout.Label("Krimbopple's MM - Credits", new GUIStyle(EditorStyles.boldLabel)
        {
            font = customFont,
            fontSize = 20, 
            alignment = TextAnchor.MiddleCenter 
        });

        GUILayout.Space(20);

        GUILayout.Label("Special Thanks", new GUIStyle(EditorStyles.boldLabel)
        {
            font = customFont,
            fontSize = 16, 
            alignment = TextAnchor.MiddleCenter 
        });


        GUILayout.Space(10);

        DrawCreditsEntry("Polybius", "For going out of his way and creating the new HeliGun Proxy. And also offering to help out with the custom icons.", polyIcon);
        DrawCreditsEntry("Mattbatt", "Offering suggestions on how to make the mapmaker beginner friendly. And putting me on Rob's watchlist. >:D");
        DrawCreditsEntry("PBegood", "Major help with the lightmapping scripts, but he made 2 bridges so he sucks.");
        DrawCreditsEntry("Rob Storm/Clor", "For making Intruder, and the scripts that make this possible.");
        DrawCreditsEntry("JakeSayingWoosh", "Making the mapmaker UI that this project was founded upon.");

        GUILayout.Space(40);

        GUILayout.Label("Thank you for using Krimbopple's MM!", new GUIStyle(EditorStyles.centeredGreyMiniLabel) { font = customFont });
    }

    private void DrawCreditsEntry(string name, string description, Texture2D icon = null)
    {
        GUIStyle nameStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            font = customFont,
            fontSize = 14, 
            alignment = TextAnchor.MiddleCenter 
        };

        GUIStyle descriptionStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
        {
            font = customFont,
            fontSize = 14, 
            alignment = TextAnchor.MiddleCenter
        };

        GUILayout.BeginVertical(EditorStyles.helpBox);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (icon != null)
        {
            GUILayout.Label(icon, GUILayout.Width(32), GUILayout.Height(32)); 
        }
        GUILayout.Label($"- {name}:", nameStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Centering the description
        GUILayout.Label(description, descriptionStyle);

        GUILayout.EndVertical();
        GUILayout.Space(10);
    }

}
