using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.IO;

public class SourceManager : MonoBehaviour
{
    private string path;
    private string filePath;
    private string filePath2;
    private string mainMusicDir;
    private string musicContentDir;
    private string spritesDir;
    private string textDir;
    private string videoDir;

    private string wrongSoundDir;
    private string rightSoundDir;

    private string letter;
    private string levelsFile = "LevelsToLoad.csv";
    private string[] listOfCounterletters;
    private string[] listOfLevelPaths;
    private int actualCounterletterPos = 0;
    private string posLetter;
    private string negLetter;
    private string text;

    private void Awake()
    {
        Debug.Log("SourceManager active.");
        /*
        path = GetPath();
        Debug.Log(path);
        listOfLevelPaths = GetLevels(path);
#if UNITY_EDITOR
        Debug.Log("In UNITY_EDITOR");
        CustomizedBuild(listOfLevelPaths);
#endif
        */
        DontDestroyOnLoad(transform.gameObject);
    }
    /*
    private string GetPath()
    {
        return Path.GetFullPath(Application.dataPath);
    }

    private string[] GetLevels(string path)
    {
        if (Directory.Exists(path))
        {
            string levelsInText = File.ReadAllText(path + "/" + levelsFile );
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
#if UNITY_EDITOR
    public static void CustomizedBuild(string[] levels)
    {
        Debug.Log("starting Customized Build...");
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions()
        {
            scenes = new[] {"donotedit_scenes/Main.unity"},
            locationPathName = "AndroidBuild",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };
        BuildPipeline.BuildPlayer(buildPlayerOptions);

        Debug.Log("Customized Build performed.");
    }
#endif

    */
    private void ChangeToScene(int number)
    {
        if (number < SceneManager.sceneCountInBuildSettings)
        {
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(number))
                SceneManager.LoadScene(number);
            else
                Debug.LogError("Scene already active.");
        }
        else
        {
            Debug.LogError("Scenenumber is not within the Build Settings.");
        }
    }

}
    /*
    private void Awake()
    {


        MakePath();
        SetRightWrongSounds();

        DontDestroyOnLoad(transform.gameObject);
        GetAllLettersFromFile();
        GetActualCounterletters(actualCounterletterPos, out posLetter, out negLetter);
        Debug.Log("after init: " +posLetter + negLetter);
        letter = posLetter;

        InitLetterSpec();
    }

    public void MakePath()
    {
#if UNITY_EDITOR
        path = Application.dataPath + "/Resources/";
        Debug.Log("in UNITY_EDITOR: " +path);
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("CHECK!");
        filePath = "jar:file://" + Application.dataPath + "!/assets/all_paths.txt";
        WWW wwwfile = new WWW(path);
        while (!wwwfile.isDone) { }
        var filepath2 = string.Format("{0}/{1}", Application.persistentDataPath, "all_paths.txt");
        File.WriteAllBytes(filepath2, wwwfile.bytes);
   
        StreamReader wr = new StreamReader(filepath2);
        string line;
        while ((line = wr.ReadLine()) != null)
        {
            path = Application.streamingAssetsPath;
            Debug.Log(line);
        }
#endif
    }

    void InitLetterSpec()
    {
        Debug.Log("Initialising SourceManager...");
#if UNITY_EDITOR
        spritesDir = "Sprites/" + letter + "/" + letter;
        Debug.Log("spritesDir" + spritesDir);
        textDir = GetPath() + "/Text/" + letter + "/" + letter;
        Debug.Log("textDir" + textDir);
        musicContentDir = "MusicContent/" + letter + "/" + letter;
        Debug.Log("musicContentDir" + musicContentDir);
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
        spritesDir = GetPath();
        musicContentDir = GetPath();
        textDir = GetPath();
#endif
    }

    void GetAllLettersFromFile()
    {
#if UNITY_EDITOR
        text = System.IO.File.ReadAllText(path +"/Text/Letters.csv");
        Debug.Log(text);
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        text = System.IO.File.ReadAllText(path + "Letters.csv");
#endif
        listOfCounterletters = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
        Debug.Log(listOfCounterletters[0]);
    }

    void GetActualCounterletters(int pos, out string posL, out string negL)
    {
        string[] counterLetters = listOfCounterletters[pos].Split(new string[] { ";" }, StringSplitOptions.None);
        posL = counterLetters[0];
        negL = counterLetters[1];
    }

    void SetRightWrongSounds()
    {
#if UNITY_EDITOR
        wrongSoundDir = "mainMusic/main_music/general/wrong";
        Debug.Log("wrongSound set: " + wrongSoundDir);
        rightSoundDir = "mainMusic/main_music/general/right";
        Debug.Log("rightSound set: " + rightSoundDir);
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        wrongSoundDir = "jar:file://" + Application.streamingAssetsPath + "!/assets/wrong";
        rightSoundDir = "jar:file://" + Application.streamingAssetsPath + "!/assets/right";
#endif
    }

    public void GoToNextCounterletterPair()
    {
        actualCounterletterPos += 1;
        if (actualCounterletterPos >= listOfCounterletters.Length)
            actualCounterletterPos = 0;
        GetActualCounterletters(actualCounterletterPos, out posLetter, out negLetter);
        Debug.Log("posletter: " + posLetter);
        Debug.Log("negLetter: " + negLetter);
    }

    public void SetLetter(string let)
    {
        letter = let;
        InitLetterSpec();
    }

    public string GetPosLetter() { return posLetter; }
    public string GetNegLetter() { return negLetter; }
    public string GetPath() { return path; }

    public string GetWrongSoundDir() { return wrongSoundDir; }  // Debug.Log(wrongSoundDir);
    public string GetRightSoundDir() { return rightSoundDir; }  // Debug.Log(rightSoundDir); 
    public string GetSpritesDir() { return spritesDir; }    // Debug.Log(spritesDir); 
    public string GetMusicContentDir() { return musicContentDir; }  // Debug.Log(musicContentDir);
    public string GetTextDir() { return textDir; }  // Debug.Log(textDir);
}
*/