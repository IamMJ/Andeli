using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterMaskHolder : MonoBehaviour
{
    [SerializeField] List<LetterMask> letterMasks = new List<LetterMask>();


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
}
