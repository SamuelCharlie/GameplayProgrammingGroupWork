using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerControls player_controls;
    public CharacterController controller;

    Vector2 move_vector;
    Vector2 rotation_vector;
    float is_sprinting;
    float is_jumping;
    public float walk_speed;
    public float sprint_speed;
    private Vector3 player_velocity;
    private Vector3 player_movement;

    public Camera player_cam;
    Vector3 cam_rotation;

    [SerializeField] private static bool is_grounded;
    [SerializeField] private float ground_distance_check;
    [SerializeField] private LayerMask ground_mask;
    [SerializeField] private float gravity;

    private void Awake()
    {
        player_controls = new PlayerControls();

        player_controls.Player.Move.performed += ctx => move_vector = ctx.ReadValue<Vector2>();
        player_controls.Player.Move.canceled += ctx => move_vector = Vector2.zero;

        player_controls.Player.Look.performed += ctx => rotation_vector = ctx.ReadValue<Vector2>();
        player_controls.Player.Look.canceled += ctx => rotation_vector = Vector2.zero;

        player_controls.Player.Sprint.performed += ctx => is_sprinting = sprint_speed;
        player_controls.Player.Sprint.canceled += ctx => is_sprinting = walk_speed;

        player_controls.Player.Jump.performed += ctx => Jump();
    }

    private void OnEnable()
    {
        player_controls.Player.Enable();
    }

    private void OnDisable()
    {
        player_controls.Player.Disable();
    }

    private void Start()
    {
        is_sprinting = walk_speed;
        cam_rotation = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        is_jumping = -1.0f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        CameraRotation();
        Move();
        Jumping();
    }

    private void Move()
    {
        is_grounded = Physics.CheckSphere(transform.position, ground_distance_check, ground_mask);

        if (is_grounded && player_velocity.y < 0)
        {
            player_velocity.y = -2.0f;
        }

        if (is_grounded)
        {
            if (player_velocity.x == 0 && player_velocity.y == 0)
            {
                //player is idle
            }
            else if (player_velocity.x != 0 || player_velocity.y != 0)
            {
                //player is on the ground

                Vector3 movement = new Vector3(move_vector.x, 0.0f, move_vector.y) * walk_speed *
                Time.deltaTime;
                player_movement = movement;
                player_movement = transform.TransformDirection(movement);
                transform.Translate(movement, Space.World);
            }
        }

        controller.Move(player_movement * walk_speed * Time.deltaTime);

        player_velocity.y += gravity * Time.deltaTime;
        controller.Move(player_velocity * Time.deltaTime);
    }

    void CameraRotation()
    {
        cam_rotation = new Vector3(cam_rotation.x + rotation_vector.y * 10, cam_rotation.y + rotation_vector.x * 10, cam_rotation.z);

        player_cam.transform.eulerAngles = cam_rotation;
        transform.eulerAngles = new Vector3(transform.rotation.x, cam_rotation.y, transform.rotation.z);
    }

    private void Jumping()
    {
        if (is_jumping + 0.5f > Time.time)
        {
            transform.Translate((Vector3.up * 10.0f * Time.deltaTime), Space.Self);
        }
        else if (is_jumping + 1.0f > Time.time)
        {
            transform.Translate((Vector3.up * -10.0f * Time.deltaTime), Space.Self);
        }
    }

    private void Jump()
    {
        if (is_jumping + 1.0f < Time.time)
        {
            is_jumping = Time.time;
        }
    }
}
