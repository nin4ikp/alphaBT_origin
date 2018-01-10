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

    private string wrongSoundDir;
    private string rightSoundDir;

    private string letter;
    private string levelsFile = "LevelsToLoad.csv";
    private string[] listOfCounterletters;
    private string[] wordsOne;
    private string[] wordsTwo;
    private string posLetter;
    private string negLetter;
    private string text;
    private string text2;

    private void Awake()
    {
        Debug.Log("SourceManager active.");

        path = GetPath();
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            filePath = GetFilePath(path);
            GetLettersFromFile(out posLetter, out negLetter);
            letter = posLetter;
            if (SceneManager.GetActiveScene().name == "Words")
            {
                GetWordsFromFile(letter, out wordsOne);
                GetWordsFromFile(negLetter, out wordsTwo);
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
        Debug.Log("Getting filePath...");
        return path.Replace("Assets/Resources/", "");
    }


    public void SetRightWrongSounds()
    {
    #if UNITY_EDITOR
        wrongSoundDir = "mainMusic/general/wrong";
        Debug.Log("wrongSound set: " + wrongSoundDir);
        rightSoundDir = "mainMusic/general/right";
        Debug.Log("rightSound set: " + rightSoundDir);
    #endif

    #if UNITY_ANDROID && !UNITY_EDITOR      // header
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
            Debug.Log("CHECK: " + posL + negL);
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

    private void GetWordsFromFile(string letter, out string[] words)
    {
        if (letter != null)
        {
            text = File.ReadAllText(path + letter + ".csv");
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

    public string GetPosLetter() { return posLetter; }
    public string GetNegLetter() { return negLetter; }
    public string[] GetWordsOne() { return wordsOne; }
    public string[] GetWordsTwo() { return wordsTwo; }
    public string GetDirectory() { return path; }
    public string GetFileDirectory() { return filePath; }

    public string GetWrongSoundDir() { return wrongSoundDir; }  //Debug.Log(wrongSoundDir); 
    public string GetRightSoundDir() { return rightSoundDir; }  // Debug.Log(rightSoundDir); }
}