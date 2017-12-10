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

        path = GetPath();
        SetRightWrongSounds();
        GetLettersFromFile(out posLetter, out negLetter);
        letter = posLetter;

        //InitLetterSpec();
        DontDestroyOnLoad(transform.gameObject);
    }

    private string GetPath()
    {
        // Returns the path of the active scene
        Debug.Log("Getting path...");
        return SceneManager.GetActiveScene().path.Remove(SceneManager.GetActiveScene().path.LastIndexOf('/') + 1);
    }

    public void SetRightWrongSounds()
    {
#if UNITY_EDITOR
        wrongSoundDir = "mainMusic/general/wrong";
        Debug.Log("wrongSound set: " + wrongSoundDir);
        rightSoundDir = "mainMusic/general/right";
        Debug.Log("rightSound set: " + rightSoundDir);
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        wrongSoundDir = "jar:file://" + Application.streamingAssetsPath + "!/assets/wrong";
        rightSoundDir = "jar:file://" + Application.streamingAssetsPath + "!/assets/right";
#endif
    }

    private void GetLettersFromFile(out string posL, out string negL)
    {
#if UNITY_EDITOR
        if (path != null)
        {
            text = File.ReadAllText(path + "letters.txt");
            listOfCounterletters = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            posL = listOfCounterletters[0];
            negL = listOfCounterletters[1];
        }
        else
        {
            Debug.LogError("Path not set (yet). Cannot set pos and neg letters.");
            posL = null;
            negL = null;
        }

#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        text = File.ReadAllText(path + "letters.txt");
        listOfCounterletters = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
        posL = listOfCounterletters[0];
        negL = listOfCounterletters[1];
#endif
    }

    /*
    public void SetLetter(string let)
    {
        letter = let;
        InitLetterSpec();
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
    }*/

    public string GetPosLetter() { return posLetter; }
    public string GetNegLetter() { return negLetter; }
    public string GetDirectory() { return path; }

    public string GetWrongSoundDir() { return wrongSoundDir; Debug.Log(wrongSoundDir); }
    public string GetRightSoundDir() { return rightSoundDir; Debug.Log(rightSoundDir); }
}