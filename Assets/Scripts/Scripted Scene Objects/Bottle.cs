using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottle : Interactible
{
    public static bool isInteractible = false;
    public string customInteractionText = "Mix";
    public static GameObject Potion;
    public static bool First_potion = true;
    public float R = 0.0f;
    public float G = 0.0f;
    public float B = 0.0f;
    public static float Potion_R = 0.0f;
    public static float Potion_G = 0.0f;
    public static float Potion_B = 0.0f;
    public Material mat_specular;
    public Material mat_transparent = null;


    private void Awake()
    {
        Potion = GameObject.Find("Potion_mix");
    }

    public override void Use()
    {
        if (isInteractible)
        {
            Mix();
            base.OnLookAway(); // after taking the object, remove the iteraction button from the screen or it will get stuck 
            CheckForCorrectMix();
        }
    }

    private void CheckForCorrectMix()
    {
        if(Potion_R == 191.25f && Potion_G == 127.5 && Potion_B == 191.25)
        {
            ScreenManager.checkpoint = ScreenManager.Checkpoints.puzzleOfColour_GameComplete;
            isInteractible = false;
        }
    }

    void Mix()
    {
        if (R == -1)
        {

            Material mat = new Material(mat_transparent);
            mat.color = new Color(159 / 255.0f, 180 / 255.0f, 207 / 255.0f, 47.0f/255.0f);
            Potion.GetComponent<MeshRenderer>().material = mat;
            First_potion = true;
        }
        else
        {


            if (First_potion)
            {
                Potion_R = R;
                Potion_G = G;
                Potion_B = B;
                First_potion = false;
            }
            else
            {
                Potion_R = (Potion_R + R) / 2.0f;
                Potion_G = (Potion_G + G) / 2.0f;
                Potion_B = (Potion_B + B) / 2.0f;
            }
            Material mat = new Material(mat_specular);
            mat.color = new Color(Potion_R / 255.0f, Potion_G / 255.0f, Potion_B / 255.0f, 255);
            Potion.GetComponent<MeshRenderer>().material = mat;

            Debug.Log(Potion_R + " " + Potion_G + " " + Potion_B);
        }
    }

    public override void OnLook()
    {
        if (isInteractible)
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
