using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // constants
    const float MOUSE_SENSITIVITY = 2.0f;
    const string INTERACTIBLE_TAG = "Interactible";
    const KeyCode USE_KEY = KeyCode.F;

    // public attributes
    public enum CameraState { TableView, FreeView } // camera's state controlls if it is locked from use inputs or not
    public static CameraState cameraState = CameraState.FreeView;
    public static bool _isSitting = false;

    // private attributes
    private GameObject _lastKnownInteractible = null; // the interactible the user is currently looking at
    private Animator _anim;
    [SerializeField]
    private Camera _cam;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _anim = GetComponentInParent<Animator>();
    }
    private void Update()
    {
        if (cameraState == CameraState.FreeView)
        {
            MouseInput();
        }
        RaycastCrosshair();


        Animations();

    }

    bool readyToPause = true;
    private void Animations()
    {
        if (!readyToPause)
        {
            // TODO:: probably shoud find a way to do this without running it every frame, i'll do it later
            if (cameraState == CameraState.TableView)
            {
                _anim.enabled = true;
                _anim.SetBool("SatAtTable", true);
            }
            if (cameraState == CameraState.FreeView)
            {
                _anim.SetBool("SatAtTable", false);
                if (_isSitting)
                {
                    readyToPause = true;
                }
            }
        }
        else
        {
            Debug.Log("Waiting");
            if( _anim.GetCurrentAnimatorStateInfo(0).IsName("Standing"))
            {
                Debug.Log("paused");
                readyToPause = false;
                _isSitting = false;
                _anim.enabled = false;

            }
        }
    }


    // find out if the user is looking at a gameobject tagged "Interactible" and run functions on that object
    private void RaycastCrosshair()
    {
        // if the player is looking at an interactible object
        RaycastHit hit;
        Debug.DrawRay(_cam.transform.position, _cam.transform.TransformDirection(Vector3.forward)*5,Color.red);
        if(Physics.Raycast(_cam.transform.position, _cam.transform.TransformDirection(Vector3.forward), out hit))
        {
            GameObject lookingAtGameobject = hit.collider.gameObject;
            //Debug.Log(lookingAtGameobject.name);
            // if the player is looking at an interactible
            if (lookingAtGameobject.tag == INTERACTIBLE_TAG)
            {
                // if the player has only just started looking at this interactible
                if (_lastKnownInteractible != lookingAtGameobject)
                {
                    // run OnLook() on it
                    lookingAtGameobject.SendMessage("OnLook", SendMessageOptions.DontRequireReceiver);
                    _lastKnownInteractible = lookingAtGameobject;
                }
                if (Input.GetKeyDown(USE_KEY))
                {
                    // run Use() on it
                    _lastKnownInteractible.SendMessage("Use", SendMessageOptions.DontRequireReceiver);
                }
            }
            // if the player has just looked away from an interactible
            else if (lookingAtGameobject != _lastKnownInteractible)
            {
                //  run OnLookAway() on it if possible
                if (_lastKnownInteractible)
                {
                    _lastKnownInteractible.SendMessage("OnLookAway", SendMessageOptions.DontRequireReceiver);
                    _lastKnownInteractible = null;
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
        transform.Rotate(horizontalRotation);
        // looking up-to-down rotates the camera
        _cam.transform.Rotate(verticalRotation);
    }

}
