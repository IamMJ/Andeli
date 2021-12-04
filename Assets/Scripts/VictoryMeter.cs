using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class VictoryMeter : MonoBehaviour
{
    ////init
    //[SerializeField] Slider victorySlider = null;
    //SceneLoader sl;
    //GameController gc;
    //CinemachineImpulseSource cis;
    //[SerializeField] Image sliderFillImage = null;

    ////param
    //float victoryAmount = 50f;
    //float defeatAmount = 0f;
    //float startingBalance = 25;
    //float decayPerSecond = 0f;




    ////state
    //float currentBalance;
    //void Start()
    //{
    //    gc = FindObjectOfType<GameController>();
    //    sl = FindObjectOfType<SceneLoader>();
    //    victorySlider.maxValue = victoryAmount;
    //    victorySlider.minValue = defeatAmount;
    //    currentBalance = startingBalance;
    //    cis = Camera.main.GetComponentInChildren<CinemachineImpulseSource>();
    //    UpdateSliderUI();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (!gc.isInArena) { return; }
    //    HandleDecay();
    //    UpdateSliderUI();

    //}

    //public void ResetArena()
    //{
    //    SetBalance(startingBalance);
    //    SetDecayAmount(0);
    //}
    //public void ResetArena(int newStartingBalance)
    //{
    //    SetBalance(newStartingBalance);
    //    SetDecayAmount(0);
    //}
    ////public bool ModifyBalanceAndCheckForArenaEnd(float amountToAdd)
    ////{
    ////    currentBalance += amountToAdd;
    ////    if (amountToAdd < 0)
    ////    {
    ////        cis.GenerateImpulse(Mathf.Abs(amountToAdd));
    ////    }

    ////    bool isOver = DetectWinLoss();
    ////    if (!isOver)
    ////    {
    ////        UpdateSliderUI();
    ////        return false; ;
    ////    }
    ////    else
    ////    {
    ////        return true;
    ////    }
    ////}

    //public void SetBalance(float newBalance)
    //{
    //    currentBalance = newBalance;
    //    UpdateSliderUI();
    //}

    

    //private void HandleDecay()
    //{
    //    currentBalance -= decayPerSecond * Time.deltaTime;
    //}

    //private void UpdateSliderUI()
    //{
    //    victorySlider.value = currentBalance;
    //    float red = (victoryAmount - currentBalance)/victoryAmount;
    //    float green = currentBalance / victoryAmount;
    //    float blue = 0.1f;
    //    sliderFillImage.color = new Color(red, green, blue);
        
    //}

    //public void SetDecayAmount(float amount)
    //{
    //    decayPerSecond = amount;
    //}
}
