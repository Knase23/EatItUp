﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class GameManager : MonoBehaviour
{

    public static GameManager INSTANCE;
    public bool Host;

    public int localPlayers = 1;

    private void Awake()
    {
        if(INSTANCE)
        {
            Destroy(gameObject);
        }
        else
        {
            INSTANCE = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public bool IsTheHost()
    {
        Host = DiscordLobbyService.INSTANCE.IsTheHost();
        return Host;
    }
    public void LoadScene(string scene)
    {
        LoadSceneData data = new LoadSceneData(scene);
        if(IsTheHost())
            DiscordNetworkLayerService.INSTANCE.SendMessegeToAllOthers(NetworkChannel.LOADSCENE, data.ToBytes());

        SceneManager.LoadScene(scene);
    }
    public struct LoadSceneData
    {
        public string scene;
        public LoadSceneData(string scene)
        {
            this.scene = scene;
        }
        public LoadSceneData(byte[] data)
        {
            scene = System.Text.Encoding.UTF8.GetString(data);
        }
        public byte[] ToBytes()
        {
            return System.Text.Encoding.UTF8.GetBytes(scene);
        }
    }
    public void AddLocalUser()
    {
        localPlayers++;
        DiscordLobbyService.INSTANCE.UpdateLobbySize((uint)localPlayers - 1);
    }

    public void AssignPlayersToControllers(ref InputController[] controllers )
    {
        long clientUserId = DiscordLobbyService.INSTANCE.currentOwnerId;
        int i = 0;
        int controllerCOunter = 1;
        IEnumerable<Discord.User> feel = DiscordLobbyService.INSTANCE.GetLobbyMembers();
        if (feel != null)
        {
            foreach (var item in feel)
            {
                if (item.Id != clientUserId)
                {
                    controllers[i].memberId = item.Id;
                    controllers[i].typ = InputController.TypeOfContoller.Online;
                    controllers[i].VerticalAxis = string.Empty;
                    controllers[i++].HorizontalAxis = string.Empty;
                }
                else
                {
                    controllers[i].memberId = item.Id;
                    controllers[i].typ = InputController.TypeOfContoller.Local;
                    controllers[i].VerticalAxis = "Vertical" + (controllerCOunter);
                    controllers[i++].HorizontalAxis = "Horizontal" + (controllerCOunter);
                    controllerCOunter++;
                }
            }
        }
    }
    public string GetCurrentGameState()
    {
        string state = "Menu";
        string scneneName = SceneManager.GetActiveScene().name;

        if (scneneName == "ConnectPlayers")
            state = "In Lobby";

        if (scneneName == "Game")
            state = "In Game";

        return state;
    }


}
