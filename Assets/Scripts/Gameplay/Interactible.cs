using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactible : MonoBehaviour
{
    // public attribues
    protected Transform defaultInteractionButtonLocation;
    protected bool isObjectBeingLookedAt = false;

    // static attributes
    public static GameObject interactionPanel;
    public static GameObject cross;
    public static bool isCrossVisible = false;
    public static Text interaction_Text;
    public const string DEFAULT_INTERACTION_TEXT = "Interact";

    // private attributes
    protected static Camera _cam;

    private void Awake()
    {
        cross = GameObject.Find("Cross");
        interaction_Text = GameObject.Find("Interaction Text").GetComponent<Text>();
        interactionPanel = GameObject.Find("InteractionPanel");
    }

    public virtual void Start()
    {
        defaultInteractionButtonLocation = this.transform;
        interactionPanel.SetActive(false);
        Init();
    }
    protected void Init()
    {
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
        //Debug.Log("OnLook");
    }
    public virtual void OnLookAway()
    {
        interactionPanel.SetActive(false);
        isObjectBeingLookedAt = false;
        //Debug.Log("OnLookAway");
    }
    public virtual void Use()
    {
        //Debug.Log("Use");
    }
}
