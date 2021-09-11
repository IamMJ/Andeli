using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategyBrain_NPC : MonoBehaviour
{
    //init
    WordBuilder_NPC wb;
    MoveBrain_NPC mb;
    ArenaBuilder ab;

    //param
    float castLength = 1.5f;
    float closeEnough = 1f;

    //state
    Vector2 workingTacticalDestination;
    Vector2 strategicDestination;

    // Start is called before the first frame update
    void Start()
    {
        ab = FindObjectOfType<ArenaBuilder>();
        wb = GetComponent<WordBuilder_NPC>();
        mb = GetComponent<MoveBrain_NPC>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GridHelper.CheckIfSnappedToGrid(transform.position))
        {
            UpdateStrategicDestination();
            UpdateTacticalDestination();
        }

    }

    private void UpdateStrategicDestination()
    {
        if (wb.TargetLetterTile)
        {
            strategicDestination = wb.TargetLetterTile.transform.position;
        }
        else
        {
            if ((transform.position - (Vector3)strategicDestination).magnitude < closeEnough)
            {
                strategicDestination = ab.CreateRandomPointWithinArena();
            }
        }
    }

    private void UpdateTacticalDestination()
    {
        workingTacticalDestination = DetermineBetweenStrategicOrTacticalDestination();

        mb.TacticalDestination = workingTacticalDestination;
    }

    private Vector2 DetermineBetweenStrategicOrTacticalDestination()
    {
        Vector3 currentDir = mb.GetValidDesMove() * castLength;
        RaycastHit2D hit = Physics2D.Linecast(transform.position, transform.position + currentDir, 1<<9);
        Debug.DrawLine(transform.position, transform.position + currentDir, Color.blue, Time.deltaTime) ;
        if (hit)
        {
            if (hit.transform.GetComponent<LetterTile>() != wb.TargetLetterTile)
            {               
                return FindAWorkingTacticalDestination(); ;
            }
            else
            {
                return strategicDestination;
            }
        }
        else  //nothing between you and target, so head towards the strategic target;
        {
            return strategicDestination;
        }

    }

    private Vector2 FindAWorkingTacticalDestination()
    {
        Vector3 currentDir = mb.GetValidDesMove() * castLength;
        Vector3 testDir = new Vector2(currentDir.y, currentDir.x);
        RaycastHit2D hit = Physics2D.Linecast(transform.position, transform.position + testDir, 1 << 9);
        Debug.DrawLine(transform.position, transform.position + testDir, Color.green, Time.deltaTime);

        if (hit)
        {
            Vector3 testDir2 = new Vector2(-currentDir.y, -currentDir.x) * castLength;
            RaycastHit2D hit2 = Physics2D.Linecast(transform.position, transform.position + testDir2, 1 << 9);
            Debug.DrawLine(transform.position, transform.position + testDir2, Color.yellow, Time.deltaTime);

            if (!hit2)
            {
                Debug.Log("using second reroute;");
                Vector2 newTacDest = transform.position + testDir2;
                newTacDest = GridHelper.SnapToGrid(newTacDest, 1);
                return newTacDest;
            }
            else
            {
                Debug.Log("second attempt didn't work either; give up");
                return Vector2.zero;
            }
        }
        else
        {
            Debug.Log("using first reroute;");
            Vector2 newTacDest = transform.position + testDir;
            newTacDest = GridHelper.SnapToGrid(newTacDest, 1);
            return newTacDest;
        }

    }

    private void AdjustToAvoidImpassableThings()
    {
        
    }
}
