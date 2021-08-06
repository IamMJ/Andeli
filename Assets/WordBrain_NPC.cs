using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordBrain_NPC : MonoBehaviour
{
    //init
    MoveBrain_NPC mb;
    LetterTile targetLetterTile;
    WordValidater wv;
    LetterTileDropper ltd;

    //state
    string currentWord = "";
    int currentPower = 0;

    void Start()
    {
        wv = FindObjectOfType<WordValidater>();
        mb = GetComponent<MoveBrain_NPC>();
        ltd = FindObjectOfType<LetterTileDropper>();
        ltd.OnLetterListModified += DetermineBestTargetLetter;
    }

    private void AddLetter(char newLetter)
    {
        currentWord += newLetter;
    }
    private void ClearCurrentWord()
    {
        currentWord = "";
    }
    private void IncreasePower(int amount)
    {
        currentPower += amount;
    }

    private void ClearPower()
    {
        currentPower = 0;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        LetterTile letterTile;
        if (collision.gameObject.TryGetComponent<LetterTile>(out letterTile))
        {
            AddLetter(letterTile.Letter);
            IncreasePower(letterTile.Power);
            Destroy(collision.gameObject);
            DetermineBestTargetLetter(null, false) ;
        }

    }

    private void DetermineBestTargetLetter(LetterTile changedLetterTile, bool wasLetterAdded)
    {
        if (changedLetterTile == null)
        {
            targetLetterTile = null;
            return;
        }
        if (!wasLetterAdded && targetLetterTile == changedLetterTile) //if a letter was removed, and it was the target letter...
        {
            // come up with a new target letter

            return;
        }
        if (wasLetterAdded) //Since a new letter was added...
        {
            // Decide if the new letter is a better choice for target letter
            if (currentWord.Length == 0)
            {
                targetLetterTile = changedLetterTile;
                return;
            }
            else
            {

            }
        }
    }

    private void OnDestroy()
    {
        ltd.OnLetterListModified -= DetermineBestTargetLetter;
    }
}
