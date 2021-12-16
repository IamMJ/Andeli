using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AdvertPanel : UI_Panel
{
    Librarian lib;

    [SerializeField] TextMeshProUGUI blurbTMP = null;
    [SerializeField] TextMeshProUGUI countTMP = null;

    //param
    float timeForAd = 5f;

    //state
    bool isDisplayed = false;
    float timeLeftOnAd;


    void Start()
    {
        lib = Librarian.GetLibrarian();
    }

    private void Update()
    {
        if (isDisplayed)
        {
            timeLeftOnAd -= Time.unscaledDeltaTime;
            countTMP.text = $" {Mathf.RoundToInt(timeLeftOnAd)} seconds left";
            if (timeLeftOnAd < 0)
            {
                lib.ui_Controller.SetContext(UI_Controller.Context.Reward);
                lib.ui_Controller.rewardPanel.ActivateRewardPanel();
                isDisplayed = false;
            }
        }
    }

    public void ActivateAdvertPanel()
    {
        timeLeftOnAd = timeForAd;
        isDisplayed = true;
    }
}
