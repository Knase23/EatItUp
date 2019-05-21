using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Discord;


public class DiscordLobbyService : MonoBehaviour
{
    public static DiscordLobbyService INSTANCE;

    public LobbyManager lobbyManager;

    public UserManager userManager;
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
        lobbyManager.OnLobbyUpdate += OnLobbyUpdate;
        lobbyManager.OnLobbyUpdate += DiscordActivityService.INSTANCE.OnLobbyUpdate;
        lobbyManager.OnMemberConnect += OnMemberConnect;
        lobbyManager.OnMemberUpdate += OnMemberUpdate;
        lobbyManager.OnMemberDisconnect += OnMemberDisconnect;
    }

    private void OnMemberConnect(long lobbyId, long userId)
    {
        if (userId != GetCurrentUserId() && lobbyManager.MemberMetadataCount(lobbyId, userId) >= 2)
            DiscordNetworkLayerService.INSTANCE.EstablishConnectionWithMember(lobbyId, userId);
    }
    private void OnMemberUpdate(long lobbyId, long userId)
    {
        if(userId != GetCurrentUserId() && lobbyManager.MemberMetadataCount(lobbyId, userId) >= 2)
            DiscordNetworkLayerService.INSTANCE.UpdateAPeer(lobbyId,userId);
    }
    private void OnMemberDisconnect(long lobbyId, long userId)
    {
        DiscordNetworkLayerService.INSTANCE.DisconnectPeer(userId);
    }

    private void Update()
    {
        if (coroutine == null && currentLobbyId != 0)
        {
            coroutine = StartCoroutine(UpdateLobbyTransaction());
        }
    }
    private void OnLobbyUpdate(long lobbyId)
    {
        foreach (var member in GetLobbyMembers())
        {
            if (member.Id != GetCurrentUserId() && lobbyManager.MemberMetadataCount(lobbyId, member.Id) >= 2)
                DiscordNetworkLayerService.INSTANCE.EstablishConnectionWithMember(lobbyId, member.Id);
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
        lobbyManager.CreateLobby(txn, (Result result, ref Lobby lobby) =>
        {
            SetCurrent(lobby.Id, lobby.Secret, lobby.OwnerId);
            DiscordNetworkLayerService.INSTANCE.SetMyPeerId();
            NewUpdateLobbyTransaction();
        });
    }
    public void UpdateLobbySize(uint numberOfLocalPlayersConnected)
    {
        if (currentLobbyId == 0)
            return;
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
        DiscordActivityService.INSTANCE.Activity(new DiscordActivityService.ActivityInformation(GameManager.INSTANCE.GetCurrentGameState()));
    }
    public void RemoveLobby()
    {
        if(currentLobbyId == 0)
            return;

        if (currentOwnerId != GetCurrentUserId())
            return;
        
        lobbyManager.DeleteLobby(currentLobbyId, (result)=>{
            if (result != Result.Ok)
                Debug.Log(result);
            else
            {
                Debug.Log("Deleted Lobby");
            }
        });
    }
    public void ConnectToLobby()
    {
        var l = GetLobby();
        lobbyManager.ConnectLobby(l.Id, l.Secret, (Result result, ref Lobby lobby) =>
        {
            if (result == Result.Ok)
            {
                SetCurrent(lobby.Id, lobby.Secret, lobby.OwnerId);
                DiscordNetworkLayerService.INSTANCE.SetMyPeerId();

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
            //Debug.Log("ConnectToLobbyThroughSecret");
            if (result == Result.Ok)
            {
                SetCurrent(lobby.Id, lobby.Secret, lobby.OwnerId);
                DiscordNetworkLayerService.INSTANCE.SetMyPeerId();
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
    public bool IsTheHost()
    {
        return currentOwnerId == 0 || userManager.GetCurrentUser().Id == currentOwnerId;
    }
    public long GetCurrentUserId()
    {
        return userManager.GetCurrentUser().Id;
    }

    //Functions for this script
    private void SetCurrent(long lobbyId, string secret, long ownerId)
    {
        currentLobbyId = lobbyId;
        currentSecret = secret;
        currentOwnerId = ownerId;
    }


    IEnumerator UpdateLobbyTransaction()
    {
        yield return new WaitForSecondsRealtime(0.6f);
        NewUpdateLobbyTransaction();
        coroutine = null;
        yield break;
    }

    void NewUpdateLobbyTransaction()
    {
        if (currentLobbyId == 0)
            return;

        var transaction = lobbyManager.GetLobbyUpdateTransaction(currentLobbyId);

#region Set Meta Data For Lobby
        if (SceneManager.GetActiveScene().name == "Game")
        {
            transaction.SetLocked(true);
        }
        else
        {
            transaction.SetLocked(false);
        }
#endregion

        lobbyManager.UpdateLobby(currentLobbyId, transaction, (newResult) =>
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
    public void SetMyMetaData(string key, string value)
    {
        if(currentLobbyId == 0)
        {
            return;
        }
        var memberTransaction = lobbyManager.GetMemberUpdateTransaction(currentLobbyId, GetCurrentUserId());
        memberTransaction.SetMetadata(key, value);
        lobbyManager.UpdateMember(currentLobbyId, GetCurrentUserId(), memberTransaction, (result) =>
        {
            if (result != Result.Ok)
                Debug.Log(result);
        });
        //Debug.Log("Setted: " + key + " : " + value);
    }
    public string GetMetaDataOfMember(long userid,string key)
    {
        return lobbyManager.GetMemberMetadataValue(currentLobbyId, userid, key);
    }
}
