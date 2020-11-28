using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    const float MOUSE_SENSITIVITY = 2.0f;
    const string INTERACTIBLE_TAG = "Interactible";
    const KeyCode USE_KEY = KeyCode.F;

    // camera's state controlls if it is locked from use inputs or not
    public enum CameraState { TableView, FreeView }
    public static CameraState cameraState = CameraState.FreeView;
    
    private GameObject lastKnownInteractible = null; // the interactible the user is currently looking at



    private void Update()
    {
        if (cameraState == CameraState.FreeView)
        {
            MouseInput();
        }
        RaycastCrosshair();
    }

    // find out if the user is looking at a gameobject tagged "Interactible" and run functions on that object
    private void RaycastCrosshair()
    {
        // if the player is looking at an interactible object
        RaycastHit hit;
        Debug.DrawRay(this.transform.position, transform.TransformDirection(Vector3.forward));
        if(Physics.Raycast(this.transform.position, transform.TransformDirection(Vector3.forward), out hit))
        {
            GameObject lookingAtGameobject = hit.collider.gameObject;
            //Debug.Log(lookingAtGameobject.name);
            // if the player is looking at an interactible
            if (lookingAtGameobject.tag == INTERACTIBLE_TAG)
            {
                // if the player has only just started looking at this interactible
                if (lastKnownInteractible != lookingAtGameobject)
                {
                    // run OnLook() on it
                    lookingAtGameobject.SendMessage("OnLook", SendMessageOptions.DontRequireReceiver);
                    lastKnownInteractible = lookingAtGameobject;
                }
                if (Input.GetKeyDown(USE_KEY))
                {
                    // run Use() on it
                    lastKnownInteractible.SendMessage("Use", SendMessageOptions.DontRequireReceiver);
                }
            }
            // if the player has just looked away from an interactible
            else if (lookingAtGameobject != lastKnownInteractible)
            {
                //  run OnLookAway() on it if possible
                if (lastKnownInteractible)
                {
                    lastKnownInteractible.SendMessage("OnLookAway", SendMessageOptions.DontRequireReceiver);
                    lastKnownInteractible = null;
                }
            }
        }
    }


    // rotates camera with mouse inputs to look around scene
    private void MouseInput()
    {
        // get mouse inputs
        float horizontalMouseMovement = Input.GetAxis("Mouse X");
        float vertialMouseMovement = Input.GetAxis("Mouse Y");

        // convert mouse inputs to a Vector3
        Vector3 horizontalRotation = new Vector3(0, horizontalMouseMovement * MOUSE_SENSITIVITY, 0);
        Vector3 verticalRotation = new Vector3(-vertialMouseMovement * MOUSE_SENSITIVITY, 0, 0);

        // looking left-to-right rotates player
        this.transform.parent.transform.Rotate(horizontalRotation);
        // looking up-to-down rotates the camera
        this.transform.Rotate(verticalRotation);
    }
}
