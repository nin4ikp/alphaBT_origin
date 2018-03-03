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

    /// <summary>
    /// Initializes a Plaen for detecting rays.
    /// </summary>
    private void Start()
    {
        objPlane = new Plane(Camera.main.transform.forward * -1, this.transform.position);
    }
    
    /// <summary>
    /// Update function. Checks, if Touchinput was received. If yes, the hit object plays it's sound.
    /// </summary>
    private void Update()
    {
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)))
        {
            distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            thisObject = GetClickedGameObject();

            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //actualPos = ray.GetPoint(distance);
            if (thisObject != null)
            {
                if (thisObject.GetComponent<AudioSource>() != null)
                {
                    thisObject.GetComponent<AudioSource>().Play();
                }
            }
        }
    }

    /// <summary>
    /// If a dragable objects enters the basket and is released inside, it becomes not dragable anymore.
    /// If it is not released inside, the object stays dragable.
    /// </summary>
    /// <param name="other"></param> The collider of the other gameobject, which hits the basket.
    void OnTriggerEnter(Collider other)
    {
        GameObject coll = other.gameObject;
        if (coll.GetComponent<Dragable>().dragable == true)
        {
            if (coll.GetComponent<Dragable>().letter == this.GetComponent<Basket>().letter)
            {
                coll.GetComponent<Dragable>().dragable = false;
                truefalseAnswer = true;
                nrrightTiles += 1;
            }
            else if (coll.GetComponent<Dragable>().letter != this.GetComponent<Basket>().letter)
            {
                coll.GetComponent<Dragable>().dragable = false;
                truefalseAnswer = false;
                nrwrongTiles += 1;
            }
        }
    }

    /// <summary>
    /// See OnTriggerEnter(). Functions work together.
    /// </summary>
    /// <param name="other"></param> The collider of the other gameobject, which hits the basket.
    private void OnTriggerExit(Collider other)
    {
        GameObject coll = other.gameObject;
        if (coll.GetComponent<Dragable>().dragable == false)
        {
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

    /// <summary>
    /// Number of the images, which were sorted in the right basket.
    /// </summary>
    /// <returns></returns> Number of the right images.
    public int getNumberofRight()
    {
        return nrrightTiles;
    }
    /// <summary>
    /// Number of the images, which were sorted in the wrong basket.
    /// </summary>
    /// <returns></returns> Number of the wrong images.
    public int getNumberofWrong()
    {
        return nrwrongTiles;
    }

    /// <summary>
    /// Returns the clicked gameobject or null, if the recieved touch did not touch
    /// any desired gameobject.
    /// </summary>
    /// <returns></returns> gameobject with tag "Tile" or "RightWrong"
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
}
