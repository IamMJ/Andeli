using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    //param
    float moveSpeed = 4f;
    float closeEnough = 0.5f;

    //state
    Vector2 movement;
    Vector2 targetDest;
    void Start()
    {
        targetDest = GridHelper.GetRandomPositionOnWorldMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (((Vector3)targetDest - transform.position).magnitude <= closeEnough)
        {
            targetDest = GridHelper.GetRandomPositionOnWorldMap();
        }


    }

    private void FixedUpdate()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        movement.x = Mathf.MoveTowards(movement.x, targetDest.x, moveSpeed * Time.deltaTime);
        movement.y = Mathf.MoveTowards(movement.y, targetDest.y, moveSpeed * Time.deltaTime);
        transform.position = movement;
    }
}
