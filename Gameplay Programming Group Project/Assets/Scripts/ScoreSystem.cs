using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour
{
    public GameObject score_text;
    public static int score;

    void Update()
    {
        score_text.GetComponent<Text>().text = "Score: " + score;
    }
}
