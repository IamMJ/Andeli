using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveArrow : MonoBehaviour
{
    [SerializeField] Sprite arrow_up = null;
    [SerializeField] Sprite arrow_down = null;
    [SerializeField] Sprite arrow_left = null;
    [SerializeField] Sprite arrow_right = null;
    public enum MoveDirection { Up, Down, Left, Right };
    SpriteRenderer sr;

    //param
    float lifetime = 1.0f;

    //state
    public MoveDirection Direction = MoveDirection.Up;
    float timeSinceStarted = 0;
    float factor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        SetCorrectArrowOrientation();
    }

    public void SetCorrectArrowOrientation()
    {
        switch (Direction)
        {
            case MoveDirection.Up:
                sr.sprite = arrow_up;
                return;

            case MoveDirection.Down:
                sr.sprite = arrow_down;
                return;

            case MoveDirection.Left:
                sr.sprite = arrow_left;
                return;

            case MoveDirection.Right:
                sr.sprite = arrow_right;
                return;


        }
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceStarted += Time.deltaTime;
        factor = (lifetime - timeSinceStarted) / lifetime;
        sr.color = new Color(1, 1, 1, factor);
        if (timeSinceStarted > lifetime)
        {
            Destroy(gameObject);
        }
    }


   
}
