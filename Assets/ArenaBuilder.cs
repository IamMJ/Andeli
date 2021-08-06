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

    //parameters
    float minX = -8f;
    float minY = -6;
    float maxX = 8;
    float maxY = 9;

    //state
    GameObject statue;
    GameObject player;


    void Start()
    {
        SetupStatuePlayerCameraMouse();
        SetupArenaBoundaries();
    }
    private void SetupArenaBoundaries()
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

    private void SetupStatuePlayerCameraMouse()
    {
        statue = Instantiate(statuePrefab, Vector2.zero, Quaternion.identity) as GameObject;
        player = Instantiate(playerPrefab, Vector2.one, Quaternion.identity) as GameObject;
        GameObject cameraMouse = Instantiate(cameraMousePrefab, Vector3.zero, Quaternion.identity) as GameObject;
        CameraMouse cameraMouse1 = cameraMouse.GetComponent<CameraMouse>();
        cameraMouse1.SetAnchor(statue);
        cameraMouse1.SetPlayer(player);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
