﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSeeker : MonoBehaviour
{
    //init
    Rigidbody2D rb;
    VictoryMeter vm;

    //param
    float thrust = 30f;
    float closeEnough = 0.1f;

    //state
    Transform target;
    Vector3 dir;
    float dist = 999;
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
        if (!target)
        {
            Debug.Log("destroying due to no target");
            Destroy(gameObject);
        }

        TargetProximityCheck();
    }

    private void FixedUpdate()
    {
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
            Debug.Log($"destroying because dist between target {target.position} and here {transform.position} is {dist}.");
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
