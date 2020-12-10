using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGame : MonoBehaviour
{
    // singleton: allows me to reference the public attributes if this class without making them static
    public static CardGame singleton;

    public GameObject[] cards = new GameObject[12];

    // private attributes
    private int[] _hiddenCardIndexes = new int[] { 6,7,8,9 };
    private int _indexer = 0;
    private Animator _anim;
    private bool _animateCards = false;
    private bool _animateHiddenCards = false;

    // states of the animator
    public enum AnimationHandlerState
    {
        playAnimation,
        waiting,
        stopped
    }
    public static AnimationHandlerState animationState = AnimationHandlerState.playAnimation;


    private void Awake()
    {
        singleton = this;
    }

    void Update()
    {
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
                        Debug.Log("animating hidden cards now");
                    }
                }
            }
            // go back and animate the hidden cards
            if (_animateHiddenCards)
            {
                if (_indexer < _hiddenCardIndexes.Length)
                {
                    if (Animations(_hiddenCardIndexes[_indexer], false))
                    {
                        _indexer++;
                    }
                }
                else
                {
                    _animateHiddenCards = false;
                    animationState = AnimationHandlerState.stopped;
                    AnimationsComplete();
                }
            }
        }
    }

    private void AnimationsComplete()
    {
        ScreenManager.checkpoint = ScreenManager.Checkpoints.puzzleOfLight_Missing_Pieces;
    }

    // returns true when ready to play next animation
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
    // disables the spriterenders on all the hidden cards
    public void HideCards(bool hidden)
    {
        foreach (int index in _hiddenCardIndexes)
        {
            cards[index].GetComponent<SpriteRenderer>().enabled = !hidden;
            cards[index].GetComponentsInChildren<SpriteRenderer>()[1].enabled = !hidden;
        }
    }
    // function to be run by other classes to run the animations to start the game;
    public void RunAnimations()
    {
        HideCards(true);
        _animateCards = true;
        animationState = AnimationHandlerState.playAnimation;
    }
}
