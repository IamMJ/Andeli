using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WordMakerMemory : MonoBehaviour
{
    int consecutiveCompletedWords = 0;
    int totalCompletedWords = 0;

    public Action OnIncrementWordCount;
    public Action OnResetConsecutiveWordCount;

    public struct ArenaData
    {
        public int powerDealtByPlayer;
        public int wordsSpelledByPlayer;
        public string bestWordSpelledByPlayer;
        public int currentBestSinglePowerGain;

        public ArenaData(int power, int wordCount, string longestWord)
        {
            powerDealtByPlayer = power;
            wordsSpelledByPlayer = wordCount;
            bestWordSpelledByPlayer = longestWord;
            currentBestSinglePowerGain = 0;
        }
    }

    //state

    Dictionary<string, int> wordsAndCountsThisArena = new Dictionary<string, int>();
    ArenaData currentArenaData = new ArenaData();

    public void UpdateCurrentArenaData(int powerDealtIncrease, string word)
    {
        currentArenaData.powerDealtByPlayer += powerDealtIncrease;
        currentArenaData.wordsSpelledByPlayer ++;
        if (powerDealtIncrease > currentArenaData.currentBestSinglePowerGain)
        {
            currentArenaData.bestWordSpelledByPlayer = word;
            currentArenaData.currentBestSinglePowerGain = powerDealtIncrease;
        }

    }

    public ArenaData GetCurrentArenaData()
    {
        return currentArenaData;
    }

    public void ResetCurrentArenaData()
    {
        currentArenaData.powerDealtByPlayer = 0;
        currentArenaData.wordsSpelledByPlayer = 0;
        currentArenaData.bestWordSpelledByPlayer = "";
        currentArenaData.currentBestSinglePowerGain = 0;
        wordsAndCountsThisArena.Clear();
    }
}
