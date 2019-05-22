using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputController : MonoBehaviour
{
    public long id;
    public long memberId;
    public AIController AI;
    public Character controlledCharacter;
    public TypeOfContoller typ;
    public string HorizontalAxis = "Horizontal", VerticalAxis = "Vertical";
    public Color color;



    Vector2 dir = new Vector2();



    MoveCommand previousMoveCommand;
    InputData latestInputUpdate;


    private void Start()
    {
        AI = GetComponent<AIController>();
    }
    public Character GetControlledCharacter()
    {
        return controlledCharacter;
    }
    public void SetTheControlledCharcter(Character character)
    {
        controlledCharacter = character;
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
                InputData data = new InputData(Input.GetAxisRaw(HorizontalAxis), Input.GetAxisRaw(VerticalAxis), memberId);
                if (latestInputUpdate.x != data.x || latestInputUpdate.y != data.y)
                {
                    latestInputUpdate = data;
                    DiscordNetworkLayerService.INSTANCE.SendMessegeToOwnerOfLobby(NetworkChannel.INPUT_DATA, latestInputUpdate.ToBytes());
                }
            }
            //Send Messege to host to update this player
        }
    }

    public void SetDirection(InputData data)
    {
        dir = new Vector2(data.x, data.y);
    }

    public struct InputData
    {
        public float x, y;
        public long id;
        public InputData(float x, float y, long id)
        {
            this.x = x;
            this.y = y;
            this.id = id;
        }
        public InputData(byte[] data)
        {
            x = BitConverter.ToSingle(data, 0);
            y = BitConverter.ToSingle(data, 4);
            id = BitConverter.ToInt64(data, 8);
        }
        public byte[] ToBytes()
        {
            List<byte> vs = new List<byte>();
            
            vs.AddRange(BitConverter.GetBytes(x));
            vs.AddRange(BitConverter.GetBytes(y));
            vs.AddRange(BitConverter.GetBytes(id));
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