using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager INSTANCE;
    public bool Host;
    public GameObject playerPortraits;

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
        return Host;
    }
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void AddLocalUser()
    {
        localPlayers++;
        DiscordLobbyService.INSTANCE.UpdateLobbySize((uint)localPlayers - 1);
    }
    public void AssignPlayersToControllers(ref InputController[] controllers )
    {
        long clientUserId = DiscordLobbyService.INSTANCE.currentOwnerId;

        for (int i = 0; i < localPlayers; i++)
        {
            controllers[i].id = clientUserId;
            controllers[i].typ = InputController.TypeOfContoller.Local;
            controllers[i].VerticalAxis = "Vertical" + (i + 1);
            controllers[i].HorizontalAxis = "Horizontal" + (i + 1);
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
