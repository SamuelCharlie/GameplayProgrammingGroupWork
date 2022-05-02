using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoor : MonoBehaviour
{
    public Animator locked_door;
    private void OnTriggerStay(Collider other)
    {
        PlayerController.in_interact_trigger = true;

        if (PickUp.got_keycard && PlayerController.is_interacting)
        {
            locked_door.SetTrigger("DoorUnlock");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController.in_interact_trigger = false;
    }
}
