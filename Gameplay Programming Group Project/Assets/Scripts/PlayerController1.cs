using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class PlayerController1 : MonoBehaviour
{
    private PlayerControls player_controls;
    private CharacterController character_controller;
    private AnimatorController animator_controller;
    private Animator animator;
    private int walking_anim_hash;
    private int running_anim_hash;

    private Vector2 move_input;
    private Vector3 current_movement;
    private bool is_movement_pressed;

    private bool is_sprinting;

    public float rotation_speed;
    
    
    private void Awake()
    {
        player_controls = new PlayerControls();

        player_controls.Player.Move.performed += ctx => HandleInput(ctx);
        player_controls.Player.Move.started += ctx => HandleInput(ctx);
        player_controls.Player.Move.canceled += ctx => HandleInput(ctx);
            
        /*
        player_controls.Player.Look.performed += ctx => rotation_vector = ctx.ReadValue<Vector2>();
        player_controls.Player.Look.canceled += ctx => rotation_vector = Vector2.zero;
        */
        
        player_controls.Player.Sprint.performed += ctx => is_sprinting = ctx.ReadValueAsButton();
        player_controls.Player.Sprint.canceled += ctx => is_sprinting = ctx.ReadValueAsButton();
        
        /*
        player_controls.Player.Jump.started += ctx => is_jump_pressed = ctx.ReadValueAsButton();
        player_controls.Player.Jump.started += ctx => is_jump_pressed = ctx.ReadValueAsButton();
        */

        character_controller = GetComponent<CharacterController>();
        animator_controller = GetComponent<AnimatorController>();
        animator = GetComponent<Animator>();

        walking_anim_hash = Animator.StringToHash("isWalking");
        running_anim_hash = Animator.StringToHash("isRunning");
    }
    
    private void OnEnable()
    {
        player_controls.Player.Enable();
    }

    private void OnDisable()
    {
        player_controls.Player.Disable();
    }

    private void Update()
    {
        //HandleRotation();
        HandleAnimation();
        character_controller.Move(current_movement * Time.deltaTime);
    }

    private void HandleInput(InputAction.CallbackContext ctx)
    {
        move_input = ctx.ReadValue<Vector2>();
        current_movement.x = move_input.x;
        current_movement.z = move_input.y;
        is_movement_pressed = move_input.x != 0 || move_input.y != 0;
    }

    private void HandleAnimation()
    {
        /*
        bool is_walking = animator.GetBool(walking_anim_hash);
        bool is_running = animator.GetBool(running_anim_hash);
        
        if (is_movement_pressed)
        {
            animator.SetBool(walking_anim_hash, !is_walking);
        }
        */

        animator_controller.UpdateAnimatorValues(move_input.x, move_input.y, is_sprinting, true);
    }

    void HandleRotation()
    {
        Vector3 target_position;
        target_position.x = current_movement.x;
        target_position.y = 0;
        target_position.z = current_movement.z;
        
        Quaternion current_rotation = transform.rotation;
        if (is_movement_pressed)
        {
            Quaternion target_rotation = Quaternion.LookRotation(target_position);
            transform.rotation = Quaternion.Slerp(current_rotation, target_rotation, rotation_speed * Time.deltaTime);
        }
    }
}
