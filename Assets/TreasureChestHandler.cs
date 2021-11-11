using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChestHandler : MonoBehaviour
{
    GameController gc;
    GameObject player;
    RewardPanel rpd;
    void Start()
    {
        rpd = FindObjectOfType<RewardPanel>();
        gc = FindObjectOfType<GameController>();
        gc.OnGameStart += HandleGameStart;
    }

    private void HandleGameStart()
    {
        player = gc.GetPlayer();
    }
    // Update is called once per frame

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == player)
        {
            //TODO play creaking open-chest sound
            rpd.ActivateRewardPanel(1);
            Destroy(gameObject);
        }
    }
}
