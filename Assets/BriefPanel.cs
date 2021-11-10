using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BriefPanel : UI_Panel
{
    [SerializeField] GameObject[] otherGOs = null;
    [SerializeField] Image enemyImage = null;
    [SerializeField] Image settingImage = null;
    [SerializeField] TextMeshProUGUI settingNameTMP = null;
    [SerializeField] TextMeshProUGUI settingDescTMP_0 = null;
    [SerializeField] TextMeshProUGUI settingDescTMP_1 = null;
    [SerializeField] TextMeshProUGUI settingDescTMP_2 = null;

    ArenaStarter arenaStarter;
    GameController gc;

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

    public void PopulateBriefPanel(ArenaStarter arst, ArenaSetting currentArenaSetting)
    {
        arenaStarter = arst;
        ShowHideElements(true);

        enemyImage.sprite = arst.ArenaEnemyPrefab.GetComponent<SpriteRenderer>().sprite;
        settingImage.sprite = currentArenaSetting.settingIcon;
        settingNameTMP.text = currentArenaSetting.settingName;
        settingDescTMP_0.text = currentArenaSetting.settingDesc_0;
        settingDescTMP_1.text = currentArenaSetting.settingDesc_1;
        settingDescTMP_2.text = currentArenaSetting.settingDesc_2;

    }

    //public void ShowHideEntirePanel(bool shouldBeShown)
    //{

    //    if (shouldBeShown)
    //    {
    //        gc.PauseGame();
    //        foreach (var GO in otherGOs)
    //        {
    //            GO.SetActive(true);
    //        }
    //        GetComponent<Image>().enabled = true;
    //        enemyImage.enabled = true;
    //        settingImage.enabled = true;
    //        settingNameTMP.enabled = true;
    //        settingDescTMP_0.enabled = true;
    //        settingDescTMP_1.enabled = true;
    //        settingDescTMP_2.enabled = true;
    //    }
    //    else
    //    {
    //        gc.ResumeGameSpeed(false);
    //        foreach (var GO in otherGOs)
    //        {
    //            GO.SetActive(false);
    //        }
    //        GetComponent<Image>().enabled = false;
    //        enemyImage.enabled = false;
    //        settingImage.enabled = false;
    //        settingNameTMP.enabled = false;
    //        settingDescTMP_0.enabled = false;
    //        settingDescTMP_1.enabled = false;
    //        settingDescTMP_2.enabled = false;
    //    }
    //}


}
