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

        if(PlayerHandler.inst)
        {
            var useManager = lobbyManager.GetMemberUsers(lobbyId);
            List<Discord.User> users = new List<Discord.User>();
            users.AddRange(useManager);
            InputController[] controllers = PlayerHandler.inst.GetInputControllers();
            for (int i = 0; i < portriaitPlayerViews.Count; i++)
            {
                if (controllers[i].id != 0)
                {
                    foreach (var item in users)
                    {
                        if (item.Id == controllers[i].id)
                        {
                            UpdateDisplayForId(item, i);
                            break;
                        }
                    }
                }
                else
                {
                    portriaitPlayerViews[i].SetToDefault();
                }
            }
        }
        else
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
                    if (GameManager.INSTANCE.IsTheHost())
                        DiscordLobbyService.INSTANCE.SetMetaDataOfMember(item.Id, "Score", score);
                }


                portriaitPlayerViews[i++].SetUser(item.Username, "Score: " + score, avatar);
            }
            for (int j = i; j < portriaitPlayerViews.Count; j++)
            {
                portriaitPlayerViews[j].SetToDefault();
            }
        }




    }

    public void UpdateDisplayForId(Discord.User user , int index)
    {
        var imageManager = DiscordManager.INSTANCE.GetDiscord().GetImageManager();
        if (user.Id != 0)
        {
            Texture2D avatar = null;
            try
            {
                avatar = imageManager.GetTexture(Discord.ImageHandle.User(user.Id));
            }
            catch (Discord.ResultException)
            {

                FetchMemberAvatar(user.Id);
            }
            string score = "0";
            try
            {
                score = lobbyManager.GetMemberMetadataValue(DiscordLobbyService.INSTANCE.currentLobbyId, user.Id, "Score");
            }
            catch (System.Exception)
            {
                if (GameManager.INSTANCE.IsTheHost())
                    DiscordLobbyService.INSTANCE.SetMetaDataOfMember(user.Id, "Score", score);
            }


            portriaitPlayerViews[index].SetUser(user.Username, "Score: " + score, avatar);
        }
        else
        {
            portriaitPlayerViews[index].SetToDefault();
        }

    }
    /// <summary>
    /// Gets the Avatar for a user, so it can later be used;
    /// </summary>
    /// <param name="userId"></param>
    private void FetchMemberAvatar(long userId)
    {
        DiscordManager.INSTANCE.GetDiscord().GetImageManager().Fetch(Discord.ImageHandle.User(userId), (result, handleResult) =>
        {
            if (result != Discord.Result.Ok)
                Debug.Log(result);


        });
    }
}
