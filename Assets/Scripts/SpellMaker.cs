using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellMaker : MonoBehaviour
{
    [SerializeField] GameObject spellPrefab = null;
    WordBuilder wbd;
    DebugHelper dh;
    WordValidater wv;
    PowerMeter pm;
    VictoryMeter vm;
    string testWord;

    // Start is called before the first frame update
    void Start()
    {
        dh = FindObjectOfType<DebugHelper>();
        wbd = FindObjectOfType<WordBuilder>();
        wv = FindObjectOfType<WordValidater>();
        pm = FindObjectOfType<PowerMeter>();
        vm = FindObjectOfType<VictoryMeter>();
    }


    public void FireCurrentWord()
    {
        testWord = wbd.GetCurrentWord();
        if (wv.CheckWordValidity(testWord))
        {
            Debug.Log("Fire off the word!");
            vm.ModifyBalance(pm.CurrentPower);
        }
        else
        {
            Debug.Log("stun the player.");
        }



    }


}
