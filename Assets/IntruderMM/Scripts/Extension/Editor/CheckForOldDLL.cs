#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

[InitializeOnLoad]
public class CheckForOldDLL
{
    private static string pluginFolderPath = "Assets/IntruderMM/Plugins";
    private static string[] validDllNames = { "KrimMM.dll" };
    private static string[] validFolderNames = { "Steam.NET" };

    static CheckForOldDLL()
    {
        checkandDeleteOLD();
    }

    static void checkandDeleteOLD()
    {
        // get dlls
        string[] dllFiles = Directory.GetFiles(pluginFolderPath, "*.dll", SearchOption.AllDirectories);

        foreach (string file in dllFiles)
        {
            string fileName = Path.GetFileName(file);

            // delete outdated dlls, but don't delete inside Steam.NET
            if (!ArrayContains(validDllNames, fileName) && !file.Contains("Steam.NET"))
            {
                Debug.Log("Outdated DLL found: " + fileName);
                File.Delete(file); // delete
                Debug.Log("Deleted DLL: " + fileName);
            }
        }

        // get folders
        string[] folders = Directory.GetDirectories(pluginFolderPath, "*", SearchOption.AllDirectories);
        foreach (string folder in folders)
        {
            string folderName = Path.GetFileName(folder);

            // delete outdated folders
            if (!ArrayContains(validFolderNames, folderName) && !folder.Contains("Steam.NET"))
            {
                Debug.Log("Outdated folder found: " + folderName);
                Directory.Delete(folder, true); // delete
                Debug.Log("Deleted folder: " + folderName);
            }
        }
    }

    static bool ArrayContains(string[] array, string value)
    {
        // check
        foreach (string item in array)
        {
            if (item.Equals(value))
            {
                return true;
            }
        }
        return false;
    }
}
#endif
