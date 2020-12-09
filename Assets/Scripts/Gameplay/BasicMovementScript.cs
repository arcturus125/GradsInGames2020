using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovementScript : MonoBehaviour
{
    Rigidbody rb;

    // the keys used to move the player
    const KeyCode MOVE_FORWARD  = KeyCode.W;  
    const KeyCode MOVE_LEFT     = KeyCode.A;  
    const KeyCode MOVE_BACK     = KeyCode.S;  
    const KeyCode MOVE_RIGHT    = KeyCode.D;  

    const float MOVEMENT_SPEED = 7.0f;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // if the user is not at the table playing minigames
        if (CameraController.cameraState == CameraController.CameraState.FreeView)
        {
            // move the player and camera based on inputs
            Movement();
        }
    }

    // moves player with WASD keys
    private void Movement()
    {
        Vector3 movement = Vector3.zero;
        if (Input.GetKey(MOVE_FORWARD))
        {
            movement += Vector3.forward;
        }
        if (Input.GetKey(MOVE_LEFT))
        {
            movement += Vector3.left;
        }
        if (Input.GetKey(MOVE_BACK))
        {
            movement += Vector3.back;
        }
        if (Input.GetKey(MOVE_RIGHT))
        {
            movement += Vector3.right;
        }
        rb.MovePosition(this.transform.position + transform.TransformDirection(movement * MOVEMENT_SPEED * Time.deltaTime));
    }
}
