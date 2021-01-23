using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : Interactible
{
    public static bool canPickUp = false;
    public static bool pickedUp = false;
    public string customInteractionText;

    public override void Use()
    {
        if (canPickUp)
        {
            Destroy(this.gameObject);
            pickedUp = true;
            base.Use();
            base.OnLookAway(); // after destorying the object, remove the iteraction button from the screen or it will get stuck 
        }
    }
    public override void OnLook()
    {
        if (canPickUp)
        {
            interactionPanel.SetActive(true);
            interaction_Text.text = customInteractionText;
            isObjectBeingLookedAt = true;
        }
    }
    public override void OnLookAway()
    {
        base.OnLookAway();
        
    }
}
