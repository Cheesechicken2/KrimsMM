using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class MapmakerWelcomePopup
{
    static MapmakerWelcomePopup()
    {
        EditorApplication.delayCall += ShowWelcomeMessage;
    }

    private static void ShowWelcomeMessage()
    {
        if (!EditorPrefs.HasKey("MapmakerPopupShown"))
        {
            WelcomeWindow.ShowWindow();
            EditorPrefs.SetBool("MapmakerPopupShown", true);
        }
    }
}

public class WelcomeWindow : EditorWindow
{
    private static Texture2D titleImage;
    private static Texture2D discordImage;
    private static Texture2D githubImage;
    private GUIStyle scaledLabelStyle;
    private static GUISkin customSkin;

    [MenuItem("Krimbopple's MM/Welcome Popup")]
    public static void ShowWindow()
    {
        titleImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IntruderMM/Scripts/Extension/Editor/GUI/KrimTitle.png");
        discordImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IntruderMM/Scripts/Extension/Editor/GUI/discord.png");
        githubImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IntruderMM/Scripts/Extension/Editor/GUI/github.png");

        WelcomeWindow window = GetWindow<WelcomeWindow>("Welcome");
        window.minSize = new Vector2(400, 400);
        window.maxSize = new Vector2(400, 400);
        window.Show();
    }

    private void OnEnable()
    {
        this.position = new Rect(200, 200, 500, 400);

        customSkin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/IntruderMM/Scripts/Extension/Editor/GUI/IntruderSkin.guiskin");

        if (customSkin != null)
        {
            scaledLabelStyle = new GUIStyle(customSkin.label);
        }
    }

    private void OnGUI()
    {
        if (customSkin != null)
        {
            GUI.skin = customSkin;
        }

        if (titleImage != null)
        {
            GUILayout.Label(titleImage, GUILayout.Height(100));
        }

        GUILayout.Space(10);

        EditorGUILayout.HelpBox("This is a heavily modded version of the regular mapmaker! Some old features may be completely changed.", MessageType.Warning);

        GUILayout.Space(5);

        EditorGUILayout.HelpBox("Hover over any tool for a tooltip explaining its function.", MessageType.Info);

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (discordImage != null)
        {
            if (GUILayout.Button(discordImage, GUILayout.Height(30), GUILayout.Width(150)))
            {
                Application.OpenURL("http://www.superbossgames.com/chat");
            }
        }

        GUILayout.Space(10);

        if (githubImage != null)
        {
            if (GUILayout.Button(githubImage, GUILayout.Height(30), GUILayout.Width(150)))
            {
                Application.OpenURL("https://github.com/Cheesechicken2/Krimbopples-IntruderMM");
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (GUILayout.Button("get off my screen"))
        {
            this.Close();
        }
    }
}
