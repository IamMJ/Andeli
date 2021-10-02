using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ArenaBriefMenuDriver : MonoBehaviour
{
    ArenaStarter arenaStarter;
    [SerializeField] TextMeshProUGUI settingDescriptionTMP = null;
    [SerializeField] Image settingImage = null;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleRetreatClick()
    {
        arenaStarter.RetreatFromArena();
    }

    public void HandleAttackClick()
    {
        arenaStarter.StartArena();

    }

    public void SetupArenaBriefMenu(ArenaStarter arst, Sprite settingSprite, string settingDesc)
    {
        arenaStarter = arst;
        settingImage.sprite = settingSprite;
        settingDescriptionTMP.text = settingDesc;

    }
}
