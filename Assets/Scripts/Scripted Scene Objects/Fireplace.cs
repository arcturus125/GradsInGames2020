using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireplace : Interactible
{
    public static bool CauldronPlaced = false;
    public static bool keyPlaced = false;

    public GameObject placedCauldron;

    public Transform interactionButtonLocation;
    public string interactionText_noKey    = "You'll burn your hand if you try to melt the key like this";
    public string interactionText_Cauldron = "Place Cauldron";
    public string interactionText_meltKey = "Place Ice in Cauldron";
    public string interactionText_takeKey = "take key from Cauldron";
    public override void Start()
    {
        base.Start();
        placedCauldron.SetActive(false);
        Interactible.cross.SetActive(false);
    }
    public override void Update()
    {
        base.Update();
    }
    public override void Use()
    {
        if (Cauldron.pickedUp)
        {
            CauldronPlaced = true;
            placedCauldron.SetActive(true);
            Cauldron.pickedUp = false;
            base.Use();
            base.OnLookAway();
            OnLook();
        }
        else if(FrozenKey.isKeyTaken && CauldronPlaced)
        {
            keyPlaced = true;
            FrozenKey.isKeyTaken = false;
            base.Use();
            base.OnLookAway();
            OnLook();
        }
        else if( CauldronPlaced && keyPlaced)
        {
            // take key
            CauldronPlaced = false;
            keyPlaced = false;
            base.Use();
            base.OnLookAway();
            ScreenManager.checkpoint = ScreenManager.Checkpoints.puzzleOfHeat_backToMenu;
            giveReward3();
        }
    }
    public static void giveReward3()
    {
        Debug.Log("Reward 3 given");
        Gem.NoOfGemsFound += 10;
        Gem.UpdateGemsUI(); 
        Keys.key3Found = true;
        Keys.updateKeyCount();
    }
    public override void OnLook()
    {
        if(!Cauldron.pickedUp && FrozenKey.isKeyTaken && !CauldronPlaced)
        {
            Interactible.cross.SetActive(true);
            interactionPanel.SetActive(true);
            interaction_Text.text = interactionText_noKey;
            isObjectBeingLookedAt = true;
        }
        else if (Cauldron.pickedUp)
        {
            interactionPanel.SetActive(true);
            interaction_Text.text = interactionText_Cauldron;
            isObjectBeingLookedAt = true;
        }
        else if (CauldronPlaced && FrozenKey.isKeyTaken)
        {
            interactionPanel.SetActive(true);
            interaction_Text.text = interactionText_meltKey;
            isObjectBeingLookedAt = true;
        }
        else if (CauldronPlaced && keyPlaced)
        {
            interactionPanel.SetActive(true);
            interaction_Text.text = interactionText_takeKey;
            isObjectBeingLookedAt = true;
        }
    }
    public override void OnLookAway()
    {
        base.OnLookAway();
        Interactible.cross.SetActive(false);
    }
}
