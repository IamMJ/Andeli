using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategyBrain_NPC : MonoBehaviour
{
    //init
    WordBrain_NPC wb;
    MoveBrain_NPC mb;

    //state
    Vector2 strategicDestination;

    // Start is called before the first frame update
    void Start()
    {
        wb = GetComponent<WordBrain_NPC>();
        mb = GetComponent<MoveBrain_NPC>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStrategicDestination();
        ConvertStrategicDestinationToTacticalDestination();
    }

    private void UpdateStrategicDestination()
    {
        if (wb.TargetLetterTile)
        {
            strategicDestination = wb.TargetLetterTile.transform.position;
        }
        else
        {
            strategicDestination = Vector2.one * 4;
            //implement a random wander while waiting.
        }

    }

    private void ConvertStrategicDestinationToTacticalDestination()
    {
        Vector2 workingTacDestination = strategicDestination;
        if (GridHelper.CheckIfSnappedToGrid(transform.position))
        {
            workingTacDestination = AdjustToAvoidUntargetedLetterTiles();
        }

        AdjustToAvoidImpassableThings();

        mb.TacticalDestination = workingTacDestination;
    }

    private Vector2 AdjustToAvoidUntargetedLetterTiles()
    {
        Vector3 currentDir = mb.GetValidDesMove();
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
        Vector3 currentDir = mb.GetValidDesMove();
        Vector3 testDir = new Vector2(currentDir.y, currentDir.x);
        RaycastHit2D hit = Physics2D.Linecast(transform.position, transform.position + testDir, 1 << 9);
        Debug.DrawLine(transform.position, transform.position + testDir, Color.green, Time.deltaTime);

        if (hit)
        {
            Vector3 testDir2 = new Vector2(-currentDir.y, -currentDir.x);
            RaycastHit2D hit2 = Physics2D.Linecast(transform.position, transform.position + testDir, 1 << 9);
            Debug.DrawLine(transform.position, transform.position + testDir, Color.yellow, Time.deltaTime);

            if (!hit2)
            {
                Debug.Log("using second reroute;");
                Vector2 newTacDest = transform.position + testDir;
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
