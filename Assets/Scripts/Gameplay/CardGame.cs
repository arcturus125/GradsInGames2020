using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGame : MonoBehaviour
{
    // singleton: allows me to reference the public attributes if this class without making them static
    public static CardGame singleton;

    public GameObject[] cards = new GameObject[12];
    public GameObject cardsPanel;
    public Text cardsText;

    // private attributes
    private int[] _hiddenCardIndexes = new int[] { 6,7,8,9 };
    private int _indexer = 0;
    private Animator _anim;
    private bool _animateCards = false;
    private bool _animateHiddenCards = false;
    private bool _animateCardsToFaceUp = false;

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
                    //    _indexer++;                                      // instead they shoudl move to the hand
                    //}


                    GameObject card = cards[_hiddenCardIndexes[_indexer]];
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
                if(!_anim.GetCurrentAnimatorStateInfo(0).IsName("holding_card 0"))
                {
                    if (_indexer != _hiddenCardIndexes.Length)
                    {
                        _indexer++;
                        if (_indexer != _hiddenCardIndexes.Length)
                            animation2State = AnimationHandlerState.playAnimation;
                    }
                    //as the last animation is playing
                    if(_indexer == _hiddenCardIndexes.Length)
                    {
                        // check when the last animation is complete
                        if(_anim.GetCurrentAnimatorStateInfo(0).IsName("face_up"))
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
            if(Animations3(_indexer, _animateCardsToFaceUp))
            {
                // animate the next card
                _indexer++;

                // when last card has been animated, stop the animations
                if(_indexer == cards.Length)
                {
                    animation3State = AnimationHandlerState.stopped;
                    Animations3Complete();
                }
            }

        }

        //### testing 
        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    RunAnimations2();
        //}
        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    RunAnimations3(true);
        //}
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    RunAnimations3(false);
        //}
    }

    // runs when animation1 is complete
    private void AnimationsComplete()
    {
        ScreenManager.checkpoint = ScreenManager.Checkpoints.puzzleOfLight_Missing_Pieces;
    }
    // runs when animation2 is complete
    private void Animations2Complete()
    {
        RunAnimations3(false);
    }
    // runs when animation3 is complete
    private void Animations3Complete()
    {
        ScreenManager.checkpoint = ScreenManager.Checkpoints.puzzleOfLight_SelectTheCards;
    }

    // plays Animation1 and returns true when ready to play next animation
    private bool Animations(int cardNumber, bool skipHiddenCards)
    {
        if (animationState != AnimationHandlerState.stopped)
        {
            if (animationState == AnimationHandlerState.playAnimation)
            {
                // if the card is hidden, skip it and move on to next one
                if(isHiddenCard(cardNumber) && skipHiddenCards)
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
        if(animation3State == AnimationHandlerState.playAnimation)
        {
            GameObject card = cards[cardNumber];
            _anim = card.GetComponent<Animator>();
            _anim.SetBool("cardFlipped", flipped);
            Debug.Log("animating card: " + cardNumber);
            animation3State = AnimationHandlerState.waiting;
        }
        else if(animation3State == AnimationHandlerState.waiting)
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
    private bool isHiddenCard( int index)
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
                cards[index].GetComponentsInChildren<SpriteRenderer>()[1].enabled = !hidden;
                cards[index].GetComponentsInChildren<SpriteRenderer>()[2].enabled = !hidden;
            }
        }
        else
        {
            foreach (GameObject card in cards)
            {
                card.GetComponentsInChildren<SpriteRenderer>()[1].enabled = !hidden;
                card.GetComponentsInChildren<SpriteRenderer>()[2].enabled = !hidden;
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

}
