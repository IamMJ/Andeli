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
    [SerializeField] GameObject letterFX_Wispy = null;
    [SerializeField] GameObject letterFX_Mystic = null;
    [SerializeField] GameObject letterFX_Healthy = null;
    [SerializeField] GameObject letterFX_Heavy = null;
    [SerializeField] GameObject letterFX_Armored = null;
    [SerializeField] GameObject letterFX_Charged = null;

    private void Start()
    {
        ab = GetComponent<ArenaBuilder>();
        uid = FindObjectOfType<UIDriver>();
    }

    public void ApplyLetterEffectOnPickup(LetterTile activatedLetter, GameObject callingWMM, int indexInWord, bool hasUI)
    {
        GameObject activatedTileInWordBar = uid.GetTileForLetterBasedOnIndexInWord(indexInWord);
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
                    FX = Instantiate(letterFX_Shiny, activatedTileInWordBar.transform);
                    FX.layer = 5;
                }
                break;

            case TrueLetter.Ability.Frozen:
                if (hasUI)
                {
                    FX = Instantiate(letterFX_Frozen, activatedTileInWordBar.transform);
                    FX.layer = 5;
                }
                break;

            case TrueLetter.Ability.Lucky:
                int amount = Mathf.RoundToInt(activatedLetter.Power / 2f);
                callingWMM.GetComponent<WordBuilder>().IncreaseWordLengthBonus(amount);
                if (hasUI)
                {
                    FX = Instantiate(letterFX_Lucky, activatedTileInWordBar.transform);
                    ParticleSystem.EmissionModule em = FX.GetComponent<ParticleSystem>().emission;
                    em.rateOverTime = amount;
                    FX.layer = 5;
                }
                break;

            case TrueLetter.Ability.Wispy:
                if (hasUI)
                {
                    FX = Instantiate(letterFX_Wispy, activatedTileInWordBar.transform);
                    FX.layer = 5;
                }
                break;

        }
    }

    public void RemoveLetterParticleEffect(int indexInWord, bool hasUI)
    {
        if (hasUI)
        {
            uid.RemoveParticleEffectsAtIndexInWord(indexInWord);
        }
    }
}
