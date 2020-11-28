using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stool : Interactible
{
    public Transform interactionButtonLocation;
    public override void Start()
    {
        defaultInteractionButtonLocation = interactionButtonLocation; 
    }

}
