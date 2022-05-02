using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerControls player_controls;
    private CharacterController character_controller;
    private AnimatorController animator_controller;

    Vector2 move_vector;
    Vector2 rotation_vector;
    public bool is_sprinting;
    public float walk_speed;
    public float sprint_speed;
    private Vector3 player_velocity;
    private Vector3 player_movement;

    private bool is_jump_pressed;
    bool is_jumping;
    public float initial_jump_velocity;
    public float max_jump_height;
    public float max_jump_duration;

    public Camera player_cam;
    Vector3 cam_rotation;

    [SerializeField] private static bool is_grounded;
    [SerializeField] private float ground_distance_check;
    [SerializeField] private LayerMask ground_mask;
    [SerializeField] private float gravity;
    [SerializeField] private float ground_gravity;

    public static bool in_interact_trigger;
    public static bool is_interacting;

    private void Awake()
    {
        player_controls = new PlayerControls();

        player_controls.Player.Move.performed += ctx => move_vector = ctx.ReadValue<Vector2>();
        player_controls.Player.Move.canceled += ctx => move_vector = Vector2.zero;

        player_controls.Player.Look.performed += ctx => rotation_vector = ctx.ReadValue<Vector2>();
        player_controls.Player.Look.canceled += ctx => rotation_vector = Vector2.zero;

        player_controls.Player.Sprint.performed += ctx => is_sprinting = ctx.ReadValueAsButton();
        player_controls.Player.Sprint.canceled += ctx => is_sprinting = ctx.ReadValueAsButton();

        player_controls.Player.Jump.started += ctx => is_jump_pressed = ctx.ReadValueAsButton();
        player_controls.Player.Jump.started += ctx => is_jump_pressed = ctx.ReadValueAsButton();

        character_controller = GetComponent<CharacterController>();
        animator_controller = GetComponent<AnimatorController>();
    }

    private void OnEnable()
    {
        player_controls.Player.Enable();
        player_controls.Player.Interact.started += DoInteract;
    }

    private void OnDisable()
    {
        player_controls.Player.Disable();
        player_controls.Player.Interact.started -= DoInteract;
    }

    private void Start()
    {
        is_sprinting = false;
        cam_rotation = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        
        is_jumping = false;
        is_jump_pressed = false;
        
        float timeToApex = max_jump_duration / 2;
        gravity = (-2 * max_jump_height) / Mathf.Pow(timeToApex, 2);
        initial_jump_velocity = (2 * max_jump_height) / timeToApex;
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        CameraRotation();
        Move();

        animator_controller.UpdateAnimatorValues(move_vector.x, move_vector.y,
            is_sprinting, is_grounded);

        Gravity();
        Jump();
    }

    private void Move()
    {
        is_grounded = Physics.CheckSphere(transform.position, ground_distance_check, ground_mask);

        if (is_grounded)
        {
            Vector3 current_movement = new Vector3(move_vector.x, 0.0f, move_vector.y);
            current_movement *= is_sprinting ? sprint_speed : walk_speed;
            
            player_movement = transform.TransformDirection(current_movement);
            transform.Translate(current_movement, Space.World);
        }
        
        character_controller.Move(player_movement * Time.deltaTime);
        character_controller.Move(player_velocity);
    }

    private void Gravity()
    {
        if (is_grounded)
        {
            player_velocity.y = ground_gravity;
        }

        else
        {
            float previous_y_velocity = player_velocity.y;
            float new_y_velocity = player_velocity.y + (gravity * Time.deltaTime);
            player_velocity.y = (previous_y_velocity + new_y_velocity) * 0.5f;
        }
    }

    void CameraRotation()
    {
        cam_rotation = new Vector3(cam_rotation.x + rotation_vector.y * 10, cam_rotation.y + rotation_vector.x * 10, cam_rotation.z);

        player_cam.transform.eulerAngles = cam_rotation;
        transform.eulerAngles = new Vector3(transform.rotation.x, cam_rotation.y, transform.rotation.z);
    }

    private void Jump()
    {
        if (!is_jumping && is_grounded && is_jump_pressed)
        {
            is_jumping = true;
            player_velocity.y = initial_jump_velocity * 0.5f;
            animator_controller.PlayTargetAnimation("Jump", true);
        }

        else if (!is_jump_pressed && is_jumping && is_grounded)
        {
            is_jumping = false;
        }
    }

    private void DoInteract(InputAction.CallbackContext obj)
    {
        if (!is_interacting && in_interact_trigger)
        {
            is_interacting = true;
        }    
    }
}
