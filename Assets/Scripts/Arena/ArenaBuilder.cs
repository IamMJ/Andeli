using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ArenaBuilder : MonoBehaviour
{
    [SerializeField] GameObject cameraMousePrefab = null;
    //[SerializeField] GameObject wallPrefab = null;
    //[SerializeField] GameObject[] enemyPrefabs = null;
    [SerializeField] GameObject letterTileDropperPrefab = null;

    DebriefPanelDriver dpd;
    GameObject enemy;
    ArenaSettingHolder ash;
    CinemachineVirtualCamera cvc;
    GameController gc;
    VictoryMeter vm;
    GameObject player;
    CombatPanel uid;
    ArenaStarter arenaStarter;
    List<ArenaWall> arenaWallObjects = new List<ArenaWall>();
    TutorDialogManager tutorDM;
    WordWeaponizer playerWWZ;
    int layerMask_Impassable = 1 << 13;
    int layerMask_Passable = 1 << 14;


    //parameters
    int minX = -6;
    int minY = -6;
    int maxX = 6;
    int maxY = 6;
    float checkRadius = 0.01f;
    Vector3 enemySpawnOffset = new Vector2(0, 0);


    //state
    public LetterTileDropper ltd;
    GameObject camMouse;
    float startTime;

    public void SetupArena(Vector2 centroid)
    {
        dpd = FindObjectOfType<DebriefPanelDriver>();
        gc = FindObjectOfType<GameController>();
        gc.isInArena = true;
        gc.RegisterCurrentArenaBuilder(this);
        gc.SetCameraToArenaOffset();
        startTime = Time.time;
        minX += Mathf.RoundToInt(transform.position.x);
        maxX += Mathf.RoundToInt(transform.position.x);
        minY += Mathf.RoundToInt(transform.position.y);
        maxY += Mathf.RoundToInt(transform.position.y);

        uid = FindObjectOfType<CombatPanel>();
        //uid.ShowArenaUIElements();

        player = gc.GetPlayer();
        playerWWZ = player.GetComponent<WordWeaponizer>();
        playerWWZ.HandleArenaEntry();
        player.GetComponent<WordBuilder>().ClearCurrentWord();
        FindObjectOfType<BagManager>().RequestClearOutBag();

        enemy = Instantiate(arenaStarter.ArenaEnemyPrefab, transform.position + enemySpawnOffset, Quaternion.identity);
        enemy.GetComponent<SpellingStrategy>().ImplementSpeedEnergySettingsFromEP();
        vm = gc.GetVictoryMeter();
        vm.OnArenaVictory_TrueForPlayerWin += HandleArenaCompletion;
        SetupStatueCameraMouse(centroid);
        ash.SetupArena(ltd, player.GetComponent<WordMakerMemory>(), enemy.GetComponent<WordMakerMemory>(),
            playerWWZ, enemy.GetComponent<WordWeaponizer>(), 
            player.GetComponent<WordBuilder>(), enemy.GetComponent<WordBuilder>(), vm, uid);


        //SetupArenaBoundaries(centroid);

    }
    //private void SetupArenaBoundaries(Vector2 centroid)
    //{
    //    Vector2 wallSection = new Vector2(0, minY);
    //    for (float x = minX; x < maxX; x += 1f)
    //    {
    //        wallSection.x = x;
    //        wallSection = GridHelper.SnapToGrid(wallSection, 1);
    //        if (Physics2D.OverlapCircle(wallSection, checkRadius, layerMask_Impassable))
    //        {
    //            continue;
    //        }
    //        else
    //        {
    //            ArenaWall wallPiece = Instantiate(wallPrefab, wallSection, Quaternion.identity).GetComponent<ArenaWall>();
    //            arenaWallObjects.Add(wallPiece);
    //        }

    //    }
    //    wallSection.y = maxY;
    //    for (float x = minX; x < maxX; x += 1f)
    //    {
    //        wallSection.x = x;
    //        wallSection = GridHelper.SnapToGrid(wallSection, 1);
    //        if (Physics2D.OverlapCircle(wallSection, checkRadius, layerMask_Impassable))
    //        {
    //            continue;
    //        }
    //        else
    //        {
    //            ArenaWall wallPiece = Instantiate(wallPrefab, wallSection, Quaternion.identity).GetComponent<ArenaWall>();
    //            arenaWallObjects.Add(wallPiece);
    //        }
    //    }
    //    wallSection.x = minX;
    //    for (float y = minY; y < maxY; y += 1f)
    //    {
    //        wallSection.y = y;
    //        wallSection = GridHelper.SnapToGrid(wallSection, 1);
    //        if (Physics2D.OverlapCircle(wallSection, checkRadius, layerMask_Impassable))
    //        {
    //            continue;
    //        }
    //        else
    //        {
    //            ArenaWall wallPiece = Instantiate(wallPrefab, wallSection, Quaternion.identity).GetComponent<ArenaWall>();
    //            arenaWallObjects.Add(wallPiece);
    //        }
    //    }
    //    wallSection.x = maxX;
    //    for (float y = minY; y < maxY; y += 1f)
    //    {
    //        wallSection.y = y;
    //        wallSection = GridHelper.SnapToGrid(wallSection, 1);
    //        if (Physics2D.OverlapCircle(wallSection, checkRadius, layerMask_Impassable))
    //        {
    //            continue;
    //        }
    //        else
    //        {
    //            ArenaWall wallPiece = Instantiate(wallPrefab, wallSection, Quaternion.identity).GetComponent<ArenaWall>();
    //            arenaWallObjects.Add(wallPiece);
    //        }
    //    }
    //}

    private void SetupStatueCameraMouse(Vector2 centroid)
    {
        ltd = Instantiate(letterTileDropperPrefab, centroid, Quaternion.identity).GetComponent<LetterTileDropper>();
        //statue = Instantiate(statuePrefab, centroid, Quaternion.identity) as GameObject;
        camMouse = Instantiate(cameraMousePrefab, centroid, Quaternion.identity) as GameObject;
        CameraMouse cameraMouse1 = camMouse.GetComponent<CameraMouse>();

        cameraMouse1.SetAnchor(arenaStarter.gameObject);
        cameraMouse1.SetPlayer(gc.GetPlayer());
        cvc = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>();
        cvc.Follow = camMouse.transform; //arenaStarter.transform;

        //for (int i = 0; i < enemyPrefabs.Length; i++)
        //{
        //    enemies[i] = Instantiate(enemyPrefabs[i], centroid + Vector2.one, Quaternion.identity) as GameObject;
        //}

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

    public void SetArenaStarter(ArenaStarter newAS, ArenaSettingHolder newASH)
    {
        arenaStarter = newAS;
        ash = newASH;
    }

    
    private void HandleArenaCompletion(bool didPlayerWin)
    {
        // if (didplayerwin) leads to different outcomes, such as awarding a True Letter, or some currency

        float timeElapsed = Mathf.Round(Time.time - startTime);

        CloseDownArena();
        dpd.ActivateDebriefPanel(didPlayerWin, player, enemy, timeElapsed);
        arenaStarter.DeactivateArenaStarter(didPlayerWin);
        vm.OnArenaVictory_TrueForPlayerWin -= HandleArenaCompletion;
    }


    public void CloseDownArena()
    {

        gc.SetCameraToOverworldOffset();
        gc.isInArena = false;
        //uid.ShowOverworldUIElements(); NERG
        Destroy(camMouse);
        cvc.Follow = player.transform;
        //Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0);
        Destroy(enemy);

        ltd.DestroyAllLetters();
        Destroy(ltd?.gameObject);
        //foreach(var element in arenaWallObjects)
        //{
        //    element.RemoveArenaWall();
        //}

        // For now, destroy the statue, but later replace with a broken statue, perhaps?
        player.GetComponent<WordBuilder>().ClearCurrentWord();
        playerWWZ.ResetEnergyStats();
        player.GetComponent<SpeedKeeper>().ResetSpeedStats();

        Destroy(gameObject);
    }

    public GameObject GetEnemyInArena()
    {
        return enemy;
    }

    public ArenaSettingHolder GetArenaSettingsHolder()
    {
        return ash;
    }
}
