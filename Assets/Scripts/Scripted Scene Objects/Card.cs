using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : Interactible
{
    public static bool lookingForCards = false;
    public static int noOfFoundCards = 0;
    public string customInteractionText;

    public override void Use()
    {
        if (lookingForCards)
        {
            noOfFoundCards++;
            CardGame.singleton.UpdateCardUI(noOfFoundCards);
            Destroy(this.gameObject);
            base.Use();
            base.OnLookAway(); // after destorying the object, remove the iteraction button from the screen or it will get stuck 
        }
    }
    public override void OnLook()
    {
        if (lookingForCards)
        {
            interactionPanel.SetActive(true);
            interaction_Text.text = customInteractionText;
            isObjectBeingLookedAt = true;
        }
    }
    public override void OnLookAway()
    {
        if (lookingForCards)
        {
            base.OnLookAway();
        }
    }
}
