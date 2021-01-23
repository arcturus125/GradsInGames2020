using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finalDoor : Interactible
{
    private bool _isOpen = false;
    public string openDoorText = "open with key";
    public string noKeytext = "you need a key to open this door";
    public Animator anim;
    public Transform interactionButtonLocation;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public override void Start()
    {
        defaultInteractionButtonLocation = interactionButtonLocation;
        Init();
    }
    public override void Use()
    {
        if (Keys.finalKeyFound)
        {
            base.Use();
            base.OnLookAway(); // after destorying the object, remove the iteraction button from the screen or it will get stuck 
            anim.SetBool("open", true);
            _isOpen = true;
        }


    }
    public override void OnLook()
    {
        if (!_isOpen)
        {
            if (Keys.finalKeyFound)
            {
                interactionPanel.SetActive(true);
                interaction_Text.text = openDoorText;
                isObjectBeingLookedAt = true;
            }
            else
            {
                Interactible.cross.SetActive(true);
                interactionPanel.SetActive(true);
                interaction_Text.text = noKeytext;
                isObjectBeingLookedAt = true;
            }
        }
    }


    public override void OnLookAway()
    {
        base.OnLookAway();
        Interactible.cross.SetActive(false);
    }
}
