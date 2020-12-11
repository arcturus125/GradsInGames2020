using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : Interactible
{
    public static int noOfFoundCards = 0;

    public override void Use()
    {
        noOfFoundCards++;
        CardGame.singleton.UpdateCardUI(noOfFoundCards);
        Destroy(this.gameObject);
        base.Use();
        base.OnLookAway(); // after destorying the object, remove the iteraction button from the screen or it will get stuck 
    }
}
