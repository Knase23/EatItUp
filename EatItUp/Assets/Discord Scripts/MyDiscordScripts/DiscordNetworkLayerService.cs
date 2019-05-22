using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
/// <summary>
/// A Singlton, handles Networking with the help of Discord's own SDK
/// </summary>
public class DiscordNetworkLayerService : MonoBehaviour
{
    public static DiscordNetworkLayerService INSTANCE;

    NetworkManager manager;

    ulong myPeerId;
    List<ulong> othersUserPeerIds = new List<ulong>();
    Dictionary<long, ulong> memberIdToPeerId = new Dictionary<long, ulong>();
    string myRoute;
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
        manager = DiscordManager.INSTANCE.GetDiscord().GetNetworkManager();
        manager.OnRouteUpdate += OnRouteUpdate;
        manager.OnMessage += OnMessage;
        myPeerId = manager.GetPeerId();
    }
    /// <summary>
    /// Takes a Message and does something with it. 
    /// </summary>
    /// <param name="peerId"></param>
    /// <param name="channelId"></param>
    /// <param name="data"></param>
    private void OnMessage(ulong peerId, byte channelId, byte[] data)
    {
        NetworkChannel channel = (NetworkChannel)channelId;
        //Debug.Log("Got Messege from: " + peerId + " on Channel: " + channel);
        switch (channel)
        {
            case NetworkChannel.INPUT_DATA:
               
                PlayerHandler.INSTANCE?.SetInputOnController(data);
                break;
            case NetworkChannel.LOADSCENE:
                GameManager.LoadSceneData sceneToLoad = new GameManager.LoadSceneData(data);
                GameManager.INSTANCE.LoadScene(sceneToLoad.scene);
                break;
            case NetworkChannel.CHARACTER_POSITION:
                
                PlayerHandler.INSTANCE?.SetPositionOfCharacter(data);
                break;
            case NetworkChannel.CONTROLLER_SYNC:
                PlayerHandler.PlayerHandlerData handlerData = new PlayerHandler.PlayerHandlerData(data);
                PlayerHandler.INSTANCE?.SetAllPlayers(handlerData.orderSelected, handlerData.orderOfId);
                break;
            case NetworkChannel.SCORE_SYNC:
                ScoreData scoreData = new ScoreData(data);
                ScoreUpdater.INSTANCE?.UpdateScore(scoreData);
                break;
            case NetworkChannel.PORTRAITS_SYNC:
                DisplayPlayers.INSTANCE.SetOrder(data);
                break;
            default:
                Debug.Log(peerId + " : Sent messege was not reognized");
                break;
        }
    }

    private void LateUpdate()
    {
        manager.Flush();
    }

    private void OnRouteUpdate(string routeData)
    {
        myRoute = routeData;
        DiscordLobbyService.INSTANCE.SetMyMetaData("route", routeData);
    }
    /// <summary>
    /// Sets the Networking MetaData for this client.
    /// </summary>
    public void SetMyPeerId()
    {
        DiscordLobbyService.INSTANCE.SetMyMetaData("peer_id", myPeerId.ToString());
        DiscordLobbyService.INSTANCE.SetMyMetaData("route", myRoute);
    }
    /// <summary>
    /// Sends a Network to everyone that is connected to me
    /// </summary>
    /// <param name="networkChannel"> The kind of data we are sending </param>
    /// <param name="data">The actual Data</param>
    /// <returns></returns>
    public bool SendMessegeToAllOthers(NetworkChannel networkChannel, byte[] data)
    {
        bool anyMessageSent = false;
        foreach (var peerId in othersUserPeerIds)
        {
            manager.SendMessage(peerId, (byte)networkChannel, data);
            anyMessageSent = true;
        }
        return anyMessageSent;
    }
    /// <summary>
    /// Sends a Network package to the owner of the lobby
    /// </summary>
    /// <param name="networkChannel"> The kind of data we are sending </param>
    /// <param name="data">The actual Data</param>
    /// <returns></returns>
    public bool SendMessegeToOwnerOfLobby(NetworkChannel networkChannel, byte[] data)
    {
        ulong peerId;
        if (memberIdToPeerId.TryGetValue(DiscordLobbyService.INSTANCE.currentOwnerId, out peerId))
        {
            manager.SendMessage(peerId, (byte)networkChannel, data);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Makes a connection to the provided member in specified lobby
    /// </summary>
    /// <param name="lobbyId">Lobby to find Metadata for the member</param>
    /// <param name="member">Member Id of the User</param>
    public void EstablishConnectionWithMember(long lobbyId, long member)
    {
        if(member == DiscordLobbyService.INSTANCE.GetCurrentUserId())
        {
            //Debug.LogWarning("Tried to connect to self");
            return;
        }

        if (memberIdToPeerId.ContainsKey(member))
        {
            //Debug.LogWarning("Already have connection to: " + member);
            return;
        }
        DiscordLobbyService.INSTANCE.SetMyMetaData("peer_id", myPeerId.ToString());
        DiscordLobbyService.INSTANCE.SetMyMetaData("route", myRoute);

        LobbyManager lobbyManager = DiscordLobbyService.INSTANCE.lobbyManager;
        ulong peer_id = ulong.Parse(lobbyManager.GetMemberMetadataValue(lobbyId, member, "peer_id"));
        string route = lobbyManager.GetMemberMetadataValue(lobbyId, member, "route");

        manager.OpenPeer(peer_id, route);
        manager.OpenChannel(peer_id, (byte)NetworkChannel.INPUT_DATA, true);
        manager.OpenChannel(peer_id, (byte)NetworkChannel.LOADSCENE, true);
        manager.OpenChannel(peer_id, (byte)NetworkChannel.CONTROLLER_SYNC, true);
        manager.OpenChannel(peer_id, (byte)NetworkChannel.SCORE_SYNC, true);
        manager.OpenChannel(peer_id, (byte)NetworkChannel.PORTRAITS_SYNC, true);
        manager.OpenChannel(peer_id, (byte)NetworkChannel.CHARACTER_POSITION, false);

        if (!othersUserPeerIds.Contains(peer_id))
        {
            othersUserPeerIds.Add(peer_id);
            memberIdToPeerId.Add(member, peer_id);
        }
        //Debug.Log("Sucess on Connection");

    }
    /// <summary>
    /// Updates so the route to the connection is correct
    /// </summary>
    /// <param name="lobbyId"></param>
    /// <param name="member"></param>
    public void UpdateAPeer(long lobbyId, long member)
    {
        if (!memberIdToPeerId.ContainsKey(member))
        {
            EstablishConnectionWithMember(lobbyId, member);
            return;
        }
        try
        {
            LobbyManager lobbyManager = DiscordLobbyService.INSTANCE.lobbyManager;
            ulong peer_id = ulong.Parse(lobbyManager.GetMemberMetadataValue(lobbyId, member, "peer_id"));
            string route = lobbyManager.GetMemberMetadataValue(lobbyId, member, "route");

            manager.UpdatePeer(peer_id, route);
        }
        catch (ResultException result)
        {
            if(result.Result != Result.InternalError && result.Result != Result.Ok)
            {
                Debug.LogError(result.Result);
            }
        }
        
    }
    /// <summary>
    /// Disconnects a connection with a member
    /// </summary>
    /// <param name="member"></param>
    public void DisconnectPeer(long member)
    {
        ulong peer_id;
        if (memberIdToPeerId.TryGetValue(member, out peer_id))
        {
            manager.ClosePeer(peer_id);
            othersUserPeerIds.Remove(peer_id);
            memberIdToPeerId.Remove(member);
        }
    }
}
/// <summary>
/// Describes what kind of data is going through channel
/// </summary>
public enum NetworkChannel
{
    INPUT_DATA = 1,
    LOADSCENE,
    CHARACTER_POSITION,
    CONTROLLER_SYNC,
    SCORE_SYNC,
    PORTRAITS_SYNC

}