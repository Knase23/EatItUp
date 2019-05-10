﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
public class DiscordActivityService : MonoBehaviour
{
    public static DiscordActivityService INSTANCE;
    ActivityManager manager;
    private void Awake()
    {
        if (INSTANCE)
        {
            Destroy(this);
        }
        else
        {
            INSTANCE = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        manager = DiscordManager.INSTANCE.GetDiscord().GetActivityManager();
        manager.ClearActivity((result) =>
        {
            if (result == Result.Ok)
            {
                //Debug.Log("Succeded with clearing activity");
            }
            else
            {
                Debug.LogError(result);
            }
        });
        manager.RegisterCommand("ProjectDiscord.exe://run");

        //Subscribe Functions
        manager.OnActivityInvite += Manager_OnActivityInvite;
        manager.OnActivityJoin += Manager_OnActivityJoin;
        manager.OnActivityJoinRequest += Manager_OnActivityJoinRequest;
        manager.OnActivitySpectate += Manager_OnActivitySpectate;
    }
    public void OnLobbyUpdate(long lobbyId)
    {
        Activity(new ActivityInformation("In Lobby", "Testing"));
    }

    #region Subscribe Functions
    private void Manager_OnActivitySpectate(string secret)
    {
        Debug.Log("Spectate on other player");
    }

    private void Manager_OnActivityJoinRequest(ref User user)
    {
        //Fires when a user asks to join the current user's game.
        Debug.Log(user.Username + " Request to Join");
        DiscordLobbyService.INSTANCE.ConnectToLobby();

    }

    private void Manager_OnActivityJoin(string secret)
    {
        Debug.Log("Fires when a user accepts a game chat invite or recives confirmation from Asking to Join");
        DiscordLobbyService.INSTANCE.ConnectToLobbyWithActivitySecret(secret);
    }

    private void Manager_OnActivityInvite(ActivityActionType type, ref User user, ref Activity activity)
    {
        //Fires when the user receives a join or spectate invite.
        Debug.Log(user.Username + (type == ActivityActionType.Join ? " wants to Join" : " wants to Spectating"));
    }
    #endregion

    public void Activity(ActivityInformation information, TimeStampInformation timeStamp = new TimeStampInformation(), AssetInformation asset = new AssetInformation())
    {
        Activity activity = new Activity()
        {

            ApplicationId = DiscordManager.INSTANCE.ApplicationId,
            State = information.PartyStatus,
            Details = information.WhatPlayerIsDoing,

        };
        #region Additional Activity Information
        if (timeStamp.StartStamp != 0)
        {
            activity.Timestamps = new ActivityTimestamps()
            {
                Start = timeStamp.StartStamp,
                End = timeStamp.EndStamp
            };
        }
        activity.Assets = new ActivityAssets()
        {
            LargeImage = asset.LargeImageKey,
            LargeText = asset.LargeImageText,
            SmallImage = asset.SmallImageKey,
            SmallText = asset.SmallImageText
        };

        DiscordLobbyService ls = DiscordLobbyService.INSTANCE;

        Lobby lob = ls.GetLobby();
        if (lob.Id != 0)
        {
            activity.Party = new ActivityParty()
            {
                Id = lob.Id.ToString(),
                Size = new PartySize()
                {
                    CurrentSize = ls.GetMemberCount(),
                    MaxSize = (int)lob.Capacity
                },
                
            };
            activity.Secrets = new ActivitySecrets
            {
                Join =  lob.Id +":"+ lob.Secret,
            };
        }
        if (lob.Id != 0)
        {
            activity.Instance = true;
        }
        #endregion

        manager.UpdateActivity(activity, (result) =>
        {
            if (result == Result.Ok)
            {
                //Debug.Log("Succeded with Updating activity");
            }
            else
            {
                Debug.LogError(result);
            }
        });
    }
    public void Invite(User user)
    {
        manager.SendInvite(user.Id, ActivityActionType.Join, "THIS IS A TEST", (result) =>
        {
            if (result == Result.Ok)
            {
                //Debug.Log("Succeded with Updating activity");
            }
            else
            {
                Debug.LogError(result);
            }
        });
    }
    public void JoinOther(string secret)
    {
        DiscordLobbyService.INSTANCE.ConnectToLobbyWithActivitySecret(secret);
    }
    #region Structs for Service
    public struct ActivityInformation
    {
        public string PartyStatus;
        public string WhatPlayerIsDoing;

        public ActivityInformation(string partyStatus = "", string whatPlayerIsDoing = "")
        {
            PartyStatus = partyStatus;
            WhatPlayerIsDoing = whatPlayerIsDoing;
        }
    }
    public struct TimeStampInformation
    {
        public long StartStamp;
        public long EndStamp;
        public TimeStampInformation(long startStamp = 0, long endStamp = 0)
        {
            StartStamp = startStamp;
            EndStamp = endStamp;
        }
    }
    public struct AssetInformation
    {
        public string LargeImageKey;
        public string LargeImageText;
        public string SmallImageKey;
        public string SmallImageText;

        public AssetInformation(string largeImageKey = "None", string largeImageText = "", string smallImageKey = "None", string smallImageText = "")
        {
            LargeImageKey = largeImageKey;
            LargeImageText = largeImageText;
            SmallImageKey = smallImageKey;
            SmallImageText = smallImageText;
        }
    }
    #endregion
}
