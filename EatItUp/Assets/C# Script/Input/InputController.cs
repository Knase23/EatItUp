using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public long id;
    public Character controlledCharacter;
    public TypeOfContoller typ;
    public string HorizontalAxis = "Horizontal", VerticalAxis = "Vertical";
    Vector2 dir;
    MoveCommand previousMoveCommand;

    private void Update()
    {
        dir = new Vector2(Input.GetAxisRaw(HorizontalAxis), Input.GetAxisRaw(VerticalAxis));
        if (GameManager.INSTANCE.IsTheHost())
        {
            MoveCommand command = new MoveCommand(dir, controlledCharacter.GetMovement());
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
            //Send Messege to host to update this player
        }
    }

    public enum TypeOfContoller
    {
        Local,
        Online
    }
}