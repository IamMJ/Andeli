using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordGlowDriver : MonoBehaviour
{
    [SerializeField] Image swordBGglow = null;
    [SerializeField] Image swordFGglow = null;

    float maxPowerForSwordGlow = 20;
    float glowRate = 0.05f;

    float targetSpellswordGlow = 0;
    float currentSpellswordGlow = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpellSwordGlow();
    }

    public void UpdateTargetSpellswordGlow(float newTarget)
    {
        targetSpellswordGlow = newTarget;

    }

    private void UpdateSpellSwordGlow()
    {
        currentSpellswordGlow = Mathf.MoveTowards(currentSpellswordGlow, targetSpellswordGlow, glowRate);
        float factor = currentSpellswordGlow / maxPowerForSwordGlow;
        factor = Mathf.Clamp01(factor);

        swordFGglow.fillAmount = factor;
        swordBGglow.fillAmount = factor * 0.97f;
    }
}
