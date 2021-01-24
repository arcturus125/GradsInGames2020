using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timer : MonoBehaviour
{
    public static bool timerActive = false;
    private float offset = 0;
    string finishingTime = "";
    // Start is called before the first frame update
    void Start()
    {
        timerActive = true;
        offset = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerActive)
        {
            float currentTime = Time.time - offset;
            this.GetComponent<Text>().text = "" + (int)(currentTime / 60.0f) + ":" + (int)(Mathf.RoundToInt(currentTime % 60) / 10) + truncateDuplicatevalues(currentTime);
            finishingTime = "" + (int)(currentTime / 60.0f) + ":" + (int)(Mathf.RoundToInt(currentTime % 60) / 10) + truncateDuplicatevalues(currentTime);

        }
        else
        {
            this.GetComponent<Text>().text = "Time : " + finishingTime;
        }
    }
    string truncateDuplicatevalues(  float time)
    {
        if (Mathf.RoundToInt((time % 60) % 10) == 10) return "0";
        else return "" + Mathf.RoundToInt((time % 60) % 10);
        
    }
}
