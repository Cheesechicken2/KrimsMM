using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Net.Http;
using System;
using System.Linq;

public class AssetDownloaderWindow : EditorWindow
{
    private List<AssetInfo> assetList = new List<AssetInfo>();
    private List<AssetInfo> filteredAssetList = new List<AssetInfo>();
    private string githubFileUrl = "https://raw.githubusercontent.com/Cheesechicken2/MMAssets/refs/heads/main/assetslist.txt";

    private string searchQuery = "";
    private string selectedTag = "All";

    private GUISkin customSkin;
    private Font customFont;
    private Texture2D fallbackIcon;
    private Texture2D refreshIcon;

    private HashSet<string> tags = new HashSet<string>();

    [MenuItem("Krimbopple's MM/Asset Downloader")]
    public static void ShowWindow()
    {
        GetWindow<AssetDownloaderWindow>("Asset Downloader");
    }

    private void OnEnable()
    {
        customSkin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/IntruderMM/Scripts/Extension/Editor/GUI/IntruderSkin.guiskin");
        customFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/IntruderMM/Scripts/Extension/Editor/GUI/Font/ShareTechMono-Regular.ttf");
        fallbackIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IntruderMM/Images/MapPreview.png");
        refreshIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/IntruderMM/Scripts/Extension/Editor/GUI/refresh.png");

        LoadAssetsFromGitHub();
    }

    private void OnGUI()
    {
        GUI.skin = customSkin;
        GUI.skin.label.font = customFont;

        GUILayout.Space(10);

        float windowWidth = position.width;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Search:", GUILayout.Width(windowWidth * 0.1f));
        searchQuery = GUILayout.TextField(searchQuery, GUILayout.Width(windowWidth * 0.4f));

        GUILayout.Space(10);

        GUILayout.Label("Filter by Tag:", GUILayout.Width(windowWidth * 0.15f));
        string[] tagOptions = new string[tags.Count + 1];
        tagOptions[0] = "All";
        tags.CopyTo(tagOptions, 1);
        int selectedIndex = Array.IndexOf(tagOptions, selectedTag);
        selectedIndex = EditorGUILayout.Popup(selectedIndex, tagOptions, GUILayout.Width(windowWidth * 0.3f));
        selectedTag = tagOptions[selectedIndex];
        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Reload Assets", GUILayout.Width(windowWidth * 1f)))
        {
            LoadAssetsFromGitHub();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        ApplyFilters();

        if (filteredAssetList.Count > 0)
        {
            foreach (var asset in filteredAssetList)
            {
                Texture2D iconToDisplay = asset.Icon ?? fallbackIcon;

                GUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent(iconToDisplay), GUILayout.Width(100), GUILayout.Height(100));
                GUILayout.Space(10);
                GUILayout.BeginVertical();

                GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.font = customFont;
                labelStyle.fontSize = Mathf.RoundToInt(GUI.skin.label.fontSize * 0.9f);

                if (!string.IsNullOrWhiteSpace(asset.Name))
                    GUILayout.Label("Name: " + asset.Name, labelStyle);

                if (!string.IsNullOrWhiteSpace(asset.Author))
                    GUILayout.Label("Author: " + asset.Author, labelStyle);

                if (!string.IsNullOrWhiteSpace(asset.Version))
                    GUILayout.Label("Version: " + asset.Version, labelStyle);

                if (!string.IsNullOrWhiteSpace(asset.Source))
                    GUILayout.Label("Source: " + asset.Source, labelStyle);

                if (asset.Tags != null && asset.Tags.Count > 0)
                    GUILayout.Label("Tags: " + string.Join(", ", asset.Tags), labelStyle);

                if (GUILayout.Button("Download " + asset.Name, GUILayout.Width(windowWidth * 0.5f)))
                {
                    DownloadAsset(asset.DownloadLink);
                }

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
        }
        else
        {
            GUILayout.Label("No assets found matching the criteria.", EditorStyles.centeredGreyMiniLabel);
        }
    }

    private void ApplyFilters()
    {
        filteredAssetList = assetList.FindAll(asset =>
            (string.IsNullOrEmpty(searchQuery) ||
             asset.Name.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0) &&
            (selectedTag == "All" ||
             (asset.Tags != null && asset.Tags.Contains(selectedTag))));
    }

    private async void LoadAssetsFromGitHub()
    {
        assetList.Clear();
        tags.Clear();

        string[] predefinedTags = { "Map", "Asset", "Texture", "Plugin" };

        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                client.DefaultRequestHeaders.Add("Pragma", "no-cache");

                string assetData = await client.GetStringAsync(githubFileUrl);

                string[] lines = assetData.Split('\n');
                foreach (string line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        string[] assetDetails = line.Split(',');

                        if (assetDetails.Length >= 5)
                        {
                            AssetInfo asset = new AssetInfo
                            {
                                Name = assetDetails[0].Trim(),
                                Author = assetDetails[1].Trim(),
                                Version = assetDetails[2].Trim(),
                                Source = assetDetails[3].Trim(),
                                DownloadLink = (assetDetails[4].Trim())
                            };

                            string iconUrl = assetDetails.Length > 5 ? assetDetails[5].Trim() : null;
                            if (!string.IsNullOrEmpty(iconUrl))
                            {
                                asset.Icon = DownloadIcon(iconUrl);
                            }

                            if (assetDetails.Length > 6)
                            {
                                string[] assetTags = assetDetails[6].Split('|');
                                asset.Tags = new List<string>();
                                foreach (var tag in assetTags)
                                {
                                    string trimmedTag = tag.Trim();
                                    if (Array.Exists(predefinedTags, predefinedTag => predefinedTag.Equals(trimmedTag, StringComparison.OrdinalIgnoreCase)))
                                    {
                                        asset.Tags.Add(trimmedTag);
                                        tags.Add(trimmedTag);
                                    }
                                }
                            }
                            else
                            {
                                asset.Tags = new List<string>();
                            }

                            assetList.Add(asset);
                        }
                    }
                }
            }

            ApplyFilters();
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading assets from GitHub: " + e.Message);
        }
    }

    private Texture2D DownloadIcon(string iconUrl)
    {
        try
        {
            string cacheBustedUrl = iconUrl + "?t=" + DateTime.Now.Ticks;

            WebClient client = new WebClient();
            byte[] imageData = client.DownloadData(cacheBustedUrl);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);
            return texture;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error downloading icon: " + e.Message);
            return null;
        }
    }

    private static readonly HttpClient client = new HttpClient();
    private async void DownloadAsset(string downloadLink)
    {
        if (string.IsNullOrEmpty(downloadLink))
        {
            return;
        }

        if (downloadLink.Contains("drive.google.com"))
        {
            EditorUtility.DisplayDialog("Unsupported Link", "Google Drive links are not supported in the editor. Opening in browser...", "OK");
            Application.OpenURL(downloadLink);
            return; 
        }

        string fileName = "UnknownFile";
        string filePath = Path.Combine(Application.dataPath, "DownloadedAssets");

        Directory.CreateDirectory(filePath);

        try
        {
            EditorUtility.DisplayProgressBar("Downloading Asset", "Starting download...", 0f);

            using (var response = await client.GetAsync(downloadLink, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                if (response.Content.Headers.ContentDisposition != null)
                {
                    fileName = response.Content.Headers.ContentDisposition.FileName.Trim('"');
                }
                else
                {
                    Uri uri = new Uri(downloadLink);
                    fileName = Path.GetFileName(uri.AbsolutePath);
                }

                string fullFilePath = Path.Combine(filePath, fileName);

                var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                var downloadedBytes = 0L;

                using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                              fileStream = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead;

                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                        downloadedBytes += bytesRead;

                        //progress bar
                        if (totalBytes > 0)
                        {
                            float progress = (float)downloadedBytes / totalBytes;
                            EditorUtility.DisplayProgressBar("Downloading Asset", $"Downloading {fileName} ({progress:P0})", progress);
                        }
                    }
                }

                Debug.Log($"Downloaded: {fileName} to {fullFilePath}");

                AssetDatabase.Refresh();

                if (fileName.EndsWith(".unitypackage"))
                {
                    EditorApplication.delayCall += () =>
                    {
                        try
                        {
                            AssetDatabase.ImportPackage(fullFilePath, true);

                            if (EditorUtility.DisplayDialog("Delete Package", "The package was successfully imported. Do you want to delete the downloaded package?", "Yes", "No"))
                            {
                                DeletePackage(fullFilePath);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Error importing package: " + e.Message);
                        }
                    };
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error downloading asset: " + e.Message);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }


    private void DeletePackage(string filePath)
    {
        if (File.Exists(filePath))
        {
            try
            {
                EditorApplication.delayCall += () =>
                {
                    if (!IsFileLocked(filePath))
                    {
                        try
                        {
                            File.Delete(filePath);
                            Debug.Log("Deleted downloaded package: " + filePath);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Error deleting package: " + e.Message);
                        }
                    }
                    else
                    {
                        Debug.LogError("File is locked or in use: " + filePath);
                    }
                };
            }
            catch (Exception e)
            {
                Debug.LogError("Error scheduling package deletion: " + e.Message);
            }
        }
    }

    private bool IsFileLocked(string filePath)
    {
        try
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                stream.Close();
            }
        }
        catch (IOException)
        {
            return true;
        }
        return false;
    }


    public class AssetInfo
    {
        public string Name;
        public string Author;
        public string Version;
        public string Source;
        public string DownloadLink;
        public Texture2D Icon;
        public List<string> Tags;
    }
}
