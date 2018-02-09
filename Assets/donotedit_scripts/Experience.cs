using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Experience : MonoBehaviour {

    public Slider exp;
    public GameControl contr;
    public GameObject knowledge;

    private float experience_val;
    private float knowledge_points;

    private void Awake()
    {
        exp = this.gameObject.GetComponent<Slider>();
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

        if (exp.gameObject.GetComponent<Slider>().value >= 100)
        {
            knowledge_points += 1;
            experience_val -= 100;
            exp.gameObject.GetComponent<Slider>().value = experience_val;
            knowledge = GameObject.Find("Knowledge");
            knowledge.GetComponent<Text>().text = knowledge_points.ToString();
        }

    }
}
