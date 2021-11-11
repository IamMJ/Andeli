using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RewardPanel : UI_Panel
{
    [SerializeField] TextMeshProUGUI glyphRewardTMP = null;
    [SerializeField] TextMeshProUGUI expRewardTMP = null;

    GameController gc;

    void Start()
    {
        gc = FindObjectOfType<GameController>();   
    }

    public void HandleAccceptRewardClick()
    {
        ShowHideElements(false);
    }

    public void ActivateRewardPanel(int testAmount)
    {
        //populate reward amounts here.
        glyphRewardTMP.text = "+" + testAmount;
        expRewardTMP.text = "+" + testAmount*5;
        //grant rewards
        ShowHideElements(true);
    }   
}
