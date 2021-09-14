using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSeeker : MonoBehaviour
{
    //init
    Rigidbody2D rb;
    VictoryMeter vm;

    //param
    float thrust = 0.3f;
    float closeEnough = 0.5f;

    //state
    Transform target;
    Vector3 trueRotation = Vector3.zero;
    Vector2 dir;
    float dist;
    float speed;
    float powerPayload;
    TrueLetter.Ability spellType;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = rb.velocity.magnitude;
    }

    public void SetupSpell(Transform targetTransform, VictoryMeter vmRef, float payload, TrueLetter.Ability spellTypeIn)
    {
        target = targetTransform;
        vm = vmRef;
        powerPayload = payload;
        spellType = spellTypeIn;
    }

    private void Update()
    {
        MoveForward();
        TargetProximityCheck();
    }

    private void MoveForward()
    {
        dir = target.position - transform.position;
        dist = dir.magnitude;

        rb.velocity = Vector2.MoveTowards(rb.velocity, dir.normalized * speed, thrust);
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
                target.GetComponent<SpeedKeeper>()?.FreezeWordMaker(powerPayload);
                return;
        }
    }
}
