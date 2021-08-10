using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerMeter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI powerMeterTMP = null;

    //state
    public int CurrentPower { get; private set; } = 0;

    public void IncreasePower(int amount)
    {
        CurrentPower += amount;
        powerMeterTMP.text = CurrentPower.ToString();
    }

    public void ClearPowerLevel()
    {
        CurrentPower = 0;
        powerMeterTMP.text = "";
    }

}
