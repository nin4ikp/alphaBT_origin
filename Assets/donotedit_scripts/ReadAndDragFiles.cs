using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class ReadAndDragFiles : MonoBehaviour {

    public GameObject tile;
    private GameObject sourceManager;
    private int nextSceneIndex;

    private string posletter;
    private string negletter;
    private string text;

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

        posletter = sourceManager.GetComponent<SourceManager>().GetPosLetter();
        negletter = sourceManager.GetComponent<SourceManager>().GetNegLetter();

        /** initialise all Tiles, which are named in the .csv-file */
        postiles = InitTiles(posletter);
        negtiles = InitTiles(negletter);
        totalScore = postiles.Length + negtiles.Length;
        /** main Plane for getting the raycasts */
        objPlane = new Plane(Camera.main.transform.forward * -1, this.transform.position);
        /** get the baskets for score-calculation and final check, if all tiles are in the baskets*/
        bask1 = GameObject.Find("Basket");
        bask2 = GameObject.Find("Basket2");

        bask1.GetComponent<Basket>().SetLetter(posletter);
        bask2.GetComponent<Basket>().SetLetter(negletter);
    } 

    void Update()
    {
        /** when a tile is touched/clicked on it shall play the corresponding sound and
         *  save the position in the distance variable. Dragging is set to true, so the 
         *  the position of the Tile can be transformed equally to the actual mouse position. */
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
            /** check if all Tiles are in a basket */
            if (bask1.GetComponent<Basket>().getNumberofRight() + bask1.GetComponent<Basket>().getNumberofWrong()
                + bask2.GetComponent<Basket>().getNumberofRight() + bask2.GetComponent<Basket>().getNumberofWrong() == postiles.Length + negtiles.Length)
            {
                score = bask1.GetComponent<Basket>().getNumberofRight() + bask2.GetComponent<Basket>().getNumberofRight();
                
                Debug.Log("DONE");
                Debug.Log(score);
                Debug.Log("of");
                Debug.Log(totalScore);
                Debug.Log((float)score / (float)totalScore);
                if (nextSceneIndex == 0)
                    sourceManager.GetComponent<SourceManager>().GoToNextCounterletterPair();
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
        /** Transform equally to the mouse position */
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

    GameObject[] InitTiles(string myletter)
    {
        sourceManager.GetComponent<SourceManager>().SetLetter(myletter);
#if UNITY_EDITOR
        text = System.IO.File.ReadAllText(sourceManager.GetComponent<SourceManager>().GetTextDir() + "_pos1.csv");
#endif
#if UNITY_ANDROID
        text = System.IO.File.ReadAllText( sourceManager.GetComponent<SourceManager>().GetTextDir() + "_pos1.csv");
#endif
        string[] splittedText = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
        GameObject[] newObj = new GameObject[splittedText.Length - 1];
        for (int i = 0; i < splittedText.Length - 1; i++)
        {
            zVal = -10.0f + ((i + 2) / 10);
            clone = (GameObject)Instantiate(tile, new Vector3(UnityEngine.Random.Range(-180, 180), UnityEngine.Random.Range(-50, 50), zVal), Quaternion.identity);
            clone.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(sourceManager.GetComponent<SourceManager>().GetSpritesDir() + "_pos1/" + splittedText[i]);
            clone.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(sourceManager.GetComponent<SourceManager>().GetMusicContentDir() + "_pos1/" + splittedText[i]);
            clone.GetComponent<Dragable>().dragable = true;
            clone.GetComponent<Dragable>().letter = myletter;
            newObj[i] = clone;
        }
        return newObj;
    }

    GameObject GetClickedGameObject()
    {
        /** Builds a ray from camera point of view to the mouse position */
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        /** Casts the ray and get the first game object hit */
        if (Physics.Raycast(ray, out hit, Mathf.Infinity,lmask.value))  //
        {
            /** draw ray from behind in the 3D view to check for collision; yellow = hit */
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            /** take only those with the tag "Tiles" */
            Debug.Log(hit.collider.gameObject.tag);
            
            if (hit.collider.gameObject.tag == "Tiles")
            {
                if (hit.transform.gameObject.GetComponent<Dragable>().dragable == true)
                    return hit.transform.gameObject;
            }
        }
        /** draw ray from behind in the 3D view to check for collision, white = no hit */
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
        return null;
    }
}
