using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Reknitter : MonoBehaviour
{
    GraphUpdateScene gus;
    Movement wmm;
    TailPieceManager tpm;
    void Start()
    {
        gus = GetComponent<GraphUpdateScene>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = wmm.GetOldestBreadcrumb();
        if (!wmm)
        {
            Destroy(gameObject);
        }
    }

    public void SetOwners(Movement owner, TailPieceManager newTPM)
    {
        wmm = owner;
        //wmm.OnLeaderMoved += ReknitGridGraph;
        tpm = newTPM;
    }

    private void ReknitGridGraph()
    {
        gus.Apply();
    }

    private void OnDestroy()
    {
        //wmm.OnLeaderMoved -= ReknitGridGraph;
    }
}
