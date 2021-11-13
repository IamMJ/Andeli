using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMemory : MonoBehaviour
{
    [SerializeField] List<string> knownKeywords = new List<string>(); //This is used to 
    //check if a particular bark or convo should be used or suppressed

    public Action<string> OnKeywordAdded;
    public Action OnAbilityGained;



    //state
    float BaseEnergyRegenRate;
    float BaseMoveSpeed;
    List<TrueLetter.Ability> knownAbilities = new List<TrueLetter.Ability>();

    private void Start()
    {
        knownAbilities.Add(TrueLetter.Ability.Normal);
        knownAbilities.Add(TrueLetter.Ability.Heavy);
    }

    #region Keyword public methods
    public bool CheckForPlayerKnowledgeOfARequiredKeyword(string testRequiredKeyword)
    {
        bool result = false;
        if (testRequiredKeyword == "")
        {
            result = true;
            return result;
        }
        foreach (var keyword in knownKeywords)
        {
            if (keyword == testRequiredKeyword)
            {
                result = true;
                return result;
            }
        }
        return result;
    }

    public bool CheckForPlayerKnowledgeOfABannedKeyword(string testBannedKeyword)
    {
        bool result = false;
        if (testBannedKeyword == "")
        {
            result = false;
            return result;
        }
        foreach (var keyword in knownKeywords)
        {
            if (keyword == testBannedKeyword)
            {
                result = true;
                return result;
            }
        }
        return result;
    }

    public void AddKeyword(string newKeyword)
    {
        knownKeywords.Add(newKeyword);
        OnKeywordAdded?.Invoke(newKeyword);

    }

    #endregion

    #region Player Stats public methods

    public float GetBaseMoveSpeed()
    {
        return BaseMoveSpeed;
    }

    public void AdjustBaseMoveSpeed(float adjustmentToSpeed)
    {
        BaseMoveSpeed += adjustmentToSpeed;
    }

    public void SetBaseMoveSpeed(float moveSpeed)
    {
        BaseMoveSpeed = moveSpeed;
    }

    public float GetBaseEnergyRegenRate()
    {
        return BaseEnergyRegenRate;
    }

    public void AdjustBaseEnergyRegenRate(float adjustmentToEnergy)
    {
        BaseEnergyRegenRate += adjustmentToEnergy;
    }

    public void SetBaseEnergyRegenRate(float regenRate)
    {
        BaseEnergyRegenRate = regenRate;
    }

    #endregion

    #region Inventory public methods

    public void LearnNewAbility(TrueLetter.Ability newAbility)
    {
        knownAbilities.Add(newAbility);
        OnAbilityGained?.Invoke();
    }

    public List<TrueLetter.Ability> GetAllKnownAbilities()
    {
        return knownAbilities;
    }

    #endregion
}
