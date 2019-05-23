using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{

    public GhostType behaviourAsGhost;
    AI aiCode;

    // Update is called once per frame
    void Update()
    {
        if(aiCode != null)
            aiCode.Sensors();
    }

    public MoveCommand GetMoveCommand(Movement movement)
    {
        if (aiCode != null)
            return aiCode.DecisionOfMovement();

        return new MoveCommand();
    }
    public void SetCharacter(Character character)
    {
        aiCode = new BlinkyAI(character);
    }

    public enum GhostType
    {
        Blinky,
        Pinky,
        Inky,
        Clyde

    }
}
