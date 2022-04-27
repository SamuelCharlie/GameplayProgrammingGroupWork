using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    public Animator door_animator;
    public GameObject player;

    public bool player_in_trig;

    /*private void Update()
    {
        if (player_in_trig)
        {
            door_animator.SetBool("Door_Open", true);
        }
        else
        {
            door_animator.SetBool("Door_Open", false);
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter Trigger");
       
        door_animator.SetBool("Door_Open", true);
      
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit Trigger");
        
        door_animator.SetBool("Door_Open", false);
        
    }
}
