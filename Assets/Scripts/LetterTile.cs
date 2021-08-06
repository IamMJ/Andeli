using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LetterTile : MonoBehaviour
{
    //init
    public char Letter;
    public int Power;
    public float Lifetime;
    [SerializeField] SpriteRenderer sr = null;
    [SerializeField] MeshRenderer mr = null;
    [SerializeField] TextMeshPro tmp = null;
    LetterTileDropper letterTileDropper;

    //state
    float lifetimeRemaining;
    float factor;

    private void Start()
    {
        lifetimeRemaining = Lifetime;
    }

    private void Update()
    {
        lifetimeRemaining -= Time.deltaTime;
        
        if (lifetimeRemaining <= 0.5f * Lifetime)
        {
            factor = lifetimeRemaining / (0.5f * Lifetime);
            FadeRenderers(factor);
        }

        if (lifetimeRemaining <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void ToggleRenderers(bool shouldBeVisible)
    {
        if (shouldBeVisible)
        {
            sr.enabled = true;
            mr.enabled = true;
        }
        else
        {
            sr.enabled = false;
            mr.enabled = false;
        }
    }

    private void FadeRenderers(float alpha)
    {
        Color fadeColor = new Color(1, 1, 1, alpha);
        sr.color = fadeColor;
        tmp.color = fadeColor;
    }

    public void SetLetterTileDropper(LetterTileDropper ltd)
    {
        letterTileDropper = ltd;
    }

    private void OnDestroy()
    {
        letterTileDropper.RemoveLetterFromSpawnedLetterList(this);
    }
}
