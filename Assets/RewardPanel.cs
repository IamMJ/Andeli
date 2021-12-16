using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RewardPanel : UI_Panel
{
    [SerializeField] TextMeshProUGUI glyphRewardTMP = null;
    [SerializeField] Image abilityRewardImage = null;
    [SerializeField] TextMeshProUGUI abilityRewardDesc = null;
    int amountToGive;
    UI_Controller uic;
    LetterTile sourceLT;
    PlayerMemory pm;
    Librarian lib;

    void Start()
    {
        lib = Librarian.GetLibrarian();
        uic = lib.ui_Controller;
        sourceLT = lib.GetSourceLetterTile();
    }

    public void HandleAccceptRewardClick()
    {
        //TODO play a cha-ching sound
        uic.SetContext(UI_Controller.Context.Upgrades);
    }

    public void SetRewardCurrencyAmount(int amount)
    {
        amountToGive = amount;
        if (!pm)
        {
            pm = lib.gameController.GetPlayer().GetComponent<PlayerMemory>();
        }
        pm.AdjustMoney(amountToGive);
    }

    public void SetRewardedAbility(TrueLetter.Ability newAbilityGained)
    {
        abilityRewardImage.sprite = sourceLT.GetSpriteColorFromAbility(newAbilityGained).Sprite;
        abilityRewardImage.color = sourceLT.GetSpriteColorFromAbility(newAbilityGained).Color;

        abilityRewardDesc.text = UpgradePanelDescriptionHelper.GetDescriptionForAbility(newAbilityGained);

        if (!pm)
        {
            pm = lib.gameController.GetPlayer().GetComponent<PlayerMemory>();
        }
        pm.LearnNewAbility(newAbilityGained);

    }
}
