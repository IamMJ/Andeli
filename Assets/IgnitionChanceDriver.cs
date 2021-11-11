using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IgnitionChanceDriver : UI_Panel
{
    [SerializeField] GameObject ignitionChancePanel = null;
    [SerializeField] TextMeshProUGUI ignitionChanceTMP = null;

    public void SetIgnitionChance(int modifiedWordLength)
    {
        //maybe have an animated flame icon that grows in size depending on the ignition chance
        int chance = modifiedWordLength * 5;
        ignitionChanceTMP.text = chance.ToString() + "%";
    }

}
