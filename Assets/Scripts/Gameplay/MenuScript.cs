using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{

    private static MenuScript _singleton; // a static reference to the 1 instance of this class running

    public string[] puzzle1Clues;  //
    public string[] puzzle2Clues;  // each puzzle has 3 clues stored in strings
    public string[] puzzle3Clues;  //
    public GameObject cluePanel;      //
    public GameObject menuPanel;      // gameobjects assigned in the inspector
    public GameObject noCluesText;    //
    public static bool menuOpen = false; // true when the user has the menu left open, false otherwise

    // clue 1
    public Text clue1Text;
    public Button clue1Button;
    // clue 2
    public Text clue2Text;
    public Button clue2Button;
    // clue 3
    public Text clue3Text;
    public Button clue3Button;

    private void Awake()
    {
        _singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        cluePanel.SetActive(false);
        menuPanel.SetActive(false);


        menuOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    CursorLockMode previousLockState = CursorLockMode.Locked;
    // Update is called once per frame
    void Update()
    {
        // if the end game screen isnt active
        if (!GameComplete.GameWon)
        {
            // toggle the UI when the user pressed Esc
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menuPanel.SetActive(!menuPanel.activeSelf);
                menuOpen = menuPanel.active;
                if (menuOpen)
                {
                    previousLockState = Cursor.lockState;
                    Cursor.lockState = CursorLockMode.None;

                }
                else
                {
                    Cursor.lockState = previousLockState;
                }
            }

            // activate / deactivate the clue buttons depending on if the  player can afford the clues
            if (Gem.NoOfGemsFound < 2) _singleton.clue1Button.interactable = false;
            else _singleton.clue1Button.interactable = true;

            if (Gem.NoOfGemsFound < 3) _singleton.clue2Button.interactable = false;
            else _singleton.clue2Button.interactable = true;

            if (Gem.NoOfGemsFound < 4) _singleton.clue3Button.interactable = false;
            else _singleton.clue3Button.interactable = true;
        }

    }
    // when run, the clues for the specified puzzle are loaded onto the UI, the buttons are reset and the text hidden until the player clicks the buttons
    public static void loadClues(int puzzleNumber)
    {
        _singleton.noCluesText.SetActive(false);
        Debug.Log("loading clues");
        _singleton.cluePanel.SetActive(true);
        if (puzzleNumber == 1)
        {
            _singleton.clue1Text.text = _singleton.puzzle1Clues[0];
            _singleton.clue2Text.text = _singleton.puzzle1Clues[1];
            _singleton.clue3Text.text = _singleton.puzzle1Clues[2];
        }
        else if (puzzleNumber == 2)
        {
            _singleton.clue1Text.text = _singleton.puzzle2Clues[0];
            _singleton.clue2Text.text = _singleton.puzzle2Clues[1];
            _singleton.clue3Text.text = _singleton.puzzle2Clues[2];
        }
        else if (puzzleNumber == 3)
        {
            _singleton.clue1Text.text = _singleton.puzzle3Clues[0];
            _singleton.clue2Text.text = _singleton.puzzle3Clues[1];
            _singleton.clue3Text.text = _singleton.puzzle3Clues[2];
        }
        _singleton.clue1Button.gameObject.SetActive(true);
        _singleton.clue1Text.enabled = false;
        _singleton.clue2Button.gameObject.SetActive(true);
        _singleton.clue2Text.enabled = false;
        _singleton.clue3Button.gameObject.SetActive(true);
        _singleton.clue3Text.enabled = false;
    }

    public void Clue1ButtonClicked()
    {
        clue1Button.gameObject.SetActive(false);
        clue1Text.enabled = true;
        Gem.NoOfGemsFound -= 2;
        Gem.UpdateGemsUI();
    }
    public void Clue2ButtonClicked()
    {
        clue2Button.gameObject.SetActive(false);
        clue2Text.enabled = true;
        Gem.NoOfGemsFound -= 3;
        Gem.UpdateGemsUI();
    }
    public void Clue3ButtonClicked()
    {
        clue3Button.gameObject.SetActive(false);
        clue3Text.enabled = true;
        Gem.NoOfGemsFound -= 4;
        Gem.UpdateGemsUI();
    }
    public void mainMenuButtonPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void ClosePressed()
    {
        Application.Quit();
    }
}
