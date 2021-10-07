using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class JewelManager : MonoBehaviour
{
    [SerializeField] Image[] jewelImages = null;
    [SerializeField] GameObject[] chargedJewelParticleFXs = null;

    //state
    int jewelsInstalled;

    private void Start()
    {
        jewelsInstalled = jewelImages.Length;
    }

    public void UpdateJewelImage(float factor)
    {
        float subamount = 100 / jewelsInstalled;
        for (int i = 0; i < jewelsInstalled; i++)
        {
            float specificFactor = Mathf.Clamp01((factor - (subamount * i)) / subamount);
            jewelImages[i].color = Color.HSVToRGB(0, specificFactor, 1);
        }
    }
}
