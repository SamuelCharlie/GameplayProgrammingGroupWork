using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCoin : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        ScoreSystem.score += 10;
        Destroy(gameObject);
    }
}
