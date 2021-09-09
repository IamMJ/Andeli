﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pathfinding;

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
    LetterTileDropShadow assignedShadow;
    LetterTileDropper letterTileDropper;

    //param
    float fallSpeed = 4.0f;

    //state
    public float LifetimeRemaining { get; private set; }
    float factor;
    bool isLatentAbilityActivated = false;
    float remainingFallDistance;
    bool isFalling = true;


    private void Start()
    {
        fallSpeed += UnityEngine.Random.Range(-1f, 1f);
        LifetimeRemaining = StartingLifetime;
        gameObject.layer = 0;
        sr.sortingLayerName = "Actors";
        sr.sortingOrder = 9;
        tmp.sortingLayerID = sr.sortingLayerID;
        tmp.sortingOrder = sr.sortingOrder + 1;
    }

    private void Update()
    {
        if (isFalling)
        {
            float amount = fallSpeed * Time.deltaTime;
            transform.position -= Vector3.up * amount;
            remainingFallDistance -= amount;
            if (remainingFallDistance <= 0)
            {
                isFalling = false;
                gameObject.layer = 9;
                sr.sortingLayerName = "Letters";
                sr.sortingOrder = 0;
                tmp.sortingLayerID = sr.sortingLayerID;
                tmp.sortingOrder = sr.sortingOrder + 1;
                if (assignedShadow)
                {
                    assignedShadow.RemoveShadow();
                }
            }
            return;
        }

        LifetimeRemaining -= Time.deltaTime;

        if (LifetimeRemaining <= 0.5f * StartingLifetime)
        {
            factor = LifetimeRemaining / (0.5f * StartingLifetime);
            FadeRenderers(factor);
        }

        if (LifetimeRemaining <= 0f)
        {
            ReknitGridGraph();
            Destroy(gameObject);
        }
               
    }

    public void ReknitGridGraph()
    {
        GraphUpdateScene gus = GetComponent<GraphUpdateScene>();
        gus.setWalkability = true;
        gus.Apply();
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

    public bool GetLatentAbilityStatus()
    {
        return isLatentAbilityActivated;
    }

    public void SetLatentAbilityStatus(bool newStatus)
    {
        isLatentAbilityActivated = newStatus;
    }

    public void SetFallDistance(float amount)
    {
        remainingFallDistance = amount;
    }

    public void AssignShadow(LetterTileDropShadow shadow)
    {
        assignedShadow = shadow;
    }

}
