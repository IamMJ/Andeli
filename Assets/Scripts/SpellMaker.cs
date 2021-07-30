using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellMaker : MonoBehaviour
{
    [SerializeField] GameObject spellPrefab = null;
    WordBuilder wbd;
    DebugHelper dh;
    WordValidater wv;
    string testWord;

    // Start is called before the first frame update
    void Start()
    {
        dh = FindObjectOfType<DebugHelper>();
        wbd = FindObjectOfType<WordBuilder>();
        wv = FindObjectOfType<WordValidater>();
    }


    public void FireCurrentWord()
    {
        testWord = wbd.GetCurrentWord();
        if (wv.CheckWordValidity(testWord))
        {
            Debug.Log("Fire off the word!");
        }
        else
        {
            Debug.Log("stun the player.");
        }



    }


}
