using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMemory : MonoBehaviour
{
    int consecutiveCompletedWords = 0;
    int totalCompletedWords = 0;

    public Action OnIncrementWordCount;
    public Action OnResetConsecutiveWordCount;

    public void IncrementWordCount()
    {
        consecutiveCompletedWords++;
        totalCompletedWords++;
        OnIncrementWordCount?.Invoke();
    }

    public void ResetConsecutiveWordCount()
    {
        consecutiveCompletedWords = 0;
        OnResetConsecutiveWordCount?.Invoke();
    }


}
