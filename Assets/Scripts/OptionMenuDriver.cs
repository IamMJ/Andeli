using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenuDriver : MonoBehaviour
{
    GameController gc;
    VictoryMeter vm;

    public void HidePauseMenu()
    {
        if (!gc)
        {
            gc = FindObjectOfType<GameController>();
        }
        gc.ResumeGameSpeed(false);
        gameObject.SetActive(false);
    }
    public void ReturnToWelcomeScene()
    {
        if (!gc)
        {
            gc = FindObjectOfType<GameController>();
        }
        gc.EndCurrentGame();
    }

    public void Debug_ArenaResetToMiddle()
    {
        if (!vm)
        {
            vm = FindObjectOfType<VictoryMeter>();
        }
        vm.SetBalance(25f);
        vm.SetDecayAmount(0f);
    }

    public void Debug_ArenaSetToLose()
    {
        if (!vm)
        {
            vm = FindObjectOfType<VictoryMeter>();
        }
        vm.SetBalance(3f);
        vm.SetDecayAmount(3f);
    }

    public void Debug_ArenaSetToWin()
    {
        if (!vm)
        {
            vm = FindObjectOfType<VictoryMeter>();
        }
        vm.SetBalance(47f);
        vm.SetDecayAmount(-3f);
    }


}
