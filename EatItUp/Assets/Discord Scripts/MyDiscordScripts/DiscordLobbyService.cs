using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        Debug.Log("Got Network Message");
        // Use a Decoder, that will then process what to do with the information
    }


    // Functions to Use to Call from other scripts
    public void CreateLobby()
    {
        if (currentLobbyId != 0)
            return;

        var txn = lobbyManager.GetLobbyCreateTransaction();
        txn.SetCapacity(6);
        txn.SetType(LobbyType.Public);
        txn.SetMetadata("Something", "123");
        lobbyManager.CreateLobby(txn, (Result result, ref Lobby lobby) =>
        {
            SetCurrent(lobby.Id, lobby.Secret, lobby.OwnerId);
            try
            {
                lobbyManager.ConnectNetwork(lobby.Id);
            }
            catch (ResultException result2)
            {
                Debug.Log(result2);
            }

            NewLobbyTransaction();

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
            Debug.Log("ConnectToLobby");
            if (result == Result.Ok)
            {
                Debug.Log("It worked?");
                InitNetworking(lobby.Id);
                SetCurrent(lobby.Id, lobby.Secret, lobby.OwnerId);
                for (int i = 0; i < lobbyManager.MemberCount(lobby.Id); i++)
                {
                    var userId = lobbyManager.GetMemberUserId(lobby.Id, i);
                    lobbyManager.SendNetworkMessage(lobby.Id, userId, 0, System.Text.Encoding.UTF8.GetBytes("Hello!"));
                }
            }
            else
            {
                Debug.Log(result);
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
                    lobbyManager.SendNetworkMessage(lobby.Id, userId, 0, System.Text.Encoding.UTF8.GetBytes("Hello!"));
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

        #endregion

        lobbyManager.UpdateLobby(currentLobbyId, newTxn, (newResult) =>
        {
            if (newResult == Result.Ok)
            {
                //Debug.Log("Lobby updated");
            }
            else
            {
                Debug.Log(newResult, gameObject);
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
        lobbyManager.OpenNetworkChannel(lobbyId, 1, false);
        // We're ready to go!
    }

}
