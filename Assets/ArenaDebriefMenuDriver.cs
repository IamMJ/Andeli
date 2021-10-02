using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using AllIn1SpriteShader;

public class ArenaDebriefMenuDriver : MonoBehaviour
{
    WordMakerMemory pm;
    GameController gc;

    string outcomeText_Win = "VICTORY!";
    string outcomeText_Loss = "defeat...";

    [SerializeField] Image victorImage = null;
    [SerializeField] Image loserImage = null;
    [SerializeField] Image loserSilhouetteImage = null;
    [SerializeField] Material loserFadeMaterial = null;

    [SerializeField] TextMeshProUGUI outcomeTMP = null;
    [SerializeField] TextMeshProUGUI timeTMP = null;
    [SerializeField] TextMeshProUGUI powerDealtTMP = null;
    [SerializeField] TextMeshProUGUI wordsSpelledTMP = null;
    [SerializeField] TextMeshProUGUI bestWordTMP = null;

    //param
    float timeBeforeLoserFade = 1f;
    float timeToFadeLoser = 4f;

    //state
    float timeSinceCreation = 0f;
    bool isFadedAlready = false;

    public void SetupDebriefMenu(GameController gcRef, bool didPlayerWin, GameObject player, GameObject enemy, float timeInArena)
    {
        gc = gcRef;
        pm = player.GetComponent<WordMakerMemory>();
        if (didPlayerWin)
        {
            outcomeTMP.text = outcomeText_Win;
            victorImage.sprite = player.GetComponent<SpriteRenderer>().sprite;
            loserImage.sprite = enemy.GetComponent<SpriteRenderer>().sprite;
            loserSilhouetteImage.sprite = enemy.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            outcomeTMP.text = outcomeText_Loss;
            victorImage.sprite = enemy.GetComponent<SpriteRenderer>().sprite;
            loserImage.sprite = player.GetComponent<SpriteRenderer>().sprite;
            loserSilhouetteImage.sprite = player.GetComponent<SpriteRenderer>().sprite;
        }

        WordMakerMemory.ArenaData ad = pm.GetCurrentArenaData();
        timeTMP.text = "Time: " + timeInArena.ToString();
        powerDealtTMP.text = "Power Dealt: " + ad.powerDealtByPlayer.ToString();
        wordsSpelledTMP.text = "Words Spelled: " + ad.wordsSpelledByPlayer.ToString();
        bestWordTMP.text = ad.bestWordSpelledByPlayer + " - " + ad.currentBestSinglePowerGain.ToString();
        pm.ResetCurrentArenaData();
    }

    private void Update()
    {
        if (isFadedAlready) { return; }
        timeSinceCreation += Time.deltaTime;
        if (timeSinceCreation > timeBeforeLoserFade)
        {
            float factor = (timeSinceCreation - timeBeforeLoserFade) / (timeToFadeLoser);
            loserFadeMaterial.SetFloat("_FadeAmount", factor);
            if (timeSinceCreation > (timeBeforeLoserFade + timeToFadeLoser))
            {
                isFadedAlready = true;
            }
        }

    }

    public void CloseArenaMenu()
    {
        Destroy(gameObject);
    }
}
