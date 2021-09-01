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
        nma.updatePosition = false;
        mb = GetComponent<MoveBrain_NPC>();
        wb = GetComponent<WordBrain_NPC>();
        wb.OnNewTargetLetterTile += SetNewTargetLetterTileAsStrategicDestination;
        ab = FindObjectOfType<ArenaBuilder>();
        strategicDestination = ab.CreatePassableRandomPointWithinArena();
        nma.SetDestination(strategicDestination);
    }

    // Update is called once per frame
    void Update()
    {
        if (!wb.TargetLetterTile)
        {
            RandomlyWander();
        }


        nma.SetDestination(strategicDestination);


        PassTacticalDestinationToMoveBrain();

        Debug.DrawLine(transform.position, mb.TacticalDestination, Color.white);
        Debug.DrawLine(transform.position, nma.destination, Color.black);

        DebugDrawPath(nma.path.corners);
    }

    private void RandomlyWander()
    {
        if ((transform.position - (Vector3)strategicDestination).magnitude < closeEnough)
        {
            strategicDestination = ab.CreatePassableRandomPointWithinArena();
            nma.SetDestination(strategicDestination);
        }        
    }

    private void SetNewTargetLetterTileAsStrategicDestination()
    {
        strategicDestination = wb.TargetLetterTile.transform.position;
    }

    private void PassTacticalDestinationToMoveBrain()
    {
        mb.TacticalDestination = GridHelper.SnapToGrid(nma.steeringTarget, 1);
    }

    private void OnDestroy()
    {
        wb.OnNewTargetLetterTile -= SetNewTargetLetterTileAsStrategicDestination;
    }

    public static void DebugDrawPath(Vector3[] corners)
    {
        if (corners.Length < 2) { return; }
        int i = 0;
        for (; i < corners.Length - 1; i++)
        {
            Debug.DrawLine(corners[i], corners[i + 1], Color.blue);
        }
        Debug.DrawLine(corners[0], corners[1], Color.red);
    }

}
