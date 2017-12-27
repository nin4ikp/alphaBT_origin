using System.Collections;
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
    private string workingDir;
    private string fileDir;
    private AudioClip rightAnswerClip;
    private AudioClip wrongAnswerClip;

    private List<string> posList;
    private List<string> negList;
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
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }

        posList = new List<string>();
        negList = new List<string>();
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
        posletter = sourceManager.GetComponent<SourceManager>().GetPosLetter();
        negletter = sourceManager.GetComponent<SourceManager>().GetNegLetter();
        workingDir = sourceManager.GetComponent<SourceManager>().GetDirectory();
        fileDir = sourceManager.GetComponent<SourceManager>().GetFileDirectory();
        rightAnswerClip = Resources.Load<AudioClip>(sourceManager.GetComponent<SourceManager>().GetRightSoundDir());
        wrongAnswerClip = Resources.Load<AudioClip>(sourceManager.GetComponent<SourceManager>().GetWrongSoundDir());

#if false
        Debug.Log(posletter);
        Debug.Log(negletter);
        Debug.Log(workingDir);
        Debug.Log(rightAnswerClip.name);
        Debug.Log(wrongAnswerClip.name);
#endif

        InitList(posletter, true);    // sets the posList
        InitList(negletter, false);   // sets the negList

        randletter = 0;
        randsyl = 0;
        setTilesNow = false;

        SetGameObj(PosTile, posletter, posletter);
        SetGameObj(QuestionTile, posList[randsyl], posletter);
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
                    StartCoroutine(PlaySound(aud));     // Zustandsdiagramm!!!!!!!!!!!!!!!!
                    //Debug.Log("*********Audio played!");
                    bool tmp = thisObject.GetComponent<Rightorwrong>().rightorwrong;
                    switch (tmp)
                    {
                        case true:
                            expSlider.GetComponent<Slider>().value += 10;
                            switch (randsyl)
                            {
                                case 0: rightList.Add(posList[listIndex]); posList.Remove(posList[listIndex]); break;
                                case 1: rightList.Add(negList[listIndex]); negList.Remove(negList[listIndex]); break;
                            }
                            setTilesNow = true;
                            break;
                        case false:
                            expSlider.GetComponent<Slider>().value -= 10;
                            setTilesNow = true;
                            break;
                    }
                }
                else
                {
                    //Debug.Log("got in here: " + thisObject.gameObject.GetComponent<AudioSource>().name);
                    //Debug.Log("*** playing sound: " + thisObject.GetComponent<AudioSource>().clip.name );
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
        Debug.Log("***** Play:" + clipSource.clip.name );
        clipSource.Play();
        yield return new WaitForSeconds(clipSource.clip.length);
    }

    void InitList(string myletter, bool pos)
    {
        if(syllable == true)
        {
            string[] newObj = new string[6];
            for (int i = 0; i < GetVocals().Length; i++)
            {
                myletter = myletter.ToLower();
                newObj[i] = myletter + vocals[i];
                newObj[i + vocals.Length] = vocals[i] + myletter;
                if (pos)
                    posList.Add(newObj[i]);
                else
                    negList.Add(newObj[i]);
            }
        }
        else
        {
            string[] text = null;

            if (pos)
                text = sourceManager.GetComponent<SourceManager>().GetPosWords();
            else
                text = sourceManager.GetComponent<SourceManager>().GetNegWords();

            string[] newObj = new string[text.Length - 1];
            for (int i = 0; i < text.Length - 1; i++) {
                newObj[i] = text[i];
                if (pos)
                    posList.Add(newObj[i]);
                else
                    negList.Add(newObj[i]);
            }
        }
    }

    private string[] GetVocals()
    {
        return vocals;
    }

    void SetTilesAnew()
    {
        randletter = UnityEngine.Random.Range(0, 2); // choose M (<0.5) or N
        randsyl = UnityEngine.Random.Range(0, 2); // syllable with M (<0.5) or N
        Debug.Log(randletter);
        Debug.Log(randsyl);

        // don't forget to set the right answer
        if (randletter == 0 && (posList.Count != 0 || negList.Count != 0))
        {
            // posLetter
            SetGameObj(PosTile, posletter, posletter);  // M
            if (randsyl == 0 && posList.Count != 0)         { SettingTilesAndAnswers(true, false, posList, posletter); }// same letter and syl M
            else if (randsyl == 1 && negList.Count != 0)    { SettingTilesAndAnswers(false, true, negList, negletter); }// Letter M, Question N
            else if (posList.Count != 0)                    { randsyl = 0; SettingTilesAndAnswers(true, false, posList, posletter); }// same letter and syl M
            else                                            { randsyl = 1; SettingTilesAndAnswers(false, true, negList, negletter); }// Letter M, Question N
        }
        else if (randletter == 1 && (posList.Count != 0 || negList.Count != 0))
        {
            // negLetter
            SetGameObj(PosTile, negletter, negletter); // N
            if (randsyl == 0 && posList.Count != 0)         { SettingTilesAndAnswers(false, true, posList, posletter); } // Letter N, Question M
            else if (randsyl == 1 && negList.Count != 0)    { SettingTilesAndAnswers(true, false, negList, negletter); } // same letter and syl N
            else if (posList.Count != 0)                    { randsyl = 0; SettingTilesAndAnswers(false, true, posList, posletter); } // Letter N, Question M
            else                                            { randsyl = 1; SettingTilesAndAnswers(true, false, negList, negletter); } // same letter and syl N
        }
        else
        {
            Debug.Log("ALL ANSWERS DONE!");
            sourceManager.GetComponent<SourceManager>().ChangeToScene(nextSceneIndex);
        }
    }

    void SettingTilesAndAnswers( bool truetile, bool falsetile, List<string> list, string letter)
    {
        SetAnswers(TrueTile, truetile);
        SetAnswers(FalseTile, falsetile);
        listIndex = UnityEngine.Random.Range(0, list.Count);
        SetGameObj(QuestionTile, list[listIndex], letter);
    }


    void SetGameObj(GameObject obj, string myletter, string realLetter)
    {
        switch (syllable)
        {
            case true:
                if (myletter.Length > 1)
                {
                    obj.gameObject.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(fileDir + myletter);
                    obj.gameObject.GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

                }
                else
                {
                    obj.gameObject.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(fileDir + realLetter);
                    obj.gameObject.GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                }
                break;
            case false:
                if (myletter.Length > 1)
                {
                    obj.gameObject.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(fileDir + myletter);
                    obj.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(fileDir + myletter);
                }
                else
                {
                    obj.gameObject.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(fileDir + realLetter);
                    obj.gameObject.GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                }
                break;
        }
    }

    void SetAnswers(GameObject obj, bool trueorfalse)
    {
        obj.gameObject.GetComponent<Rightorwrong>().rightorwrong = trueorfalse;
        if (trueorfalse == true)
        {
            obj.gameObject.GetComponent<AudioSource>().clip = rightAnswerClip;
            //Debug.Log(obj.gameObject.name + " Audioclip set: " + obj.GetComponent<AudioSource>().clip.name);
        }
        else if (trueorfalse == false)
        {
            obj.gameObject.GetComponent<AudioSource>().clip = wrongAnswerClip;
            //Debug.Log(obj.gameObject.name + " Audioclip set: " + obj.GetComponent<AudioSource>().clip.name);
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
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red); //assert
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
