using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterPowerMenuDriver : MonoBehaviour
{
    GameController gc;
    SceneLoader sl;
    VictoryMeter vm;
    List<PlayerLetterMod> letters = new List<PlayerLetterMod>();
    string topBand = "ACEGIKMOQSUWY";
    string bottomBand = "BDFHJLNPRTVXZ";
    [SerializeField] List<PlayerLetterMod> topBandLetters = new List<PlayerLetterMod>();
    [SerializeField] List<PlayerLetterMod> bottomBandLetters = new List<PlayerLetterMod>();
    private void Start()
    {
        gc = FindObjectOfType<GameController>();
        letters = gc.GetPlayer().GetComponent<LetterModHolder>().GetLetterMods();
        foreach(var letter in letters)
        {
            if (topBand.Contains(letter.GetLetter().ToString()))
            {
                topBandLetters.Add(letter);
            }
            else
            {
                bottomBandLetters.Add(letter);
            }
        }
    }

    public void HideMenu()
    {
        if (!gc)
        {
            gc = FindObjectOfType<GameController>();
        }
        gc.ResumeGameSpeed();
        gameObject.SetActive(false);
    }

    public void SelectLetterToInspect(string letter)
    {

        //Do stuff
    }

    public void ScrollLettersLeft()
    {

    }

    public void ScrollLettersRight()
    {

    }
}
