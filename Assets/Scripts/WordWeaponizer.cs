using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WordWeaponizer : MonoBehaviour
{
    [SerializeField] GameObject puffPrefab = null;
    [SerializeField] GameObject normalSpellPrefab = null;
    [SerializeField] GameObject freezeSpellPrefab = null;
    [SerializeField] GameObject wispySpellPrefab = null;

    GameController gc;
    WordBuilder wbd;
    DebugHelper dh;
    WordValidater wv;
    VictoryMeter vm;
    [SerializeField] WordMakerMemory memory;
    ArenaBuilder ab;
    ArenaLetterEffectsHandler aleh;
    UIDriver uid;
    JewelManager jm;
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
    float energyRegenRate_Target = 0.5f; // units per second;
    float energyRegenRate_Current;
    float energyRegenDriftRate = 0.1f; //how fast the current energy regen rate drifts back to its target. 
    float currentEnergyLevel;
    TrueLetter.Ability abilityToAutoIgnite = TrueLetter.Ability.Normal;

    public Action OnFireWord;

    void Start()
    {
        memory = GetComponent<WordMakerMemory>();
        wbd = GetComponent<WordBuilder>();
        
        if (GetComponent<PlayerInput>()) //Must be a player
        {
            isPlayer = true;
            powerSign = 1;
            uid = FindObjectOfType<UIDriver>();
            jm = FindObjectOfType<JewelManager>();
            jm.UpdateJewelImage(100);
        }
        gc = FindObjectOfType<GameController>();
        dh = FindObjectOfType<DebugHelper>();
        wv = FindObjectOfType<WordValidater>();
        vm = FindObjectOfType<VictoryMeter>();
        currentEnergyLevel = maxEnergy; //maxEnergy;
        energyRegenRate_Current = energyRegenRate_Target;
    }

    void Update()
    {
        energyRegenRate_Current = Mathf.MoveTowards(
            energyRegenRate_Current, energyRegenRate_Target, energyRegenDriftRate * Time.deltaTime);
        currentEnergyLevel += energyRegenRate_Current * Time.deltaTime;
        currentEnergyLevel = Mathf.Clamp(currentEnergyLevel, 0, maxEnergy);
        if (uid)
        {
            //uid.UpdateSpellEnergySlider(currentEnergyLevel);
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
    //public bool CheckSpendForLetterErasure()
    //{
    //    if (wbd.GetCurrentWord().Length == 0) { return false; }
    //    if (letterErasureCost <= currentEnergyLevel)
    //    {
    //        currentEnergyLevel -= letterErasureCost;
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    #region Public Methods
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
            memory.UpdateCurrentArenaData(wbd.CurrentPower, testWord);
            CreateWordPuff(testWord, wbd.CurrentPower);
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
            Debug.Log("insufficient energy to fire at this moment");
            return false;
        }
        else
        {
            testWord = wbd.GetCurrentWord();
            //playmem.IncrementWordCount(); // implement a memory for the enemy IF combat requires tracking played words
            CreateWordPuff(testWord, wbd.CurrentPower);
            FireKnownValidWord();
            wbd.ClearCurrentWord();
            currentEnergyLevel -= spellFiringCost;
            return true;
        }

    }

    public void FireKnownValidWord()
    {
        TargetBestEnemy();
        float spellpower = (wbd.CurrentPower + memory.GetCurrentArenaData().wordsSpelled 
            * ab.GetArenaSettingsHolder().arenaSetting.powerModifierForWordCount);
        CreateSpell(currentEnemy.transform, spellpower * powerSign, TrueLetter.Ability.Normal); ;
        foreach (var letter in wbd.GetLettersCollected())
        {
            if (letter.Ability_Player != abilityToAutoIgnite)
            {
                int roll = UnityEngine.Random.Range(1, 20);


                if (wbd.GetModifiedWordLength() >= roll)
                {
                    TriggerActiveLetterEffects(letter, gameObject, currentEnemy);
                }
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
        energyRegenRate_Current = energyRegenRate_Target;
        currentEnergyLevel = maxEnergy;
        jm.ProvideFeedbackAboutInsufficientEnergy(99);
    }

    #endregion
    private void CreateWordPuff(string word, int powerForColor)
    {
        GameObject puff = Instantiate(puffPrefab, transform.position, Quaternion.identity) as GameObject;
        WordPuff wordPuff = puff.GetComponent<WordPuff>();
        wordPuff.SetText(testWord);
        wordPuff.SetColorByPower(wbd.CurrentPower);
    }
    private void TriggerActiveLetterEffects(LetterTile activatedLetter, GameObject sourceWMM, GameObject targetWMM)
    {
        if (isPlayer)
        {
            switch (activatedLetter.Ability_Player)
            {

                case TrueLetter.Ability.Shiny:
                    //
                    break;

                case TrueLetter.Ability.Frozen:
                    // Freezing combines the Frozen letters power with the wordlength to get actual freezing penalty to apply
                    float freezePower = activatedLetter.Power_Player / 10f * sourceWMM.GetComponent<WordBuilder>().CurrentPower;
                    CreateSpell(targetWMM.transform, freezePower, TrueLetter.Ability.Frozen);
                    break;

                case TrueLetter.Ability.Lucky:
                    //
                    break;

                case TrueLetter.Ability.Wispy:
                    float speedBoost = activatedLetter.Power_Player / 10f * sourceWMM.GetComponent<WordBuilder>().CurrentPower;
                    CreateSpell(sourceWMM.transform, speedBoost, TrueLetter.Ability.Wispy);
                    break;

                case TrueLetter.Ability.Mystic:
                    float mysticPower = activatedLetter.Power_Player + sourceWMM.GetComponent<WordBuilder>().CurrentPower;
                    int count = Mathf.RoundToInt(mysticPower / 2);
                    ab.ltd.SpawnMysticLetters(count, mysticPower);
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
                    float freezePower = activatedLetter.Power_Player / 10f * sourceWMM.GetComponent<WordBuilder>().CurrentPower;
                    CreateSpell(targetWMM.transform, freezePower, TrueLetter.Ability.Frozen);
                    break;

                case TrueLetter.Ability.Lucky:
                    //
                    break;

                case TrueLetter.Ability.Wispy:
                    float speedBoost = activatedLetter.Power_Player / 10f * sourceWMM.GetComponent<WordBuilder>().CurrentPower;
                    CreateSpell(sourceWMM.transform, speedBoost, TrueLetter.Ability.Wispy);
                    break;

                case TrueLetter.Ability.Mystic:
                    float mysticPower = activatedLetter.Power_Player + sourceWMM.GetComponent<WordBuilder>().CurrentPower;
                    int count = Mathf.RoundToInt(mysticPower / 2);
                    ab.ltd.SpawnMysticLetters(count, mysticPower);
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
                spell.SetupSpell(target, vm, gc, spellPotency, TrueLetter.Ability.Normal);
                return;

            case TrueLetter.Ability.Frozen:
                spell = Instantiate(freezeSpellPrefab, transform.position, Quaternion.identity).GetComponent<SpellSeeker>();
                spell.GetComponent<Rigidbody2D>().velocity = (randRot * spell.transform.up) * spellSpeed;
                spell.SetupSpell(target, vm, gc, spellPotency, TrueLetter.Ability.Frozen);
                return;

            case TrueLetter.Ability.Wispy:
                spell = Instantiate(wispySpellPrefab, transform.position, Quaternion.identity).GetComponent<SpellSeeker>();
                spell.GetComponent<Rigidbody2D>().velocity = (randRot * spell.transform.up) * spellSpeed*3;
                spell.SetupSpell(target, vm, gc, spellPotency, TrueLetter.Ability.Wispy);
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
