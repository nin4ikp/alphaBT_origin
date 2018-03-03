using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerClient : MonoBehaviour {

    void Start()
    {
        StartCoroutine(Upload());
    }

    /// <summary>
    /// Loads the Playerdata to the server. (Not working completely because there is no server yet.)
    /// </summary>
    /// <returns></returns>
    IEnumerator Upload()
    {
        WWWForm formData = new WWWForm();
        //formData.Add(new MultipartFormDataSection("field1=foo&field2=bar"));
        formData.AddField("Player data", "playerInfo.dat"); //Application.persistentDataPath + "/playerInfo.dat"

        UnityWebRequest www = UnityWebRequest.Post("http://192.168.0.1", formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }
}
