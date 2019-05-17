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
    InputData latestInputUpdate;
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
            // What the Host whil do.
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
        }
        else
        {
            // Client
            if (typ == TypeOfContoller.Local)
            {
                InputData data = new InputData((short)Mathf.RoundToInt(dir.x), (short)Mathf.RoundToInt(dir.y), id);
                if (latestInputUpdate.x != data.x || latestInputUpdate.y != data.y)
                {
                    latestInputUpdate = data;
                    if (DiscordLobbyService.INSTANCE.SendNetworkMessageToHost(0, data.ToBytes()))
                        Debug.Log("Send Input to Host");
                }
            }
            //Send Messege to host to update this player
        }

    }
    public void SetDirection(InputData data)
    {
        Debug.Log("Data X: " + data.x);
        Debug.Log("Data Y: " + data.y);
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