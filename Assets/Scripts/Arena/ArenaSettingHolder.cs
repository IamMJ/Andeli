using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaSettingHolder : MonoBehaviour
{
    [SerializeField] ArenaSetting arenaSetting_Default = null;
    [SerializeField] public ArenaSetting arenaSetting_Specific = null;
    LetterTileDropper ltd;
    WordMakerMemory playerMemory;
    WordMakerMemory enemyMemory;
    WordWeaponizer wwz_Player;
    WordWeaponizer wwz_Enemy;
    WordBuilder wbd_Player;
    WordBuilder_NPC wbd_Enemy;
    UIDriver uid;

    

    public enum ArenaSettingOptions {Plain, Mists, Sandy, Blizzard, Jungle, Graveyard }

    public void SetupArena(LetterTileDropper letterTileDropper, WordMakerMemory playerPlayMem, WordMakerMemory enemyPlayMem,
        WordWeaponizer wwzPlayer, WordWeaponizer wwzEnemy, WordBuilder wbPlayer, WordBuilder_NPC wbEnemy, UIDriver uIDriver)
    {
        ltd = letterTileDropper;
        playerMemory = playerPlayMem;
        enemyMemory = enemyPlayMem;
        wwz_Player = wwzPlayer;
        wwz_Enemy = wwzEnemy;
        wbd_Player = wbPlayer;
        wbd_Enemy = wbEnemy;
        uid = uIDriver;
        //Modify various things affected in the arena by the arena setttings
        ImplementSelectedArenaSettings();

    }

    private void ImplementSelectedArenaSettings()
    {
        //Implement Plain first as the default
        ltd.SetupArenaParameters_LettersInWave(arenaSetting_Default.lettersPerWave);
        ltd.SetupArenaParameters_Lifetime(arenaSetting_Default.letterLifetime);
        ltd.SetupArenaParameters_LettersToIgnore(arenaSetting_Default.lettersToNotDrop);
        ltd.SetupArenaParameters_LettersAsMisty(arenaSetting_Default.percentageOfLettersAsMisty);

        switch (arenaSetting_Specific.aso)
        {

            case ArenaSettingOptions.Blizzard:
                
                return;

            case ArenaSettingOptions.Graveyard:

                return;

            case ArenaSettingOptions.Jungle:

                return;

            case ArenaSettingOptions.Mists:

                return;

            case ArenaSettingOptions.Sandy:
                ltd.SetupArenaParameters_LettersInWave(arenaSetting_Specific.lettersPerWave);
                ltd.SetupArenaParameters_Lifetime(arenaSetting_Specific.letterLifetime);
                return;

        }
    }

}
