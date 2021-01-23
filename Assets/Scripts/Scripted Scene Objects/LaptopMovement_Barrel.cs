using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaptopMovement_Barrel : Interactible
{
    public static bool isLaptopPlaceable = false;
    public static bool isLaptopDown = false;
    public string customInteractionText = "Place Laptop";
    public GameObject Laptop;

    public override void Start()
    {
        base.Start();
    }
    public override void Use()
    {
        // placing the laptop on the barrel
        if (isLaptopPlaceable)
        {
            Laptop.transform.localPosition = new Vector3(-0.997f, 0.897f, -5.454f);
            Laptop.transform.Rotate(0.0f, 225f, 0.0f);
            base.OnLookAway();
            Debug.Log("Laptop Placed");
            isLaptopPlaceable = false;
            isLaptopDown = true;
            Bottle.isInteractible = true;
            ScreenManager.checkpoint = ScreenManager.Checkpoints.puzzleOfColour_PickColour;
            LaptopMovement_Desk.isLaptopMoveable = false;

        }
        // pisking  the laptop up off the barrel
        if(isLaptopDown && LaptopMovement_Desk.putLaptopBack)
        {
            isLaptopDown = false;
            Laptop.transform.localPosition = new Vector3(-1.663f, 0.803f, 6.569f);
            Laptop.transform.Rotate(0.0f, -225f, 0.0f);
            base.OnLookAway();
        }
    }
    public override void OnLook()
    {
        if(LaptopMovement_Desk.putLaptopBack)
        {
            interactionPanel.SetActive(true);
            interaction_Text.text = "pick up Laptop";
            isObjectBeingLookedAt = true;
        }
        else if (isLaptopPlaceable)
        {
            interactionPanel.SetActive(true);
            interaction_Text.text = customInteractionText;
            isObjectBeingLookedAt = true;
        }
    }
    public override void OnLookAway()
    {
        if (isLaptopPlaceable)
        {
            base.OnLookAway();

        }
    }
}
