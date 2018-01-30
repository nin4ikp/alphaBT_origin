using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class Question : MonoBehaviour {

    public bool syllable = false;
    public string[] vocals = new string[3] { "a", "i", "u" };   // hardcoded firts. Will be extended afterwards
    public GameObject TileOne;
    public GameObject TileTwo;
    public GameObject CheckmarkTile;
    public GameObject CrossTile;
    public Slider experienceSlider;

    private string letterOne;
    private string letterTwo;
    private string workingDir;
    private string fileDir;
    private AudioClip rightAnswerClip;
    private AudioClip wrongAnswerClip;

    private List<string> posList;
    private List<string> negList;
    private List<string> rightList;
    private float distance;
    private int randomLetter;
    private int randomSyllable;
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
        letterOne = sourceManager.GetComponent<SourceManager>().GetLetterOne();
        letterTwo = sourceManager.GetComponent<SourceManager>().GetLetterTwo();
        workingDir = sourceManager.GetComponent<SourceManager>().GetDirectory();
        fileDir = sourceManager.GetComponent<SourceManager>().GetFileDirectory();
        rightAnswerClip = Resources.Load<AudioClip>(sourceManager.GetComponent<SourceManager>().GetRightSoundDir());
        wrongAnswerClip = Resources.Load<AudioClip>(sourceManager.GetComponent<SourceManager>().GetWrongSoundDir());

        InitList(letterOne, true);    // sets the posList
        InitList(letterTwo, false);   // sets the negList

        randomLetter = 0;
        randomSyllable = 0;
        setTilesNow = false;

        SetGameObjAudioAndColor(TileOne, letterOne, letterOne);
        SetGameObjAudioAndColor(TileTwo, posList[randomSyllable], letterOne);
        SetAnswers(CheckmarkTile, true);
        SetAnswers(CrossTile, false);
        rightList = new List<string>();
    }
    
    /// <summary>
    /// The Update function is the Gameloop.
    /// If it gets Input, it runs the Main-Routine.
    /// </summary>
    private void Update()
    {
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)))
        {
            MainRoutine();
        }
    }

    /// <summary>
    /// The MainRoutine is called, if touch-input is registered (see Update()).
    /// It checkt, if a game object was touched and differentiates between Answer-
    /// Tiles and simple Sound-Tiles with image or color.
    /// If an Answer-Tile was clicked, the Function checks, if the answer was right,
    /// sets the experience points respectively, plays the right or wrong sound and
    /// triggers the SetTilesAnew() function to reset all objects with other images,
    /// sounds and true/false-values.
    /// </summary>
    private void MainRoutine()
    {
        thisObject = GetClickedGameObject();
        //Assert.IsNotNull(thisObject);
        if (thisObject != null)
        {
            // if this is a RightWrong Tile, play its sound and check if answer is correct
            if (thisObject.tag == "RightWrong" && setTilesNow == false)
            {
                aud = thisObject.gameObject.GetComponent<AudioSource>();
                StartCoroutine(PlaySound(aud));
                bool answer = thisObject.GetComponent<Rightorwrong>().rightorwrong;
                switch (answer)
                {
                    case true:
                        experienceSlider.GetComponent<Slider>().value += 10;
                        switch (randomSyllable)
                        {
                            case 0:
                                rightList.Add(posList[listIndex]);
                                posList.Remove(posList[listIndex]);
                                break;
                            case 1:
                                rightList.Add(negList[listIndex]);
                                negList.Remove(negList[listIndex]);
                                break;
                        }
                        setTilesNow = true;
                        break;
                    case false:
                        experienceSlider.GetComponent<Slider>().value -= 10;
                        setTilesNow = true;
                        break;
                }
            }
            // otherwise only play the audio of the tile
            else
            {
                thisObject.gameObject.GetComponent<AudioSource>().Play();
            }
        }
    }

    private void FixedUpdate()
    {
        if (setTilesNow)
        {
            if (!aud.isPlaying)
            {
                SetTilesAnew();
                setTilesNow = false;
            }
        }
    }
    /// <summary> 
    /// Plays Sound ob an object and yields until it played.
    /// </summary>
    /// <param name="clipSource"></param> The clip source, which is to be played.
    /// <returns></returns> returns 
    IEnumerator PlaySound(AudioSource clipSource)
    {
        clipSource.Play();
        yield return new WaitForSeconds(clipSource.clip.length);
    }

    /// <summary>
    /// Initializes the Lists with words or syllables.
    /// </summary>
    /// <param name="myletter"></param>
    /// <param name="pos"></param> 
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
                text = sourceManager.GetComponent<SourceManager>().GetWordsOne();
            else
                text = sourceManager.GetComponent<SourceManager>().GetWordsTwo();

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
        randomLetter = Random.Range(0, 2); // choose M (<0.5) or N
        randomSyllable = Random.Range(0, 2); // syllable with M (<0.5) or N

        // don't forget to set the right answer
        if (randomLetter == 0 && (posList.Count != 0 || negList.Count != 0))
        {
            // letterOne
            SetGameObjAudioAndColor(TileOne, letterOne, letterOne);  // M
            if (randomSyllable == 0 && posList.Count != 0)      { SettingTilesAndAnswers(true, false, posList, letterOne); }// same letter and syl M
            else if (randomSyllable == 1 && negList.Count != 0) { SettingTilesAndAnswers(false, true, negList, letterTwo); }// Letter M, Question N
            else if (posList.Count != 0)                        { randomSyllable = 0; SettingTilesAndAnswers(true, false, posList, letterOne); }// same letter and syl M
            else                                                { randomSyllable = 1; SettingTilesAndAnswers(false, true, negList, letterTwo); }// Letter M, Question N
        }
        else if (randomLetter == 1 && (posList.Count != 0 || negList.Count != 0))
        {
            // letterTwo
            SetGameObjAudioAndColor(TileOne, letterTwo, letterTwo); // N
            if (randomSyllable == 0 && posList.Count != 0)      { SettingTilesAndAnswers(false, true, posList, letterOne); } // Letter N, Question M
            else if (randomSyllable == 1 && negList.Count != 0) { SettingTilesAndAnswers(true, false, negList, letterTwo); } // same letter and syl N
            else if (posList.Count != 0)                        { randomSyllable = 0; SettingTilesAndAnswers(false, true, posList, letterOne); } // Letter N, Question M
            else                                                { randomSyllable = 1; SettingTilesAndAnswers(true, false, negList, letterTwo); } // same letter and syl N
        }
        else
        {
            Debug.Log("ALL ANSWERS DONE!");
            sourceManager.GetComponent<SourceManager>().ChangeToScene(nextSceneIndex);
        }
    }

    void SettingTilesAndAnswers( bool truetile, bool falsetile, List<string> list, string letter)
    {
        SetAnswers(CheckmarkTile, truetile);
        SetAnswers(CrossTile, falsetile);
        listIndex = UnityEngine.Random.Range(0, list.Count);
        SetGameObjAudioAndColor(TileTwo, list[listIndex], letter);
    }

    /// <summary>
    /// The function sets the audioclip and the color/image
    /// of the object, depending if it is a syllable or words scene
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="letter"></param>
    /// <param name="realLetter"></param>
    void SetGameObjAudioAndColor(GameObject obj, string letter, string realLetter)
    {
        switch (syllable)
        {
            case true:
                if (letter.Length > 1)
                    SetAudioClipInTile(obj, letter, true);
                else
                    SetAudioClipInTile(obj, realLetter, true);
                break;
            case false:
                if (letter.Length > 1)
                    SetAudioClipInTile(obj, letter, false);
                else
                    SetAudioClipInTile(obj, realLetter, true);
                break;
        }
    }

    void SetAudioClipInTile(GameObject obj, string letter, bool color) {
        /** The function sets the audioclip and color/sprite of the object */
        obj.gameObject.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(fileDir + letter);
        if (color)
        {
            // give visual feedback by changing the color of the tile after each changing teh tile content
            obj.gameObject.GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
        else
        {
            // setting sprite in tile
            obj.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(fileDir + letter);
        }
    }

    void SetAnswers(GameObject obj, bool trueorfalse)
    {
        obj.gameObject.GetComponent<Rightorwrong>().rightorwrong = trueorfalse;
        if (trueorfalse == true)
        {
            obj.gameObject.GetComponent<AudioSource>().clip = rightAnswerClip;
        }
        else if (trueorfalse == false)
        {
            obj.gameObject.GetComponent<AudioSource>().clip = wrongAnswerClip;
        }
    }

    /// <summary>
    /// Returns the clicked gameobject or null, if teh recieved touch did not touch
    /// any desired gameobject.
    /// </summary>
    /// <returns></returns> gameobject with tag "Tile" or "RightWrong"
    GameObject GetClickedGameObject()
    {
        // Builds a ray from camera point of view to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Casts the ray and get the first game object hit
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.tag == "Tiles" || hit.collider.gameObject.tag == "RightWrong")
            {
                return hit.transform.gameObject;
            }
        }
        return null;
    }
}
