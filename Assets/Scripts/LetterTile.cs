using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LetterTile : MonoBehaviour
{
    //init
    public char Letter;
    public int Power;
    public float StartingLifetime;
    public TrueLetter.Ability Ability = TrueLetter.Ability.Nothing;
    [SerializeField] SpriteRenderer sr = null;
    [SerializeField] MeshRenderer mr = null;
    [SerializeField] TextMeshPro tmp = null;
    LetterTileDropper letterTileDropper;

    //state
    public float LifetimeRemaining { get; private set; }
    float factor;


    private void Start()
    {
        LifetimeRemaining = StartingLifetime;
    }

    private void Update()
    {
        LifetimeRemaining -= Time.deltaTime;
        
        if (LifetimeRemaining <= 0.5f * StartingLifetime)
        {
            factor = LifetimeRemaining / (0.5f * StartingLifetime);
            FadeRenderers(factor);
        }

        if (LifetimeRemaining <= 0f)
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
