using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // constants
    const float MOUSE_SENSITIVITY = 2.0f;           // the sensitivity of the mouse
    const string INTERACTIBLE_TAG = "Interactible"; // the Tag unity will search for to determine an interactible object
    const KeyCode USE_KEY = KeyCode.F;              // the key to use an interactible gameobject
    const float USER_REACH = 2.0f;                  // how far away the user can be to interacti with an object
    const float CAMERA_X_UPPERBOUND = 290;
    const float CAMERA_X_LOWERBOUND = 70;

    // public attributes
    public static CameraController singleton;                       // a reference to an instance of this class to help static and non-static functions communicate
    public static CameraState cameraState = CameraState.FreeView;   // changeing the camera's state will trigger appropriate animations
    public static bool _isSitting = false;                          // if the user is sat down (used during the transition between camera states) 
    public enum CameraState { TableView, FreeView }                 // camera's state controlls if it is locked from use inputs or not    
    public GameObject LookUpPanel;                                  // the panel containing the prompt to look up when the player sits down



    // private attributes
    private GameObject _lastKnownInteractible = null; // the interactible the user is currently looking at
    private Animator _anim; // reference to the animator
    [SerializeField]
    private Camera _cam; // reference to the camera
    private float _CameraX = 12.0f; // default starting camera angle = 12 (just because it looks nicer)
    private static GameObject _objectLookingAt;   // the gameobject the player has the crosshair over
    private bool readyToPause = true; // when true, the camera animations will be paused  the next frame, allowing players movements to override the animation

    // assign the singleton to the 1 instance of the class running
    private void Awake()
    {
        singleton = this;
    }

    // setup cursor and animator for later use
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraState = CameraState.FreeView;
        _anim = GetComponentInParent<Animator>();
    }

    // handles the mouse inputs, animations, raycasts and some keypresses
    private void LateUpdate()
    {
        // if the winning-screen is not currently showing
        if (!GameComplete.GameWon)
        {
            // if the user is not sat at the stool
            if (cameraState == CameraState.FreeView)
            {
                // use the mouse inputs to rotate the camera and then raycast to get the object the crosshair is over
                if (!MenuScript.menuOpen)
                {
                    MouseInput();
                    RaycastCrosshair();
                }
            }
            // if the user is sat at the stool
            else
            {
                // raycast to detect which object the cursor is over (instead of the crosshair)
                RaycastCursor();

                // holding W will make the camera look up when sat down
                if (Input.GetKeyDown(KeyCode.W) ) this.GetComponent<Animator>().SetBool("lookUp", true);
                if (Input.GetKeyUp  (KeyCode.W) ) this.GetComponent<Animator>().SetBool("lookUp", false);
            }

            // depending on the animation state, lets the playes movements override the animations
            Animations();

            // UI management for when the player sits down  -  shows and hides "W to look up" if the user is sitting down
            if (cameraState == CameraState.TableView) LookUpPanel.SetActive(true);
            else                                      LookUpPanel.SetActive(false);
        }
    }

    // when sitting, this stores the gameobject the user has their cursor over in the variable _objectLookingAt
    private void RaycastCursor()
    {
        Ray ray =_cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)) _objectLookingAt = hit.collider.gameObject;
    }
    // takes a Gameobject as a parameter, returns true if the user has theie cursor over that obeject, otherwise false
    public static bool IsCursorOver(GameObject gameobjectToCheck)
    {
        return gameobjectToCheck == _objectLookingAt;
    }

    private void Animations()
    {
        if (!readyToPause)
        {
            // TODO:: probably should find a way to do this without running it every frame

            // turn the animator back on and  play the animation to sit down
            if (cameraState == CameraState.TableView)
            {
                _anim.enabled = true;
                _anim.SetBool("SatAtTable", true);
            }
            // when the state of the camera is changed, play the animation to stand, then pause the animations
            if (cameraState == CameraState.FreeView)
            {
                _anim.SetBool("SatAtTable", false);
                if (_isSitting)
                {
                    readyToPause = true;
                }
            }
        }
        // if the user is in the process of standing up from the chair
        else
        {
            // wait until the player has finished the transition between animations - otherwise the game may resume the wrong animation later
            if( _anim.GetCurrentAnimatorStateInfo(0).IsName("Standing"))
            {
                // pause the animator so the players keybaord inputs overwrite the animations  - until the animations are resumed
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
        Debug.DrawRay(_cam.transform.position, _cam.transform.TransformDirection(Vector3.forward)*USER_REACH,Color.red);
        if(Physics.Raycast(_cam.transform.position, _cam.transform.TransformDirection(Vector3.forward), out hit))
        {
            GameObject lookingAtGameobject = hit.collider.gameObject;

            // if the player  has jsut looked away from an interactible object
            if (lookingAtGameobject != _lastKnownInteractible)
            {
                //  run OnLookAway() on it if possible
                if (_lastKnownInteractible)
                {
                    _lastKnownInteractible.SendMessage("OnLookAway", SendMessageOptions.DontRequireReceiver);
                    _lastKnownInteractible = null;
                }
            }

            // only allow interaction if it is within range of the player
            if (Vector3.Distance(_cam.transform.position, lookingAtGameobject.transform.position) < USER_REACH)
            {
                // if the player is looking at an interactible
                if (lookingAtGameobject.tag == INTERACTIBLE_TAG)
                {
                    // if the player has only just started looking at this interactible
                    if (_lastKnownInteractible != lookingAtGameobject)
                    {
                        // run OnLook() on it
                        //Debug.Log(Interactible.isCrossVisible);
                        lookingAtGameobject.SendMessage("OnLook", SendMessageOptions.DontRequireReceiver);
                        _lastKnownInteractible = lookingAtGameobject;
                    }
                    // when the player presses the "F" key
                    if (Input.GetKeyDown(USE_KEY))
                    {
                        // run Use() on it
                        _lastKnownInteractible.SendMessage("Use", SendMessageOptions.DontRequireReceiver);
                    }
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

        //stops the user from looking up or down too far(clamps the X rotation of the camera betweenthe bounds)
        if (_cam.transform.eulerAngles.x < 180)
        {
            if (_cam.transform.eulerAngles.x  > CAMERA_X_LOWERBOUND)
            {
                vertialMouseMovement = 0;
                _cam.transform.eulerAngles = new Vector3(CAMERA_X_LOWERBOUND, _cam.transform.eulerAngles.y, _cam.transform.eulerAngles.z);
            }
        }
        if (_cam.transform.eulerAngles.x > 180)
        {
            if (_cam.transform.eulerAngles.x  < CAMERA_X_UPPERBOUND)
            {
                vertialMouseMovement = 0;
                _cam.transform.eulerAngles = new Vector3(CAMERA_X_UPPERBOUND, _cam.transform.eulerAngles.y, _cam.transform.eulerAngles.z);
            }
        }


        //convert mouse inputs to a Vector3
        Vector3 horizontalRotation = new Vector3(0, horizontalMouseMovement * MOUSE_SENSITIVITY, 0);
            _CameraX += -vertialMouseMovement * MOUSE_SENSITIVITY;
            Vector3 verticalRotation = new Vector3(-vertialMouseMovement * MOUSE_SENSITIVITY, 0, 0);

            // looking left-to-right rotates player
            transform.Rotate(horizontalRotation);
            // looking up-to-down rotates the camera
            _cam.transform.Rotate(verticalRotation);


        
        
    }

    // when the user has finished the "escape" animation, pause the animator again or the player cant move
    public void OnEscape()
    {
        this.GetComponent<Animator>().SetBool("escape", false);
        readyToPause = true;
    }
    // during a specific dialogue, run an animation that makes the user look at the door when it is slammed
    public void slamDoor()
    {
        door.singleton.SlamDoor();
        this.GetComponent<Animator>().SetBool("lookAtDoor", true);
    }
    // once the door is slammed, amend the parameter on the animator so the player can look back at the table
    public void doorSlammed()
    {
        this.GetComponent<Animator>().SetBool("lookAtDoor", false);
    }

}
