using System;
using System.Collections;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Singleton;
    [SerializeField] private StoryData _data;
    public enum Checkpoints
    {
        beginning,
        welcome,

        minigameTitle,
        minigameSelection,

        puzzleOfLight_Dealing_cards,
        puzzleOfLight_Missing_Pieces,
        puzzleOfLight_WaitingForPieces,
        puzzleOfLight_Animations,
        puzzleOfLight_SelectTheCards,
        puzzleOfLight_Idle,
        puzzleOfLight_Correct,
        puzzleOfLight_Incorrect,
        puzzleOfLight_Complete,

        puzzleOfColour_TakeLaptop,
        puzzleOfColour_PickColour,
        puzzleOfColour_Idle,
        puzzleOfColour_GameComplete,
        puzzleOfColour_PutLaptopBack,

        puzzleOfHeat_backToMenu,
        puzzleOfHeat_Idle



    }
    public static Checkpoints checkpoint;

    private TextDisplay _output;
    private BeatData _currentBeat;
    private WaitForSeconds _wait;
    public bool puzzleOfLight_complete = false;
    public bool puzzleOfColour_complete = false;
    public bool puzzleOfHeat_complete = false;
    public GameObject ObjectiveLight;


    //int[] beatsWithoutInputs = new int[] { 9, 10 };


    private void Awake()
    {
        Singleton = this;
        _output = GetComponentInChildren<TextDisplay>(); 
        _currentBeat = null; // is only null on the first frame. after that, it always has a value                             
        _wait = new WaitForSeconds(0.5f);
        checkpoint = Checkpoints.beginning;
    }
    private void Update()
    {
        if(_output.IsIdle)
        {
            if (CameraController.cameraState == CameraController.CameraState.TableView  || LaptopMovement_Barrel.isLaptopDown)
            {
                // runs the first beat when the player first sits down
                if (checkpoint == Checkpoints.beginning)
                {
                    checkpoint = Checkpoints.welcome;
                    DisplayBeat(1);
                }
                // the minigame selection screen
                else if(checkpoint == Checkpoints.minigameTitle)
                {
                    checkpoint = Checkpoints.minigameSelection;
                    DisplayBeat(8);
                }
                // pizzle of light : find the missing pieces
                else if(checkpoint == Checkpoints.puzzleOfLight_Missing_Pieces)
                {
                    checkpoint = Checkpoints.puzzleOfLight_WaitingForPieces;
                    CardGame.singleton.cardsPanel.SetActive(true);
                    Card.lookingForCards = true;
                    DisplayBeat(10);
                }
                // puzzle of light: missing pieces found
                else if (checkpoint == Checkpoints.puzzleOfLight_WaitingForPieces && Card.noOfFoundCards == CardGame.singleton.getNumOfHiddenCards())
                {
                    checkpoint = Checkpoints.puzzleOfLight_Animations;
                    CardGame.singleton.RunAnimations2();
                    CardGame.singleton.cardsPanel.SetActive(false);
                }
                else if (checkpoint == Checkpoints.puzzleOfLight_SelectTheCards)
                {
                    checkpoint = Checkpoints.puzzleOfLight_Idle;
                    DisplayBeat(11);
                }
                else if (checkpoint == Checkpoints.puzzleOfLight_Correct)
                {
                    checkpoint = Checkpoints.puzzleOfLight_Idle;
                    DisplayBeat(12);
                }
                else if (checkpoint == Checkpoints.puzzleOfLight_Incorrect)
                {
                    checkpoint = Checkpoints.puzzleOfLight_Idle;
                    DisplayBeat(13);
                }
                else if (checkpoint == Checkpoints.puzzleOfLight_Complete)
                {
                    puzzleOfLight_complete = true;
                    checkpoint = Checkpoints.puzzleOfLight_Idle;
                    DisplayBeat(14);
                }
                else if (checkpoint == Checkpoints.puzzleOfColour_PickColour)
                {
                    checkpoint = Checkpoints.puzzleOfColour_Idle;
                    DisplayBeat(16);
                }
                else if (checkpoint == Checkpoints.puzzleOfColour_GameComplete)
                {
                    puzzleOfColour_complete = true;
                    checkpoint = Checkpoints.puzzleOfColour_PutLaptopBack;
                    DisplayBeat(17);
                    Debug.Log("puzzle of heat complete");
                }
                else if (checkpoint == Checkpoints.puzzleOfHeat_backToMenu)
                {
                    puzzleOfHeat_complete = true;
                    checkpoint = Checkpoints.puzzleOfHeat_Idle;
                    DisplayBeat(21);
                }
                //monitors for inputs
                else
                {
                    //if(!isCurrentBeatChoiceless())
                    //{
                        UpdateInput();
                    //}
                }
            }
        }
    }

    // if this functions returns false, it will stop trying to run the dialogue, and run a different one instead
    private bool CheckForCheckpoints(int id)
    {
        if(id == 2)
        {
            ObjectiveLight.SetActive(false);
        }
        else if (id == 8)
        {
            ObjectiveLight.SetActive(false);
            if(puzzleOfColour_complete &&puzzleOfHeat_complete&&puzzleOfLight_complete)
            {
                DisplayBeat(22);
                return false;
            }
        }
        else if(id == 9)
        {
            if (puzzleOfLight_complete)
            {
                DisplayBeat(19);
                return false;
            }
            else
            {
                MenuScript.loadClues(1);
                CardGame.singleton.RunAnimations();
            }
        }
        else if(id == 5)
        {
            Debug.Log("slamming door");
            CameraController.singleton.slamDoor();
        }
        else if (id == 15)
        {
            if (puzzleOfColour_complete)
            {
                DisplayBeat(19);
                return false;
            }
            else
            {
                ObjectiveLight.SetActive(true);
                ObjectiveLight.transform.position = new Vector3(-15.79f, 0.936f, -13.298f);
                MenuScript.loadClues(2);
                LaptopMovement_Desk.isLaptopMoveable = true;
                checkpoint = Checkpoints.puzzleOfColour_TakeLaptop;
            }
        }
        else if (id == 16)
        {
            ObjectiveLight.SetActive(false);
        }
        else if (id == 18)
        {
            LaptopMovement_Desk.isLaptopMoveable = true;
            LaptopMovement_Desk.putLaptopBack = true;
            ObjectiveLight.SetActive(true);
            ObjectiveLight.transform.position = new Vector3(-15.363f, 0.936f, -3.94f);
        }
        else if (id ==20)
        {
            if (puzzleOfHeat_complete)
            {
                DisplayBeat(19);
                return false;
            }
            else
            {
                MenuScript.loadClues(3);
                Cauldron.canPickUp = true;
                FrozenKey.isKeyInteractible = true;
                ObjectiveLight.SetActive(true);
                ObjectiveLight.transform.position = new Vector3(-8.881f, 2.021f, -8.423f);
            }
        }
        return true;
    }





    // #################

    private void UpdateInput()
    {
        // ESC will quit the game if on the first "beat", otherwise it will return to the first beat
        // the first beat is acting as a menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(_currentBeat != null)
            {
                // removing this for functionality of the game
                //if (_currentBeat.ID == 1)
                //{
                //    Application.Quit(); 
                //}
                //else
                //{
                //    DisplayBeat(1);
                //}
            }
        }
        else
        {
            KeyCode alpha = KeyCode.Alpha1;
            KeyCode keypad = KeyCode.Keypad1;

            for (int count = 0; count < _currentBeat.Decision.Count; ++count)
            {
                if (alpha <= KeyCode.Alpha9 && keypad <= KeyCode.Keypad9)
                {
                    if (Input.GetKeyDown(alpha) || Input.GetKeyDown(keypad))
                    {
                        ChoiceData choice = _currentBeat.Decision[count];
                        DisplayBeat(choice.NextID);
                        break;
                    }
                }

                ++alpha;
                ++keypad;
            }
        }
    }

    private void DisplayBeat(int id)
    {
        if (CheckForCheckpoints(id))
        {
            BeatData data = _data.GetBeatById(id);
            StartCoroutine(DoDisplay(data));
            _currentBeat = data;
        }
    }

    private IEnumerator DoDisplay(BeatData data)
    {
        _output.Clear();

        while (_output.IsBusy)
        {
            yield return null;
        }

        _output.Display(data.DisplayText);

        while(_output.IsBusy)
        {
            yield return null;
        }


        //if (!isCurrentBeatChoiceless())
        //{
            for (int count = 0; count < data.Decision.Count; ++count)
            {
                ChoiceData choice = data.Decision[count];
                _output.Display(string.Format("{0}: {1}", (count + 1), choice.DisplayText));

                while (_output.IsBusy)
                {
                    yield return null;
                }
            }

            if (data.Decision.Count > 0)
            {

                _output.ShowWaitingForInput();

            }
        //}
    }
}
