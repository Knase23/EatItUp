using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{


    InputController currentController;
    Movement movement;

    protected void OnStart()
    {
        movement = GetComponent<Movement>();
    }
    public Movement GetMovement()
    {
        return movement;
    }
    public void SetCurrentController(InputController controller)
    {
        currentController = controller;
        controller.controlledCharacter = this;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = currentController.color;

        if (tag == "Pac")
            PlayerHandler.inst.SetCurrentHolderOfPac(controller);

        PlayerHandler.inst.ResetCharactersPosition();
    }
    public InputController GetCurrentController()
    {
        return currentController;
    }

}
