using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaBuilder : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab = null;
    [SerializeField] GameObject statuePrefab = null;
    [SerializeField] GameObject cameraMousePrefab = null;
    [SerializeField] GameObject wallPrefab = null;
    [SerializeField] GameObject catPrefab = null;
    [SerializeField] GameObject letterTileDropperPrefab = null;

    //parameters
    int minX = -9;
    int minY = -7;
    int maxX = 9;
    int maxY = 10;

    //state
    GameObject statue;
    GameObject player;
    GameObject cat;
    GameObject letterTileDropper;
    

    public void SetupArena(Vector2 centroid)
    {
        SetupStatuePlayerCameraMouseCat(centroid);
        SetupArenaBoundaries(centroid);
    }
    private void SetupArenaBoundaries(Vector2 centroid)
    {
        Vector2 wallSection = new Vector2(0, minY);
        for (float x = minX; x < maxX; x += 1f)
        {

            wallSection.x = x;
            wallSection = GridHelper.SnapToGrid(wallSection, 1);
            Instantiate(wallPrefab, wallSection, Quaternion.identity);
        }
        wallSection.y = maxY;
        for (float x = minX; x < maxX; x += 1f)
        {
            wallSection.x = x;
            wallSection = GridHelper.SnapToGrid(wallSection, 1);
            Instantiate(wallPrefab, wallSection, Quaternion.identity);
        }
        wallSection.x = minX;
        for (float y = minY; y < maxY; y += 1f)
        {
            wallSection.y = y;
            wallSection = GridHelper.SnapToGrid(wallSection, 1);
            Instantiate(wallPrefab, wallSection, Quaternion.identity);
        }
        wallSection.x = maxX;
        for (float y = minY; y < maxY; y += 1f)
        {
            wallSection.y = y;
            wallSection = GridHelper.SnapToGrid(wallSection, 1);
            Instantiate(wallPrefab, wallSection, Quaternion.identity);
        }
    }

    private void SetupStatuePlayerCameraMouseCat(Vector2 centroid)
    {
        letterTileDropper = Instantiate(letterTileDropperPrefab, centroid, Quaternion.identity) as GameObject;
        statue = Instantiate(statuePrefab, centroid, Quaternion.identity) as GameObject;
        player = Instantiate(playerPrefab, centroid + Vector2.one, Quaternion.identity) as GameObject;
        GameObject cameraMouse = Instantiate(cameraMousePrefab, centroid, Quaternion.identity) as GameObject;
        CameraMouse cameraMouse1 = cameraMouse.GetComponent<CameraMouse>();
        cameraMouse1.SetAnchor(statue);
        cameraMouse1.SetPlayer(player);

        cat = Instantiate(catPrefab, centroid, Quaternion.identity) as GameObject;
    }

    public Vector2 CreateRandomPointWithinArena()
    {
        float randX = UnityEngine.Random.Range(minX + 1, maxX);
        float randY = UnityEngine.Random.Range(minY + 1, maxY);
        Vector2 randPos = new Vector2(randX, randY);
        return randPos;
    }


}
