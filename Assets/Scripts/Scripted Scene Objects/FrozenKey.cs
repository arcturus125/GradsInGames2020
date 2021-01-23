using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenKey : Interactible
{
    public static bool isKeyInteractible = false;
    public static bool isKeyTaken = false;
    public string customInteractionText;


    public override void Use()
    {
        if (isKeyInteractible)
        {
            Destroy(this.gameObject);
            isKeyTaken = true;
            ScreenManager.Singleton.ObjectiveLight.SetActive(false);
            base.Use();
            base.OnLookAway(); // after destorying the object, remove the iteraction button from the screen or it will get stuck 
        }
    }
    public override void OnLook()
    {
        if (isKeyInteractible)
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
