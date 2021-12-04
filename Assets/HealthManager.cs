using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] Image enemyHealthBar = null;
    [SerializeField] Image playerHealthBar = null;
    [SerializeField] Image playerImage = null;
    [SerializeField] Image enemyImage = null;
    CinemachineImpulseSource cis;

    public Action<bool> OnArenaVictory_TrueForPlayerWin;

    private void Start()
    {
        cis = Camera.main.GetComponentInChildren<CinemachineImpulseSource>();
    }

    public void SetHealthBarIcons(Sprite playerSprite, Sprite enemySprite)
    {
        playerImage.sprite = playerSprite;
        enemyImage.sprite = enemySprite;
    }

    public void ResetHealthBars()
    {
        enemyHealthBar.fillAmount = 1;
        playerHealthBar.fillAmount = 1;
    }

    public void ModifyPlayerHealth(float factorAmount)
    {
        playerHealthBar.fillAmount += factorAmount;
        if (factorAmount < 0)
        {
            cis.GenerateImpulse(Mathf.Abs(factorAmount * 100));
        }

        DetectWinLoss();
    }

    public void ModifyEnemyHealth(float factorAmount)
    {
        enemyHealthBar.fillAmount += factorAmount;
        DetectWinLoss();
    }

    #region Helpers
    private void DetectWinLoss()
    {
        if (enemyHealthBar.fillAmount <= 0)
        {
            OnArenaVictory_TrueForPlayerWin?.Invoke(true);
        }

        if (playerHealthBar.fillAmount <= 0)
        {
            OnArenaVictory_TrueForPlayerWin?.Invoke(false);
        }
    }

    #endregion




}

