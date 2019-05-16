using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerHandler : MonoBehaviour
{

    public static PlayerHandler inst;

    InputController[] controllers;
    public Character[] playableChracters;

    private InputController currentHolderOfPac;


    private void Awake()
    {
        inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        controllers = GetComponentsInChildren<InputController>();
        GameManager.INSTANCE.AssignPlayersToControllers(ref controllers);
        if (GameManager.INSTANCE.IsTheHost())
        {
            List<int> selected = new List<int>();
            List<long> selectediD = new List<long>();
            //Randomize the Controllers Playercharacter

            foreach (var controller in controllers)
            {
                int rand = UnityEngine.Random.Range(0, playableChracters.Length);
                while (selected.Contains(rand))
                    rand = UnityEngine.Random.Range(0, playableChracters.Length);

                selected.Add(rand);
                selectediD.Add(controller.id);
            }

            SetAllPlayers(selected.ToArray(), selectediD.ToArray());
            //TODO: Host sends sends a data that will

            PlayerHandlerData data = new PlayerHandlerData(selected.ToArray(), selectediD.ToArray());
            DiscordLobbyService.INSTANCE.SendNetworkMessageToClients(3,data.ToBytes());
        }

    }
    public void SetAllPlayers(int[] orderdSelected, long [] idSelected)
    {
        int i = 0;
        foreach (var controller in controllers)
        {
            controller.id = idSelected[i];
            controller.controlledCharacter = playableChracters[orderdSelected[i++]];
            controller.controlledCharacter.SetCurrentController(controller);

            if(controller.id == DiscordLobbyService.INSTANCE.GetCurrentUserId())
            {
                controller.VerticalAxis = "Vertical" + (0 + 1);
                controller.HorizontalAxis = "Horizontal" + (0 + 1);
                controller.typ = InputController.TypeOfContoller.Local;
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
    public void ResetCharactersPosition()
    {
        foreach (var character in playableChracters)
        {
            character.GetMovement()?.TeleportToSpawnPoint();
        }
    }
    public InputController GetInputControllerFromUserId(long userid)
    {
        foreach (var item in controllers)
        {
            if (item.id == userid)
                return item;
        }

        return null;
    }
    public InputController[] GetInputControllers()
    {
        return controllers;
    }

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
            int size = BitConverter.ToInt32(data,0);
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
