using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    float rotate_speed = 1.0f;
    public static bool got_keycard_one;

    void Update()
    {
        transform.Rotate(0, 1, 0);
    }
    void OnTriggerEnter(Collider other)
    {
        got_keycard_one = true;
        Destroy(gameObject);
    }
}
