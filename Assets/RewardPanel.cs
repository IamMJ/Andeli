using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RewardPanel : UI_Panel
{
    [SerializeField] TextMeshProUGUI glyphRewardTMP = null;
    [SerializeField] TextMeshProUGUI expRewardTMP = null;

    UI_Controller uic;

    void Start()
    {
        uic = Librarian.GetLibrarian().ui_Controller;
    }

    public void HandleAccceptRewardClick()
    {
        //grant rewards
        uic.SetContext(UI_Controller.Context.Upgrades);
    }

    public void ActivateRewardPanel(int testAmount)
    {
        //populate reward amounts here.
        glyphRewardTMP.text = "+" + testAmount;
        expRewardTMP.text = "+" + testAmount*5;

    }   
}
