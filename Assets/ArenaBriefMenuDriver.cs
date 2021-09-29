using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaBriefMenuDriver : MonoBehaviour
{
    ArenaStarter arenaStarter;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleRetreatClick()
    {
        arenaStarter.RetreatFromArena();
    }

    public void HandleAttackClick()
    {
        arenaStarter.StartArena();

    }

    public void SetArenaStarterReference(ArenaStarter arst)
    {
        arenaStarter = arst;
    }
}
