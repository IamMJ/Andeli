using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterMaskHolder : MonoBehaviour
{
    [SerializeField] List<LetterMask> letterMasks = new List<LetterMask>();

    private void Start()
    {
        //ResetLetterAbilities();
    }

    private void ResetLetterAbilities()
    {
        //TODO have a way for this not need to occur each time. Save the state of each letter mask separately?
        foreach(var elem in letterMasks)
        {
            elem.SetAbility(TrueLetter.Ability.Normal);
        }
    }

    public List<LetterMask> GetLetterMods()
    {
        return letterMasks;
    }

    public LetterMask GetLetterMaskForTrueLetter(TrueLetter targetLetter)
    {
        LetterMask targetLetterMask = null;

        foreach (var letterMask in letterMasks)
        {
            if (letterMask.AssociatedTrueLetter == targetLetter)
            {
                targetLetterMask = letterMask;
                break;
            }
        }

        return targetLetterMask;
    }

    public void ModifyLetterMaskAbilityForGivenLetter(char givenLetter, TrueLetter.Ability newAbility)
    {
        for (int i = 0; i < letterMasks.Count; i++)
        {
            if (letterMasks[i].GetLetter() == givenLetter)
            {
                letterMasks[i].SetAbility(newAbility);
                break;
            }
        }
    }
}
