using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stool : Interactible
{
    public Transform interactionButtonLocation;
    public override void Start()
    {
        defaultInteractionButtonLocation = interactionButtonLocation;
        Debug.LogError("Test Start");
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            StandUp();
        }
    }
    public override void Use()
    {
        SitDown();
        base.Use();
    }
    private void SitDown()
    {
        CameraController.cameraState = CameraController.CameraState.TableView;
        Cursor.lockState = CursorLockMode.None;
        CameraController._isSitting = true;
    }
    private void StandUp()
    {
        CameraController.cameraState = CameraController.CameraState.FreeView;
        Cursor.lockState = CursorLockMode.Locked;
    }

}
