using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedKeeper : MonoBehaviour
{
    GameController gc;
    PlayerMemory pm;

    //param
    float accelPerValidWord = 0.3f;
    float minSpeed = 0f;
    float maxSpeed = 6.0f;
    float speedRecoveryRate = 0.1f; // speed recovered every second

    //state
    float stunTimeRemaining = 0;
    [SerializeField] float targetCurrentSpeed = 2f;

    public float CurrentSpeed; //{ get; private set; } = 0;

    bool isPlayer = false;

    private void Start()
    {
        gc = FindObjectOfType<GameController>();
        if (GetComponent<PlayerInput>())
        {
            isPlayer = true;
            pm = GetComponent<PlayerMemory>();
            pm.SetBaseMoveSpeed(targetCurrentSpeed);
        }
        //WordMakerMemory pm = GetComponent<WordMakerMemory>();
        //if (pm)
        //{
        //    pm.OnResetConsecutiveWordCount += ResetToStartingSpeedOnInvalidWord;
        //    pm.OnIncrementWordCount += IncreaseSpeedOnValidWord;
        //}

        ResetSpeedStats();
    }

    public void ResetSpeedStats()
    {
        if (isPlayer)
        {
            targetCurrentSpeed = pm.GetBaseMoveSpeed();
        }


        CurrentSpeed = targetCurrentSpeed;
    }

    private void Update()
    {
        if (!gc.isInArena) { return; }
        CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, targetCurrentSpeed, speedRecoveryRate * Time.deltaTime);
        CurrentSpeed = Mathf.Clamp(CurrentSpeed, minSpeed, maxSpeed);
    }

    public float GetStartingSpeed()
    {
        return targetCurrentSpeed;
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

    public void ModifyTargetSpeed(float multiplier)
    {
        targetCurrentSpeed *= multiplier;
        CurrentSpeed = targetCurrentSpeed;
    }
}
