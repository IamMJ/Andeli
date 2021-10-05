using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedKeeper : MonoBehaviour
{
    //param
    float startingSpeed = 3f;
    float accelPerValidWord = 0.3f;
    float minSpeed = 1.0f;
    float maxSpeed = 6.0f;
    float speedRecoveryRate = 0.1f; // speed recovered every second

    //state
    float stunTimeRemaining = 0;
    public float targetCurrentSpeed;

    public float CurrentSpeed; //{ get; private set; } = 0;


    private void Start()
    {
        //WordMakerMemory pm = GetComponent<WordMakerMemory>();
        //if (pm)
        //{
        //    pm.OnResetConsecutiveWordCount += ResetToStartingSpeedOnInvalidWord;
        //    pm.OnIncrementWordCount += IncreaseSpeedOnValidWord;
        //}

        targetCurrentSpeed = startingSpeed;
        CurrentSpeed = targetCurrentSpeed;
    }

    private void Update()
    {
        CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, targetCurrentSpeed, speedRecoveryRate * Time.deltaTime);
        CurrentSpeed = Mathf.Clamp(CurrentSpeed, minSpeed, maxSpeed);
    }

    //private void IncreaseSpeedOnValidWord()
    //{
    //    CurrentSpeed += accelPerValidWord;
    //    CurrentSpeed = Mathf.Clamp(CurrentSpeed, startingSpeed, maxSpeed);
    //}

    //private void ResetToStartingSpeedOnInvalidWord()
    //{
    //    CurrentSpeed = startingSpeed;
    //}

    public void StunPlayer()
    {

    }

    //private void OnDestroy()
    //{
    //    WordMakerMemory pm = GetComponent<WordMakerMemory>();
    //    if (pm)
    //    {
    //        pm.OnResetConsecutiveWordCount -= ResetToStartingSpeedOnInvalidWord;
    //        pm.OnIncrementWordCount -= IncreaseSpeedOnValidWord;
    //    }
    //}

    public void ModifyCurrentSpeed(float freezePenalty)
    {
        //Debug.Log($"{gameObject.name} just was slowed by {freezePenalty}");
        CurrentSpeed -= freezePenalty;
    }
}
