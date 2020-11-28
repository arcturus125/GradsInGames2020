using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactible : MonoBehaviour
{
    public Transform defaultInteractionButtonLocation;
    public virtual void Start()
    {
        defaultInteractionButtonLocation = this.transform;
    }
    public virtual void OnLook()
    {
        Debug.Log("OnLook");
    }
    public virtual void OnLookAway()
    {
        Debug.Log("OnLookAway");
    }
    public virtual void Use()
    {
        Debug.Log("Use");
    }
}
