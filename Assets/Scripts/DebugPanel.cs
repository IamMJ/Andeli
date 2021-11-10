using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugPanel : UI_Panel
{
    GameController gc;
    VictoryMeter vm;
    [SerializeField] TextMeshProUGUI letterRoutingTMP = null;
    [SerializeField] TextMeshProUGUI AIvaluesTMP = null;
    [SerializeField] GameObject debugButton = null;

    public void HidePauseMenu()
    {
        if (!gc)
        {
            gc = FindObjectOfType<GameController>();
        }
        gc.ResumeGameSpeed(false);
        ShowHideElements(false);
        debugButton.SetActive(gc.debug_ShowDebugMenuButton);
    }

    public void BringUpDebugMenu()
    {
        foreach (var element in elements)
        {
            element.SetActive(true);
        }
        debugButton.SetActive(false);
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

    public void ToggleLetterRoutingOption()
    {
        if (!gc)
        {
            gc = FindObjectOfType<GameController>();
        }
        if (gc.GetPlayer().GetComponent<WordBuilder>().ToggleLetterRoutingMode())
        {
            letterRoutingTMP.text = $"Letter Routing: Sword";
        }
        else
        {
            letterRoutingTMP.text = $"Letter Routing: Bag";
        }
    }

    public void ToggleAIValues()
    {
        if (!gc)
        {
            gc = FindObjectOfType<GameController>();
        }
        gc.debug_ShowAILetterValues = !gc.debug_ShowAILetterValues;
        if (gc.debug_ShowAILetterValues)
        {
            AIvaluesTMP.text = "Debug: AI letter values: ON";
        }
        else
        {
            AIvaluesTMP.text = "Debug: AI letter values: OFF";
        }
    }

    public void ShowHideDebugButton(bool shouldBeShown)
    {
        debugButton.SetActive(shouldBeShown);
    }


}
