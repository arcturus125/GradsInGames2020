using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : Interactible
{
    public string noKeys;
    public string use_key2;
    public string waiting_for_key_3;
    public string use_key3;
    public string take_Key;

    public bool keyTaken = false;
    public bool key2Used = false;
    public bool key3Used = false;

    public GameObject small_chest_closed;
    public GameObject small_chest_open;
    public GameObject large_chest_closed;
    public GameObject large_chest_open;
    public GameObject key;

    public override void Start()
    {
        base.Start();
        small_chest_open.GetComponent<MeshRenderer>().enabled = false;
        large_chest_open.GetComponent<MeshRenderer>().enabled = false;
    }

    public override void Use()
    {
        if (Keys.key2Found && !key2Used)
        {
            key2Used = true;
            large_chest_closed.GetComponent<MeshRenderer>().enabled = false;
            large_chest_open.GetComponent<MeshRenderer>().enabled = true;
        }
        else if (Keys.key3Found && !key3Used && key2Used)
        {
            key3Used = true;
            small_chest_closed.GetComponent<MeshRenderer>().enabled = false;
            small_chest_open.GetComponent<MeshRenderer>().enabled = true;
        }
        else if (key2Used && key3Used && !keyTaken)
        {
            keyTaken = true;
            Keys.finalKeyFound = true;
            key.SetActive(false);
        }
        base.Use();
        base.OnLookAway();
        OnLook();

    }
    public override void OnLook()
    {
        if (Keys.key2Found && !key2Used)
        {
            showTooltip(use_key2);
        }
        else if (Keys.key2Found && key2Used && !Keys.key3Found)
        {
            showTooltip(waiting_for_key_3, true);
        }
        else if (Keys.key3Found && !key3Used && key2Used)
        {
            showTooltip(use_key3);
        }
        else if(key2Used && key3Used && key2Used)
        {
            showTooltip(take_Key);
        }
        else
        {
            showTooltip(noKeys, true);
        }
    }
    private void showTooltip(string text, bool showCross = false)
    {
        interactionPanel.SetActive(true);
        interaction_Text.text = text;
        isObjectBeingLookedAt = true;
        if (showCross) Interactible.cross.SetActive(true);
    }
    public override void OnLookAway()
    {
        base.OnLookAway(); 
        Interactible.cross.SetActive(false);
    }
}
