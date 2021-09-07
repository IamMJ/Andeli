using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustCloud : MonoBehaviour
{
    //init
    SpriteRenderer sr;

    //state
    float life;
    float lifespan;


    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        lifespan = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        life = lifespan;
        Destroy(gameObject, lifespan);   
    }

    private void Update()
    {
        life -= Time.deltaTime;
        sr.color = new Color(1, 1, 1, life/lifespan);
    }

}
