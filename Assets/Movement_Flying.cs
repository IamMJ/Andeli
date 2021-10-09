using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Flying : Movement
{
    //param
    float closeEnough = 0.3f;
    
    // State
    Vector2 movement_Flying;

    // Update is called once per frame

    protected override void Update()
    {
        if (gc.isPaused) { return; }
        if (anim)
        {
            UpdateRawDesMove();
            validDesMove = rawDesMove;
            HandleAnimation();
        }
    }

    private void FixedUpdate()
    {
        HandleFlyingMovement();

    }


    private void HandleFlyingMovement()
    {
        movement_Flying.x = Mathf.MoveTowards(movement_Flying.x, TacticalDestination.x, sk.CurrentSpeed * Time.deltaTime);
        movement_Flying.y = Mathf.MoveTowards(movement_Flying.y, TacticalDestination.y, sk.CurrentSpeed * Time.deltaTime);
        transform.position = movement_Flying;
    }

}
