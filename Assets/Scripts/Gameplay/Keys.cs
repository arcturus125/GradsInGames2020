using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keys : MonoBehaviour
{
    private static Keys _singleton;

    public static bool key1Found = false;
    public static bool key2Found = false;
    public static bool key3Found = false;
    public static bool finalKeyFound = false;

    public Text keyText;

    private void Start()
    {
        _singleton = this;
    }
    public static  void updateKeyCount()
    {
        int keysFound = 0;
        if (key1Found) keysFound++;
        if (key2Found) keysFound++;
        if (key3Found) keysFound++;

        _singleton.keyText.text = "x " + keysFound;

    }
}


