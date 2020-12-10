using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class stool : Interactible
{
    public Transform interactionButtonLocation;
    public string customInteractionText;
    public override void Start()
    {
        defaultInteractionButtonLocation = interactionButtonLocation;
        Init();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            StandUp();
        }
        base.Update();
    }
    public override void Use()
    {
        SitDown();
        base.Use();
    }
    public virtual void OnLook()
    {
        interactionPanel.SetActive(true);
        interaction_Text.text = customInteractionText;
        isObjectBeingLookedAt = true;
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
