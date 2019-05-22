using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AI 
{
    //Sensors
    protected Character charachter;
    protected bool changeMoveCommmand;
    protected MoveCommand latestMoveCommand;

    public abstract void Sensors();
    public abstract MoveCommand DecisionOfMovement();
}
