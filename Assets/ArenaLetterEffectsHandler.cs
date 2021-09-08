using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaLetterEffectsHandler : MonoBehaviour
{
    /// This class is intended to receive Letter effects, plus the caller's identity, and then manifest them.

    //init
    ArenaBuilder ab;
    UIDriver uid;

    [SerializeField] GameObject letterFX_Shiny = null;
    [SerializeField] GameObject letterFX_Frozen = null;
    [SerializeField] GameObject letterFX_Lucky = null;

    private void Start()
    {
        ab = GetComponent<ArenaBuilder>();
        uid = FindObjectOfType<UIDriver>();
    }

    public void ApplyLetterEffectOnPickup(LetterTile activatedLetter, GameObject callingWMM, int letterIndex)
    {
        GameObject activatedLetterInWordBar = uid.GetGameObjectAt(letterIndex);
        GameObject FX;

        switch (activatedLetter.Ability)
        {
            case TrueLetter.Ability.Nothing:
                //
                break;

            case TrueLetter.Ability.Shiny:
                int power = activatedLetter.Power;
                callingWMM.GetComponent<WordBuilder>().IncreasePower(power); //power has already been added once with normal pickup. This effectively doubles the letter power.
                FX = Instantiate(letterFX_Shiny, activatedLetterInWordBar.transform);
                FX.layer = 5;
                break;

            case TrueLetter.Ability.Frozen:
                FX = Instantiate(letterFX_Frozen, activatedLetterInWordBar.transform);
                FX.layer = 5;
                break;

            case TrueLetter.Ability.Lucky:
                FX = Instantiate(letterFX_Lucky, activatedLetterInWordBar.transform);
                int amount = Mathf.RoundToInt(activatedLetter.Power / 2f);
                ParticleSystem.EmissionModule em = FX.GetComponent<ParticleSystem>().emission;
                em.rateOverTime = amount;
                callingWMM.GetComponent<WordBuilder>().IncreaseWordLengthBonus(amount);
                FX.layer = 5;
                break;

        }
    }

    public void ApplyLetterEffectOnFiring(LetterTile activatedLetter, GameObject callingWMM)
    {
        switch (activatedLetter.Ability)
        {
            case TrueLetter.Ability.Nothing:
                //
                break;

            case TrueLetter.Ability.Shiny:
                break;

            case TrueLetter.Ability.Frozen:
                // Freezing combines the Frozen letters power with the wordlength to get actual freezing penalty to apply
                float freezePower = activatedLetter.Power / 10f * callingWMM.GetComponent<WordBuilder>().CurrentPower;
                if (callingWMM.GetComponent<PlayerInput>())  // This 'if' should find something unique to a player
                {
                    GameObject[] enemies = ab.GetEnemiesInArena();
                    foreach (var enemy in enemies)
                    {
                        enemy.GetComponent<SpeedKeeper>().FreezeWordMaker(freezePower);
                    }
                }
                break;

            case TrueLetter.Ability.Lucky:
                //
                break;

        }
    }
}
