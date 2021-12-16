using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WordMakerMemory : MonoBehaviour
{
    int consecutiveCompletedWords = 0;
    int totalCompletedWords = 0;
    public int currentArenaCompletedWords_Debug = 0;

    public Action OnIncrementWordCount;
    public Action OnResetConsecutiveWordCount;

    //state
    bool shouldAllowRepeatWords = true;

    public struct ArenaData
    {
        public int powerDealt;
        public int wordsSpelled;
        public string bestWordSpelled;
        public int currentBestSinglePowerGain;
        public List<string> playedWords;

        public ArenaData(int power, int wordCount, string longestWord)
        {
            powerDealt = power;
            wordsSpelled = wordCount;
            bestWordSpelled = longestWord;
            currentBestSinglePowerGain = 0;
            playedWords = new List<string>();
        }
    }

    //state
    ArenaData currentArenaData = new ArenaData(0,0, "");

    public void UpdateCurrentArenaData(int powerDealtIncrease, string word)
    {
        currentArenaData.powerDealt += powerDealtIncrease;
        currentArenaData.wordsSpelled ++;
        currentArenaCompletedWords_Debug++; //This is just for debugging.
        currentArenaData.playedWords.Add(word);
        if (powerDealtIncrease > currentArenaData.currentBestSinglePowerGain)
        {
            currentArenaData.bestWordSpelled = word;
            currentArenaData.currentBestSinglePowerGain = powerDealtIncrease;
        }

    }

    public ArenaData GetCurrentArenaData()
    {
        return currentArenaData;
    }

    public void ResetCurrentArenaData()
    {
        currentArenaData.powerDealt = 0;
        currentArenaData.wordsSpelled = 0;
        currentArenaData.bestWordSpelled = "";
        currentArenaData.currentBestSinglePowerGain = 0;
        currentArenaData.playedWords.Clear();
        currentArenaCompletedWords_Debug = 0; //This is just for debugging.
    }

    public bool CheckIfWordHasBeenPlayedByPlayerAlready(string testWord)
    {
        if (currentArenaData.playedWords.Contains(testWord))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #region Public Arena Parameter Setting

    public void SetupArenaParameters_AllowRepeatWords(bool shouldAllowRepeatWords)
    {
        this.shouldAllowRepeatWords = shouldAllowRepeatWords;
    }

    #endregion
}
