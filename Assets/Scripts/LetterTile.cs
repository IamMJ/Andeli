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
    [SerializeField] float NormalYMod = 0;
    [SerializeField] Sprite LuckyTileSprite = null;
    [SerializeField] Color LuckyTileColor = Color.green;
    [SerializeField] float LuckyYMod = 0;
    [SerializeField] Sprite FrozenTileSprite = null;
    [SerializeField] Color FrozenTileColor = Color.white;
    [SerializeField] float FrozenYMod = 0;
    [SerializeField] Sprite ShinyTileSprite = null;
    [SerializeField] Color ShinyTileColor = Color.white;
    [SerializeField] float ShinyYMod = 0;
    [SerializeField] Sprite WispyTileSprite = null;
    [SerializeField] Color WispyTileColor = Color.white;
    [SerializeField] float WispyYMod = 0;
    [SerializeField] Sprite MysticTileSprite = null;
    [SerializeField] Color MysticTileColor = Color.white;
    [SerializeField] float MysticYMod = 0;
    [SerializeField] Sprite HealthyTileSprite = null;
    [SerializeField] Color HealthyTileColor = Color.white;
    [SerializeField] float HealthyYMod = 0;
    [SerializeField] Sprite HeavyTileSprite = null;
    [SerializeField] Color HeavyTileColor = Color.white;
    [SerializeField] float HeavyYMod = 0;
    [SerializeField] Sprite ArmoredTileSprite = null;
    [SerializeField] Color ArmoredTileColor = Color.white;
    [SerializeField] float ArmoredYMod = 0;
    [SerializeField] Sprite ChargedTileSprite = null;
    [SerializeField] Color ChargedTileColor = Color.white;
    [SerializeField] float ChargedYMod = 0;

    Color fadeColor_sr;
    Color fadeColor_mr;

    public struct SpriteColorYMod
    {
        public Sprite Sprite;
        public Color Color;
        public float YMod;
        public SpriteColorYMod(Sprite sprite, Color color, float yMod)
        {
            Sprite = sprite;
            Color = color;
            YMod = yMod;
        }

    }

    //param
    float fallSpeed = 4.0f;
    float rotationSpeed = 10f; //deg per sec

    //state
    public float LifetimeRemaining { get; private set; }
    float factor;
    bool isLatentAbilityActivated = false;
    float remainingFallDistance;
    bool isFalling = true;
    bool isRotating = false;
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
        SpriteColorYMod sc = GetSpriteColorFromAbility(Ability);
        sr.sprite = sc.Sprite;
        sr.color = sc.Color;
        tmp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, sc.YMod);


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

        if (isRotating)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            tmp.transform.Rotate(0, 0, -1 * rotationSpeed * Time.deltaTime);
        }

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

    public SpriteColorYMod GetSpriteColorFromAbility(TrueLetter.Ability ability)
    {
        SpriteColorYMod sc;
        switch (ability)
        {

            case TrueLetter.Ability.Lucky:
                sc = new SpriteColorYMod(LuckyTileSprite, LuckyTileColor, LuckyYMod);
                return sc;

            case TrueLetter.Ability.Frozen:
                sc = new SpriteColorYMod(FrozenTileSprite, FrozenTileColor, FrozenYMod);
                return sc;

            case TrueLetter.Ability.Shiny:
                sc = new SpriteColorYMod(ShinyTileSprite, ShinyTileColor, ShinyYMod);
                return sc;

            case TrueLetter.Ability.Wispy:
                sc = new SpriteColorYMod(WispyTileSprite, WispyTileColor, WispyYMod);
                return sc;

            case TrueLetter.Ability.Mystic:
                sc = new SpriteColorYMod(MysticTileSprite, MysticTileColor, MysticYMod);
                return sc;

            case TrueLetter.Ability.Healthy:
                sc = new SpriteColorYMod(HealthyTileSprite, HealthyTileColor, HealthyYMod);
                return sc;

            case TrueLetter.Ability.Heavy:
                sc = new SpriteColorYMod(HeavyTileSprite, HeavyTileColor, HeavyYMod);
                return sc;

            case TrueLetter.Ability.Armored:
                sc = new SpriteColorYMod(ArmoredTileSprite, ArmoredTileColor, ArmoredYMod);
                return sc;

            case TrueLetter.Ability.Charged:
                sc = new SpriteColorYMod(ChargedTileSprite, ChargedTileColor, ChargedYMod);
                return sc;

            default:
                sc = new SpriteColorYMod(NormalTileSprite, NormalTileColor, NormalYMod);
                return sc;

        }
    }

    #endregion

}
