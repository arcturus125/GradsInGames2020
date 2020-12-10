using System;
using System.Collections;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] private StoryData _data;
    public enum Checkpoints
    {
        beginning,
        welcome,

        minigameTitle,
        minigameSelection,

        puzzleOfLight_Dealing_cards,
        puzzleOfLight_Missing_Pieces,
        puzzleOfLisht_WaitingForPieces
    }
    public static Checkpoints checkpoint;

    private TextDisplay _output;
    private BeatData _currentBeat;
    private WaitForSeconds _wait;


    int[] beatsWithoutInputs = new int[] { 9, 10 };


    private void Awake()
    {
        _output = GetComponentInChildren<TextDisplay>(); 
        _currentBeat = null; // is only null on the first frame. after that, it always has a value                             
        _wait = new WaitForSeconds(0.5f);
        checkpoint = Checkpoints.beginning;
    }
    private void Update()
    {
        if(_output.IsIdle)
        {
            if (CameraController.cameraState == CameraController.CameraState.TableView)
            {
                // runs the first beat when the player first sits down
                if (checkpoint == Checkpoints.beginning)
                {
                    checkpoint = Checkpoints.welcome;
                    DisplayBeat(1);
                }
                else if(checkpoint == Checkpoints.minigameTitle)
                {
                    checkpoint = Checkpoints.minigameSelection;
                    //DisplayBeat(8);
                }
                else if(checkpoint == Checkpoints.puzzleOfLight_Missing_Pieces)
                {
                    checkpoint = Checkpoints.puzzleOfLisht_WaitingForPieces;
                    DisplayBeat(10);
                }
                //monitors for inputs
                else
                {
                    if(!isCurrentBeatChoiceless())
                    {
                        UpdateInput();
                    }
                }
            }
        }
    }

    private void CheckForCheckpoints(int id)
    {
        if (id == 8)
        {
            checkpoint = Checkpoints.minigameTitle;
        }
        else if(id == 9)
        {
            CardGame.singleton.RunAnimations();
        }
    }
    // is the current beat one without any choices (something else will trigger the next beat)
    private bool isCurrentBeatChoiceless()
    {
        foreach(int beatID in beatsWithoutInputs)
        {
            if (_currentBeat.ID == beatID)
            {
                return true;
            }
        }
        return false;
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
        CheckForCheckpoints(id);
        BeatData data = _data.GetBeatById(id);
        StartCoroutine(DoDisplay(data));
        _currentBeat = data;
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


        if (!isCurrentBeatChoiceless())
        {
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
        }
    }
}
