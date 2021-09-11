using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class ArenaWall : MonoBehaviour
{
    [SerializeField] Sprite[] spriteOptions = null;
    GraphUpdateScene gus;
    void Start()
    {
        PickSprite();
        gus = GetComponent<GraphUpdateScene>();
    }

    private void PickSprite()
    {
        int rand = UnityEngine.Random.Range(0, spriteOptions.Length);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = spriteOptions[rand];
        sr.flipX = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
    }


    public void RemoveArenaWall()
    {
        gus.setWalkability = true;
        gus.Apply();
        Destroy(gameObject);
    }
    

}
