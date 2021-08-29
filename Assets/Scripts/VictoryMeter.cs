using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryMeter : MonoBehaviour
{
    //init
    [SerializeField] Slider victorySlider = null;
    SceneLoader sl;
    GameController gc;
    [SerializeField] Image sliderFillImage = null;

    //param
    float victoryAmount = 50f;
    float defeatAmount = 0f;
    float startingBalance = 25;
    float decayPerSecond = 0f;

    public Action<bool> OnArenaVictory_TrueForPlayerWin;


    //state
    float currentBalance;
    void Start()
    {
        gc = FindObjectOfType<GameController>();
        sl = FindObjectOfType<SceneLoader>();
        victorySlider.maxValue = victoryAmount;
        victorySlider.minValue = defeatAmount;
        currentBalance = startingBalance;
        UpdateSliderUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gc.isInArena) { return; }
        HandleDecay();
        UpdateSliderUI();
        DetectWinLoss();
    }

    public void ModifyBalance(float amountToAdd)
    {
        currentBalance += amountToAdd;
        UpdateSliderUI();
    }

    public void SetBalance(float newBalance)
    {
        currentBalance = newBalance;
        UpdateSliderUI();
    }

    private void DetectWinLoss()
    {
        if (currentBalance >= victoryAmount)
        {
            //handle victory;
            OnArenaVictory_TrueForPlayerWin?.Invoke(true);
            //sl.LoadEndingScene();
        }

        if (currentBalance <= defeatAmount)
        {
            //handle defeat;
            OnArenaVictory_TrueForPlayerWin?.Invoke(false);
            //sl.LoadEndingScene();
        }
    }

    private void HandleDecay()
    {
        currentBalance -= decayPerSecond * Time.deltaTime;
    }

    private void UpdateSliderUI()
    {
        victorySlider.value = currentBalance;
        float red = (victoryAmount - currentBalance)/victoryAmount;
        float green = currentBalance / victoryAmount;
        float blue = 0.1f;
        sliderFillImage.color = new Color(red, green, blue);
        
    }

    public void SetDecayAmount(float amount)
    {
        decayPerSecond = amount;
    }
}
