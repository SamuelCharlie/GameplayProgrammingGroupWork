using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killvolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController1>().DamageHP(9999);
            Debug.Log("Player entered killvolume");
        }
    }
}
