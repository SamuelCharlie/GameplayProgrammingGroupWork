using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private Animator animator;
    
    private int horizontal;
    private int vertical;
    private int occupied;
    private int grounded;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
        occupied = Animator.StringToHash("isOccupied");
        grounded = Animator.StringToHash("isGrounded");
    }
    
    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting, bool isGrounded)
    {
        if (isSprinting)
        {
            verticalMovement = 2;
        }
        
        animator.SetFloat(horizontal, horizontalMovement, 0.1f, Time.deltaTime);
        animator.SetFloat(vertical, verticalMovement, 0.1f, Time.deltaTime);
        animator.SetBool(grounded, isGrounded);
    }
    
    public void PlayTargetAnimation(string targetAnimation, bool isOccupied)
    {
        animator.CrossFade(targetAnimation, 0.1f);
        animator.SetBool(occupied, isOccupied);
    }
}
