using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaBuilder : MonoBehaviour
{
    [SerializeField] GameObject cameraMousePrefab = null;
    [SerializeField] GameObject wallPrefab = null;
    [SerializeField] GameObject catPrefab = null;
    [SerializeField] GameObject letterTileDropperPrefab = null;
    GameController gc;
    VictoryMeter vm;
    UIDriver uid;
    public GameObject arenaStarter;
    List<GameObject> arenaWallObjects = new List<GameObject>();
    int layerMask_Impassable = 1 << 13;
    int layerMask_Passable = 1 << 14;

    //parameters
    int minX = -9;
    int minY = -7;
    int maxX = 9;
    int maxY = 10;
    float checkRadius = 0.01f;

    //state
    GameObject cat;
    GameObject letterTileDropper;
    GameObject camMouse;

    public void SetupArena(Vector2 centroid)
    {
        minX += Mathf.RoundToInt(transform.position.x);
        maxX += Mathf.RoundToInt(transform.position.x);
        minY += Mathf.RoundToInt(transform.position.y);
        maxY += Mathf.RoundToInt(transform.position.y);

        uid = FindObjectOfType<UIDriver>();
        uid.EnterArena();
        gc = FindObjectOfType<GameController>();
        gc.isInArena = true;
        vm = gc.GetVictoryMeter();
        vm.ResetArena();
        vm.OnArenaVictory_TrueForPlayerWin += HandleArenaCompletion;
        SetupStatueCameraMouseCat(centroid);
        SetupArenaBoundaries(centroid);

    }
    private void SetupArenaBoundaries(Vector2 centroid)
    {
        Vector2 wallSection = new Vector2(0, minY);
        for (float x = minX; x < maxX; x += 1f)
        {
            wallSection.x = x;
            wallSection = GridHelper.SnapToGrid(wallSection, 1);
            if (Physics2D.OverlapCircle(wallSection, checkRadius, layerMask_Impassable))
            {
                continue;
            }
            else
            {
                GameObject wallPiece = Instantiate(wallPrefab, wallSection, Quaternion.identity);
                arenaWallObjects.Add(wallPiece);
            }

        }
        wallSection.y = maxY;
        for (float x = minX; x < maxX; x += 1f)
        {
            wallSection.x = x;
            wallSection = GridHelper.SnapToGrid(wallSection, 1);
            if (Physics2D.OverlapCircle(wallSection, checkRadius, layerMask_Impassable))
            {
                continue;
            }
            else
            {
                GameObject wallPiece = Instantiate(wallPrefab, wallSection, Quaternion.identity);
                arenaWallObjects.Add(wallPiece);
            }
        }
        wallSection.x = minX;
        for (float y = minY; y < maxY; y += 1f)
        {
            wallSection.y = y;
            wallSection = GridHelper.SnapToGrid(wallSection, 1);
            if (Physics2D.OverlapCircle(wallSection, checkRadius, layerMask_Impassable))
            {
                continue;
            }
            else
            {
                GameObject wallPiece = Instantiate(wallPrefab, wallSection, Quaternion.identity);
                arenaWallObjects.Add(wallPiece);
            }
        }
        wallSection.x = maxX;
        for (float y = minY; y < maxY; y += 1f)
        {
            wallSection.y = y;
            wallSection = GridHelper.SnapToGrid(wallSection, 1);
            if (Physics2D.OverlapCircle(wallSection, checkRadius, layerMask_Impassable))
            {
                continue;
            }
            else
            {
                GameObject wallPiece = Instantiate(wallPrefab, wallSection, Quaternion.identity);
                arenaWallObjects.Add(wallPiece);
            }
        }
    }

    private void SetupStatueCameraMouseCat(Vector2 centroid)
    {
        letterTileDropper = Instantiate(letterTileDropperPrefab, centroid, Quaternion.identity) as GameObject;
        //statue = Instantiate(statuePrefab, centroid, Quaternion.identity) as GameObject;
        camMouse = Instantiate(cameraMousePrefab, centroid, Quaternion.identity) as GameObject;
        CameraMouse cameraMouse1 = camMouse.GetComponent<CameraMouse>();
       
        cameraMouse1.SetAnchor(arenaStarter);
        cameraMouse1.SetPlayer(gc.GetPlayer());

        cat = Instantiate(catPrefab, centroid + Vector2.one, Quaternion.identity) as GameObject;
    }

    public Vector2 CreateRandomPointWithinArena()
    {
        float randX = UnityEngine.Random.Range(minX + 1, maxX);
        float randY = UnityEngine.Random.Range(minY + 1, maxY);
        Vector2 randPos = new Vector2(randX, randY);
        return randPos;
    }

    public Vector2 CreatePassableRandomPointWithinArena()
    {
        Vector2 randPos;
        do
        {
            float randX = UnityEngine.Random.Range(minX + 1, maxX);
            float randY = UnityEngine.Random.Range(minY + 1, maxY);
            randPos = new Vector2(randX, randY);
        }
        while (Physics2D.OverlapCircle(randPos, checkRadius, layerMask_Impassable) != null);

        return randPos;
    }

    public void SetArenaStarter(GameObject go)
    {
        arenaStarter = go;
    }

    
    private void HandleArenaCompletion(bool didPlayerWin)
    {
        // if (didplayerwin) leads to different outcomes, such as awarding a True Letter, or some currency
        CloseDownArena();
        vm.OnArenaVictory_TrueForPlayerWin -= HandleArenaCompletion;
    }

    public void CloseDownArena()
    {
        gc.isInArena = false;
        uid.EnterOverworld();
        Destroy(camMouse);
        Destroy(cat);
        Destroy(letterTileDropper);
        foreach(var element in arenaWallObjects)
        {
            Destroy(element);
        }
        Destroy(arenaStarter); // For now, destroy the statue, but later replace with a broken statue, perhaps?
        gc.GetPlayer().GetComponent<WordBuilder>().ClearCurrentWord();
        Destroy(gameObject);
    }


}
