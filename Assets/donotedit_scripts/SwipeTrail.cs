using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwipeTrail : MonoBehaviour {

    public GameObject trailPrefab;
    public GameObject drawBase;

    private GameObject soundManager;
    private GameObject sourceManager;
    private GameObject green;
    private GameObject black;
    private GameObject thisTrail;

    private BoxCollider2D drawBaseColl;
    private Plane objPlane;
    private List<int> greenval;
    private List<string> randLetters;
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
    private float repeatingrate = 0.3f;

    /** check for input for a certain timeamount */
    private float nexttimeout = 0;
    private float nexttimeoutinsec = 3;

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

    /*
    void Start()
    {
        sourceManager = GameObject.Find("SourceManager");
        if (sourceManager == null)
        {
            Debug.LogError("Careful! No SourceManager!");
        }
        nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        Debug.Log("SANZAHL SCENES!: " + SceneManager.sceneCountInBuildSettings);
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        soundManager = GameObject.Find("SoundManager");
        if (soundManager == null)
        {
            Debug.LogError("Careful! No SoundManager!");
        }
        soundManager.GetComponent<SoundManager>().singleSoundSource.clip = Resources.Load<AudioClip>(sourceManager.GetComponent<SourceManager>().GetWrongSoundDir());

        InitBase();
        InitLetterBased();
    }

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

        randLetters = new List<string>();
        Debug.Log(sourceManager.GetComponent<SourceManager>().GetPosLetter());
        Debug.Log(sourceManager.GetComponent<SourceManager>().GetNegLetter());
        randLetters.Add(sourceManager.GetComponent<SourceManager>().GetPosLetter());
        randLetters.Add(sourceManager.GetComponent<SourceManager>().GetNegLetter());
    }

    void InitLetterBased()
    {
        int rand = Random.Range(0,2);
        letter = randLetters[rand];

        //init sprites for direction calculation
        green = GameObject.Find("Draw_green");
        if (green == null)
            Debug.LogError("Careful! No Draw_green Object");
        black = GameObject.Find("Draw_black");
        if (black == null)
            Debug.LogError("Careful! No Draw_black Object");

        InitImage(green, letter, "green", true);    // set the Audioclip only on the green image
        InitImage(black, letter, "black", false);

        gtex = green.GetComponent<SpriteRenderer>().sprite.texture;
        greenval = new List<int>();
        nexttimeout = Time.time + nexttimeoutinsec;

        // scaling values for converting from drawing coordinates (x: -205.6 to 205.6, y: -277.5 to 277.5) to image coordinates (x: 0 to 1080, y: 0 to 1920)
        xscale = bmaxx / (green.GetComponent<SpriteRenderer>().sprite.texture.width / 2f);
        yscale = bmaxy / (green.GetComponent<SpriteRenderer>().sprite.texture.height / 2f);
        
        imwidthhalf = (float)(green.GetComponent<SpriteRenderer>().sprite.texture.width) / 2f;
        imheighthalf = (float)(green.GetComponent<SpriteRenderer>().sprite.texture.height) / 2f;
    }

    void InitImage(GameObject obj, string letter, string greenorblack, bool setAudio)
    {
        if (obj != null)
        {
            sourceManager.GetComponent<SourceManager>().SetLetter(letter);
            obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(sourceManager.GetComponent<SourceManager>().GetSpritesDir() + "_" +  greenorblack);
            if (setAudio)
            {
                obj.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(sourceManager.GetComponent<SourceManager>().GetMusicContentDir());
                letterSound = obj.GetComponent<AudioSource>();
            }
        }
        else
            Debug.LogError(obj.name + "letter not available");
    }

    void Update() {
        if (Time.time >= nexttimeout)
        {
            soundManager.GetComponent<SoundManager>().singleSoundSource.Play();
            Reset();
            Debug.Log("TRY AGAAAAIN!!!");
        }

        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            || (Input.GetMouseButtonDown(0)))
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
                {
                    Destroy(thisTrail);
                }
            }
            nexttimeout = Time.time + nexttimeoutinsec;
        }
        else if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) || (Input.GetMouseButton(0)))
        {
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
                    {
                        Destroy(thisTrail);
                    }
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
        else if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) || (Input.GetMouseButtonUp(0)))
        {
            if (thisTrail != null)
            {
                if (Vector3.Distance(thisTrail.transform.position, startPos) < 0.1)
                {
                    Destroy(thisTrail);
                }
            }
        }
    }

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
                Debug.Log("YEAAAA!!");
                letterSound.Play();
                goodanswers += 1;
                if (goodanswers >= loadSceneAfterNumberofGood)
                {
                    if (nextSceneIndex == 0)
                        sourceManager.GetComponent<SourceManager>().GoToNextCounterletterPair();
                    SceneManager.LoadScene(nextSceneIndex);
                }
                Reset();
            }
            else
            {
                soundManager.GetComponent<SoundManager>().singleSoundSource.Play();
                Reset();
                if (goodanswers > 0)
                    goodanswers -= 1;
                Debug.Log("TRY AGAAAAIN!!!");
            }
        }
    }

    bool CheckGColor()
    {
        for (int i = 0; i < greenval.Count-1; i++)
        {
            if (greenval[i] > greenval[i + 1])
                return false;
        }
        return true;
    }

    private void Reset()
    {
        StartCoroutine(Waiting());
        GameObject[] toClean = GameObject.FindGameObjectsWithTag("Trail");
        foreach (GameObject obj in toClean)
            Destroy(obj);
        StartCoroutine(Waiting());
        InitLetterBased();
    }

    private IEnumerator Waiting()
    {
        yield return new WaitForSeconds(5);
    }
    */
}
