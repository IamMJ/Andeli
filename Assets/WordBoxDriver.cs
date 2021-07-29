using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class WordBoxDriver : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI wordBoxTMP = null;
    [SerializeField] Slider wordEraseSliderBG = null;
    [SerializeField] Slider wordFiringSliderBG = null;
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
    }

    public void ClearOutWordBox()
    {
        Debug.Log("Clear out word box");
        currentWord = "";
        wordBoxTMP.text = currentWord;
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
