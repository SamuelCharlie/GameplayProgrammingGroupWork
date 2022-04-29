using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetIsJumping : StateMachineBehaviour
{
    private int jumping;
    
    private void Awake()
    {
        jumping = Animator.StringToHash("isJumping");
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(jumping, false);
    }
}
