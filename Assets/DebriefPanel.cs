using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DebriefPanel : UI_Panel
{
    [SerializeField] Image defeatedImage_bottom = null;
    [SerializeField] Image defeatedImage_top = null;
    
    [SerializeField] TextMeshProUGUI outcomeTMP = null;
    [SerializeField] TextMeshProUGUI TimeTMP = null;
    [SerializeField] TextMeshProUGUI PowerDealtTMP = null;
    [SerializeField] TextMeshProUGUI WordsSpelledTMP = null;
    [SerializeField] TextMeshProUGUI BestWordTMP = null;
    [SerializeField] Material loserFadeMaterial = null;

    [SerializeField] GameObject[] rewardButtons = null;
    [SerializeField] GameObject acceptDefeatButton = null;

    Librarian lib;

    //param
    float timeBeforeLoserFade = 1f;
    float timeToFadeLoser = 4f;

    //state
    float timeSinceActivation = 0f;
    bool isFadedAlready = false;
    void Start()
    {
        lib = Librarian.GetLibrarian();
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
        lib.ui_Controller.SetContext(UI_Controller.Context.Reward);
        lib.ui_Controller.rewardPanel.SetRewardCurrencyAmount(50);
        lib.ui_Controller.rewardPanel.SetRewardedAbility(TrueLetter.Ability.Frozen); //Ability should be selected by the arena starter
    }

    public void HandleAdRewardClick()
    {
        Debug.Log("get the ad reward");
        //rpd.ActivateRewardPanel(100);
        lib.ui_Controller.SetContext(UI_Controller.Context.Advert);
        lib.ui_Controller.rewardPanel.SetRewardCurrencyAmount(75);
        lib.ui_Controller.rewardPanel.SetRewardedAbility(TrueLetter.Ability.Frozen); //Ability should be selected by the arena starter
    }

    public void HandleAcceptDefeatClick()
    {
        lib.ui_Controller.SetContext(UI_Controller.Context.Overworld);
    }

    public void ActivateDebriefPanel(bool didPlayerWin, GameObject playerRef, GameObject enemyRef, float timeInArena)
    {
        isFadedAlready = false;
        timeSinceActivation = 0;
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
        SetButtonsForVictoryDefeat(didPlayerWin);
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
