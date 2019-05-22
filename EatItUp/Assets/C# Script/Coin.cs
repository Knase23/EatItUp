using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Pac")
        {
            //Give the Player point
            PlayerHandler.INSTANCE.GivePointsForPacman();
            //"Remove" the coin
            gameObject.SetActive(false);
        }
    }
}
