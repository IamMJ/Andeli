using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RewardPanelDriver : MonoBehaviour
{
    [SerializeField] GameObject[] miscGOs = null;
    [SerializeField] Image[] images = null;
    [SerializeField] TextMeshProUGUI glyphRewardTMP = null;
    [SerializeField] TextMeshProUGUI expRewardTMP = null;

    GameController gc;

    void Start()
    {
        gc = FindObjectOfType<GameController>();   
    }

    public void HandleAccceptRewardClick()
    {
        ShowHideEntirePanel(false);
    }

    public void ActivateRewardPanel(int testAmount)
    {
        //populate reward amounts here.
        glyphRewardTMP.text = "+" + testAmount;
        expRewardTMP.text = "+" + testAmount*5;
        //grant rewards
        ShowHideEntirePanel(true);
    }

    public void ShowHideEntirePanel(bool shouldBeShown)
    {
        if (shouldBeShown)
        {
            gc.PauseGame();
            GetComponent<Image>().enabled = true;
            foreach (var GO in miscGOs)
            {
                GO.SetActive(true);
            }foreach (var image in images)
            {
                image.enabled = true;
            }
            glyphRewardTMP.enabled = true;
            expRewardTMP.enabled = true;
        }
        else
        {
            gc.ResumeGameSpeed(false);
            GetComponent<Image>().enabled = false;
            foreach (var GO in miscGOs)
            {
                GO.SetActive(false);
            }
            foreach (var image in images)
            {
                image.enabled = false;
            }
            glyphRewardTMP.enabled = false;
            expRewardTMP.enabled = false;
        }
    }
}
