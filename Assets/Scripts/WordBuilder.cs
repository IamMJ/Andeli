using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class WordBuilder : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI wordBoxTMP = null;
    [SerializeField] Slider wordEraseSliderBG = null;
    [SerializeField] Slider wordFiringSliderBG = null;

    //state
    public bool HasLetters { get; private set; } = false;
    string currentWord;

    //state
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddLetter(char newLetter)
    {
        //Debug.Log("adding: " + newLetter);
        currentWord += newLetter;
        //Debug.Log("Current word: " + currentWord);
        wordBoxTMP.text = currentWord;
        HasLetters = true;
    }

    public string GetCurrentWord()
    {
        return currentWord;
    }

    public void ClearOutWordBox()
    {
        Debug.Log("Clear out word box");
        currentWord = "";
        wordBoxTMP.text = currentWord;
        HasLetters = false;
    }

    public void FillWordEraseSlider(float amount)
    {
        wordEraseSliderBG.value = amount;
    }

    public void ClearWordEraseSlider()
    {
        wordEraseSliderBG.value = 0f;
    }

    public void FillWordFiringSlider(float amount)
    {
        wordFiringSliderBG.value = amount;
    }

    public void ClearWordFiringSlider()
    {
        wordFiringSliderBG.value = 0;
    }
}
