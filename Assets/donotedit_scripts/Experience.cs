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

    private void Awake()
    {
        exp = this.gameObject.GetComponent<Slider>();
        knowledge = GameObject.Find("Knowledge");
        contr = GameObject.Find("GameControl").gameObject.GetComponent<GameControl>();
        contr.Load();

        experience_val = contr.experience;
        this.gameObject.GetComponent<Slider>().value = experience_val;
    }

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


        if (exp.gameObject.GetComponent<Slider>().value > 99)
        {
            knowledge_points += 1;
            experience_val -= 100;
            exp.gameObject.GetComponent<Slider>().value = experience_val;
            knowledge.GetComponent<Text>().text = knowledge_points.ToString();
        }

    }
}
