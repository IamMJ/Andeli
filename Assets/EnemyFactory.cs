using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    [SerializeField] EnemyProfile[] enemyProfiles = null;

    public List<EnemyProfile> GetEnemyProfiles(int numberOfProfiles)
    {
        List<EnemyProfile> requestedProfiles = new List<EnemyProfile>();

        for (int i = 0; i <numberOfProfiles; i++)
        {
            requestedProfiles.Add(enemyProfiles[i]);
        }
        return requestedProfiles;
    }

}
