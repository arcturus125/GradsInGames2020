using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGame : MonoBehaviour
{
    // singleton: allows me to reference the public attributes if this class without making them static
    public static CardGame singleton;

    public Light[] Candles;
    public Sprite[] cardFaces; // assigned in inspector
    public GameObject[] cards = new GameObject[12];
    public GameObject cardsPanel;
    public Text cardsText;

    // private attributes
    private int[] _hiddenCardIndexes = new int[] { 6, 7, 8, 9 };
    private int _indexer = 0;
    private Animator _anim;
    private bool _animateCards = false;
    private bool _animateHiddenCards = false;
    private bool _animateCardsToFaceUp = false;
    private int _correctCardNumber = -1;
    private System.Random _rand = new System.Random();
    private bool _playingGame = false;

    // states of the animator
    public enum AnimationHandlerState
    {
        playAnimation,
        waiting,
        stopped
    }
    public static AnimationHandlerState animationState = AnimationHandlerState.playAnimation;
    public static AnimationHandlerState animation2State = AnimationHandlerState.stopped;
    public static AnimationHandlerState animation3State = AnimationHandlerState.stopped;

    /*
     * Animation 1: dealing the cards out from the deck
     * 
     * Animation 2: placing the cards found around the room onto the table
     * 
     * Animation 3: flipping all the cards over, ready to start the game
     * 
     */

    enum GameStates
    {
        pickACard,
        correct,
        incorrect
    }
    GameStates gameState = GameStates.pickACard;

    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        cardsPanel.SetActive(false);
        //RunAnimations();
    }
    void Update()
    {
        // animations 1
        if (animationState != AnimationHandlerState.stopped)
        {
            // animate the cards, skipping the hidden ones
            if (_animateCards)
            {
                if (Animations(_indexer, true))
                {
                    _indexer++;
                    if (_indexer >= cards.Length)
                    {
                        _indexer = 0;
                        _animateCards = false;
                        _animateHiddenCards = true;
                    }
                }
            }
            // go back and animate the hidden cards
            if (_animateHiddenCards)
            {
                if (_indexer < _hiddenCardIndexes.Length)
                {
                    //if (Animations(_hiddenCardIndexes[_indexer], false)) // This is the old way of doing it
                    //{                                                    // i no longer want the hidden cards to be placed on the table
                    //    _indexer++;                                      // instead they should move to the hand
                    //}


                    GameObject card = cards[_hiddenCardIndexes[_indexer]];
                    pickNewSprite(card);
                    _anim = card.GetComponent<Animator>();
                    _anim.SetBool("holding", true);
                    _indexer++;
                }
                else
                {
                    _animateHiddenCards = false;
                    animationState = AnimationHandlerState.stopped;
                    AnimationsComplete();
                }
            }
        }
        // animations 2
        if (animation2State != AnimationHandlerState.stopped)
        {
            // animate the current card to be in the players hand
            if (animation2State == AnimationHandlerState.playAnimation)
            {
                int num = _hiddenCardIndexes[_indexer];
                GameObject card = cards[num];
                _anim = card.GetComponent<Animator>();
                _anim.SetBool("holding", false);
                animation2State = AnimationHandlerState.waiting;
            }
            // wait until the animation is complete before playing the next one
            else if (animation2State == AnimationHandlerState.waiting)
            {
                if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("holding_card 0"))
                {
                    if (_indexer != _hiddenCardIndexes.Length)
                    {
                        _indexer++;
                        if (_indexer != _hiddenCardIndexes.Length)
                            animation2State = AnimationHandlerState.playAnimation;
                    }
                    //as the last animation is playing
                    if (_indexer == _hiddenCardIndexes.Length)
                    {
                        // check when the last animation is complete
                        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("face_up"))
                        {
                            // stop running animation2
                            animation2State = AnimationHandlerState.stopped;
                            // run code to trigger an event on completion of animation2
                            Animations2Complete();
                        }
                    }
                }
            }

        }
        // animations 3
        if (animation3State != AnimationHandlerState.stopped)
        {
            // animate the current card to flip over, runs the if statement when the animation is complete
            if (Animations3(_indexer, _animateCardsToFaceUp))
            {
                // animate the next card
                _indexer++;

                // when last card has been animated, stop the animations
                if (_indexer == cards.Length)
                {
                    animation3State = AnimationHandlerState.stopped;
                    Animations3Complete();
                }
            }

        }



        if (_playingGame)
        {
            // when the player clicks a card
            for (int i = 0; i < cards.Length; i++)
            {
                if (CameraController.IsCursorOver(cards[i]))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Animator anim = cards[i].GetComponent<Animator>();
                        // if it is the first card clicked, or the correct card is picked, flip the card over
                        if (_correctCardNumber == -1 || _correctCardNumber == i)
                        {
                            Debug.Log("Correct" + _correctCardNumber);
                            ScreenManager.checkpoint = ScreenManager.Checkpoints.puzzleOfLight_Correct;
                            //correct
                            pickNewSprite(cards[i]);
                            anim.SetBool("cardFlipped", !anim.GetBool("cardFlipped"));
                            _correctCardNumber = pickNextCardNumber(i);
                            if (_correctCardNumber == -3)
                            {
                                Debug.Log("Puzzle of light complete");
                                puzzleComplete();
                                _playingGame = false;
                            }
                            Debug.Log("correct card number changed to:" + _correctCardNumber);
                        }
                        // if the incorrect card was clicked, flip all the cards to face down
                        else
                        {
                            //incorrect
                            Debug.Log("Incorrect");
                            ScreenManager.checkpoint = ScreenManager.Checkpoints.puzzleOfLight_Incorrect;
                            RunAnimations3(false);
                            _correctCardNumber = -1;
                        }

                    }
                }
            }
        }
    }
    // run once when the puzzle of light is complete
    public  void puzzleComplete()
    {
        ScreenManager.checkpoint = ScreenManager.Checkpoints.puzzleOfLight_Complete; // run a dialogue when the game is complete
        Debug.Log("Reward 1 given");
        Keys.key1Found = true;
        Keys.updateKeyCount();
        Gem.NoOfGemsFound += 10; // gives the use 10 gems for completing this puzzle
        Gem.UpdateGemsUI();
    }
    // runs when animation1 is complete
    private void AnimationsComplete()
    {
        // runs a dialogue to prompt the user to find the missing cards aroudn the room
        ScreenManager.checkpoint = ScreenManager.Checkpoints.puzzleOfLight_Missing_Pieces;
    }
    // runs when animation2 is complete
    private void Animations2Complete()
    {
        RunAnimations3(false); // flips all the cards over to face down, starting the game
    }
    // runs when animation3 is complete
    private void Animations3Complete()
    {
        // starts the game
        ScreenManager.checkpoint = ScreenManager.Checkpoints.puzzleOfLight_SelectTheCards;
        _playingGame = true;
    }

    // plays Animation1 and returns true when ready to play next animation
    private bool Animations(int cardNumber, bool skipHiddenCards)
    {
        if (animationState != AnimationHandlerState.stopped)
        {
            if (animationState == AnimationHandlerState.playAnimation)
            {
                pickNewSprite(cards[cardNumber]);
                // if the card is hidden, skip it and move on to next one
                if (isHiddenCard(cardNumber) && skipHiddenCards)
                {
                    return true;
                }
                GameObject card = cards[cardNumber];
                _anim = card.GetComponent<Animator>();
                _anim.SetBool("sortDeck", true);
                animationState = AnimationHandlerState.waiting;
            }
            else if (animationState == AnimationHandlerState.waiting)
            {
                if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("in_deck 0"))
                {
                    animationState = AnimationHandlerState.playAnimation;
                    return true;
                }
            }
        }
        return false;
    }
    // plays Animation3 and returns true when ready to play next animation
    private bool Animations3(int cardNumber, bool flipped)
    {
        if (animation3State == AnimationHandlerState.playAnimation)
        {
            GameObject card = cards[cardNumber];
            _anim = card.GetComponent<Animator>();
            if (_anim.GetBool("cardFlipped") != flipped)
            {
                _anim.SetBool("cardFlipped", flipped);
                animation3State = AnimationHandlerState.waiting;
            }
            else
            {
                // if the card is already flipped, skip to the next one
                return true;
            }
        }
        else if (animation3State == AnimationHandlerState.waiting)
        {
            if (flipped)
            {
                if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("face_up") && !_anim.GetCurrentAnimatorStateInfo(0).IsName("FlipCard_UpToDown 0"))
                {
                    animation3State = AnimationHandlerState.playAnimation;
                    return true;
                }
            }
            else
            {
                if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("face_down") && !_anim.GetCurrentAnimatorStateInfo(0).IsName("FlipCard_DownToUp"))
                {
                    animation3State = AnimationHandlerState.playAnimation;
                    return true;
                }
            }
        }
        return false;
    }

    // returns true/false based on if this card is hidden or not
    private bool isHiddenCard(int index)
    {
        foreach (int hiddenCardIndex in _hiddenCardIndexes)
        {
            if (index == hiddenCardIndex)
            {
                return true;
            }
        }
        return false;
    }
    // changes the visibility of spriterenders on all the hidden cards
    public void HideCards(bool hidden, bool allcards = false)
    {
        if (!allcards)
        {
            foreach (int index in _hiddenCardIndexes)
            {
                cards[index].GetComponentsInChildren<SpriteRenderer>()[0].enabled = !hidden;
                cards[index].GetComponentsInChildren<SpriteRenderer>()[1].enabled = !hidden;
            }
        }
        else
        {
            foreach (GameObject card in cards)
            {
                card.GetComponentsInChildren<SpriteRenderer>()[0].enabled = !hidden;
                card.GetComponentsInChildren<SpriteRenderer>()[1].enabled = !hidden;
            }
        }
    }
    // updates the text on the ui
    public void UpdateCardUI(int numOfCardsFound)
    {
        if (numOfCardsFound < _hiddenCardIndexes.Length)
        {
            cardsText.text = "x " + numOfCardsFound;
        }
        else
        {
            cardsText.text = "Complete";
        }
    }
    // returns the number of cards the player has found
    public int getNumOfHiddenCards()
    {
        return _hiddenCardIndexes.Length;
    }


    // functions to be run by other classes to run the animations to start the game
    public void RunAnimations()
    {
        HideCards(true);
        _animateCards = true;
        animationState = AnimationHandlerState.playAnimation;
    }
    public void RunAnimations2()
    {
        _indexer = 0;
        HideCards(false);
        animation2State = AnimationHandlerState.playAnimation;
    }
    public void RunAnimations3(bool areCardsFacingUp)
    {
        _indexer = 0;
        _animateCardsToFaceUp = areCardsFacingUp;
        animation3State = AnimationHandlerState.playAnimation;
    }


    // Card numbers: for reference
    // 0  1  2  3
    // 4  5  6  7
    // 8  9 10 11

    // picks the next card
    //    the next card must be adjacent to the one you last clicked, but must not already be flipped, if no suitable cards can be found, you win the game
    //    if the next card is Right from the lst card you clicked. the candles on the wall will all turn off except the  right most candle
    private int pickNextCardNumber(int currentCardNumber)
    {
        bool DirectionFound = false;
        int randDirection  = -1;
        int retry_count = 0;
        while (!DirectionFound && retry_count < 25)
        {
            retry_count++;
            randDirection = _rand.Next(1, 5);
            /* 1 = up
             * 2 =right
             * 3 = down
             * 4 = left
             */
            if (randDirection == 1 && currentCardNumber <= 3)
            {
                Debug.Log("trying to go UP on card number " + currentCardNumber + "... trying again");
                continue;
            }
            if (randDirection == 3 && currentCardNumber >= 8)
            {
                Debug.Log("trying to go DOWN on card number " + currentCardNumber + "... trying again");
                continue;
            }
            if (randDirection == 2 && (currentCardNumber + 1) % 4 == 0)
            {
                Debug.Log("trying to go Right on card number " + currentCardNumber + "... trying again");
                continue;
            }
            if (randDirection >= 4 && (currentCardNumber) % 4 == 0)
            {
                Debug.Log("trying to go LEFT on card number " + currentCardNumber + "... trying again");
                continue;
            }

            if (randDirection == 1)
            {
                if (cards[currentCardNumber - 4].GetComponent<Animator>().GetBool("cardFlipped"))
                {
                    Debug.Log("trying to go UP on card number " + currentCardNumber + " (card already flipped) ... trying again");
                    continue;
                }
            }
            if (randDirection == 3)
            {
                if (cards[currentCardNumber + 4].GetComponent<Animator>().GetBool("cardFlipped"))
                {
                    Debug.Log("trying to go DOWN on card number " + currentCardNumber + " (card already flipped) ... trying again");
                    continue;
                }
            }
            if (randDirection == 2)
            {
                if (cards[currentCardNumber + 1].GetComponent<Animator>().GetBool("cardFlipped"))
                {
                    Debug.Log("trying to go Right on card number " + currentCardNumber + " (card already flipped) ... trying again");
                    continue;
                }
            }
            if (randDirection == 4)
            {
                if (cards[currentCardNumber - 1].GetComponent<Animator>().GetBool("cardFlipped"))
                {
                    Debug.Log("trying to go LEFT on card number " + currentCardNumber + " (card already flipped) ... trying again");
                    continue;
                }
            }

            DirectionFound = true;
        }
        if(!DirectionFound)
        {
            return -3;
        }
        if (randDirection == 1)
        {
            Debug.Log("UP");
            Candles[0].gameObject.SetActive(true);
            Candles[1].gameObject.SetActive(false);
            Candles[2].gameObject.SetActive(false);
            Candles[3].gameObject.SetActive(false);
            return currentCardNumber - 4;
        }
        else if (randDirection == 3)
        {
            Debug.Log("DOWN");
            Candles[0].gameObject.SetActive(false);
            Candles[1].gameObject.SetActive(true);
            Candles[2].gameObject.SetActive(false);
            Candles[3].gameObject.SetActive(false);
            return currentCardNumber + 4;
        }
        else if (randDirection == 2)
        {
            Debug.Log("RIGHT");
            Candles[0].gameObject.SetActive(false);
            Candles[1].gameObject.SetActive(false);
            Candles[2].gameObject.SetActive(false);
            Candles[3].gameObject.SetActive(true);
            return currentCardNumber + 1;
        }
        else if (randDirection == 4)
        {
            Debug.Log("LEFT");
            Candles[0].gameObject.SetActive(false);
            Candles[1].gameObject.SetActive(false);
            Candles[2].gameObject.SetActive(true);
            Candles[3].gameObject.SetActive(false);
            return currentCardNumber - 1;
        }
        else
        {
            Debug.LogError("error");
            return -2;
        }


    }

    // picks a random sprite for the face of the card
    private void pickNewSprite(GameObject card)
    {
        int r = _rand.Next(0, 51);
        card.GetComponentInChildren<SpriteRenderer>().sprite = cardFaces[r];
    }
}
