using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

public class BuildUnityPlayer : MonoBehaviour
{
    private static string levelsFile = "LevelsToLoad.csv";

    public static void PerformBuild()
    {
        Debug.Log("BuildUnityPlayer active.");
        string path = GetPath();
        Debug.Log(path);
        string[] listOfLevelPaths = GetLevels(path);

        BuildPlayerOptions resetBuildPlayerOptions = new BuildPlayerOptions()
        {
            scenes = {},
            locationPathName = Application.dataPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };
        BuildPipeline.BuildPlayer(resetBuildPlayerOptions);

        EditorBuildSettingsScene[] original = new EditorBuildSettingsScene[0];
        EditorBuildSettings.scenes = original;

        foreach (var scenePath in listOfLevelPaths)
        {
            original = EditorBuildSettings.scenes;
            EditorBuildSettingsScene[] newSettings = new EditorBuildSettingsScene[original.Length + 1];
            Array.Copy(original, newSettings, original.Length);
            var sceneToAdd = new EditorBuildSettingsScene(scenePath, true);
            newSettings[newSettings.Length - 1] = sceneToAdd;
            EditorBuildSettings.scenes = newSettings;
        }

        Debug.Log("starting Customized Build...");
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions()
        {
            scenes = listOfLevelPaths,
            locationPathName = Application.dataPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        
        Debug.Log("Customized Build performed.");
    }

    private static string GetPath()
    {
        return Path.GetFullPath(Application.dataPath);
    }
    
    private static string[] GetLevels(string path)
    {
        if (Directory.Exists(path))
        {
            string levelsInText = File.ReadAllText(path + "/" + levelsFile);
            if (File.Exists("Assets/" + levelsFile))
            {
                string[] levelsInArray = levelsInText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                return levelsInArray;
            }
            else
            {
                Debug.LogError("Could not get file for levels. Check if file 'LevelsToLoad.csv' exists.");
                return null;
            }
        }
        else
        {
            Debug.LogError("Could not get path for levels. Check path for file 'LevelsToLoad.csv'.");
            return null;
        }
    }
}