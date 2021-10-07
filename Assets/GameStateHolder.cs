using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateHolder : MonoBehaviour
{
    [SerializeField] List<GameObject> obelisks = new List<GameObject>();

    void Start()
    {
        FindAllObeliskTags();
    }

    #region Private Helpers
    private void FindAllObeliskTags()
    {
        GameObject[] obelisks_Array = GameObject.FindGameObjectsWithTag("Obelisk");
        foreach (var elem in obelisks_Array)
        {
            obelisks.Add(elem);
        }
    }

    #endregion

    #region Public Methods
    public void RestoreAllObelisks()
    {
        foreach(var obel in obelisks)
        {
            obel.GetComponent<ArenaStarter>().ActivateArenaStarter();
        }
    }

    #endregion
}
