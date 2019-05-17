using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{

    private InputController controller;
    private int value;
    private PortriaitPlayerView playerView;
    private void Start()
    {
        controller = GetComponent<InputController>();
        List<PortriaitPlayerView> views =  PlayerPortraitsCanvas.INSTANCE.portriaits;
        foreach (var item in views)
        {
            if (item.name == name)
            {
                playerView = item;
                break;
            }
        }

    }
    public void AddToValue(int amount)
    {
        value += amount;
        playerView.SetScoreText(value);
        //Update Value in key for player
        if(GameManager.INSTANCE.IsTheHost())
            DiscordLobbyService.INSTANCE.SetMetaDataOfMember(controller.id, "Score", value.ToString());

    }
    public void UpdateBasedOnLobbyMemeber()
    {
        string newValue = DiscordLobbyService.INSTANCE.GetMetaDataOfMember(controller.id, "Score");
        value = int.Parse(newValue);

    }
    public void SetValue(int v)
    {
        value = v;
        playerView.SetScoreText(value);
    }
}
