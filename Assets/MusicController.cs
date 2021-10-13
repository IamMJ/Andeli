using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicController : MonoBehaviour
{
    [SerializeField] AudioClip windLoop = null;
    [SerializeField] AudioClip mainTheme = null;
    [SerializeField] AudioSource auso_pri = null;
    [SerializeField] AudioSource auso_aux = null;
    GameController gc;

    void Start()
    {
        gc = GetComponent<GameController>();
        auso_pri.clip = mainTheme;
        auso_pri.loop = true;
        auso_pri.Play();
        auso_aux.clip = windLoop;
        auso_aux.loop = true;
        auso_aux.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeMainThemeWithZoom(float factor)
    {
        auso_pri.volume = (Mathf.Clamp(factor, 0.5f, 1));
        auso_aux.volume = factor;
    }
}
