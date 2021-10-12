using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
[RequireComponent(typeof(SpellingStrategy))]
public class WordBuilder_NPC : WordBuilder
{
    // This class is supposed to constantly maintain a Target Letter Tile. 
    // The LTT is updated anytime a letter tile is added or removed from the board

    //init
    //TailPieceManager tpm;
    LetterTileDropper ltd;
    SpellingStrategy ss;
    StrategyBrainV2 sb;

    public Action OnNewTargetLetterTile;

    //state
    public LetterTile TargetLetterTile; //{ get; private set; }
    [SerializeField] char currentTargetChar;

    protected override void Start()
    {
        base.Start();
        ss = GetComponent<SpellingStrategy>();
        sb = GetComponent<StrategyBrainV2>();
        ltd = FindObjectOfType<LetterTileDropper>();
        if (ltd)
        {
            ltd.OnLetterListModified += DetermineBestTargetLetter;
        }

        //tpm = GetComponent<TailPieceManager>();        
    }

    private void Update()
    {
        if (TargetLetterTile)
        {
            currentTargetChar = TargetLetterTile.Letter;
        }
        else { currentTargetChar = '?'; }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.gameObject.GetComponent<LetterTile>())
        {
            ss.EvaluateWordAfterGainingALetter();
        }

    }

    #region Simple Private Tasks
    private void OnDestroy()
    {
        ltd.OnLetterListModified -= DetermineBestTargetLetter;
    }
    protected override void AddLetterToSword(LetterTile newLetter)
    {
        base.AddLetterToSword(newLetter);
        //tpm.AddNewTailPiece(newLetter.Letter);
    }
    public override void ClearCurrentWord()
    {
        base.ClearCurrentWord();
        //tpm.DestroyEntireTail();
    }
    #endregion

    #region Hooks For Spelling Strategy
    public void EraseWord()
    {        
        ClearCurrentWord();
        ClearPowerLevel();
    }
    #endregion

    private void DetermineBestTargetLetter(LetterTile changedLetterTile, bool wasLetterAdded)
    {
        if (TargetLetterTile)
        {
            
        }

        if (ltd.doesBoardHaveLettersAvailable == false)
        {
            TargetLetterTile = null;
            return;
        }
        if (!wasLetterAdded && TargetLetterTile == changedLetterTile) //if a letter was removed, and it was the target letter...
        {
            LetterTile oldLTT = TargetLetterTile;
            TargetLetterTile = ss.FindBestLetterFromAllOnBoard();
            if (TargetLetterTile != oldLTT)
            {
                GridModifier.UnknitSpecificGridGraph(oldLTT.transform, sb.GetGraphIndex());
                GridModifier.ReknitSpecificGridGraph(TargetLetterTile.transform, sb.GetGraphIndex());
                OnNewTargetLetterTile?.Invoke();
            }

            return;
        }
        if (wasLetterAdded) //Since a new letter was added...
        {
            // Decide if the new letter is a better choice for target letter
            if (currentWord.Length == 0 && !TargetLetterTile)
            {
                TargetLetterTile = changedLetterTile;
                GridModifier.ReknitSpecificGridGraph(TargetLetterTile.transform, sb.GetGraphIndex());
                return;
            }
            if (TargetLetterTile)
            {
                LetterTile oldLTT = TargetLetterTile;
                GridModifier.UnknitSpecificGridGraph(TargetLetterTile.transform, sb.GetGraphIndex());
                TargetLetterTile = ss.FindBestLetterFromAllOnBoard();
                if (TargetLetterTile)
                {
                    GridModifier.ReknitSpecificGridGraph(TargetLetterTile.transform, sb.GetGraphIndex());
                }

                if (TargetLetterTile != oldLTT)
                {
                    OnNewTargetLetterTile?.Invoke();
                }
            }
        }
    }   
}
