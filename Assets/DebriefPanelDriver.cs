using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DebriefPanelDriver : MonoBehaviour
{
    [SerializeField] Image defeatedImage_bottom = null;
    [SerializeField] Image defeatedImage_top = null;
    [SerializeField] GameObject[] otherGOs = null;
    
    [SerializeField] TextMeshProUGUI outcomeTMP = null;
    [SerializeField] TextMeshProUGUI TimeTMP = null;
    [SerializeField] TextMeshProUGUI PowerDealtTMP = null;
    [SerializeField] TextMeshProUGUI WordsSpelledTMP = null;
    [SerializeField] TextMeshProUGUI BestWordTMP = null;
    [SerializeField] Material loserFadeMaterial = null;

    [SerializeField] GameObject[] rewardButtons = null;
    [SerializeField] GameObject acceptDefeatButton = null;

    GameController gc;

    //param
    float timeBeforeLoserFade = 1f;
    float timeToFadeLoser = 4f;

    //state
    float timeSinceActivation = 0f;
    bool isFadedAlready = false;
    void Start()
    {
        gc = FindObjectOfType<GameController>();
        ShowHideEntirePanel(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (isFadedAlready) { return; }
        timeSinceActivation += Time.unscaledDeltaTime;
        if (timeSinceActivation > timeBeforeLoserFade)
        {
            float factor = (timeSinceActivation - timeBeforeLoserFade) / (timeToFadeLoser);
            loserFadeMaterial.SetFloat("_FadeAmount", factor);
            if (timeSinceActivation > (timeBeforeLoserFade + timeToFadeLoser))
            {
                isFadedAlready = true;
            }
        }

    }

    public void HandleBaseRewardClick()
    {
        Debug.Log("get the base reward");
        ShowHideEntirePanel(false);
    }

    public void HandleAdRewardClick()
    {
        Debug.Log("get the ad reward");
        ShowHideEntirePanel(false);
    }

    public void HandleAcceptDefeatClick()
    {
        ShowHideEntirePanel(false);
    }

    public void ActivateDebriefPanel(bool didPlayerWin, GameObject playerRef, GameObject enemyRef, float timeInArena)
    {
        isFadedAlready = false;
        timeSinceActivation = 0;
        ShowHideEntirePanel(true);
        if (didPlayerWin)
        {
            outcomeTMP.text = "Victory";
            defeatedImage_top.sprite = enemyRef.GetComponent<SpriteRenderer>().sprite;
            defeatedImage_bottom.sprite = enemyRef.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            outcomeTMP.text = "Defeat";
            defeatedImage_top.sprite = playerRef.GetComponent<SpriteRenderer>().sprite;
            defeatedImage_bottom.sprite = playerRef.GetComponent<SpriteRenderer>().sprite;
        }
        loserFadeMaterial.SetFloat("_FadeAmount", 0);
        WordMakerMemory pm = playerRef.GetComponent<WordMakerMemory>();
        WordMakerMemory.ArenaData ad = pm.GetCurrentArenaData();
        TimeTMP.text = "Time: " + timeInArena.ToString();
        PowerDealtTMP.text = "Power Dealt: " + ad.powerDealt.ToString();
        WordsSpelledTMP.text = "Words Spelled: " + ad.wordsSpelled.ToString();
        BestWordTMP.text = ad.bestWordSpelled + " - " + ad.currentBestSinglePowerGain.ToString();
        pm.ResetCurrentArenaData();
    }

    public void ShowHideEntirePanel(bool shouldBeShown)
    {
        if (shouldBeShown)
        {
            gc.PauseGame();
            GetComponent<Image>().enabled = true;
            foreach (var GO in otherGOs)
            {
                GO.SetActive(true);
            }
            outcomeTMP.enabled = true;
            defeatedImage_bottom.enabled = true;
            defeatedImage_top.enabled = true;
            TimeTMP.enabled = true;
            PowerDealtTMP.enabled = true;
            WordsSpelledTMP.enabled = true;
            BestWordTMP.enabled = true;
        }
        else
        {
            gc.ResumeGameSpeed(false);
            GetComponent<Image>().enabled = false;
            foreach (var GO in otherGOs)
            {
                GO.SetActive(false);
            }
            foreach (var GO in rewardButtons)
            {
                GO.SetActive(false);
            }
            acceptDefeatButton.SetActive(false);
            outcomeTMP.enabled = false;
            defeatedImage_bottom.enabled = false;
            defeatedImage_top.enabled = false;
            TimeTMP.enabled = false;
            PowerDealtTMP.enabled = false;
            WordsSpelledTMP.enabled = false;
            BestWordTMP.enabled = false;
        }
    }

    private void SetButtonsForVictoryDefeat(bool shouldShowVictory)
    {
        if (shouldShowVictory)
        {
            foreach (var GO in rewardButtons)
            {
                GO.SetActive(true);
            }
            acceptDefeatButton.SetActive(false);
        }
        else
        {
            foreach (var GO in rewardButtons)
            {
                GO.SetActive(false);
            }
            acceptDefeatButton.SetActive(true);
        }
    }

}
