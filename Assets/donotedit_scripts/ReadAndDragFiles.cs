using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReadAndDragFiles : MonoBehaviour {

    public GameObject tile;

    private ParticleSystem particle;
    private Slider experienceSlider;
    private GameControl control;
    private GameObject sourceManager;
    private int nextSceneIndex;

    private string letterOne;
    private string letterTwo;
    private string files_dir;
    private string[] textOne;
    private string[] textTwo;

    private GameObject[] postiles;
    private GameObject[] negtiles;
    private Vector3 startPos;
    private Vector3 actualPos;
    private Plane objPlane;
    private GameObject clone;
    private GameObject thisObject;
    private GameObject bask1;
    private GameObject bask2;

    private bool dragging = false;
    private float distance;
    private float zVal;
    private int score = 0;
    private int totalScore;
    private LayerMask lmask = 1 << 8;// Tiles
    
    void Start() {
        sourceManager = GameObject.Find("SourceManager");
        files_dir = sourceManager.GetComponent<SourceManager>().GetFileDirectory();

        nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }

        experienceSlider = GameObject.Find("Experience_Slider").GetComponent<Slider>();
        control = GameObject.Find("GameControl").GetComponent<GameControl>();
        particle = GameObject.Find("Particles").GetComponent<ParticleSystem>(); 

        letterOne = sourceManager.GetComponent<SourceManager>().GetLetterOne();
        letterTwo = sourceManager.GetComponent<SourceManager>().GetLetterTwo();
        textOne = sourceManager.GetComponent<SourceManager>().GetWordsOne();
        textTwo = sourceManager.GetComponent<SourceManager>().GetWordsTwo();

        // initialise all Tiles, which are named in the .csv-file
        postiles = InitTiles(letterOne, textOne);
        negtiles = InitTiles(letterTwo, textTwo);
        totalScore = postiles.Length + negtiles.Length;

        // main Plane for getting the raycasts
        objPlane = new Plane(Camera.main.transform.forward * -1, this.transform.position);

        // get the baskets for score-calculation and final check, if all tiles are in the baskets
        bask1 = GameObject.Find("Basket");
        bask2 = GameObject.Find("Basket2");

        bask1.GetComponent<Basket>().letter = letterOne;
        bask1.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(files_dir + letterOne + ".wav");

        bask2.GetComponent<Basket>().letter = letterTwo;
        bask2.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(files_dir + letterTwo + ".wav");
    } 

    void Update()
    {
        // when a tile is touched/clicked on it shall play the corresponding sound and 
        // save the position in the distance variable. Dragging is set to true, so the 
        // the position of the Tile can be transformed equally to the actual mouse position.
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            || (Input.GetMouseButtonDown(0)))
        {
            distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            dragging = true;
            thisObject = GetClickedGameObject();
            if (thisObject != null)
                thisObject.GetComponent<AudioSource>().Play();
        }
        else if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) || (Input.GetMouseButton(0)))
        {
            distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        }
        else if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || (Input.GetMouseButtonUp(0)))
        {
            dragging = false;
            //check if all Tiles are in a basket
            // Show score. Later: decide whether this task has to be made again
            // load next scene
            if (bask1.GetComponent<Basket>().getNumberofRight() + bask1.GetComponent<Basket>().getNumberofWrong()
                + bask2.GetComponent<Basket>().getNumberofRight() + bask2.GetComponent<Basket>().getNumberofWrong() == postiles.Length + negtiles.Length)
            {
                score = bask1.GetComponent<Basket>().getNumberofRight() + bask2.GetComponent<Basket>().getNumberofRight();

                particle.Emit(50);
                experienceSlider.GetComponent<Slider>().value += 30;    // experienceSlider.GetComponent<Slider>().value
                GameControl.control.updateAttributes();
                //Debug.Log("DONE: "+ score + "of" + totalScore + ": " + (float)score / (float)totalScore);
                GameControl.control.Save();
                //SceneManager.LoadScene(nextSceneIndex);
                sourceManager.GetComponent<SourceManager>().ChangeToScene(nextSceneIndex);
            }
        }
        //Transform equally to the mouse position
        if (dragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            actualPos = ray.GetPoint(distance);
            if (thisObject != null && thisObject.tag != "Basket")
            {
                Vector3 pos = thisObject.transform.position;
                pos[0] = actualPos.x;
                pos[1] = actualPos.y;
                thisObject.transform.position = pos;
            }
        }
    }

    GameObject[] InitTiles(string myletter, string[] text)
    {
        //sourceManager.GetComponent<SourceManager>().SetLetter(myletter);
        GameObject[] newObj = new GameObject[text.Length - 1];
        for (int i = 0; i < text.Length - 1; i++)
        {
            zVal = -20.0f + ((i + 2) / 20);
            clone = (GameObject)Instantiate(tile, new Vector3(UnityEngine.Random.Range(-180, 180), UnityEngine.Random.Range(-50, 50), zVal), Quaternion.identity);
            clone.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(files_dir + text[i]);
            clone.gameObject.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(files_dir + text[i]);
            clone.gameObject.GetComponent<Dragable>().dragable = true;
            clone.gameObject.GetComponent<Dragable>().letter = myletter;
            newObj[i] = clone;
        }
        return newObj;
    }

    /// <summary>
    /// Returns the clicked gameobject or null, if the recieved touch did not touch
    /// any desired gameobject.
    /// </summary>
    /// <returns></returns> gameobject with tag "Tile" or "RightWrong"
    GameObject GetClickedGameObject()
    {
        //Builds a ray from camera point of view to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Input.mousePosition.z))
        //Ray ray = new Ray(transform.position, transform.forward * -1);
        RaycastHit hit;
        //Casts the ray and get the first game object hit
        if (Physics.Raycast(ray, out hit, Mathf.Infinity,lmask.value))
        {
            //draw ray from behind in the 3D view to check for collision; yellow = hit
            Debug.DrawRay(transform.position, -transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //take only those with the tag "Tiles"
            
            if (hit.collider.gameObject.tag == "Tiles")
            {
                if (hit.transform.gameObject.GetComponent<Dragable>().dragable == true)
                    return hit.transform.gameObject;
            }
        }
        //draw ray from behind in the 3D view to check for collision, white = no hit
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
        return null;
    }
}
