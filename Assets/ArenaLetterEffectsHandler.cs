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

    public void ApplyLetterEffectOnPickup(LetterTile activatedLetter, GameObject callingWMM, int letterIndex, bool hasUI)
    {
        GameObject activatedLetterInWordBar = uid.GetGameObjectAt(letterIndex);
        GameObject FX;

        switch (activatedLetter.Ability)
        {
            case TrueLetter.Ability.Normal:
                //
                break;

            case TrueLetter.Ability.Shiny:
                int power = activatedLetter.Power;
                if (hasUI)
                {
                    callingWMM.GetComponent<WordBuilder>().IncreasePower(power); //power has already been added once with normal pickup. This effectively doubles the letter power.
                    FX = Instantiate(letterFX_Shiny, activatedLetterInWordBar.transform);
                    FX.layer = 5;
                }
                break;

            case TrueLetter.Ability.Frozen:
                if (hasUI)
                {
                    FX = Instantiate(letterFX_Frozen, activatedLetterInWordBar.transform);
                    FX.layer = 5;
                }
                break;

            case TrueLetter.Ability.Lucky:
                int amount = Mathf.RoundToInt(activatedLetter.Power / 2f);
                callingWMM.GetComponent<WordBuilder>().IncreaseWordLengthBonus(amount);
                if (hasUI)
                {
                    FX = Instantiate(letterFX_Lucky, activatedLetterInWordBar.transform);
                    ParticleSystem.EmissionModule em = FX.GetComponent<ParticleSystem>().emission;
                    em.rateOverTime = amount;
                    FX.layer = 5;
                }
                break;

        }
    }

    
}
