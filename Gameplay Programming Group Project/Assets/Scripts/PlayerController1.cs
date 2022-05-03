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
    private Camera player_cam;
    private int walking_anim_hash;
    private int running_anim_hash;
    private int attacking_anim_hash;
    private int attack_count_hash;
    private int jumping_anim_hash;
    private int jump_count_hash;
    private int dying_anim_hash;

    [Header("Gameplay")]
    private GameObject respawn_point;
    private bool is_dying;
    private Coroutine active_dying_routine = null;
    private int attack_combo_count;
    private Coroutine active_attacking_routine = null;

    public static bool in_interact_trigger;
    public static bool is_interacting;
    public static bool is_attacking;
    public static bool in_obj_menu;

    public int max_hp;
    public int hp;
    
    [Header("Movement")]
    private Vector2 move_input;
    public Vector3 current_movement;
    private bool is_movement_pressed;
    private bool is_sprinting;

    public float walk_speed;
    public float sprint_speed;
    
    [Header("Camera")]
    private Vector3 cam_rotation;
    private Vector2 rotation_vector;
    
    public float rotation_speed;
    public float min_vertical_angle;
    public float max_vertical_angle;
    
    [Header("Jumping")]
    private bool is_jump_pressed;
    private bool is_jumping;
    private bool in_jump_animation;
    public bool can_double_jump;
    private float gravity;
    private int jump_combo_step;
    public int jumps_remaining;
    private Dictionary<int, float> initial_jump_velocities = new Dictionary<int, float>();
    private Dictionary<int, float> initial_jump_gravities = new Dictionary<int, float>();
    private Coroutine active_jump_reset_routine = null;

    public float grounded_gravity;
    public float initial_jump_velocity;
    public float max_jump_height;
    public float max_jump_duration;
    public float jump_combo_window;
    public float air_movement_modifier;

    private void Awake()
    {
        player_controls = new PlayerControls();

        player_controls.Player.Move.performed += ctx => HandleInput(ctx);
        player_controls.Player.Move.started += ctx => HandleInput(ctx);
        player_controls.Player.Move.canceled += ctx => HandleInput(ctx);
        
        player_controls.Player.Look.performed += ctx => rotation_vector = ctx.ReadValue<Vector2>();
        player_controls.Player.Look.canceled += ctx => rotation_vector = Vector2.zero;
        
        player_controls.Player.Sprint.performed += ctx => HandleSprint(ctx);
        player_controls.Player.Sprint.canceled += ctx => HandleSprint(ctx);

        player_controls.Player.Jump.started += ctx => is_jump_pressed = ctx.ReadValueAsButton();
        player_controls.Player.Jump.canceled += ctx => is_jump_pressed = ctx.ReadValueAsButton();

        respawn_point = GameObject.FindGameObjectWithTag("Respawn");
        
        character_controller = GetComponent<CharacterController>();
        animator_controller = GetComponent<AnimatorController>();
        animator = GetComponent<Animator>();
        player_cam = Camera.main;

        walking_anim_hash = Animator.StringToHash("isWalking");
        running_anim_hash = Animator.StringToHash("isRunning");
        jumping_anim_hash = Animator.StringToHash("isJumping");
        jump_count_hash = Animator.StringToHash("jumpCount");
        dying_anim_hash = Animator.StringToHash("isDying");
        attacking_anim_hash = Animator.StringToHash("isAttacking");
        attack_count_hash = Animator.StringToHash("attackCount");
        
        Cursor.lockState = CursorLockMode.Locked;
        
        InitGameplayValues();
    }

    private void InitGameplayValues()
    {
        cam_rotation = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        sprint_speed = walk_speed * 1.5f;
        hp = max_hp;
        
        float timeToApex = max_jump_duration / 2;
        gravity = (-2 * max_jump_height) / Mathf.Pow(timeToApex, 2);
        initial_jump_velocity = (2 * max_jump_height) / timeToApex;
        float second_jump_gravity = (-2 * (max_jump_height * 1.5f)) / Mathf.Pow((timeToApex * 1.25f), 2);
        float second_jump_initial_velocity = (2 * (max_jump_height * 1.5f)) / (timeToApex * 1.25f);
        float third_jump_gravity = (-2 * (max_jump_height * 2)) / Mathf.Pow((timeToApex * 1.5f), 2);
        float third_jump_initial_velocity = (2 * (max_jump_height * 2)) / (timeToApex * 1.5f);
        
        initial_jump_velocities.Add(1, initial_jump_velocity);
        initial_jump_velocities.Add(2, second_jump_initial_velocity);
        initial_jump_velocities.Add(3, third_jump_initial_velocity);

        initial_jump_gravities.Add(0, gravity);
        initial_jump_gravities.Add(1, gravity);
        initial_jump_gravities.Add(2, second_jump_gravity);
        initial_jump_gravities.Add(3, third_jump_gravity);
    }
    
    private void OnEnable()
    {
        player_controls.Player.Enable();

        player_controls.Player.Interact.started += DoInteract;
        player_controls.Player.Attack.started += DoAttack;
        player_controls.Player.OpenMenu.started += OpenMenu;
    }

    private void OnDisable()
    {
        player_controls.Player.Disable();

        player_controls.Player.Interact.started -= DoInteract;
        player_controls.Player.Attack.started -= DoAttack;
        player_controls.Player.OpenMenu.started -= OpenMenu;
    }


    private void Update()
    {
        HandleCameraRotation();
        HandleAnimation();

        /*
        if (!is_attacking && attack_combo_count != 1)
        {
            active_attacking_routine = StartCoroutine(ResetAttackCombo());
        }
        */

        character_controller.Move(current_movement * Time.deltaTime);
        HandleGravity();
        HandleJumping();

        if (hp <= 0 && !is_dying && active_dying_routine == null)
        {
            active_dying_routine = StartCoroutine(PlayerDeath());
        }
    }

    private void HandleInput(InputAction.CallbackContext ctx)
    {
        move_input = ctx.ReadValue<Vector2>();
        Vector3 rotated_movement = transform.right * move_input.x + transform.forward * move_input.y;
        rotated_movement.Normalize();
        
        rotated_movement *= is_sprinting ? sprint_speed : walk_speed;
        current_movement.x = rotated_movement.x;
        current_movement.z = rotated_movement.z;
        
        is_movement_pressed = move_input.x != 0 || move_input.y != 0;

        if (!character_controller.isGrounded)
        {
            current_movement.x *= air_movement_modifier;
            current_movement.z *= air_movement_modifier;
        }
    }

    private void HandleSprint(InputAction.CallbackContext ctx)
    {
        if (character_controller.isGrounded)
        {
            is_sprinting = ctx.ReadValueAsButton();
        }
    }

    private void HandleAnimation()
    {
        animator_controller.UpdateAnimatorValues(move_input.x, move_input.y,
            is_sprinting, character_controller.isGrounded);
    }

    void HandleCameraRotation()
    {
        cam_rotation = new Vector3(cam_rotation.x + rotation_vector.y * rotation_speed,
            cam_rotation.y + rotation_vector.x * rotation_speed, cam_rotation.z);
        cam_rotation.x = Mathf.Clamp(cam_rotation.x, min_vertical_angle, max_vertical_angle);
        
        player_cam.transform.eulerAngles = cam_rotation;
        transform.eulerAngles = new Vector3(transform.rotation.x, cam_rotation.y, transform.rotation.z);
    }

    void HandleGravity()
    {
        bool is_falling = current_movement.y <= 0.0f || !is_jump_pressed;
        float fall_multiplier = 2.0f;

        if (can_double_jump && jumps_remaining == 1)
        {
            if (in_jump_animation)
            {
                if (jump_combo_step == 3)
                {
                    jump_combo_step = 0;
                    animator.SetInteger(jump_count_hash, jump_combo_step);
                }
            }
        }
        
        if (character_controller.isGrounded)
        {
            current_movement.y = grounded_gravity;
            jumps_remaining = can_double_jump ? 2 : 1;

            if (in_jump_animation)
            {
                animator.SetBool(jumping_anim_hash, false);
                in_jump_animation = false;
                active_jump_reset_routine = StartCoroutine(ResetJumpCombo());

                if (jump_combo_step == 3)
                {
                    jump_combo_step = 0;
                    animator.SetInteger(jump_count_hash, jump_combo_step);
                }
            }
        }
        
        else if (is_falling)
        {
            float previous_y_velocity = current_movement.y;
            float new_y_velocity = current_movement.y + (initial_jump_gravities[jump_combo_step]
                                                         * fall_multiplier * Time.deltaTime);
            current_movement.y = Mathf.Max((previous_y_velocity + new_y_velocity) * 0.5f, -20.0f);
        }

        else
        {
            float previous_y_velocity = current_movement.y;
            float new_y_velocity = current_movement.y + (initial_jump_gravities[jump_combo_step] * Time.deltaTime);
            current_movement.y = Mathf.Max((previous_y_velocity + new_y_velocity) * 0.5f, -20.0f);
        }
    }

    void HandleJumping()
    {
        if ((!is_jumping && character_controller.isGrounded && is_jump_pressed)
            || (!is_jumping && can_double_jump && jumps_remaining > 0 && is_jump_pressed))
        {
            if (jump_combo_step < 3 && active_jump_reset_routine != null)
            {
                StopCoroutine(active_jump_reset_routine);
            }
            
            is_jumping = true;
            jumps_remaining -= 1;
            jump_combo_step += 1;
            animator.SetInteger(jump_count_hash, jump_combo_step);
            current_movement.y = initial_jump_velocities[jump_combo_step] * 0.5f;

            animator.SetBool(jumping_anim_hash, true);
            in_jump_animation = true;
        }
        
        else if ((is_jumping && character_controller.isGrounded && !is_jump_pressed)
                 || (is_jumping && can_double_jump && jumps_remaining == 1 && !is_jump_pressed && current_movement.y < 0))
        {
            is_jumping = false;
        }
    }

    IEnumerator ResetJumpCombo()
    {
        yield return new WaitForSeconds(jump_combo_window);
        jump_combo_step = 0;
    }

    IEnumerator ResetAttackCombo()
    {
        yield return new WaitForSeconds(jump_combo_window);
        attack_combo_count = 1;
    }

    private void DoInteract(InputAction.CallbackContext obj)
    {
        if (!is_interacting && in_interact_trigger)
        {
            is_interacting = true;
        }
        else
        {
            is_interacting = false;
        }
    }

    private void DoAttack(InputAction.CallbackContext obj)
    {
        if(!is_attacking)
        {
            /*
            if (attack_combo_count < 3 && active_attacking_routine != null)
            {
                StopCoroutine(active_attacking_routine);
            }
            */

            is_attacking = true;
            animator.SetBool(attacking_anim_hash, true);

            /*
            animator.SetInteger(attack_count_hash, attack_combo_count);
            attack_combo_count += 1;
            */
        }
    }

    public void DamageHP(int damage)
    {
        hp -= damage;
        hp = Mathf.Clamp(hp, 0, max_hp);
    }

    private IEnumerator PlayerDeath()
    {
        is_dying = true;
        animator.SetBool(dying_anim_hash, true);
        player_controls.Disable();
        character_controller.enabled = false;
        yield return new WaitForSeconds(3f);

        transform.position = respawn_point.transform.position;
        transform.rotation = respawn_point.transform.rotation;
        hp = max_hp;
        
        is_dying = false;
        animator.SetBool(dying_anim_hash, false);
        player_controls.Enable();
        character_controller.enabled = true;

        active_dying_routine = null;
        StopCoroutine(PlayerDeath());
    }

    private void OpenMenu(InputAction.CallbackContext obj)
    {
        if (!in_obj_menu)
        {
            in_obj_menu = true;
        }
        else if (in_obj_menu)
        {
            in_obj_menu = false;
        }
    }
}
