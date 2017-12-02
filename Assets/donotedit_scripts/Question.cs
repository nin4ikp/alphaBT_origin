﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class Question : MonoBehaviour {

    public bool syllable = false;
    public string[] vocals = new string[3] { "a", "i", "u" };
    public GameObject PosTile;
    public GameObject QuestionTile;
    public GameObject TrueTile;
    public GameObject FalseTile;
    public Slider expSlider;

    private string posletter;
    private string negletter;
    private AudioClip rightAnswerClip;
    private AudioClip wrongAnswerClip;

    private string[] posList;
    private string[] negList;
    private List<string> wrongPosList;
    private List<string> wrongNegList;
    private List<string> rightList;
    private float distance;
    private int randletter;
    private int randsyl;
    private GameObject thisObject;
    private GameObject clone;
    private AudioSource aud;
    private int listIndex;
    private bool setTilesNow;

    private GameObject sourceManager;
    private int nextSceneIndex;

    private void Awake()
    {
        sourceManager = GameObject.Find("SourceManager");
        if (sourceManager == null)
        {
            Debug.LogError("Careful! No SourceManager!");
        }
        nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        //Debug.Log("ANZAHL SCENES!: " + SceneManager.sceneCountInBuildSettings);
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        posletter = sourceManager.GetComponent<SourceManager>().GetPosLetter();
        negletter = sourceManager.GetComponent<SourceManager>().GetNegLetter();

        //StartCoroutine(LoadSoundFromFile("right"));
        rightAnswerClip = Resources.Load<AudioClip>(sourceManager.GetComponent<SourceManager>().GetRightSoundDir());
        //Debug.Log(rightAnswerClip.name);
        wrongAnswerClip = Resources.Load<AudioClip>(sourceManager.GetComponent<SourceManager>().GetWrongSoundDir());
        //Debug.Log(wrongAnswerClip.name);

        wrongPosList = new List<string>();
        wrongNegList = new List<string>();
    }

    IEnumerator LoadSoundFromFile(string filename)
    {
        WWW www = new WWW("jar:file://" + Application.persistentDataPath + "!/assets/"+ filename +".wav");
        while (!www.isDone)
            yield return null;
        rightAnswerClip = www.GetAudioClip();
        yield return new WaitForSeconds(5);
    }

    private void Start()
    {
        posList = InitList(posletter, true);    // sets also the wrongPosList
        negList = InitList(negletter, false);   // sets also the wrongNegList

        randletter = 0;
        randsyl = 0;
        setTilesNow = false;

        SetGameObj(PosTile, posletter, posletter);
        SetGameObj(QuestionTile, wrongPosList[randsyl], posletter);
        SetAnswers(TrueTile, true);
        SetAnswers(FalseTile, false);
        rightList = new List<string>();
    }

    private void Update()
    {
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)))
        {
            thisObject = GetClickedGameObject();
            if (thisObject != null)
            {
                if (thisObject.tag == "RightWrong" && setTilesNow == false)
                {
                    aud = thisObject.gameObject.GetComponent<AudioSource>();
                    //Debug.Log("**********check Audio");
                    StartCoroutine(PlaySound(aud));
                    //Debug.Log("*********Audio played!");

                    if (thisObject.GetComponent<Rightorwrong>().rightorwrong == true)
                    {
                        expSlider.GetComponent<Slider>().value += 10;
                        if (randsyl < 0.5)
                        {
                            Debug.Log("correct answer");
                            Debug.Log(wrongPosList[listIndex]);
                            rightList.Add(wrongPosList[listIndex]);
                            wrongPosList.Remove(wrongPosList[listIndex]);
                        }
                        else if (randsyl >= 0.5)
                        {
                            Debug.Log("correct answer");
                            Debug.Log(wrongNegList[listIndex]);
                            rightList.Add(wrongNegList[listIndex]);
                            wrongNegList.Remove(wrongNegList[listIndex]);
                        }
                        setTilesNow = true;
                    }
                    else if (thisObject.GetComponent<Rightorwrong>().rightorwrong == false)
                    {
                        expSlider.GetComponent<Slider>().value -= 10;
                        setTilesNow = true;
                    }
                }
                else
                {
                    thisObject.gameObject.GetComponent<AudioSource>().Play();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (setTilesNow)
        {
            SetTilesAnew();
            setTilesNow = false;
        }
    }

    IEnumerator PlaySound(AudioSource clipSource)
    {
        clipSource.Play();
        yield return new WaitForSeconds(clipSource.clip.length);
    }

    string[] InitList(string myletter, bool pos)
    {
        sourceManager.GetComponent<SourceManager>().SetLetter(myletter);
        if(syllable == true)
        {
            string[] newObj = new string[6];
            for (int i = 0; i < vocals.Length; i++)
            {
                myletter = myletter.ToLower();
                newObj[i] = myletter + vocals[i];
                newObj[i + vocals.Length] = vocals[i] + myletter;
                if (pos)
                    wrongPosList.Add(newObj[i]);
                else
                    wrongNegList.Add(newObj[i]);
            }
            return newObj;
        }
        else
        {
            Debug.Log("readalltext from: " + sourceManager.GetComponent<SourceManager>().GetTextDir() + "_pos1.csv");
            string text = System.IO.File.ReadAllText(sourceManager.GetComponent<SourceManager>().GetTextDir() + "_pos1.csv");
            string[] splittedText = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            string[] newObj = new string[splittedText.Length - 1];
            for (int i = 0; i < splittedText.Length - 1; i++) {
                newObj[i] = splittedText[i];
                if (pos)
                    wrongPosList.Add(newObj[i]);
                else
                    wrongNegList.Add(newObj[i]);
            }
            return newObj;
        }
    }

    void SetTilesAnew()
    {
        randletter = UnityEngine.Random.Range(0, 2); // choose M (<0.5) or N
        randsyl = UnityEngine.Random.Range(0, 2); // syllable with M (<0.5) or N

        // don't forget to set the right answer
        if (randletter < 0.5 && (wrongPosList.Count != 0 || wrongNegList.Count != 0))
        {
            Debug.Log("inside posletter");
            SetGameObj(PosTile, posletter, posletter);  // M
            Debug.Log("inside posletter2");
            if (randsyl < 0.5 && wrongPosList.Count != 0) // M
            {
                Debug.Log("same letter and syl M");
                SetAnswers(TrueTile, true);
                SetAnswers(FalseTile, false);
                listIndex = UnityEngine.Random.Range(0, wrongPosList.Count);
                SetGameObj(QuestionTile, wrongPosList[listIndex], posletter);
            }
            else if (randsyl >= 0.5 && wrongNegList.Count != 0)  // N
            {
                Debug.Log("Letter M, Question N");
                SetAnswers(TrueTile, false);
                SetAnswers(FalseTile, true);
                listIndex = UnityEngine.Random.Range(0, wrongNegList.Count);
                SetGameObj(QuestionTile, wrongNegList[listIndex], negletter);
            }
            else if (wrongPosList.Count != 0)
            {
                randsyl = 0;
                Debug.Log("same letter and syl M");
                SetAnswers(TrueTile, true);
                SetAnswers(FalseTile, false);
                listIndex = UnityEngine.Random.Range(0, wrongPosList.Count);
                SetGameObj(QuestionTile, wrongPosList[listIndex], posletter);
            }
            else
            {
                randsyl = 1;
                Debug.Log("Letter M, Question N");
                SetAnswers(TrueTile, false);
                SetAnswers(FalseTile, true);
                listIndex = UnityEngine.Random.Range(0, wrongNegList.Count);
                SetGameObj(QuestionTile, wrongNegList[listIndex], negletter);
            }
        }
        else if (randletter >= 0.5 && (wrongPosList.Count != 0 || wrongNegList.Count != 0))
        {
            Debug.Log("inside negletter");
            // negletter
            SetGameObj(PosTile, negletter, negletter); // N
            Debug.Log("inside negletter2");
            if (randsyl < 0.5 && wrongPosList.Count != 0) // M
            {
                Debug.Log("Letter N, Question M");
                SetAnswers(TrueTile, false);
                SetAnswers(FalseTile, true);
                listIndex = UnityEngine.Random.Range(0, wrongPosList.Count);
                SetGameObj(QuestionTile, wrongPosList[listIndex], posletter);
            }
            else if (randsyl >= 0.5 && wrongNegList.Count != 0) // N
            {
                Debug.Log("same letter and syl N");
                SetAnswers(TrueTile, true);
                SetAnswers(FalseTile, false);
                listIndex = UnityEngine.Random.Range(0, wrongNegList.Count);
                SetGameObj(QuestionTile, wrongNegList[listIndex], negletter);
            }
            else if (wrongPosList.Count != 0)
            {
                randsyl = 0;
                Debug.Log("Letter N, Question M");
                SetAnswers(TrueTile, false);
                SetAnswers(FalseTile, true);
                listIndex = UnityEngine.Random.Range(0, wrongPosList.Count);
                SetGameObj(QuestionTile, wrongPosList[listIndex], posletter);
            }
            else
            {
                randsyl = 1;
                Debug.Log("same letter and syl N");
                SetAnswers(TrueTile, true);
                SetAnswers(FalseTile, false);
                listIndex = UnityEngine.Random.Range(0, wrongNegList.Count);
                SetGameObj(QuestionTile, wrongNegList[listIndex], negletter);
            }
        }
        else
        {
            Debug.Log("ALL ANSWERS DONE!");
            if (nextSceneIndex == 0)
                sourceManager.GetComponent<SourceManager>().GoToNextCounterletterPair();
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    void SetGameObj(GameObject obj, string myletter, string realLetter)
    {
        sourceManager.GetComponent<SourceManager>().SetLetter(realLetter);
        if (syllable == true)
        {
            if (myletter.Length > 1)
                obj.gameObject.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(sourceManager.GetComponent<SourceManager>().GetMusicContentDir() + "_syl/" + myletter);
            else
                obj.gameObject.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(sourceManager.GetComponent<SourceManager>().GetMusicContentDir());
        }
        else
        {
            if (myletter.Length > 1)
            {
                obj.gameObject.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(sourceManager.GetComponent<SourceManager>().GetMusicContentDir()+ "_pos1/" + myletter);
                obj.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(sourceManager.GetComponent<SourceManager>().GetSpritesDir() + "_pos1/" + myletter);
            }
            else
                obj.gameObject.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(sourceManager.GetComponent<SourceManager>().GetMusicContentDir());
        }
    }

    void SetAnswers(GameObject obj, bool trueorfalse)
    {
        obj.gameObject.GetComponent<Rightorwrong>().rightorwrong = trueorfalse;
        if (trueorfalse == true)
        {
            obj.gameObject.GetComponent<AudioSource>().clip = rightAnswerClip;
            Debug.Log(obj.GetComponent<AudioSource>().clip.name);
        }
        else if (trueorfalse == false)
        {
            obj.gameObject.GetComponent<AudioSource>().clip = wrongAnswerClip;
            Debug.Log(obj.GetComponent<AudioSource>().clip.name);
        }
    }

    GameObject GetClickedGameObject()
    {
        // Builds a ray from camera point of view to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Casts the ray and get the first game object hit
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
            // take only those with the tag "Tiles"
            if (hit.collider.gameObject.tag == "Tiles" || hit.collider.gameObject.tag == "RightWrong")
            {
                return hit.transform.gameObject;
            }
        }
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
        return null;
    }
}
