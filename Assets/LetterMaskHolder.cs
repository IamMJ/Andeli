using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterMaskHolder : MonoBehaviour
{
    //[SerializeField] List<LetterMaskOld> letterMasks = new List<LetterMaskOld>();
    [SerializeField] List<LetterMask> letterMasks = new List<LetterMask>();


    //state
    public int numberOfLetterMasks = 0;

    Librarian lib;

    private void Start()
    {
        lib = Librarian.GetLibrarian();
        GenerateInitialLetterMaskStructs();
        ApplySpecialLetterMasksStructs();
    }
    private void GenerateInitialLetterMaskStructs()
    {
        LetterTileDropper ltd = lib.letterTileDropper;
        List<TrueLetter> trueLettersList = ltd.GetAllTrueLetters();

        foreach (var trueLetter in trueLettersList)
        {
            LetterMask newLetterMask = new LetterMask();
            newLetterMask.letter = trueLetter.GetLetter();
            newLetterMask.rarity = trueLetter.GetRarity();
            newLetterMask.ability = TrueLetter.Ability.Normal;
            letterMasks.Add(newLetterMask);
        }
        numberOfLetterMasks = letterMasks.Count;
    }
    private void ApplySpecialLetterMasksStructs()
    {
        //use either the player character or the enemy profile's scriptable object to modify
        //certain Letter Masks 
    }

    public List<LetterMask> GetLetterMasks()
    {
        return letterMasks;
    }

    public LetterMask GetLetterMaskForTrueLetter(char targetLetter)
    {
        LetterMask targetLetterMask = null;

        foreach (var letterMask in letterMasks)
        {
            if (letterMask.letter == targetLetter)
            {
                targetLetterMask = letterMask;
                break;
            }
        }

        return targetLetterMask;
    }

    //GetLetterMaskStructForTrueLetterChar

    public void ModifyLetterMaskAbilityForGivenLetter(char givenLetter, TrueLetter.Ability newAbility)
    {
        for (int i = 0; i < letterMasks.Count; i++)
        {
            if (letterMasks[i].letter == givenLetter)
            {
                letterMasks[i].ability = newAbility;
                break;
            }
        }
    }
}
