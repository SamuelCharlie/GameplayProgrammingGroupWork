using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerControls player_controls;
    Vector2 move_vector;

    public CharacterController controller;

    public float player_speed;
    private Vector3 player_velocity;
    private static Vector3 player_movement;

    [SerializeField] private static bool is_grounded;
    [SerializeField] private float ground_distance_check;
    [SerializeField] private LayerMask ground_mask;
    [SerializeField] private float gravity;

    private void Awake()
    {
        player_controls = new PlayerControls();

        player_controls.Player.Move.performed += ctx => move_vector = ctx.ReadValue<Vector2>();
        player_controls.Player.Move.canceled += ctx => move_vector = Vector2.zero;
    }

    private void OnEnable()
    {
        player_controls.Player.Enable();
    }

    private void OnDisable()
    {
        player_controls.Player.Disable();
    }

    private void FixedUpdate()
    {
        Move();
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

                Vector3 movement = new Vector3(move_vector.x, 0.0f, move_vector.y) * player_speed *
                Time.deltaTime;
                player_movement = movement;
                player_movement = transform.TransformDirection(movement);
                transform.Translate(movement, Space.World);
            }
        }

        controller.Move(player_movement * player_speed * Time.deltaTime);

        player_velocity.y += gravity * Time.deltaTime;
        controller.Move(player_velocity * Time.deltaTime);
    }
}
