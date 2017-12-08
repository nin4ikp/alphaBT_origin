using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour {

    public string letter = "M";
    public SourceManager sourceManager;

    private int nrrightTiles = 0;
    private int nrwrongTiles = 0;
    private bool truefalseAnswer = false;
    private float distance;

    private Vector3 actualPos;
    private Plane objPlane;
    private LayerMask lmask = 1 << 9;// Basket
    private GameObject thisObject;

    private void Start()
    {
        objPlane = new Plane(Camera.main.transform.forward * -1, this.transform.position);
    }

    /*
    private void Update()
    {
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)))
        {
            distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            thisObject = GetClickedGameObject();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            actualPos = ray.GetPoint(distance);
            if (thisObject != null)
            {
                if (thisObject.GetComponent<AudioSource>() != null)
                {
                    Debug.Log("Wie oft werd ich gespielt?");
                    thisObject.GetComponent<AudioSource>().Play();
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject coll = other.gameObject;
        if (coll.GetComponent<Dragable>().dragable == true)
        {
            if (coll.GetComponent<Dragable>().letter == this.GetComponent<Basket>().letter)
            {
                Debug.Log("yes, ended! right");
                coll.GetComponent<Dragable>().dragable = false;
                truefalseAnswer = true;
                nrrightTiles += 1;
            }
            else if (coll.GetComponent<Dragable>().letter != this.GetComponent<Basket>().letter)
            {
                Debug.Log("yes, ended! wrong");
                coll.GetComponent<Dragable>().dragable = false;
                truefalseAnswer = false;
                nrwrongTiles += 1;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject coll = other.gameObject;
        if (coll.GetComponent<Dragable>().dragable == false)
        {
            Debug.Log("Exited! ");
            coll.GetComponent<Dragable>().dragable = true;
            if (truefalseAnswer == true)
            {
                nrrightTiles -= 1;
            }
            else if (truefalseAnswer == false)
            {
                nrwrongTiles -= 1;
            }
        }
    }

    public int getNumberofRight()
    {
        return nrrightTiles;
    }
    public int getNumberofWrong()
    {
        return nrwrongTiles;
    }
    public void SetLetter(string let)
    {
        letter = let;
        sourceManager.GetComponent<SourceManager>().SetLetter(letter);
        this.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>(sourceManager.GetComponent<SourceManager>().GetMusicContentDir());

    }

    GameObject GetClickedGameObject()
    {
        //Builds a ray from camera point of view to the mouse position 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //Casts the ray and get the first game object hit 
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, lmask.value))  //, lmask.value
        {
            //draw ray from behind in the 3D view to check for collision; yellow = hit
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);#
            Debug.Log("Drin");
            return hit.transform.gameObject;
        }
        //draw ray from behind in the 3D view to check for collision, white = no hit
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
        return null;
    }
    */
}
