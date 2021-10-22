using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AdvertPanelDriver : MonoBehaviour
{
    RewardPanelDriver rpd;
    GameController gc;

    [SerializeField] TextMeshProUGUI blurbTMP = null;
    [SerializeField] TextMeshProUGUI countTMP = null;

    //param
    float timeForAd = 5f;

    //state
    bool isDisplayed = false;
    float timeLeftOnAd;


    void Start()
    {
        gc = FindObjectOfType<GameController>();
        rpd = FindObjectOfType<RewardPanelDriver>();
    }

    private void Update()
    {
        if (isDisplayed)
        {
            timeLeftOnAd -= Time.unscaledDeltaTime;
            countTMP.text = $" {Mathf.RoundToInt(timeLeftOnAd)} seconds left";
            if (timeLeftOnAd < 0)
            {
                rpd.ActivateRewardPanel(100);
                ShowHideEntirePanel(false);
            }
        }
    }

    public void ActivateAdvertPanel()
    {
        ShowHideEntirePanel(true);
        timeLeftOnAd = timeForAd;
    }

    public void ShowHideEntirePanel(bool shouldBeShown)
    {
        GetComponent<Image>().enabled = shouldBeShown;
        blurbTMP.enabled = shouldBeShown;
        countTMP.enabled = shouldBeShown;
        if (shouldBeShown)
        {
            isDisplayed = true;
            gc.PauseGame();
        }
        else
        {
            isDisplayed = false;
            gc.ResumeGameSpeed(false);
        }
    }
}
