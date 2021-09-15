using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pathfinding;

public class LetterTile : MonoBehaviour, IGridModifier
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
    GraphUpdateScene gus;
    List<GameObject> claimants = new List<GameObject>();
    Bounds bounds;

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
        gus = GetComponent<GraphUpdateScene>();
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
                transform.position = GridHelper.SnapToGrid(transform.position, 1);
                if (assignedShadow)
                {
                    assignedShadow.RemoveShadow();
                }
                UnknitAllGridGraphs();
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

        if (LifetimeRemaining <= 0f)
        {
            PickupLetterTile();
        }
               
    }

    public void UnknitAllGridGraphs() //int graphToUnknit)
    {
        gus.modifyWalkability = true;
        gus.setWalkability = false;
        gus.Apply();
    }
    public void ReknitAllGridGraphs()
    {
        gus.modifyWalkability = true;
        gus.setWalkability = true;
        gus.Apply();
    }
    public void UnknitSpecificGridGraph(int graphIndexToUnknit)
    {
        UpdateBounds();
        var guo = new GraphUpdateObject(bounds);
        guo.modifyWalkability = true;
        guo.setWalkability = false;
        //gus.modifyWalkability = true;
        //gus.setWalkability = false;
        guo.nnConstraint.graphMask = 1 << graphIndexToUnknit;
        AstarPath.active.UpdateGraphs(guo);
    }


    public void ReknitSpecificGridGraph(int graphIndexToReknit)
    {
        UpdateBounds();
        var guo = new GraphUpdateObject(bounds);
        guo.modifyWalkability = true;
        guo.setWalkability = true;
        //gus.modifyWalkability = true;
        //gus.setWalkability = true;
        guo.nnConstraint.graphMask = 1 << graphIndexToReknit;
        AstarPath.active.UpdateGraphs(guo);
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

    public void PickupLetterTile()
    {
        letterTileDropper.RemoveLetterFromSpawnedLetterList(this);
        DestroyThisLetterTile();
    }

    public void DestroyThisLetterTile()
    {
        if (assignedShadow)
        {
            assignedShadow.RemoveShadow();
        }
        ReknitAllGridGraphs();
        Destroy(gameObject);
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
    
    private void UpdateBounds()
    {
        bounds.center = transform.position;
        bounds.extents = new Vector3(.5f, .5f, 1);
    }

}
