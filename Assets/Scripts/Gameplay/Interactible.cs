using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactible : MonoBehaviour
{
    // public attribues
    public Transform defaultInteractionButtonLocation;
    public bool isObjectBeingLookedAt = false;

    // static attributes
    public static GameObject interactionPanel;
    public static Text interaction_Text;
    public const string DEFAULT_INTERACTION_TEXT = "Interact";

    // private attributes
    protected static Camera _cam;

    public virtual void Start()
    {
        defaultInteractionButtonLocation = this.transform;
        Init();
    }
    protected void Init()
    {
        interaction_Text = GameObject.Find("Interaction Text").GetComponent<Text>();
        interactionPanel = GameObject.Find("InteractionPanel");
        interactionPanel.SetActive(false);
        _cam = GameObject.Find("Camera").GetComponent<Camera>();
    }
    public virtual void Update()
    {
        if (isObjectBeingLookedAt)
        {
            Vector3 screenPos = _cam.WorldToScreenPoint(defaultInteractionButtonLocation.position);
            interactionPanel.transform.position = screenPos;
        }

    }

    public virtual void OnLook()
    {
        interactionPanel.SetActive(true);
        interaction_Text.text = DEFAULT_INTERACTION_TEXT;
        isObjectBeingLookedAt = true;
        Debug.Log("OnLook");
    }
    public virtual void OnLookAway()
    {
        interactionPanel.SetActive(false);
        isObjectBeingLookedAt = false;
        Debug.Log("OnLookAway");
    }
    public virtual void Use()
    {
        Debug.Log("Use");
    }
}
