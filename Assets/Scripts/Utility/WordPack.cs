using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct WordPack
{
    public Sprite[] letterSprites;
    public Color[] letterColors;
    public string[] letterLetters;
    public string Word;
    public int Power;
    public int ModifiedWordLength;
    public bool IsValid;

    public WordPack(int wordLength, int power, string word, int modifiedWordLength, bool validity)
    {
        letterSprites = new Sprite[wordLength];
        letterColors = new Color[wordLength];
        letterLetters = new string[wordLength];
        Power = power;
        Word = word;
        ModifiedWordLength = modifiedWordLength;
        IsValid = validity;
    }
}

