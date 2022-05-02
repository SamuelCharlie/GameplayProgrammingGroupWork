using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateMove : MonoBehaviour
{
    public Animator crate_anim;

    public int crate_stage;
    private void OnTriggerStay(Collider other)
    {
        PlayerController1.in_interact_trigger = true;

        if ((crate_stage == 1) && PlayerController1.is_interacting)
        {
            crate_anim.SetTrigger("CrateDownOne");
            crate_stage = 2;
            PlayerController1.is_interacting = false;
        }
        else if ((crate_stage == 2) && PlayerController1.is_interacting)
        {
            crate_anim.SetTrigger("CrateDownTwo");
            crate_stage = 3;
            PlayerController1.is_interacting = false;
        }
        else if ((crate_stage == 3) && PlayerController1.is_interacting)
        {
            crate_anim.SetTrigger("CrateUpOne");
            crate_stage = 4;
            PlayerController1.is_interacting = false;
        }
        else if ((crate_stage == 4) && PlayerController1.is_interacting)
        {
            crate_anim.SetTrigger("CrateUpTwo");
            crate_stage = 1;
            PlayerController1.is_interacting = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController1.in_interact_trigger = false;
    }
}
