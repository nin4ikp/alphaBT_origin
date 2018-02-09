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

    void updateAttributes()
    {
        experience = GameObject.Find("Experience_Slider").GetComponent<Slider>().value;
        knowledge = GameObject.Find("Knowledge").GetComponent<Text>().text;
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
}
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