using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Reknitter : MonoBehaviour
{
    GraphUpdateScene gus;
    WordMakerMovement wmm;
    TailPieceManager tpm;
    void Start()
    {
        gus = GetComponent<GraphUpdateScene>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = tpm.ReturnLastBreadcrumbOfLastTailPiece();
        if (!wmm)
        {
            Destroy(gameObject);
        }
    }

    public void SetOwners(WordMakerMovement owner, TailPieceManager newTPM)
    {
        wmm = owner;
        wmm.OnLeaderMoved += ReknitGridGraph;
        tpm = newTPM;
    }

    private void ReknitGridGraph()
    {
        gus.Apply();
    }

    private void OnDestroy()
    {
        wmm.OnLeaderMoved -= ReknitGridGraph;
    }
}
