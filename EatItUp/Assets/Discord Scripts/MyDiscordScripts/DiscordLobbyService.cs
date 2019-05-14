﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Discord;


public class DiscordLobbyService : MonoBehaviour
{
    public static DiscordLobbyService INSTANCE;

    public LobbyManager lobbyManager;

    UserManager userManager;
    public long currentLobbyId;
    public string currentSecret;
    public long currentOwnerId;
    Coroutine coroutine;

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
        lobbyManager = DiscordManager.INSTANCE.GetDiscord().GetLobbyManager();
        userManager = DiscordManager.INSTANCE.GetDiscord().GetUserManager();

        lobbyManager.OnLobbyUpdate += LobbyManager_OnLobbyUpdate;
        lobbyManager.OnLobbyUpdate += DiscordActivityService.INSTANCE.OnLobbyUpdate;
        lobbyManager.OnNetworkMessage += LobbyManager_OnNetworkMessage;

    }


    private void Update()
    {
        if (coroutine == null && currentLobbyId != 0)
        {
            coroutine = StartCoroutine(LobbyTransaction());
        }
    }

    private void LateUpdate()
    {
        lobbyManager.FlushNetwork();
    }

    private void LobbyManager_OnLobbyUpdate(long lobbyId)
    {

    }
    private void LobbyManager_OnNetworkMessage(long lobbyId, long userId, byte channelId, byte[] data)
    {
        switch (channelId)
        {
            case 0:
                Debug.Log(userId + "Wants To Move");
                PlayerHandler.inst?.GetInputControllerFromUserId(userId).SetDirection(new InputController.InputData(data));
                break;
            case 1:
                Debug.Log(userId + "Wants you to change scene");
                GameManager.LoadSceneData sceneToLoad = new GameManager.LoadSceneData(data);
                GameManager.INSTANCE.LoadScene(sceneToLoad.scene);
                break;
            case 2:
                Debug.Log(userId + "Wants you to change position of a object");
                Movement.MovmentData movmentData = new Movement.MovmentData(data);
                Transform theThingToMove = PlayerHandler.inst?.GetInputControllerFromUserId(userId).controlledCharacter.transform; //= movmentData.GetPosition();
                theThingToMove.position = movmentData.GetPosition();
                break;
            case 3:
                Debug.Log(userId +"Do this Stupid thing");
                PlayerHandler.PlayerHandlerData handlerData = new PlayerHandler.PlayerHandlerData(data);
                PlayerHandler.inst.SetAllPlayers(handlerData.orderSelected,handlerData.orderOfId);
                break;
            default:
                Debug.Log(userId + "Sendet messege not reqognized");
                break;
        }


    }


    // Functions to Use to Call from other scripts
    public void CreateLobby()
    {
        if (currentLobbyId != 0)
            return;

        var txn = lobbyManager.GetLobbyCreateTransaction();
        txn.SetCapacity(5);
        txn.SetType(LobbyType.Public);
        txn.SetMetadata("Something", "123");
        lobbyManager.CreateLobby(txn, (Result result, ref Lobby lobby) =>
        {
            SetCurrent(lobby.Id, lobby.Secret, lobby.OwnerId);
            try
            {
                InitNetworking(lobby.Id);
            }
            catch (ResultException result2)
            {
                Debug.Log(result2);
            }

            NewLobbyTransaction();

        });


    }
    public void UpdateLobbySize(uint numberOfLocalPlayersConnected)
    {
        if (currentLobbyId == 0)
            return;

        Debug.Log(numberOfLocalPlayersConnected);
        var transaction = lobbyManager.GetLobbyUpdateTransaction(currentLobbyId);
        transaction.SetCapacity(5 - numberOfLocalPlayersConnected);
        lobbyManager.UpdateLobby(currentLobbyId, transaction, (Result result) =>
         {
             if (result != Result.Ok)
                 Debug.Log(result);
         });
    }

    public void DisconnectLobby()
    {
        if (currentLobbyId == 0)
            return;

        Debug.Log("Try to leave lobby");
        lobbyManager.DisconnectLobby(currentLobbyId, (Result result) =>
        {
            if (result != Result.Ok)
                Debug.Log(result);
            else
            {
                Debug.Log("Left Lobby");
                SetCurrent(0, string.Empty, 0);
            }
        });
    }

    public void ConnectToLobby()
    {
        var l = GetLobby();
        lobbyManager.ConnectLobby(l.Id, l.Secret, (Result result, ref Lobby lobby) =>
        {
            //Debug.Log("ConnectToLobby");
            if (result == Result.Ok)
            {
                Debug.Log("It worked?");
                InitNetworking(lobby.Id);
                SetCurrent(lobby.Id, lobby.Secret, lobby.OwnerId);
                for (int i = 0; i < lobbyManager.MemberCount(lobby.Id); i++)
                {
                    var userId = lobbyManager.GetMemberUserId(lobby.Id, i);
                    
                }
            }
            else
            {
                //Debug.Log(result);
            }

        });
    }
    public void ConnectToLobbyWithActivitySecret(string activitySecret)
    {
        lobbyManager.ConnectLobbyWithActivitySecret(activitySecret, (Result result, ref Lobby lobby) =>
        {
            Debug.Log("ConnectToLobbyThroughSecret");
            if (result == Result.Ok)
            {
                InitNetworking(lobby.Id);

                SetCurrent(lobby.Id, lobby.Secret, lobby.OwnerId);

                for (int i = 0; i < lobbyManager.MemberCount(lobby.Id); i++)
                {
                    var userId = lobbyManager.GetMemberUserId(lobby.Id, i);
                    //FetchMemberAvatar(userId);
                }
            }
            else
            {
                Debug.Log(result);
            }
        });
    }


    //Getters

    public Lobby GetLobby()
    {
        if (lobbyManager == null || currentLobbyId == 0)
            return new Lobby();

        return lobbyManager.GetLobby(currentLobbyId);
    }
    public int GetMemberCount()
    {
        if (lobbyManager == null)
            return 0;

        return lobbyManager.MemberCount(currentLobbyId);
    }
    public IEnumerable<User> GetLobbyMembers()
    {
        if (currentLobbyId == 0)
            return null;
        return lobbyManager.GetMemberUsers(currentLobbyId);
    }


    // Statements 
    public bool IsInLobby()
    {
        return (currentLobbyId != 0);
    }
    public bool IsThereSpaceLeft()
    {
        return lobbyManager.MemberCount(currentLobbyId) < lobbyManager.GetLobby(currentLobbyId).Capacity;
    }
    public bool IsTheHost()
    {
        return currentOwnerId == 0 || userManager.GetCurrentUser().Id == currentOwnerId;
    }

    //Functions for this script
    private void SetCurrent(long lobbyId, string secret, long ownerId)
    {
        currentLobbyId = lobbyId;
        currentSecret = secret;
        currentOwnerId = ownerId;
    }


    IEnumerator LobbyTransaction()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        NewLobbyTransaction();
        coroutine = null;
        yield break;
    }

    void NewLobbyTransaction()
    {
        if (currentLobbyId == 0)
            return;

        var newTxn = lobbyManager.GetLobbyUpdateTransaction(currentLobbyId);

        #region Set Meta Data For Lobby
        if (SceneManager.GetActiveScene().name == "Game")
        {
            newTxn.SetLocked(true);
        }
        else
        {
            newTxn.SetLocked(false);
        }
        #endregion

        lobbyManager.UpdateLobby(currentLobbyId, newTxn, (newResult) =>
        {
            if (newResult == Result.Ok)
            {
                //Debug.Log("Lobby updated");
            }
            else
            {
                //Debug.Log(newResult, gameObject);
            }
        });
    }

    // We can create a helper method to easily connect to the networking layer of the lobby
    public void InitNetworking(System.Int64 lobbyId)
    {
        // First, connect to the lobby network layer
        lobbyManager.ConnectNetwork(lobbyId);

        // Next, deterministically open our channels
        // Reliable on 0, unreliable on 1
        lobbyManager.OpenNetworkChannel(lobbyId, 0, true);
        lobbyManager.OpenNetworkChannel(lobbyId, 1, true);
        lobbyManager.OpenNetworkChannel(lobbyId, 2, true);
        lobbyManager.OpenNetworkChannel(lobbyId, 3, true);
        // We're ready to go!
    }
    private void FetchMemberAvatar(long userId)
    {
        DiscordManager.INSTANCE.GetDiscord().GetImageManager().Fetch(Discord.ImageHandle.User(userId), (result, handleResult) =>
        {
            if (result != Discord.Result.Ok)
                Debug.Log(result);


        });
    }
    public void SendNetworkMessageToHost(byte channelId, byte[] data)
    {
        if (currentLobbyId == 0)
            return;
        lobbyManager.SendNetworkMessage(currentLobbyId, currentOwnerId, channelId, data);
    }

    public bool SendNetworkMessageToClients(byte channelId, byte[] data)
    {

        if (currentLobbyId == 0)
            return false;

        foreach (var item in lobbyManager.GetMemberUsers(currentLobbyId))
        {
            if(item.Id != currentOwnerId)
                lobbyManager.SendNetworkMessage(currentLobbyId, item.Id, channelId, data);
        }
        return true;
    }

    public void SetMetaDataOfMember(long userid,string key,string value)
    {
        if (userid == 0)
            return;

        var memberTransaction = lobbyManager.GetMemberUpdateTransaction(currentLobbyId, userid);
        memberTransaction.SetMetadata(key, value);
        lobbyManager.UpdateMember(currentLobbyId, userid, memberTransaction, (result) =>
           {
               if (result != Result.Ok)
                   Debug.Log(result);
           });
    }
    public string GetMetaDataOfMember(long userid,string key)
    {
        return lobbyManager.GetMemberMetadataValue(currentLobbyId, userid, key);
    }
}
