using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaptopMovement_Desk : Interactible
{
    public static bool isLaptopMoveable = false;
    public static bool putLaptopBack = false;

    public string customInteractionText = "Take Laptop";
    public GameObject Laptop;

    public override void Use()
    {
        // pick laptop up from table
        if (isLaptopMoveable && !putLaptopBack)
        {
            Laptop.transform.localPosition = new Vector3(-1.663f, 0.803f, 6.569f);
            LaptopMovement_Barrel.isLaptopPlaceable = true;
            base.OnLookAway(); // after taking the object, remove the iteraction button from the screen or it will get stuck 
        }
        // put laptop down on table
        else if(isLaptopMoveable && putLaptopBack)
        {
            Laptop.transform.localPosition = new Vector3(-0.608f, 0.803f, 3.868f);
            LaptopMovement_Barrel.isLaptopPlaceable = false;
            isLaptopMoveable = false;
            putLaptopBack = false;
            base.OnLookAway(); // after taking the object, remove the iteraction button from the screen or it will get stuck 
            ScreenManager.checkpoint = ScreenManager.Checkpoints.minigameTitle;
            Debug.Log("Reward 2 given");
            ScreenManager.Singleton.ObjectiveLight.SetActive(false);
            reward2();
        }
    }
    public static void reward2()
    {
        Keys.key2Found = true;
        Keys.updateKeyCount();
        Gem.NoOfGemsFound += 10;
        Gem.UpdateGemsUI();
    }
    public override void OnLook()
    {   
        if(putLaptopBack)
        {
            interactionPanel.SetActive(true);
            interaction_Text.text = "Place laptop down";
            isObjectBeingLookedAt = true;
        }
        else if (isLaptopMoveable)
        {
            interactionPanel.SetActive(true);
            interaction_Text.text = customInteractionText;
            isObjectBeingLookedAt = true;
        }
    }
    public override void OnLookAway()
    {
        if (isLaptopMoveable)
        {
            base.OnLookAway();

        }
    }
}
