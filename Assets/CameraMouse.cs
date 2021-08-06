using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMouse : MonoBehaviour
{
    //init
    GameObject anchorObject;
    GameObject player;

    void Start()
    {
        Camera.main.GetComponentInChildren<CinemachineVirtualCamera>().Follow = transform;
    }
    public void SetPlayer(GameObject newPlayer)
    {
        player = newPlayer;
    }

    public void SetAnchor(GameObject newAnchor)
    {
        anchorObject = newAnchor;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = (player.transform.position - anchorObject.transform.position) * (2f/3f);
        transform.position = anchorObject.transform.position + dir;
    }
}
