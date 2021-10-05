using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSeeker : MonoBehaviour
{
    //init
    Rigidbody2D rb;
    VictoryMeter vm;
    GameController gc;

    //param
    float thrust = 30f;
    float closeEnough = 0.1f;
    float minTimeAlive = 1.0f;

    //state
    Transform target;
    Vector3 dir;
    float dist = 999;
    float speed;
    float powerPayload;
    TrueLetter.Ability spellType;
    float proximityTurnOnTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = rb.velocity.magnitude;

    }

    public void SetupSpell(Transform targetTransform, VictoryMeter vmRef, GameController gcRef, float payload, TrueLetter.Ability spellTypeIn)
    {
        target = targetTransform;
        vm = vmRef;
        powerPayload = payload;
        spellType = spellTypeIn;
        gc = gcRef;
        proximityTurnOnTime = Time.time + minTimeAlive;
    }

    private void Update()
    {
        if (!target)
        {
            Destroy(gameObject);
            return;
        }
        if (gc.isInArena == false)
        {
            Destroy(gameObject);
            return;
        }

        if (Time.time > proximityTurnOnTime)
        {
            TargetProximityCheck();
        }

    }

    private void FixedUpdate()
    {
        if (gc.isInArena == false)
        {
            Destroy(gameObject);
            return;
        }
        MoveForward();
    }
    private void MoveForward()
    {
        dir = target.position - transform.position;
        dist = dir.magnitude;

        rb.velocity = Vector2.MoveTowards(rb.velocity, dir.normalized * speed, thrust*Time.fixedDeltaTime);
    }

    private void TargetProximityCheck()
    {
        if (dist <= closeEnough)
        {
            HandleImpact();
            Destroy(gameObject);
        }
    }

    private void HandleImpact()
    {
        switch (spellType)
        {
            case TrueLetter.Ability.Normal:
                vm.ModifyBalanceAndCheckForArenaEnd(powerPayload);
                return;

            case TrueLetter.Ability.Frozen:
                target.GetComponent<SpeedKeeper>()?.ModifyCurrentSpeed(powerPayload);
                return;

            case TrueLetter.Ability.Wispy:
                target.GetComponent<SpeedKeeper>()?.ModifyCurrentSpeed(-1 * powerPayload);
                return;
        }
    }
}
