using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameComplete : MonoBehaviour
{
    public static GameComplete singleton;
    public static bool GameWon = false;
    public Animator anim;
    private void Awake()
    {
        singleton = this;
    }
    public static void Wingame()
    {
        GameWon = true;
        singleton.anim.SetBool("win", true);
        Cursor.lockState = CursorLockMode.None;
    }
}
