using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RewardPanel : UI_Panel
{
    [SerializeField] TextMeshProUGUI glyphRewardTMP = null;
    [SerializeField] TextMeshProUGUI expRewardTMP = null;
    int amountToGive;
    UI_Controller uic;

    void Start()
    {
        uic = Librarian.GetLibrarian().ui_Controller;
    }

    public void HandleAccceptRewardClick()
    {
        //grant rewards
        Debug.Log("Button clicked");
        uic.SetContext(UI_Controller.Context.Upgrades);
    }

    public void SetRewardPanelAmount(int amount)
    {
        amountToGive = amount;
    }

    public void ActivateRewardPanel()
    {
        //populate reward amounts here.
        glyphRewardTMP.text = "+" + amountToGive;
        expRewardTMP.text = "+" + amountToGive * 5;

    }   
}
