using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public static int numberOfActiveCoins = 0;
    public static List<Coin> allDeactivatedCoins = new List<Coin>();
    private void Start()
    {
        numberOfActiveCoins++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Pac")
        {
            //Give the Player point
            PlayerHandler.INSTANCE.GivePointsToPacman();
            //"Remove" the coin
            numberOfActiveCoins--;
            allDeactivatedCoins.Add(this);
            gameObject.SetActive(false);
            LevelUpdater.INSTANCE.CheckEndOfLevel();
        }
    }
}
