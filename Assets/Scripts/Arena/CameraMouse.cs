using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMouse : MonoBehaviour
{
    //init
    GameObject anchorObject;
    GameObject player;
    CinemachineVirtualCamera cvc;

    void Start()
    {
        cvc = Camera.main.GetComponentInChildren<CinemachineVirtualCamera>();
        cvc.Follow = transform;
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
        Vector3 dir = (player.transform.position - anchorObject.transform.position) * 0.2f;
        transform.position = anchorObject.transform.position + dir;
    }

    private void OnDestroy()
    {
        cvc.Follow = player.transform;
    }
}
