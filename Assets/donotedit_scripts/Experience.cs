using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Experience : MonoBehaviour {

    public GameControl contr;
    private Slider exp;
    private GameObject knowledge;

    private float experience_val;
    private float knowledge_points;

    /// <summary>
    /// Gets the slider and knowledge objects.
    /// Loads the right values with the GameControl-object.
    /// </summary>
    private void Awake()
    {
        exp = this.gameObject.GetComponent<Slider>();
        knowledge = GameObject.Find("Knowledge");
        contr = GameObject.Find("GameControl").gameObject.GetComponent<GameControl>();
        contr.Load();

        experience_val = contr.experience;
        this.gameObject.GetComponent<Slider>().value = experience_val;
    }

    /// <summary>
    /// If the values of the experience or knowledge are changed by the scenens, change them too.
    /// If experience is full, add a knowledge point.
    /// </summary>
    private void Update()
    {
        if (exp.gameObject.GetComponent<Slider>().value != experience_val)
        {
            experience_val = exp.gameObject.GetComponent<Slider>().value;
        }

        if (knowledge.gameObject.GetComponent<Text>().text != knowledge_points.ToString())
        {
            knowledge_points = float.Parse(knowledge.gameObject.GetComponent<Text>().text);
        }

        
        // if more than 100 points are added, more knowledgepointshave to be added
        while(exp.gameObject.GetComponent<Slider>().value > 99)
        {
            knowledge_points += 1;
            experience_val -= 100;
            exp.gameObject.GetComponent<Slider>().value = experience_val;
            knowledge.GetComponent<Text>().text = knowledge_points.ToString();
        }
    }
}
