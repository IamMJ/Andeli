using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSeeker : MonoBehaviour
{
    //init
    Rigidbody2D rb;

    //param
    float thrust = 0.3f;
    float closeEnough = 0.5f;

    //state
    Transform target;
    Vector3 trueRotation = Vector3.zero;
    Vector2 dir;
    float dist;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    private void Update()
    {
        MoveForward();
        CheckForTarget();
    }

    private void MoveForward()
    {
        dir = transform.position - target.position;
        dist = dir.magnitude;
        rb.AddForce(dir.normalized * -1 * thrust, ForceMode2D.Impulse);
    }

    private void CheckForTarget()
    {
        if (dist <= closeEnough)
        {
            Destroy(gameObject);
        }
    }
}
