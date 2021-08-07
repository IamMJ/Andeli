﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellMaker : MonoBehaviour
{
    [SerializeField] GameObject puffPrefab = null;
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
        if (wv.CheckWordValidity(testWord, gameObject))
        {
            GameObject puff = Instantiate(puffPrefab, transform.position, Quaternion.identity) as GameObject;
            WordPuff wordPuff = puff.GetComponent<WordPuff>();
            wordPuff.SetText(testWord);
            wordPuff.SetColorByPower(pm.CurrentPower);

            vm.ModifyBalance(pm.CurrentPower);
        }
        else
        {
            Debug.Log("stun the player.");
        }



    }


}
