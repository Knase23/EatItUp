using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpdater : MonoBehaviour
{
    public static LevelUpdater INSTANCE;
    private void Awake()
    {
        INSTANCE = this;
    }

    public void CheckEndOfLevel()
    {
        if (Coin.numberOfActiveCoins <= 0)
        {
            //Reset the "level" 
            foreach (var item in Coin.allDeactivatedCoins)
            {
                item.gameObject.SetActive(true);
                Coin.numberOfActiveCoins++;
            }
            Coin.allDeactivatedCoins.Clear();
            if (GameManager.INSTANCE.IsTheHost())
            {
                foreach (var item in ScoreUpdater.INSTANCE.allScores)
                {
                    ScoreUpdater.CheckHigestOrLowestScore(item);
                }
                //Set the player with lowest score to be pacman
                // If there is a tie, take the first that comes up in the list
                Character pac = PlayerHandler.INSTANCE.currentHolderOfPac.controlledCharacter;
                pac.SwitchControllers(ScoreUpdater.lowestScore.controller.controlledCharacter);

                //Reset there positions
                PlayerHandler.INSTANCE.ResetCharactersPosition();
                PlayerHandler.INSTANCE.SyncInputControllers();
            }

        }
    }
}
