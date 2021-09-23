using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterModHolder : MonoBehaviour
{
    [SerializeField] List<PlayerLetterMod> lettersModifiers = new List<PlayerLetterMod>();


    public List<PlayerLetterMod> GetLetterMods()
    {
        return lettersModifiers;
    }
}
