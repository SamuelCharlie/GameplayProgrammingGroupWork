using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetIsAttacking : StateMachineBehaviour
{
    private int attacking;
    
    private void Awake()
    {
        attacking = Animator.StringToHash("isAttacking");
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(attacking, false);
    }
}
