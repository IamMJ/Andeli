using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.U2D;

public class CameraController : MonoBehaviour
{
    //init
    CinemachineVirtualCamera cvc;
    PixelPerfectCamera ppc;

    //param
    Vector3 cameraOffset_Arena = new Vector3(0, -1.5f, 0);
    Vector3 cameraOffset_Overworld = Vector3.zero;
    //int cameraSize_ZoomedIn = 10;
    //int cameraSize_ZoomedOut = 30;
    //int zoomRate = 10;

    //state
    [SerializeField] float currentZoom;

    void Start()
    {
        cvc = GetComponentInChildren<CinemachineVirtualCamera>();
        ppc = GetComponent<PixelPerfectCamera>();
        ppc.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCameraToArenaOffset()
    {
        cvc.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = cameraOffset_Arena;

    }

    public void SetCameraToOverworldOffset()
    {
        cvc.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = cameraOffset_Overworld;
    }

    public void SetCameraToFollowObject(GameObject targetGO)
    {
        cvc.Follow = targetGO.transform;
        //StopAllCoroutines();
        //StartCoroutine(ZoomCamera(true));
    }
}
