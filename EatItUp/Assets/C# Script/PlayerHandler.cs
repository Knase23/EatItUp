using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerHandler : MonoBehaviour
{

    public static PlayerHandler inst;

    InputController[] controllers;
    public List<Character> playableChracters = new List<Character>();
    private InputController currentHolderOfPac;
    private Dictionary<long, Character> charactherDictionary = new Dictionary<long, Character>();
    private Dictionary<long, InputController> controllerDictionary = new Dictionary<long, InputController>();
    private void Awake()
    {
        inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        controllers = GetComponentsInChildren<InputController>();
        foreach (var item in playableChracters)
        {
            charactherDictionary.Add(item.id, item);
        }


        if (GameManager.INSTANCE.IsTheHost())
        {
            GameManager.INSTANCE.AssignPlayersToControllers(ref controllers);
            List<int> selected = new List<int>();
            List<long> selectediD = new List<long>();
            //Randomize the Controllers Playercharacter

            foreach (var controller in controllers)
            {
                int rand = UnityEngine.Random.Range(0, playableChracters.Count);
                while (selected.Contains(rand))
                    rand = UnityEngine.Random.Range(0, playableChracters.Count);

                selected.Add(rand);
                selectediD.Add(controller.id);
                if (controller.id != 0)
                    controllerDictionary.Add(controller.id, controller);
            }

            SetAllPlayers(selected.ToArray(), selectediD.ToArray());
            //TODO: Host sends sends a data that will

            PlayerHandlerData data = new PlayerHandlerData(selected.ToArray(), selectediD.ToArray());
            DiscordLobbyService.INSTANCE.SendNetworkMessageToClients(3, data.ToBytes());
        }

    }
    public void SyncInputControllers()
    {
        List<int> selected = new List<int>();
        List<long> selectediD = new List<long>();
        foreach (var controller in controllers)
        {
            selectediD.Add(controller.id);
            selected.Add(GetIndexOfCharacter(controller.controlledCharacter));
        }
        PlayerHandlerData data = new PlayerHandlerData(selected.ToArray(), selectediD.ToArray());
        DiscordLobbyService.INSTANCE.SendNetworkMessageToClients(3, data.ToBytes());
    }

    public void SetAllPlayers(int[] orderdSelected, long[] idSelected)
    {
        int i = 0;
        int localControllers = 1;
        foreach (var controller in controllers)
        {
            controller.id = idSelected[i];
            controller.controlledCharacter = playableChracters[orderdSelected[i++]];
            controller.controlledCharacter.SetCurrentController(controller);

            if (DiscordLobbyService.INSTANCE.currentLobbyId == 0 || controller.id == DiscordLobbyService.INSTANCE.GetCurrentUserId())
            {
                controller.VerticalAxis = "Vertical" + localControllers;
                controller.HorizontalAxis = "Horizontal" + localControllers;
                controller.typ = InputController.TypeOfContoller.Local;
                localControllers++;
            }
            else
            {
                controller.VerticalAxis = string.Empty;
                controller.HorizontalAxis = string.Empty;
                controller.typ = InputController.TypeOfContoller.Online;
            }
        }
    }

    public void SetCurrentHolderOfPac(InputController current)
    {
        currentHolderOfPac = current;
    }

    public void GivePointsForPacman()
    {
        currentHolderOfPac.GetComponent<Score>().AddToValue(10);
    }

    public int GetIndexOfCharacter(Character character)
    {
        return playableChracters.IndexOf(character);
    }

    /// <summary>
    /// Resets each character to their origional start position
    /// </summary>
    public void ResetCharactersPosition()
    {
        foreach (var character in playableChracters)
        {
            character.GetMovement()?.TeleportToSpawnPoint();
        }
    }

    public InputController GetInputControllerFromUserId(long userid)
    {
        if (userid == 0)
            return null;

        foreach (var item in controllers)
        {
            if (item.id == userid)
                return item;
        }

        return null;
    }
    public bool SetInputOnController(InputController.InputData data)
    {
        InputController controller = null;
        if (!controllerDictionary.TryGetValue(data.id, out controller))
            return false;

        controller.SetDirection(data);
        return true;
    }
    public bool SetPositionOfCharacter(Movement.MovementData data)
    {
        Character character = null;
        if (!charactherDictionary.TryGetValue(data.characterId, out character))
            return false;

        character.SetPosition(data.x, data.y);
        character.SetRotation(data.rotationZ);
        return true;
    }
    public InputController[] GetInputControllers()
    {
        return controllers;
    }
    /// <summary>
    /// Data sent to the host and convernet for handeling the order and selected character for all InputControllers 
    /// </summary>
    public struct PlayerHandlerData
    {
        public int[] orderSelected;
        public long[] orderOfId;
        public PlayerHandlerData(int[] orderSelected, long[] orderOfId)
        {
            this.orderSelected = orderSelected;
            this.orderOfId = orderOfId;
        }
        public PlayerHandlerData(byte[] data)
        {
            int size = BitConverter.ToInt32(data, 0);
            orderSelected = new int[size];
            for (int i = 0; i < size; i++)
            {
                orderSelected[i] = BitConverter.ToInt32(data, i * 4 + 4);
            }
            orderOfId = new long[size];
            for (int i = 0; i < size; i++)
            {
                orderOfId[i] = BitConverter.ToInt64(data, i * 8 + size * 4 + 4);
            }
        }
        public byte[] ToBytes()
        {
            List<byte> byteList = new List<byte>();
            byteList.AddRange(BitConverter.GetBytes(orderSelected.Length));

            for (int i = 0; i < orderSelected.Length; i++)
            {
                byteList.AddRange(BitConverter.GetBytes(orderSelected[i]));
            }
            for (int i = 0; i < orderSelected.Length; i++)
            {
                byteList.AddRange(BitConverter.GetBytes(orderOfId[i]));
            }


            //BitConverter.GetBytes()
            return byteList.ToArray();
        }
    }
}
