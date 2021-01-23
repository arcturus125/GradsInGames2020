using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door : MonoBehaviour
{
    public static door singleton;
    void Start()
    {
        singleton = this;
    }

    public void SlamDoor()
    {
        this.GetComponent<Animator>().SetBool("slamDoor", true);
    }
}
