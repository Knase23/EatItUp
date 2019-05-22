using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Score : MonoBehaviour
{
    public long id;
    private InputController controller;
    private int value;
    private PortriaitPlayerView playerView;
    private void Start()
    {
        controller = GetComponent<InputController>();
        PlayerPortraitsCanvas.INSTANCE.pairedPortriats.TryGetValue(id, out playerView);
    }
    public void AddToValue(int amount)
    {
        value += amount;
        playerView.SetScoreText(value);
        ////Update Value in key for player
        if (GameManager.INSTANCE.IsTheHost() && DiscordLobbyService.INSTANCE.Online())
        {
            ScoreData data = new ScoreData(value, id);
            DiscordNetworkLayerService.INSTANCE.SendMessegeToAllOthers(NetworkChannel.SCORE_SYNC, data.ToBytes());
        }

    }
    public void UpdateBasedOnLobbyMemeber()
    {
        string newValue = DiscordLobbyService.INSTANCE.GetMetaDataOfMember(controller.memberId, "Score");
        value = int.Parse(newValue);

    }
    public void SetValue(int v)
    {
        value = v;
        playerView.SetScoreText(value);
    }
    public int GetValue()
    {
        return value;
    }
}
public struct ScoreData
{
    public int value;
    public long scoreId;

    public ScoreData(int value, long scoreId)
    {
        this.value = value;
        this.scoreId = scoreId;
    }
    public ScoreData(byte[] data)
    {
        value = BitConverter.ToInt32(data, 0);
        scoreId = BitConverter.ToInt64(data, 4);
    }
    public byte[] ToBytes()
    {
        List<byte> vs = new List<byte>();
        vs.AddRange(BitConverter.GetBytes(value));
        vs.AddRange(BitConverter.GetBytes(scoreId));
        //BitConverter.GetBytes()
        return vs.ToArray();
    }
}
