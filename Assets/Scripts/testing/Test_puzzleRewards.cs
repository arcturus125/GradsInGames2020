using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_puzzleRewards : MonoBehaviour
{
    public bool Reward1 = false;
    public bool Reward2 = false;
    public bool Reward3 = false;

    // Update is called once per frame
    void Update()
    {
        if(Reward1)
        {
            CardGame.singleton.puzzleComplete();
            Reward1 = false;
        }
        if(Reward2)
        {
            LaptopMovement_Desk.reward2();
            Reward2 = false;
        }
        if(Reward3)
        {
            Fireplace.giveReward3();
            Reward3 = false;
        }
    }
}
