using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class JewelManager : MonoBehaviour
{
    [SerializeField] Image[] jewelImages = null;
    [SerializeField] ParticleSystem jewel0PS = null;

    //state
    int jewelsInstalled = 1;

    private void Start()
    {
        //jewelsInstalled = jewelImages.Length;
        ModifyJewelCount(3);

    }

    public void UpdateJewelImage(float factor)
    {
        float subamount = 100 / jewelsInstalled;
        for (int i = 0; i < jewelsInstalled; i++)
        {
            float specificFactor = Mathf.Clamp01((factor - (subamount * i)) / subamount);
            jewelImages[i].color = Color.HSVToRGB(0, specificFactor, 1);
            if (i == 0 && specificFactor < 1)
            {
                Debug.Log("insuff");
                ProvideFeedbackAboutInsufficientEnergy(true);
            }
            if (i == 0 && specificFactor >= 1)
            {
                Debug.Log("plenty");
                ProvideFeedbackAboutInsufficientEnergy(false);
            }
        }
    }

    public void ModifyJewelCount(int newJewelCount)
    {
        jewelsInstalled = newJewelCount;
        foreach (var jewel in jewelImages)
        {
            jewel.color = Color.clear;
        }
    }

    public void ProvideFeedbackAboutInsufficientEnergy(bool isInsufficient)
    {
        if (isInsufficient)
        {
            ParticleSystem.EmissionModule em = jewel0PS.emission;
            em.rateOverTime = 6;
        }
        else
        {
            ParticleSystem.EmissionModule em = jewel0PS.emission;
            em.rateOverTime = 0;
        }
    }
}
