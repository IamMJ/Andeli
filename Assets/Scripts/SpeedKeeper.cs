using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedKeeper : MonoBehaviour
{
    //param
    float startingSpeed = 1.7f;
    float accelPerValidWord = 0.3f;
    float maxSpeed = 6.0f;

    //state
    float stunTimeRemaining = 0;
    public float CurrentSpeed { get; private set; } = 0;



    private void Start()
    {
        PlayerMemory pm = GetComponent<PlayerMemory>();
        pm.OnResetConsecutiveWordCount += ResetToStartingSpeedOnInvalidWord;
        pm.OnIncrementWordCount += IncreaseSpeedOnValidWord;
        CurrentSpeed = startingSpeed;
    }

    private void IncreaseSpeedOnValidWord()
    {
        CurrentSpeed += accelPerValidWord;
        Debug.Log("new speed: " + CurrentSpeed);
        CurrentSpeed = Mathf.Clamp(CurrentSpeed, startingSpeed, maxSpeed);
        Debug.Log("clamped new speed: " + CurrentSpeed);
    }

    private void ResetToStartingSpeedOnInvalidWord()
    {
        CurrentSpeed = startingSpeed;
        Debug.Log("reset speed to: " + CurrentSpeed);
    }

    public void StunPlayer()
    {

    }

    private void OnDestroy()
    {
        PlayerMemory pm = GetComponent<PlayerMemory>();
        pm.OnResetConsecutiveWordCount -= ResetToStartingSpeedOnInvalidWord;
        pm.OnIncrementWordCount -= IncreaseSpeedOnValidWord;
    }
}
