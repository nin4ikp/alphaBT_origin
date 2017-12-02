using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SourceManager : MonoBehaviour {

    private string path;
    private string filePath;
    private string filePath2;
    /** directories */

    /** main Music like background */
    private string mainMusicDir;
    /** content audio files like letters, syllables, words*/
    private string musicContentDir;

    /** sprites: images for the words */
    private string spritesDir;
    /** text files: every csv (list of contents) */
    private string textDir;

    /** videos (for later video output like mouth) */
    private string videoDir;

    private string wrongSoundDir;
    private string rightSoundDir;

    /** change letter on */
    private string letter;

    private string[] listOfCounterletters;
    private int actualCounterletterPos = 0;
    private string posLetter;
    private string negLetter;
    private string text;

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
