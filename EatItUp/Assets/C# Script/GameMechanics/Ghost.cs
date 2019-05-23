﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Character
{
    // Start is called before the first frame update
    void Start()
    {
        OnStart();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Pac")
        { 
            Character other = collision.collider.GetComponent<Character>();
            if (GameManager.INSTANCE.IsTheHost())
                GetCurrentController().GetComponent<Score>().AddToValue(50);

            SwitchControllers(other);

            if (GameManager.INSTANCE.IsTheHost())
            {
                PlayerHandler.INSTANCE.ResetCharactersPosition();
                //PlayerHandler.inst.SyncInputControllers();
            }
            //TODO: Host sends out change of owner to clients

        }
    }
}