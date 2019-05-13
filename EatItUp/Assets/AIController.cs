using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{

    public GhostType behaviourAsGhost;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public MoveCommand GetMoveCommand(Movement movement)
    {
        MoveCommand command = new MoveCommand(Vector2.zero, movement);

        //Decicsion for the direction it wants to move

        return command;
    }

    public enum GhostType
    {
        Blinky,
        Pinky,
        Inky,
        Clyde

    }
}
