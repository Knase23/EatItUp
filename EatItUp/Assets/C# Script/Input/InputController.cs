using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputController : MonoBehaviour
{
    public long id;
    public AIController AI;
    public Character controlledCharacter;
    public TypeOfContoller typ;
    public string HorizontalAxis = "Horizontal", VerticalAxis = "Vertical";
    public Color color;
    Vector2 dir;
    MoveCommand previousMoveCommand;
    InputData prev;
    Movement.MovmentData prevMove;
    private void Start()
    {
        AI = GetComponent<AIController>();
    }
    private void Update()
    {

        if (typ == TypeOfContoller.Local)
            dir = new Vector2(Input.GetAxisRaw(HorizontalAxis), Input.GetAxisRaw(VerticalAxis));

        if (GameManager.INSTANCE.IsTheHost())
        {
            MoveCommand command = new MoveCommand(dir, controlledCharacter.GetMovement());
            if (typ == TypeOfContoller.AI)
                command = AI.GetMoveCommand(controlledCharacter.GetMovement());

            if (previousMoveCommand != command)
            {
                previousMoveCommand = command;
                command.Do();
            }
            else
            {
                previousMoveCommand.Do();
            }
            Movement.MovmentData movmentData = new Movement.MovmentData(controlledCharacter.transform.position.x, controlledCharacter.transform.position.y, id);
            if (prevMove.x != movmentData.x || prevMove.y != movmentData.y)
            {
                prevMove = movmentData;
                
                if(DiscordLobbyService.INSTANCE.SendNetworkMessageToClients(2, movmentData.ToBytes()))
                    Debug.Log("Update clients, Charachter Position");

            }
        }
        else
        {
            if (typ == TypeOfContoller.Local)
            {
                InputData data = new InputData((short)Mathf.RoundToInt(dir.x), (short)Mathf.RoundToInt(dir.y), id);
                if (data.x != prev.x || data.y != prev.y)
                {
                    prev = data;
                    Debug.Log("Client wants to Change Direcion");
                    DiscordLobbyService.INSTANCE.SendNetworkMessageToHost(0, data.ToBytes());
                }
            }
                //Send Messege to host to update this player
        }

    }
    public void SetDirection(InputData data)
    {
        Debug.Log("Data X: " + data.x);
        Debug.Log("Data X: " + data.y);

        dir = new Vector2(data.x, data.y);
    }

    public struct InputData
    {
        public short x, y;
        public long id;
        public InputData(short x, short y, long id)
        {
            this.x = x;
            this.y = y;
            this.id = id;
        }
        public InputData(byte[] data)
        {
            x = BitConverter.ToInt16(data, 0);
            y = BitConverter.ToInt16(data, 2);
            id = BitConverter.ToInt64(data, 4);
        }
        public byte[] ToBytes()
        {
            List<byte> vs = new List<byte>();
            vs.AddRange(BitConverter.GetBytes(x));
            vs.AddRange(BitConverter.GetBytes(y));
            vs.AddRange(BitConverter.GetBytes(id));
            //BitConverter.GetBytes()
            return vs.ToArray();
        }
    }
    public enum TypeOfContoller
    {
        Local,
        Online,
        AI
    }
}