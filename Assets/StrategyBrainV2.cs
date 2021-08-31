using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StrategyBrainV2 : MonoBehaviour
{
    //init
    NavMeshAgent nma;
    MoveBrain_NPC mb;
    WordBrain_NPC wb;
    ArenaBuilder ab;

    public Vector2 strategicDestination;

    //param
    float closeEnough = 1.0f;

    //state
    bool hasValidPath = false;
    public NavMeshPathStatus status;



    void Start()
    {
        nma = GetComponent<NavMeshAgent>();
        nma.updatePosition = true;
        mb = GetComponent<MoveBrain_NPC>();
        wb = GetComponent<WordBrain_NPC>();
        wb.OnNewTargetLetterTile += RecalculatePathToTargetLetterTile;
        ab = FindObjectOfType<ArenaBuilder>();
        strategicDestination = ab.CreatePassableRandomPointWithinArena();
        nma.SetDestination(strategicDestination);
    }

    // Update is called once per frame
    void Update()
    {
        RandomlyWander();
        if (!wb.TargetLetterTile)
        {

        }

        Debug.DrawLine(transform.position, mb.TacticalDestination, Color.white);
        Debug.DrawLine(transform.position, nma.destination, Color.black);

        if (nma.pathStatus == NavMeshPathStatus.PathComplete)  // This loop just keeps updating the path to strategic destination
        {
            PassTacticalDestinationToMoveBrain();
            nma.SetDestination(strategicDestination);
        }
    }

    private void RandomlyWander()
    {
        if ((transform.position - (Vector3)strategicDestination).magnitude < closeEnough)
        {
            strategicDestination = ab.CreatePassableRandomPointWithinArena();
            nma.SetDestination(strategicDestination);
        }        
    }

    private void RecalculatePathToTargetLetterTile()
    {
        Debug.Log("asked to recalculate path");
        nma.SetDestination(wb.TargetLetterTile.transform.position);
    }

    private void PassTacticalDestinationToMoveBrain()
    {
        mb.TacticalDestination = GridHelper.SnapToGrid(nma.steeringTarget, 1);
    }

    private void OnDestroy()
    {
        wb.OnNewTargetLetterTile -= RecalculatePathToTargetLetterTile;
    }

    
}
