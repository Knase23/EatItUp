using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPlayers : MonoBehaviour
{
    List<PortriaitPlayerView> portriaitPlayerViews = new List<PortriaitPlayerView>();
    Discord.LobbyManager lobbyManager;
    private void Start()
    {
        portriaitPlayerViews.AddRange(GetComponentsInChildren<PortriaitPlayerView>());
        lobbyManager = DiscordLobbyService.INSTANCE.lobbyManager;
        lobbyManager.OnLobbyUpdate += OnLobbyUpdate;
    }

    private void OnLobbyUpdate(long lobbyId)
    {
        var imageManager = DiscordManager.INSTANCE.GetDiscord().GetImageManager();
        int i = 0;
        foreach (var item in lobbyManager.GetMemberUsers(lobbyId))
        {
            Texture2D avatar = null;
            try
            {
                avatar = imageManager.GetTexture(Discord.ImageHandle.User(item.Id));
            }
            catch (Discord.ResultException)
            {
                
                FetchMemberAvatar(item.Id);
            }
            string score = "0";
            try
            {
                score = lobbyManager.GetMemberMetadataValue(lobbyId, item.Id, "Score");
            }
            catch (System.Exception)
            {
                DiscordLobbyService.INSTANCE.SetMetaDataOfMember(item.Id, "Score", score);
            }
            
                
            portriaitPlayerViews[i++].SetUser(item.Username, "Score: " + score, Color.yellow, avatar);
        }
        for (int j = i; j < portriaitPlayerViews.Count; j++)
        {
            portriaitPlayerViews[j].SetToDefault();
        }

        
    }
    private void FetchMemberAvatar(long userId)
    {
        DiscordManager.INSTANCE.GetDiscord().GetImageManager().Fetch(Discord.ImageHandle.User(userId), (result, handleResult) =>
        {
            if (result != Discord.Result.Ok)
                Debug.Log(result);


        });
    }
}
