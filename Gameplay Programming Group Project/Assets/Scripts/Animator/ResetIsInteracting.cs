using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetIsInteracting : StateMachineBehaviour
{
    private int interacting;
    
    private void Awake()
    {
        interacting = Animator.StringToHash("isInteracting");
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(interacting, false);
    }
}
