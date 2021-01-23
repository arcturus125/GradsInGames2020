using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayPressed()
    {
        SceneManager.LoadScene("MainScene");
        GameComplete.GameWon = false;
    }

    public void ClosePressed()
    {
        Application.Quit();
    }
}
