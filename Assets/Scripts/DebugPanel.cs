using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugPanel : UI_Panel
{
    Librarian lib;
    GameController gc;
    HealthManager hm;
    [SerializeField] TextMeshProUGUI letterRoutingTMP = null;
    [SerializeField] TextMeshProUGUI AIvaluesTMP = null;
    [SerializeField] TextMeshProUGUI autoigniteTMP = null;
    [SerializeField] GameObject debugButton = null;

    private void Start()
    {
        lib = Librarian.GetLibrarian();
        gc = lib.gameController;
    }

    public void HidePauseMenu()
    {
        gc.ResumeGameSpeed(false);
        ShowHideElements(false);
        debugButton.SetActive(gc.debug_ShowDebugMenuButton);
    }

    public void ShowHideDebugButton(bool shouldBeShown)
    {
        debugButton.SetActive(shouldBeShown);
    }

    public void BringUpDebugMenu()
    {
        foreach (var element in elements)
        {
            element.SetActive(true);
        }
        gc.PauseGame();
        ShowHideElements(true);
        debugButton.SetActive(false);
    }

    #region Debug Options
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
        if (!hm)
        {
            hm = lib.ui_Controller.combatPanel.GetComponent<HealthManager>();
        }
        hm.ResetHealthBars();
    }

    public void Debug_ArenaSetToLose()
    {
        if (!hm)
        {
            hm = lib.ui_Controller.combatPanel.GetComponent<HealthManager>();
        }
        hm.ModifyPlayerHealth(-1.1f);
    }

    public void Debug_ArenaSetToWin()
    {
        if (!hm)
        {
            hm = lib.ui_Controller.combatPanel.GetComponent<HealthManager>();
        }
        hm.ModifyEnemyHealth(-1.1f);
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

    public void Debug_SwitchToUpgradeMenu()
    {
        lib.ui_Controller.SetContext(UI_Controller.Context.Upgrades);
    }

    public void Debug_ToggleAutoIgnite()
    {
        gc.debug_AlwaysIgniteLetters = !gc.debug_AlwaysIgniteLetters;
        if (gc.debug_AlwaysIgniteLetters)
        {
            autoigniteTMP.text = "Autoignite: ON";
        }
        else
        {
            autoigniteTMP.text = "Autoignite: OFF";
        }

    }

    public void Debug_UnlockAllUpgrades()
    {
        gc.GetPlayer().GetComponent<PlayerMemory>().Debug_GainAllAbilities();
    }

    public void Debug_Add1000Glifs()
    {
        gc.GetPlayer().GetComponent<PlayerMemory>().AdjustMoney(1000);
    }
    #endregion
}
