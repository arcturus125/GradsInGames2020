using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gem : Interactible
{
    // private attributes
    private static Gem _singleton;
    // public attributes
    public static int NoOfGemsFound = 0;
    public string customInteractionText;
    public Text UItext;
    public Text Scoretext;
    public Animator anim;
    public Animator keys_anim;

    private void Awake()
    {
        _singleton = this;
    }
    public override void Update()
    {
        base.Update();
    }
    public static void UpdateGemsUI()
    {
        _singleton.UItext.text = "x " + NoOfGemsFound;
        _singleton.Scoretext.text = ""+NoOfGemsFound;
        _singleton.anim.SetTrigger("New Trigger");
        _singleton.keys_anim.SetTrigger("New Trigger");
    }

    public override void Use()
    {
        NoOfGemsFound++;
        Destroy(this.gameObject);
        base.Use();
        base.OnLookAway();
        UpdateGemsUI();


    }
    public override void OnLook()
    {
        interactionPanel.SetActive(true);
        interaction_Text.text = customInteractionText;
        isObjectBeingLookedAt = true;
    }
    public override void OnLookAway()
    {
        base.OnLookAway();
    }

}
