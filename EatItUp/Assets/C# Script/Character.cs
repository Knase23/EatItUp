using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    Movement movement;

    protected void OnStart()
    {
        movement = GetComponent<Movement>();
    }
    public Movement GetMovement()
    {
        return movement;
    }

}
