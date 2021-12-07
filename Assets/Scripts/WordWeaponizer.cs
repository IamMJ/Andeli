using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WordWeaponizer : MonoBehaviour
{
    [SerializeField] GameObject puffPrefab = null;
    [SerializeField] GameObject normalSpellPrefab = null;
    [SerializeField] GameObject shinySpellPrefab = null;
    [SerializeField] GameObject freezeSpellPrefab = null;
    [SerializeField] GameObject wispySpellPrefab = null;
    [SerializeField] GameObject heavySpellPrefab = null;
    [SerializeField] AudioClip normalSpellCastClip = null;

    AudioSource auso;
    GameController gc;
    WordBuilder wbd;
    WordValidater wv;
    HealthManager hm;
    [SerializeField] WordMakerMemory memory;
    ArenaBuilder ab;
    ArenaLetterEffectsHandler aleh;
    CombatPanel uid;
    JewelManager jm;
    PlayerMemory pm;
    LetterTileDropper ltd;
    string testWord;


    //param
    float spellSpeed = 6.0f;
    public GameObject currentEnemy;
    float spellFiringCost = 20;
    float letterErasureCost = 10f;
    float maxEnergy = 100f;

    //state
    bool isPlayer = false;
    int powerSign = -1;
    float energyRegenRate_Target = 1.0f; // units per second; previously 0.5
    float energyRegenRate_Current;
    float energyRegenDriftRate = 0.1f; //how fast the current energy regen rate drifts back to its target. 
    [SerializeField] float currentEnergyLevel;
    TrueLetter.Ability abilityToAutoIgnite = TrueLetter.Ability.Normal;

    public Action OnFireWord;

    void Start()
    {
        Librarian lib = Librarian.GetLibrarian();
        ltd = lib.letterTileDropper;
        memory = GetComponent<WordMakerMemory>();
        wbd = GetComponent<WordBuilder>();
        auso = GetComponent<AudioSource>();

        if (GetComponent<PlayerInput>()) //Must be a player
        {
            isPlayer = true;
            powerSign = 1;

            uid = lib.ui_Controller.combatPanel;
            jm = uid.GetComponent<JewelManager>();
            jm.UpdateJewelImage(100);

            pm = GetComponent<PlayerMemory>();
            pm.SetBaseEnergyRegenRate(energyRegenRate_Target);
        }
        gc = lib.gameController;
        wv = lib.wordValidater;
        hm = lib.ui_Controller.combatPanel.GetComponent<HealthManager>();

        ResetEnergyStats();
    }

    public void ResetEnergyStats()
    {
        if (isPlayer)
        {
            energyRegenRate_Target = pm.GetBaseEnergyRegenRate(); ;
        }

        currentEnergyLevel = maxEnergy; //maxEnergy;
        energyRegenRate_Current = energyRegenRate_Target;
    }

    void Update()
    {
        if (!gc.isInArena) { return; }
        energyRegenRate_Current = Mathf.MoveTowards(
            energyRegenRate_Current, energyRegenRate_Target, energyRegenDriftRate * Time.deltaTime);
        currentEnergyLevel += energyRegenRate_Current * Time.deltaTime;
        currentEnergyLevel = Mathf.Clamp(currentEnergyLevel, 0, maxEnergy);
        if (uid)
        {
            if (gc.isInArena)
            {
                jm.UpdateJewelImage(currentEnergyLevel / maxEnergy * 100);
            }
        }
    }

    /// <summary>
    /// Checks the current energy supply. Returns 'true' if sufficient energy to erase a letter
    /// and deducts that amount. Returns 'false' if insufficient energy, but doesn't change energy supply.
    /// </summary>
    public bool CheckSpendForLetterErasure()
    {
        if (wbd.GetCurrentWord().Length == 0) { return false; }
        if (letterErasureCost <= currentEnergyLevel)
        {
            currentEnergyLevel -= letterErasureCost;
            return true;
        }
        else
        {
            return false;
        }
    }

    #region Public Methods

    public void ModifyEnergyRate(float multiplier)
    {
        energyRegenRate_Target *= multiplier;
        energyRegenRate_Current = energyRegenRate_Target;
    }

    public bool CheckIfSufficientEnergyToCast()
    {
        if (currentEnergyLevel >= spellFiringCost)
        {
            return true;
        }
        else { return false; }
    }

    public bool AttemptToFireWordAsPlayer()
    {
        //Check for sufficient Spell Energy...
        if (spellFiringCost > currentEnergyLevel)
        {
            Debug.Log("insufficient energy to fire at this moment");
            return false;
        }

        testWord = wbd.GetCurrentWord();
        if (wv.CheckWordValidity(testWord))
        {
            memory.UpdateCurrentArenaData(wbd.CurrentWordPack.Power, testWord);
            CreateWordPuff(testWord, wbd.CurrentWordPack.Power);
            FireKnownValidWord();
            currentEnergyLevel -= spellFiringCost;
            if (isPlayer)
            {
                //uid.UpdateSpellEnergySlider(currentEnergyLevel);
                jm.UpdateJewelImage(currentEnergyLevel / maxEnergy * 100);
            }
            return true;
        }
        else
        {
            // stun the player?
            return false;
        }
    }

    /// <summary>
    /// This is the same as 'AttemptToFireWordAsPlayer' except it does not check for word validity first. The NPC
    /// should be checking word validity earlier as part of its strategy.
    /// </summary>
    /// <returns></returns>
    public bool AttemptToFireWordAsNPC()
    {
        //Check for sufficient Spell Energy...
        if (spellFiringCost > currentEnergyLevel)
        {
            Debug.Log("NPC has insufficient energy to fire at this moment");
            return false;
        }
        else
        {
            testWord = wbd.GetCurrentWord();
            Debug.Log($"attempting to fire {testWord}");
            //playmem.IncrementWordCount(); // implement a memory for the enemy IF combat requires tracking played words
            CreateWordPuff(testWord, wbd.CurrentWordPack.Power);
            FireKnownValidWord();
            wbd.ClearLettersOnSword();
            currentEnergyLevel -= spellFiringCost;
            return true;
        }

    }

    public void FireKnownValidWord()
    {
        TargetBestEnemy();
        float spellpower = (wbd.CurrentWordPack.Power + memory.GetCurrentArenaData().wordsSpelled 
            * ab.GetArenaSettingsHolder().arenaSetting.powerModifierForWordCount);
        CreateSpell(currentEnemy.transform, spellpower * powerSign, TrueLetter.Ability.Normal); ;
        auso?.PlayOneShot(normalSpellCastClip);
        List<LetterTile> letterTilesInWord = wbd.GetLettersCollected();
        for (int index = 0; index < letterTilesInWord.Count; index++)
        {
            if (letterTilesInWord[index].Ability_Player != abilityToAutoIgnite)
            {
                int roll = UnityEngine.Random.Range(1, 20);


                if (wbd.CurrentWordPack.ModifiedWordLength >= roll)
                {
                    TriggerActiveLetterEffects(letterTilesInWord[index], gameObject, currentEnemy, index);
                }
            }
            else
            {
                TriggerActiveLetterEffects(letterTilesInWord[index], gameObject, currentEnemy, index);
            }

        }
        OnFireWord?.Invoke();
        
    }
    public void ModifyEnergyRegent(float amount)
    {
        energyRegenRate_Current += amount;
    }

    public void HandleArenaEntry()
    {
        ResetEnergyStats();
        jm.ProvideFeedbackAboutInsufficientEnergy(99);
    }

    #endregion
    private void CreateWordPuff(string word, int powerForColor)
    {
        GameObject puff = Instantiate(puffPrefab, transform.position, Quaternion.identity) as GameObject;
        WordPuff wordPuff = puff.GetComponent<WordPuff>();
        wordPuff.SetText(testWord);
        wordPuff.SetColorByPower(wbd.CurrentWordPack.Power);
    }
    private void TriggerActiveLetterEffects(LetterTile activatedLetter, GameObject sourceWMM, GameObject targetWMM, int indexOfLetterInWord)
    {
        if (isPlayer)
        {
            switch (activatedLetter.Ability_Player)
            {

                case TrueLetter.Ability.Shiny:
                    Debug.Log($" {activatedLetter.Ability_Player} just triggered");
                    float bonusPower = activatedLetter.Power_Player;
                    CreateSpell(targetWMM.transform, bonusPower, TrueLetter.Ability.Shiny);
                    break;

                case TrueLetter.Ability.Frozen:
                    Debug.Log($" {activatedLetter.Ability_Player} just triggered");
                    // Freezing combines the Frozen letters power with the wordlength to get actual freezing penalty to apply
                    float freezePower = activatedLetter.Power_Player / 10f * sourceWMM.GetComponent<WordBuilder>().CurrentWordPack.Power;
                    CreateSpell(targetWMM.transform, freezePower, TrueLetter.Ability.Frozen);
                    break;

                case TrueLetter.Ability.Lucky:
                    //
                    break;

                case TrueLetter.Ability.Wispy:
                    Debug.Log($" {activatedLetter.Ability_Player} just triggered");
                    float speedBoost = activatedLetter.Power_Player / 10f * sourceWMM.GetComponent<WordBuilder>().CurrentWordPack.Power;
                    CreateSpell(sourceWMM.transform, speedBoost, TrueLetter.Ability.Wispy);
                    break;

                case TrueLetter.Ability.Mystic:
                    Debug.Log($" {activatedLetter.Ability_Player} just triggered");
                    float mysticPower = activatedLetter.Power_Player; //+ sourceWMM.GetComponent<WordBuilder>().CurrentWordPack.Power;
                    int count = Mathf.Clamp(Mathf.RoundToInt(mysticPower / 2), 1, 5);
                    ltd.SpawnMysticLetters(count, mysticPower);
                    break;

                case TrueLetter.Ability.Heavy:
                    Debug.Log($" {activatedLetter.Ability_Player} just triggered");
                    float heavyPower = indexOfLetterInWord + 1;
                    CreateSpell(targetWMM.transform, heavyPower, TrueLetter.Ability.Heavy);
                    break;
            }
        }
        else
        {

            switch (activatedLetter.Ability_Enemy)
            {

                case TrueLetter.Ability.Shiny:
                    //
                    break;

                case TrueLetter.Ability.Frozen:
                    // Freezing combines the Frozen letters power with the wordlength to get actual freezing penalty to apply
                    float freezePower = activatedLetter.Power_Player / 10f * sourceWMM.GetComponent<WordBuilder>().CurrentWordPack.Power;
                    CreateSpell(targetWMM.transform, freezePower, TrueLetter.Ability.Frozen);
                    break;

                case TrueLetter.Ability.Lucky:
                    //
                    break;

                case TrueLetter.Ability.Wispy:
                    float speedBoost = activatedLetter.Power_Player / 10f * sourceWMM.GetComponent<WordBuilder>().CurrentWordPack.Power;
                    CreateSpell(sourceWMM.transform, speedBoost, TrueLetter.Ability.Wispy);
                    break;

                case TrueLetter.Ability.Mystic:
                    float mysticPower = activatedLetter.Power_Player + sourceWMM.GetComponent<WordBuilder>().CurrentWordPack.Power;
                    int count = Mathf.RoundToInt(mysticPower / 2);
                    ltd.SpawnMysticLetters(count, mysticPower);
                    break;
            }
        }

    }

    private void CreateSpell(Transform target, float spellPotency, TrueLetter.Ability spellType)
    {
        float amount = UnityEngine.Random.Range(-180f, 179f);
        Quaternion randRot = Quaternion.Euler(0, 0, amount);
        SpellSeeker spell;        

        switch (spellType)
        {
            case TrueLetter.Ability.Normal:
                spell = Instantiate(normalSpellPrefab, transform.position, Quaternion.identity).GetComponent<SpellSeeker>();
                spell.GetComponent<Rigidbody2D>().velocity = (randRot * spell.transform.up) * spellSpeed;
                spell.SetupSpell(target, hm, gc, spellPotency, TrueLetter.Ability.Normal);
                return;

            case TrueLetter.Ability.Shiny:
                spell = Instantiate(shinySpellPrefab, transform.position, Quaternion.identity).GetComponent<SpellSeeker>();
                spell.GetComponent<Rigidbody2D>().velocity = (randRot * spell.transform.up) * spellSpeed/2f;
                spell.SetupSpell(target, hm, gc, spellPotency, TrueLetter.Ability.Normal);
                return;

            case TrueLetter.Ability.Frozen:
                spell = Instantiate(freezeSpellPrefab, transform.position, Quaternion.identity).GetComponent<SpellSeeker>();
                spell.GetComponent<Rigidbody2D>().velocity = (randRot * spell.transform.up) * spellSpeed;
                spell.SetupSpell(target, hm, gc, spellPotency, TrueLetter.Ability.Frozen);
                return;

            case TrueLetter.Ability.Wispy:
                spell = Instantiate(wispySpellPrefab, transform.position, Quaternion.identity).GetComponent<SpellSeeker>();
                spell.GetComponent<Rigidbody2D>().velocity = (randRot * spell.transform.up) * spellSpeed*3;
                spell.SetupSpell(target, hm, gc, spellPotency, TrueLetter.Ability.Wispy);
                return;

            case TrueLetter.Ability.Heavy:
                spell = Instantiate(heavySpellPrefab, transform.position, Quaternion.identity).GetComponent<SpellSeeker>();
                float speed = spellSpeed / spellPotency;
                speed = Mathf.Clamp(speed, spellSpeed * 0.1f, spellSpeed * 0.5f);
                spell.GetComponent<Rigidbody2D>().velocity = (randRot * spell.transform.up) * speed;
                spell.SetupSpell(target, hm, gc, spellPotency, TrueLetter.Ability.Normal);
                return;
        }
    }

    private void TargetBestEnemy()
    {
        if (!ab)
        {
            ab = FindObjectOfType<ArenaBuilder>();
        }
        if (isPlayer)
        {

            currentEnemy = ab.GetEnemyInArena();
        }
        else
        {
            currentEnemy = gc.GetPlayer();
        }
    }



    #region Public Arena Parameter Setting

    public void SetupArenaParameters_AbilityToAutoIgnite(TrueLetter.Ability abilityToIgnite)
    {
        abilityToAutoIgnite = abilityToIgnite;
    }
    public void SetupArenaParameters_EnergyRegenRate(float energyRegenModifier)
    {
        energyRegenRate_Target *= energyRegenModifier;
    }

    #endregion
}
