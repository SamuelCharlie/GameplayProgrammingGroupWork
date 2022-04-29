using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetIsOccupied : StateMachineBehaviour
{
    public string isOccupiedBool;
    public bool isOccupiedStatus;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(isOccupiedBool, isOccupiedStatus);
    }
}
