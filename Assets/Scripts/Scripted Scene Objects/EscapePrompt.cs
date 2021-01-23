using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapePrompt : Interactible
{
    public Transform interactionButtonLocation;
    public string customInteractionText;
    bool escaped = false;

    
    public override void OnLook()
    {
        if (!escaped)
        {
            interactionPanel.SetActive(true);
            interaction_Text.text = customInteractionText;
            isObjectBeingLookedAt = true;
        }
    }
    public override void Use()
    {
        if (!escaped)
        {
            Animator anim = GameObject.Find("Player").GetComponent<Animator>();
            anim.enabled = true;
            anim.SetBool("escape", true);
            escaped = true;
            interactionPanel.SetActive(false);
        }
    }

}