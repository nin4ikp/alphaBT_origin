using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SwipeTrail : MonoBehaviour {

    public GameObject trailPrefab;
    public GameObject drawBase;

    private GameObject soundManager;
    private GameObject sourceManager;
    private GameObject go_green;
    private GameObject go_black;
    private GameObject thisTrail;
    private Slider experienceSlider;

    private BoxCollider2D drawBaseColl;
    private Plane objPlane;
    private List<int> greenval;
    private List<string> randomLetters;
    private AudioClip wrongSound;
    private AudioSource letterSound;
    public string letter;
    private int goodanswers = 0;
    private int loadSceneAfterNumberofGood = 5;
    private int nextSceneIndex;

    private Texture2D gtex;
    private Color32 gcolor;

    private Vector3 startPos;
    private Vector3 actualPos;
    private Vector3 boxLimitsMin;
    private Vector3 boxLimitsMax;

    /** update the drawing */
    private float nexttime = 0;
    private float repeatingrate = 0.5f;

    /** check for input for a certain timeamount */
    private float nexttimeout = 0;
    private float nexttimeoutinsec = 4;

    private float bminx;
    private float bminy;
    private float bmaxx;
    private float bmaxy;
    private float imwidthhalf;
    private float imheighthalf;
    private float xscale;
    private float yscale;
    private float xmouse;
    private float ymouse;

    private string workingDir;
    private string fileDir;

    private bool setLetterNow;
    private bool setNextlevelNow;

    /// <summary>
    /// Sets the manager, directories and the nextSceneIndex for this Scene
    /// Initializes the base and the content for drawing.
    /// </summary>
    void Start()
    {
        sourceManager = GameObject.Find("SourceManager");

        nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        experienceSlider = GameObject.Find("Experience_Slider").GetComponent<Slider>();
        soundManager = GameObject.Find("SoundManager");

        soundManager.GetComponent<SoundManager>().singleSoundSource.clip = Resources.Load<AudioClip>(sourceManager.GetComponent<SourceManager>().GetWrongSoundDir());
        workingDir = sourceManager.GetComponent<SourceManager>().GetDirectory();
        fileDir = sourceManager.GetComponent<SourceManager>().GetFileDirectory();

        setLetterNow = false;

        InitBase();
        InitLetterBase();
    }

    /// <summary>
    /// Initializes the Plane for drawing,
    /// sets all boxlimints and max/min values and sets the randomLetters array with the letters
    /// </summary>
    void InitBase()
    {
        objPlane = new Plane(Camera.main.transform.forward * -1, this.transform.position);

        drawBaseColl = drawBase.GetComponent<BoxCollider2D>();
        boxLimitsMin = drawBaseColl.bounds.min;
        boxLimitsMax = drawBaseColl.bounds.max;
        bminx = boxLimitsMin.x; // -205.6
        bminy = boxLimitsMin.y; // -277.5
        bmaxx = boxLimitsMax.x; // 205.6
        bmaxy = boxLimitsMax.y; // 277.5

        randomLetters = new List<string>();
        randomLetters.Add(sourceManager.GetComponent<SourceManager>().GetLetterOne());
        randomLetters.Add(sourceManager.GetComponent<SourceManager>().GetLetterTwo());
    }

    /// <summary>
    /// Initializes the Base for the letters:
    /// - Loads the letters
    /// - sets the go_green and go_black Gameobjects to the corresponding objects in the prefab
    /// - Initializes the objects
    /// - sets the timeout time
    /// - sets the scaling values for converting from drawing coordinates
    /// </summary>
    void InitLetterBase()
    {
        int rand = Random.Range(0,2);
        letter = randomLetters[rand];

        //init sprites for direction calculation
        go_green = GameObject.Find("Draw_green");
        if (go_green == null)
            Debug.LogError("Careful! No Draw_green Object");
        go_black = GameObject.Find("Draw_black");
        if (go_black == null)
            Debug.LogError("Careful! No Draw_black Object");

        InitImage(go_green, letter, "green", true);    // set the Audioclip only on the green image
        InitImage(go_black, letter, "black", false);

        gtex = go_green.GetComponent<SpriteRenderer>().sprite.texture;
        greenval = new List<int>();
        nexttimeout = Time.time + nexttimeoutinsec;

        // scaling values for converting from drawing coordinates (x: -205.6 to 205.6, y: -277.5 to 277.5) to image coordinates (x: 0 to 1080, y: 0 to 1920)
        xscale = bmaxx / (go_green.GetComponent<SpriteRenderer>().sprite.texture.width / 2f);
        yscale = bmaxy / (go_green.GetComponent<SpriteRenderer>().sprite.texture.height / 2f);
        
        imwidthhalf = (float)(go_green.GetComponent<SpriteRenderer>().sprite.texture.width) / 2f;
        imheighthalf = (float)(go_green.GetComponent<SpriteRenderer>().sprite.texture.height) / 2f;
    }

    /// <summary>
    /// Initializes the drawing surface by loading the green or black sprite  version of a letter
    /// </summary>
    /// <param name="obj"></param> to which object belongs this sprite
    /// <param name="letter"></param> which letter shall be loaded
    /// <param name="greenorblack"></param> shall the green or black sprite be loaded for the given letter
    /// <param name="setAudio"></param> black sprites do not get the audio set. Only green ones, so if the green
    /// values are right, the letter will be spoken out loud.
    void InitImage(GameObject obj, string letter, string greenorblack, bool setAudio)
    {
        if (obj != null)
        {
            //sourceManager.GetComponent<SourceManager>().SetLetter(letter);
            obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(fileDir + letter + greenorblack);
            if (setAudio)
            {
                obj.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(fileDir + letter);
                letterSound = obj.GetComponent<AudioSource>();
            }
        }
        else
            Debug.LogError(obj.name + "letter not available");
    }

    /// <summary>
    /// Draw a Swipetrail, if the screen is touched
    /// </summary>
    void Update() {
        // if the touche ended, but it's wrong
        if (Time.time >= nexttimeout)
        {
            soundManager.GetComponent<SoundManager>().singleSoundSource.Play();
            Reset();
            Debug.Log("TRY AGAAAAIN!!!");
        }
        // if touched, start to draw
        if (setLetterNow == false && ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0))))
        {
            thisTrail = (GameObject)Instantiate(trailPrefab, this.transform.position, Quaternion.identity);
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            float rayDistance;
            if (objPlane.Raycast(mRay, out rayDistance))
            {
                startPos = mRay.GetPoint(rayDistance);
            }
            if (startPos.x < bminx || startPos.x > bmaxx || startPos.y < bminy || startPos.y > bmaxy)
            {
                if (thisTrail != null)
                    Destroy(thisTrail);
            }
            nexttimeout = Time.time + nexttimeoutinsec;
        }
        else if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) || (Input.GetMouseButton(0)))
        {
            // else draw the swipe
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float rayDistance;
            if (objPlane.Raycast(mRay, out rayDistance))
            {
                actualPos = mRay.GetPoint(rayDistance);
                xmouse = actualPos.x;
                ymouse = actualPos.y;

                if (thisTrail != null)
                {
                    if (xmouse < bminx || xmouse > bmaxx || ymouse < bminy || ymouse > bmaxy)
                        Destroy(thisTrail);
                    else
                    {
                        if (Time.time >= nexttime)
                        {
                            SetGColor(xmouse, ymouse);
                            nexttime = Time.time + repeatingrate;
                        }
                        thisTrail.transform.position = actualPos;
                    }
                }
            }
        }
        // if touch ended, finish drawing
        else if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) || (Input.GetMouseButtonUp(0)))
        {
            if (thisTrail != null)
            {
                if (Vector3.Distance(thisTrail.transform.position, startPos) < 0.1)
                    Destroy(thisTrail);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!letterSound.isPlaying)
        {
            if (setNextlevelNow)
            {
                GameControl.control.updateAttributes();
                GameControl.control.Save();
                sourceManager.GetComponent<SourceManager>().ChangeToScene(nextSceneIndex);
            }
            if (setLetterNow && !setNextlevelNow)
            {
                Reset();
                setLetterNow = false;
            }
        }
    }

    /// <summary>
    /// fills the greenval array with green values.
    /// If greenvalue surpasses the value 230, the player has made it to the end.
    /// If now all greenvalues are in the right order, the counter for good answers goes up,
    /// and either a new letter or the next scene are loaded. Depending, on the number of
    /// already good answers.
    /// </summary>
    /// <param name="xmouse"></param> x mouse position
    /// <param name="ymouse"></param> y mouse position
    void SetGColor(float xmouse, float ymouse)
    {
        gcolor = gtex.GetPixel((int)((xmouse / xscale) + imwidthhalf), (int)((ymouse / yscale) + imheighthalf));
        if (gcolor.a > 230)
            greenval.Add(gcolor.g);
        else
            greenval.Add(-1);

        if (gcolor.g >230)
        {
            if (CheckGColor())
            {
                StartCoroutine(PlaySound());
                goodanswers += 1;
                if (goodanswers >= loadSceneAfterNumberofGood)
                {
                    setNextlevelNow = true;
                }
                setLetterNow = true;
            }
            else
            {
                soundManager.GetComponent<SoundManager>().singleSoundSource.Play();
                setLetterNow = true;
                if (goodanswers > 0)
                    goodanswers -= 1;
                Debug.Log("TRY AGAAAAIN!!!");
            }
        }
    }
    /// <summary>
    /// checks, if the green values are all in the correct order
    /// </summary>
    /// <returns></returns> true or false
    bool CheckGColor()
    {
        if (greenval[0] != 0) {
            experienceSlider.GetComponent<Slider>().value -= 10;    // experienceSlider.GetComponent<Slider>().value
            GameControl.control.updateAttributes();
            return false;
        }
        for (int i = 0; i < greenval.Count-1; i++)
        {
            if (greenval[i] > greenval[i + 1])
            {
                experienceSlider.GetComponent<Slider>().value -= 10;    // experienceSlider.GetComponent<Slider>().value
                GameControl.control.updateAttributes();
                return false;
            }
        }
        experienceSlider.GetComponent<Slider>().value += 20;    // experienceSlider.GetComponent<Slider>().value
        GameControl.control.updateAttributes();
        return true;
    }

    /// <summary>
    /// Destroys the Wsipetrail and initializes the board with a letter
    /// </summary>
    private void Reset()
    {
        GameObject[] toClean = GameObject.FindGameObjectsWithTag("Trail");
        foreach (GameObject obj in toClean)
            Destroy(obj);
        InitLetterBase();
    }

    IEnumerator PlaySound()
    {
        letterSound.Play();
        yield return new WaitWhile( () => letterSound.isPlaying ); 
    }
}
