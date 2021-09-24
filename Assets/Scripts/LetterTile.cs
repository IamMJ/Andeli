using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pathfinding;
using System;

public class LetterTile : MonoBehaviour
{
    //init
    public char Letter;
    public int Power;
    public float StartingLifetime;
    public TrueLetter.Ability Ability = TrueLetter.Ability.Normal;
    [SerializeField] SpriteRenderer sr = null;
    [SerializeField] MeshRenderer mr = null;
    [SerializeField] TextMeshPro tmp = null;
    LetterTileDropShadow assignedShadow;
    LetterTileDropper letterTileDropper;

    [SerializeField] Sprite NormalTileSprite = null;
    [SerializeField] Color NormalTileColor = Color.white;
    [SerializeField] Sprite LuckyTileSprite = null;
    [SerializeField] Color LuckyTileColor = Color.green;
    [SerializeField] Sprite FrozenTileSprite = null;
    [SerializeField] Color FrozenTileColor = Color.white;
    [SerializeField] Sprite ShinyTileSprite = null;
    [SerializeField] Color ShinyTileColor = Color.white;

    Color fadeColor_sr;
    Color fadeColor_mr;

    public struct SpriteColor
    {
        public Sprite Sprite;
        public Color Color;
        public SpriteColor(Sprite sprite, Color color)
        {
            Sprite = sprite;
            Color = color;
        }

    }

    //param
    float fallSpeed = 4.0f;

    //state
    public float LifetimeRemaining { get; private set; }
    float factor;
    bool isLatentAbilityActivated = false;
    float remainingFallDistance;
    bool isFalling = true;
    public bool IsInactivated { get; private set; } = false;


    private void Start()
    {
        fallSpeed += UnityEngine.Random.Range(-1f, 1f);
        LifetimeRemaining = StartingLifetime;
        gameObject.layer = 0;
        sr.sortingLayerName = "Actors";
        sr.sortingOrder = 9;
        tmp.sortingLayerID = sr.sortingLayerID;
        tmp.sortingOrder = sr.sortingOrder + 1;
        AssignStartingSprite();
        fadeColor_sr = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
        fadeColor_mr = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 1);

    }

    private void AssignStartingSprite()
    {
        SpriteColor sc = GetSpriteColorFromAbility(Ability);
        sr.sprite = sc.Sprite;
        sr.color = sc.Color;

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
                transform.position = GridHelper.SnapToGrid(transform.position, 1);
                if (assignedShadow)
                {
                    assignedShadow.RemoveShadow();
                }
                GridModifier.UnknitAllGridGraphs(transform);
                letterTileDropper.AddLetterToSpawnedLetterList(this);

            }
            return;
        }

        LifetimeRemaining -= Time.deltaTime;

        if (LifetimeRemaining <= 0.5f * StartingLifetime)
        {
            factor = LifetimeRemaining / (0.5f * StartingLifetime);
            FadeRenderers(factor);
        }

        if (!IsInactivated && LifetimeRemaining <= 0f)
        {
            DestroyLetterTile();
        }
               
    }


    #region Appearance
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
        fadeColor_sr.a = alpha;
        fadeColor_mr.a = alpha;
        sr.color = fadeColor_sr;
        tmp.color = fadeColor_mr;
    }

    public void SetLetterTileDropper(LetterTileDropper ltd)
    {
        letterTileDropper = ltd;
    }

    #endregion

    #region Public Methods
    public void InactivateLetterTile()
    {
        IsInactivated = true;
        GetComponent<Collider2D>().enabled = false;
        sr.enabled = false;
        GetComponentInChildren<TextMeshPro>().enabled = false;

        letterTileDropper.RemoveLetterFromSpawnedLetterList(this);

        if (assignedShadow)
        {
            assignedShadow.RemoveShadow();
        }
        GridModifier.ReknitAllGridGraphs(transform);

    }

    public void DestroyLetterTile()
    {
        InactivateLetterTile();
        letterTileDropper.RemoveLetterFromAllLettersSpawnedList(this);
        Destroy(gameObject);
    }

    public bool GetLatentAbilityStatus()
    {
        return isLatentAbilityActivated;
    }

    public void SetLatentAbilityStatus(bool shouldBeActivated)
    {
        isLatentAbilityActivated = shouldBeActivated;
    }

    public void SetFallDistance(float amount)
    {
        remainingFallDistance = amount;
    }

    public void AssignShadow(LetterTileDropShadow shadow)
    {
        assignedShadow = shadow;
    }

    public SpriteColor GetSpriteColorFromAbility(TrueLetter.Ability ability)
    {
        switch (ability)
        {
            case TrueLetter.Ability.Normal:
                SpriteColor sc_Normal = new SpriteColor(NormalTileSprite, NormalTileColor);
                return sc_Normal;

            case TrueLetter.Ability.Lucky:
                SpriteColor sc_Lucky = new SpriteColor(LuckyTileSprite, LuckyTileColor);
                return sc_Lucky;

            case TrueLetter.Ability.Frozen:
                SpriteColor sc_Frozen = new SpriteColor(FrozenTileSprite, FrozenTileColor);
                return sc_Frozen;

            case TrueLetter.Ability.Shiny:
                SpriteColor sc_Shiny = new SpriteColor(ShinyTileSprite, ShinyTileColor);
                return sc_Shiny;

            default:
                SpriteColor sc_Default = new SpriteColor(NormalTileSprite, NormalTileColor);
                return sc_Default;
        }
    }

    #endregion

}
