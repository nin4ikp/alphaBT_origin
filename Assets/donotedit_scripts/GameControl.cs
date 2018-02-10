using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour {

    public static GameControl control;

    public float experience;
    public float knowledge;

    private Slider experienceSlider;

    /// <summary>
    /// Makes the class a Singleton.
    /// </summary>
	void Awake () {
		if (control == null)
        {
            Debug.Log("Control was null.. Setting new");
            DontDestroyOnLoad(gameObject);
            control = this;
            control.Load();
        }
        else if (control != this)
        {

            Debug.Log("Already existing GameCOntrol. Destroy old");
            Destroy(gameObject);
        }
	}

    public void updateAttributes()
    {
        Debug.Log("Experience, Knowledge before: " + experience + ", " + knowledge);
        experience = GameObject.Find("Experience_Slider").GetComponent<Slider>().value;
        knowledge = float.Parse(GameObject.Find("Knowledge").GetComponent<Text>().text);
        Debug.Log("Experience, Knowledge after: " + experience + ", " + knowledge);
    }

    private void loadAttributesIntoGameObjects()
    {
        GameObject.Find("Experience_Slider").GetComponent<Slider>().value = experience;
        GameObject.Find("Knowledge").GetComponent<Text>().text = knowledge.ToString();
        Debug.Log(GameObject.Find("Experience_Slider").GetComponent<Slider>().value);
        Debug.Log(GameObject.Find("Knowledge").GetComponent<Text>().text);
    }


    /// <summary>
    /// Saves the current data to a binary file, so it is not accessable for players
    /// </summary>
    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData data = new PlayerData();
        data.experience = experience;
        data.knowledge = knowledge;

        bf.Serialize(file, data);
        file.Close();
    }

    /// <summary>
    /// Loads the saved data from a binary file and sets the saved values.
    /// </summary>
    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            experience = data.experience;
            knowledge = data.knowledge;

            loadAttributesIntoGameObjects();
        }
    }

    public void DeletePlayerData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData data = new PlayerData();
        data.experience = 0;
        data.knowledge = 0;

        bf.Serialize(file, data);
        file.Close();

        Load();
    }
}

/// <summary>
/// Player data.
/// </summary>
[Serializable]
class PlayerData
{
    public float experience;
    public float knowledge;
}