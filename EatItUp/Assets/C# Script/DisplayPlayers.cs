using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class DisplayPlayers : MonoBehaviour
{
    Discord.LobbyManager lobbyManager;
    Discord.UserManager userManager;
    public Dictionary<long, long> memberIDToIndex = new Dictionary<long, long>();
    public static DisplayPlayers INSTANCE;

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

    private void Start()
    {
        lobbyManager = DiscordLobbyService.INSTANCE.lobbyManager;
        lobbyManager.OnLobbyUpdate += UpdateWithLobbyMembers;
    }
    public void UpdateWithLobbyMembers(long lobbyId)
    {
        var members = lobbyManager.GetMemberUsers(lobbyId);
        long counter = 1;
        foreach (var item in members)
        {
            long index;
            if (memberIDToIndex.TryGetValue(item.Id, out index))
            {
                UpdateDisplayForId(item, index);
                counter++;
            }
            else
            {
                UpdateDisplayForId(item, counter++);
                if (GameManager.INSTANCE.IsTheHost())
                {
                    memberIDToIndex.Add(item.Id, counter - 1);
                }
            }
        }
        for (int i = (int)counter; i < PlayerPortraitsCanvas.INSTANCE.pairedPortriats.Count; i++)
        {
            UpdateDisplayForId(new Discord.User(), i);
        }
        if (GameManager.INSTANCE.IsTheHost() && DiscordLobbyService.INSTANCE.Online())
        {
            List<long> keys = new List<long>();
            List<long> values = new List<long>();
            foreach (var item in memberIDToIndex)
            {
                keys.Add(item.Key);
                values.Add(item.Value);
            }
            DisplayOrderData data = new DisplayOrderData(keys.ToArray(), values.ToArray());
            DiscordNetworkLayerService.INSTANCE.SendMessegeToAllOthers(NetworkChannel.PORTRAITS_SYNC, data.ToBytes());
        }
    }
    public void SetOrder(byte[] data)
    {
        DisplayOrderData newData = new DisplayOrderData(data);
        memberIDToIndex.Clear();
        for (int i = 0; i < newData.keys.Length; i++)
        {
            memberIDToIndex.Add(newData.keys[i], newData.values[i]);
        }
    }

    public void UpdateDisplayForId(Discord.User user, long key)
    {
        PortriaitPlayerView view;
        PlayerPortraitsCanvas.INSTANCE.pairedPortriats.TryGetValue(key, out view);

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
            Score score = null;
            ScoreUpdater.INSTANCE?.scores.TryGetValue(key, out score);
            if (score != null)
                view.SetUser(user.Username, "Score: " + score.GetValue(), avatar);
            else
                view.SetUser(user.Username, "Score: " + 0, avatar);
        }
        else
        {
            view.SetToDefault();
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
    public struct DisplayOrderData
    {
        public long[] keys;
        public long[] values;
        public DisplayOrderData(long[] keys, long[] values)
        {
            this.keys = keys;
            this.values = values;
        }
        public DisplayOrderData(byte[] data)
        {
            int size = BitConverter.ToInt32(data, 0);
            keys = new long[size];
            for (int i = 0; i < size; i++)
            {
                keys[i] = BitConverter.ToInt64(data, i * 8 + 4);
            }
            values = new long[size];
            for (int i = 0; i < size; i++)
            {
                values[i] = BitConverter.ToInt64(data, i * 8 + size * 8 + 4);
            }
        }
        public byte[] ToBytes()
        {
            List<byte> byteList = new List<byte>();
            byteList.AddRange(BitConverter.GetBytes(keys.Length));

            for (int i = 0; i < keys.Length; i++)
            {
                byteList.AddRange(BitConverter.GetBytes(keys[i]));
            }
            for (int i = 0; i < keys.Length; i++)
            {
                byteList.AddRange(BitConverter.GetBytes(values[i]));
            }


            //BitConverter.GetBytes()
            return byteList.ToArray();
        }
    }
}
