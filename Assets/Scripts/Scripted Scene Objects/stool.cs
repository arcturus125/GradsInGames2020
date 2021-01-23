using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class stool : Interactible
{
    public Transform interactionButtonLocation;
    public string customInteractionText;
    public static bool disabled = false;
    
    public override void Start()
    {
        defaultInteractionButtonLocation = interactionButtonLocation;
        Init();
    }
    private void Update()
    {
        if (CameraController.cameraState == CameraController.CameraState.TableView)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StandUp();
            }
        }
        base.Update();
    }
    public override void Use()
    {
        if (!disabled)
        {
            SitDown();
            base.Use();
        }
    }
    public virtual void OnLook()
    {
        if (LaptopMovement_Desk.isLaptopMoveable && LaptopMovement_Desk.putLaptopBack)
        {
            interactionPanel.SetActive(true);
            Interactible.cross.SetActive(true);
            interaction_Text.text = "Put Laptop Back First";
            isObjectBeingLookedAt = true;
            disabled = true;
        }
        else
        {
            interactionPanel.SetActive(true);
            interaction_Text.text = customInteractionText;
            isObjectBeingLookedAt = true;
        }
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
    public override void OnLookAway()
    {
        base.OnLookAway();
        disabled = false;
        cross.SetActive(false);
    }

}
