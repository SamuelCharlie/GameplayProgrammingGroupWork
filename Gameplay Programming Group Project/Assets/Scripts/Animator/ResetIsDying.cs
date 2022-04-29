using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetIsDying : StateMachineBehaviour
{
    private int dying;
    
    private void Awake()
    {
        dying = Animator.StringToHash("isDying");
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(dying, false);
    }
}
