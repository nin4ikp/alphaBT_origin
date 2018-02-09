using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System;

public class SourceManager : MonoBehaviour
{
    private string path;
    private string filePath;

    private string wrongSoundDir;
    private string rightSoundDir;

    private string letter;
    private string levelsFile = "LevelsToLoad.csv";
    private string[] listOfCounterletters;
    private string[] wordsOne;
    private string[] wordsTwo;
    private string letterOne;
    private string letterTwo;
    private string text;
    private string text2;

    private void Awake()
    {
        Debug.Log("SourceManager active.");

        path = GetPath();
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            filePath = GetFilePath(path);
            GetLettersFromFile(out letterOne, out letterTwo);
            letter = letterOne;
            if (SceneManager.GetActiveScene().name == "Words" || SceneManager.GetActiveScene().name == "Container")
            {
                GetWordsFromFile(letter, out wordsOne);
                GetWordsFromFile(letterTwo, out wordsTwo);
            }
            SetRightWrongSounds();
        }
    }

    private string GetPath()
    {
        // Returns the path of the active scene
        Debug.Log("Getting path...");
        return SceneManager.GetActiveScene().path.Remove(SceneManager.GetActiveScene().path.LastIndexOf('/') + 1);
    }

    private string GetFilePath(string path)
    {
        // Returns the filepath of the active scene (removes "Assets/Resources/" part)
        Debug.Log("Getting filePath...");
        return path.Replace("Assets/Resources/", "");
    }


    public void SetRightWrongSounds()
    {
        // sets right and wrong sounds
        wrongSoundDir = "mainMusic/general/wrong";
        rightSoundDir = "mainMusic/general/right";
    }

    private void GetLettersFromFile(out string posL, out string negL)
    {
        if (path != null)
        {
            TextAsset txtAsset = (TextAsset)Resources.Load(filePath + "letters", typeof(TextAsset));
            string text = txtAsset.text;
            //text = File.ReadAllText(path + "letters.txt");
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
    }

    private void GetWordsFromFile(string letter, out string[] words)
    {
        if (letter != null)
        {
            TextAsset txtAsset = (TextAsset)Resources.Load(filePath + letter, typeof(TextAsset));
            string text = txtAsset.text;
            //text = File.ReadAllText(path + letter + ".csv");
            words = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
        }
        else
        {
            Debug.LogError("Letter not set (yet). Cannot set words list.");
            words = null;
        }
    }

    public void ChangeToScene(int number)
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

    public string GetLetterOne() { return letterOne; }
    public string GetLetterTwo() { return letterTwo; }
    public string[] GetWordsOne() { return wordsOne; }
    public string[] GetWordsTwo() { return wordsTwo; }
    public string GetDirectory() { return path; }
    public string GetFileDirectory() { return filePath; }

    public string GetWrongSoundDir() { return wrongSoundDir; }
    public string GetRightSoundDir() { return rightSoundDir; }
}