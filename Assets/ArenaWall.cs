using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaWall : MonoBehaviour
{
    [SerializeField] Sprite[] spriteOptions = null;
    void Start()
    {
        PickSprite();
    }

    private void PickSprite()
    {
        int rand = UnityEngine.Random.Range(0, spriteOptions.Length);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = spriteOptions[rand];
        sr.flipX = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
    }

}
