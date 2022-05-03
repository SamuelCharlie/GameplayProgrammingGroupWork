using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public int destination_index;
    public int index_direction;
    public bool is_waiting;

    public GameObject[] destinations;
    public float move_speed;
    public float wait_time;

    private void Start()
    {
        is_waiting = false;
        destination_index = 1;
        index_direction = 1;
        Debug.Log(destinations.Length);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.parent = transform;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.parent = null;
        }
    }

    private void FixedUpdate()
    {
        if (!is_waiting)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                destinations[destination_index].transform.position, move_speed);
        }

        if (Vector3.Distance(transform.position, destinations[destination_index].transform.position) < 0.01f)
        {
            if (destination_index == 0 || destination_index == destinations.Length - 1)
            {
                StartCoroutine(WaitAtEnds());
            }
            
            destination_index += index_direction;

            if (destination_index == 0 || destination_index == destinations.Length - 1)
            {
                index_direction *= -1;
            }
        }
    }
    
    IEnumerator WaitAtEnds()
    {
        is_waiting = true;
        yield return new WaitForSeconds(wait_time);
        is_waiting = false;
    }
}
