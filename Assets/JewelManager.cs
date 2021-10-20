using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class JewelManager : MonoBehaviour
{
    [SerializeField] Image[] jewelImages = null;
    [SerializeField] ParticleSystem[] jewelPSs = null;

    //state
    int jewelsInstalled = 1;

    private void Start()
    {
        //jewelsInstalled = jewelImages.Length;
        ModifyJewelCount(3);

    }

    public void UpdateJewelImage(float factor)
    {
        int lowestEmpty = -1;
        float subamount = 100 / jewelsInstalled;
        for (int i = jewelsInstalled-1; i >= 0; i--)
        {
            float specificFactor = Mathf.Clamp01((factor - (subamount * i)) / subamount);
            jewelImages[i].color = Color.HSVToRGB(0, specificFactor, 1);
            if (specificFactor < 1)
            {
                lowestEmpty = i;
            }

        }
        Debug.Log($"Lowest empty jewel: {lowestEmpty}");
        ProvideFeedbackAboutInsufficientEnergy(lowestEmpty);

    }

    public void ModifyJewelCount(int newJewelCount)
    {
        jewelsInstalled = newJewelCount;
        foreach (var jewel in jewelImages)
        {
            jewel.color = Color.clear;
        }
    }

    public void ProvideFeedbackAboutInsufficientEnergy(int chargingJewelIndex)
    {
        if (chargingJewelIndex == -1) { return; }
        for (int i = 0; i < jewelsInstalled; i++)
        {
            if (i == chargingJewelIndex)
            {
                ParticleSystem.EmissionModule em = jewelPSs[i].emission;
                em.rateOverTime = 6;
            }
            else
            {
                ParticleSystem.EmissionModule em = jewelPSs[i].emission;
                em.rateOverTime = 0;
            }
        }
    }
}
