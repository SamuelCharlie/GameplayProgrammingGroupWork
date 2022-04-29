using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController1 : MonoBehaviour
{
    PlayerControls player_controls;

    Vector2 move_vector;
    Vector2 rotation_vector;
    float is_sprinting;
    float is_jumping;
    public float walk_speed;
    public float sprint_speed;

    public Camera player_cam;
    Vector3 cam_rotation;

    private void OnEnable()
    {
        player_controls.Player.Enable();
    }

    private void OnDisable()
    {
        player_controls.Player.Disable();
    }

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

    

    private void Start()
    {
        is_sprinting = walk_speed;
        cam_rotation = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        is_jumping = -1.0f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CameraRotation();
        Move();
        Jumping();
    }

    void CameraRotation()
    {
        cam_rotation = new Vector3(cam_rotation.x + rotation_vector.y, cam_rotation.y + rotation_vector.x, cam_rotation.z);

        player_cam.transform.eulerAngles = cam_rotation;
        transform.eulerAngles = new Vector3(transform.rotation.x, cam_rotation.y, transform.rotation.z);
    }

    private void Move()
    {
        transform.Translate(Vector3.right * Time.deltaTime * move_vector.x * is_sprinting, Space.Self);
        transform.Translate(Vector3.forward * Time.deltaTime * move_vector.y * is_sprinting, Space.Self);
    }
    
    private void Jumping()
    {
        if (is_jumping + 0.5f > Time.time)
        {
            transform.Translate((Vector3.up * 8.0f * Time.deltaTime), Space.Self);
        }
        else if (is_jumping + 1.0f > Time.time)
        {
            transform.Translate((Vector3.up * -8.0f * Time.deltaTime), Space.Self);
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
